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
    /// <summary>Provide methods for dealing with IDA databases.</summary>
    public class IdaDatabase
    {
        #region CONSTRUCTORS
        private IdaDatabase()
        {
            Current = this;
            return;
        }
        #endregion

        #region PROPERTIES
        /// <summary>Retrieve the current IDA database instance if any.</summary>
        public static IdaDatabase Current
        {
            get { lock (_globalLock) { return _currentDatabase; } }
            private set { lock(_globalLock) { _currentDatabase = value; } }
        }
        #endregion

        #region METHODS
        /// <summary>Create an IDA database from file having the given path. Attempting to create
        /// a database while <see cref="Current"/> property is not null will tirgger an exception.
        /// </summary>
        /// <param name="filePath">Fully qualified or relative path to the file to be used.</param>
        /// <returns></returns>
        public static IdaDatabase Create(string filePath, LoadFlags loadFlags)
        {
            lock (_globalLock) {
                if (null != Current) { throw new IdaNetException(); }
                IntPtr input = IntPtr.Zero;
                
                try {
                    input = IdaNatives.open_linput(filePath, false);
                    LoaderManager.LoadFile(filePath, loadFlags);
                    return new IdaDatabase();
                } finally { if (IntPtr.Zero != input) { IdaNatives.close_linput(input); } }
            }
        }

        /// <summary>Save current database to the given file path.</summary>
        /// <param name="toFilePath">Target file path.</param>
        /// <param name="flags"></param>
        public void Save(string toFilePath, DatabaseFlags flags)
        {
            if (null == Current) {
                // Should never occur.
                throw new InvalidOperationException(Messages.NoDatabase);
            }
            if (!IdaNatives.save_database_ex(toFilePath, flags, IntPtr.Zero, IntPtr.Zero)) {
                throw new IdaNetException(Messages.DatabaseFailedToSave, toFilePath);
            }
        }
        #endregion

        #region FIELDS
        private static IdaDatabase _currentDatabase;
        private static object _globalLock = new object();
        #endregion
    }
}
