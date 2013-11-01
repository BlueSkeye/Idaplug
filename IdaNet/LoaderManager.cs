using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using SegmentSelector = System.UInt64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using SegmentSelector = System.UInt32;
#endif

namespace IdaNet
{
    /// <summary>A delegate than can be used for choosing amongst several possible loaders for a file.
    /// </summary>
    /// <param name="candidates">A list of candidate loaders.</param>
    /// <returns>The selected loader or a null reference if no loader is suitable. The returned loader
    /// if not a null reference MUST be one from the passed in collection otherwise an exceptio is
    /// thrown.</returns>
    public delegate LoaderManager.ILoaderDescriptor CandidateLoaderChooserDelegate(
        IEnumerable<LoaderManager.ILoaderDescriptor> candidates, ref LoadFlags loadFlags);

    /// <summary>This class handles loading files into the current database. It takes care of
    /// IDA loader discovery and interaction.</summary>
    public static class LoaderManager
    {
        internal static void LoadFile(string filePath, LoadFlags loadFlags,
            CandidateLoaderChooserDelegate chooser = null)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr firstLoader = IntPtr.Zero;

            try {
                input = IdaNatives.open_linput(filePath, false);
                firstLoader = IdaNatives.build_loaders_list(input);
                if (IntPtr.Zero == firstLoader) { throw new IdaNetException(Messages.NoLoaderFound); }
                List<ILoaderDescriptor> candidates = CandidateLoader.BuildChain(firstLoader);
                ILoaderDescriptor selectedLoader = (null == chooser) ? candidates[0] : chooser(candidates, ref loadFlags);
                if (IntPtr.Zero == firstLoader) { throw new IdaNetException(Messages.NoLoaderFound); }
                bool fakeLoader = true;

                // Make sure the chooser didn't attempted to return a loader of it's own.
                foreach (ILoaderDescriptor knownCandidate in candidates) {
                    if (object.ReferenceEquals(knownCandidate, selectedLoader)) {
                        fakeLoader = false;
                        break;
                    }
                }
                if (fakeLoader) { throw new InvalidOperationException(Messages.SelectedLoaderNotGenuine); }

                // Go on with loading file and make sure the mandatory LoadFlags.FirstLoadedFile
                // is set.
                loadFlags &= LoadFlags.FirstLoadedFile;
                // TODO : Should make this a little more flexible for x86/x64 disambiguation.
                string systemFolderPath =
#if __EA64__
                    Environment.GetFolderPath(Environment.SpecialFolder.System);
#else
                    Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
#endif
                if (!IdaNatives.load_nonbinary_file(filePath, input, systemFolderPath, loadFlags, ((CandidateLoader)selectedLoader)._at)) {
                    throw new IdaNetException(Messages.FileLoadFailed, filePath);
                }
            }
            finally {
                if (IntPtr.Zero != input) { IdaNatives.close_linput(input); }
                if (IntPtr.Zero != firstLoader) { IdaNatives.free_loaders_list(firstLoader); }
            }
        }

        /// <summary>An interface that provide the public view for a loader descriptor.</summary>
        public interface ILoaderDescriptor
        {
            string DllName { get; }
            string FileTypeName { get; }
            KnownFileType FileType { get; }
            int Priority { get; }
        }

        /// <summary>This class describes a candidate loader for a specific file.</summary>
        private class CandidateLoader : ILoaderDescriptor
        {
            #region CONSTRUCTORS
            /// <summary>Unmarshal a native loader description into a managed object.</summary>
            /// <param name="from">The native address of the native descriptor.</param>
            /// <param name="next">On return this parameter is updated with the native address of the
            /// next descriptor in chain or with IntPtr.Zero at end of chain.</param>
            private CandidateLoader(IntPtr from, out IntPtr next)
            {
                _at = from;

                int offset = 0;
                next = Marshal.ReadIntPtr(from, offset);
                offset += IntPtr.Size;
                DllName = Marshal.PtrToStringAnsi(from + offset);
                offset += Constants.QMAXPATH;
                FileTypeName = Marshal.PtrToStringAnsi(from + offset);
                offset += Constants.MAX_FILE_FORMAT_NAME;
                FileType = (KnownFileType)(Marshal.ReadInt32(from, offset));
                offset += sizeof(int);
                Priority = Marshal.ReadInt32(from, offset);
                offset += sizeof(int);
                return;
            }
            #endregion

            #region PROPERTIES
            public string DllName { get; private set;  }

            public string FileTypeName { get; private set; }
            
            public KnownFileType FileType { get; private set; }
            
            public int Priority { get; private set; }
            #endregion

            #region METHODS
            /// <summary>Build a chain of loader's descriptors starting with the descriptor at given
            /// native address.</summary>
            /// <param name="firstLoader">Native address of first descriptor in chain.</param>
            /// <returns>A list of descriptors.</returns>
            internal static List<ILoaderDescriptor> BuildChain(IntPtr firstLoader)
            {
                List<ILoaderDescriptor> result = new List<ILoaderDescriptor>();
                IntPtr nativeLoader = firstLoader;

                while (IntPtr.Zero != nativeLoader) {
                    result.Add(new CandidateLoader(nativeLoader, out nativeLoader));
                }
                return result;
            }
            #endregion

            #region FIELDS
            // Addres of native loader.
            internal IntPtr _at;
            #endregion
        }
    }
}
