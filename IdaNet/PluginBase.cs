using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IdaNet
{
    /// <summary>An abstract base class your plugin could extend. This is intended to group several
    /// key methods that are expected to be available in every plugin. Using a base class allows for
    /// easier update than direct code insertion in the plugin code itself.</summary>
    public abstract class PluginBase : IIdaPlugin
    {
        #region CONSTRUCTORS
        protected PluginBase()
        {
            InitializeId();
            _synchronizer = GetSynchronizationEvent();
            return;
        }
        #endregion

        #region PROPERTIES
        /// <summary>Get the identifier of the plugin assembly.</summary>
        protected string Id { get; private set; }

        protected string SynchronizationEventName
        {
            get { return string.Format(@"Global\{0}", Id); }
        }
        #endregion

        #region METHODS
        /// <summary>An abstract method to be implemented by your plugin.</summary>
        protected abstract void DoRun();

        /// <summary>Make sure out private log file path is initialized.</summary>
        private void EnsurePrivateLogFile()
        {
            if(null == _privateLogFile) {
                _privateLogFile = Environment.ExpandEnvironmentVariables(
                    @"%LOCALAPPDATA%\idaplug\logs\Plugin.log");
            }
            return;
        }

        /// <summary>Retrieve a strema writer on our private log file.</summary>
        /// <returns></returns>
        private StreamWriter GetPrivateLogWriter()
        {
            EnsurePrivateLogFile();
            return new StreamWriter(_privateLogFile, true, Encoding.UTF8);
        }

        // TODO : Make this a SafeHandle
        private IntPtr GetSynchronizationEvent()
        {
            try { return CreateEvent(IntPtr.Zero, true, false, SynchronizationEventName); }
            catch(Exception e) {
                WriteDebugMessage("Failed to create synchronization event");
                TraceException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>This method is invoked when IDA discovers the plugin and loads it. The
        /// plugin tells back IDA whether it wants to cooperate on the current database.
        /// WARNING : When this method is invoked, the database is not yet active. You CANT
        /// invoke any method that requires anything other than calling back the UI for message
        /// display.</summary>
        /// <returns>The wished plugin behavior.</returns>
        public virtual int Initialize()
        {
            return PLUGIN_KEEP;
        }

        /// <summary>This method is solely intended to be invoked from the constructor. It retrieves
        /// the Guid of the assembly that is currently instanciating the class. From that we seek for
        /// the assembly Guid attribute.</summary>
        private void InitializeId()
        {
            StackTrace currentStack = new StackTrace();
            GuidAttribute myGuidAttribute =
                Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(GuidAttribute)) as GuidAttribute;
            if(null == myGuidAttribute) {
                throw new InvalidOperationException(Messages.NoGuidFoundOnIdaNet);
            }
            string idaNetGuid = myGuidAttribute.Value;
            int framesCount = currentStack.GetFrames().Length;
            bool myGuidFoundOnStack = false;

            for(int frameIndex = 0; frameIndex < framesCount; frameIndex++) {
                Assembly candidateAssembly = currentStack.GetFrame(frameIndex).GetMethod().Module.Assembly;
                GuidAttribute guidAttribute = candidateAssembly.GetCustomAttribute(typeof(GuidAttribute)) as GuidAttribute;

                if(null == guidAttribute) { continue; }
                if(guidAttribute.Value == idaNetGuid) {
                    myGuidFoundOnStack = true;
                    continue;
                }
                if(!myGuidFoundOnStack) { continue; }
                if(null == guidAttribute) {
                    throw new InvalidOperationException(
                        string.Format(Messages.NoGuidFoundOnCallingAssembly, candidateAssembly.FullName));
                }
                Id = guidAttribute.Value;
                break;
            }
            if (string.IsNullOrEmpty(Id)) {
                throw new InvalidOperationException(Messages.CantFindCallingAssembly);
            }
            return;
        }

        /// <summary></summary>
        public void Run()
        {
            try { DoRun(); }
            finally {
                lock(this) {
                    if(IntPtr.Zero != _synchronizer) {
                        SetEvent(_synchronizer);
                        CloseHandle(_synchronizer);
                        _synchronizer = IntPtr.Zero;
                    }
                }
            }
            return;
        }

        public virtual void Terminate()
        {
            return;
        }

        /// <summary>Trace an exception and the inner ones in our private log file.</summary>
        /// <param name="e">Main exception.</param>
        protected void TraceException(Exception e)
        {
            try {
                using(StreamWriter writer = GetPrivateLogWriter()) {
                    while(null != e) {
                        writer.WriteLine(e.Message);
                        writer.WriteLine(e.StackTrace);
                        e = e.InnerException;
                    }
                }
            } catch { }
            return;
        }

        /// <summary>Write a debug message to our very own log file.</summary>
        /// <param name="pattern">Message pattern</param>
        /// <param name="parameters">Placeholder replacement parameters.</param>
        protected void WriteDebugMessage(string pattern, params object[] parameters)
        {
            using(StreamWriter writer = GetPrivateLogWriter()) {
                writer.WriteLine(pattern, parameters);
            }
            return;
        }
        #endregion

        #region FIELDS
        // Plugin doesn't want to be loaded
        public const int PLUGIN_SKIP = 0;
        // Plugin agrees to work with the current database; It will be loaded as soon as the user presses the hotkey
        public const int PLUGIN_OK = 1;
        // Plugin agrees to work with the current database and wants to stay in the memory
        public const int PLUGIN_KEEP = 2;

        /// <summary>Path of a private file used to debug tricky cases.</summary>
        private string _privateLogFile = null;
        /// <summary>The native event that is intended to signal plugin completion. This event is
        /// probably useless since we managed to have a single mixed assembly.
        /// TODO : Consider removing the event.</summary>
        private IntPtr _synchronizer;
        #endregion

        #region INTEROP
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        internal static extern bool CloseHandle([In] IntPtr handle);

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        internal static extern IntPtr CreateEvent([In] IntPtr securityAttributes, [In] bool manualReset, [In] bool initialState, [In] string name);

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        internal static extern IntPtr SetEvent([In] IntPtr hEvent);
        #endregion
    }
}
