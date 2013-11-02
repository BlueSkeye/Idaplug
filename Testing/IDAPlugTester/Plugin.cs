using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IdaNet;

namespace IDAPlugTester
{
    public class Plugin : PluginBase
    {
        #region METHODS
        protected override void DoRun()
        {
            bridge = BridgeToNative.get_Singleton();
            try {
                FileInfo notepadDatabase;

                CreateNotepadDatabase(out notepadDatabase);
            } catch(Exception e) {
                WriteDebugMessage(".Net plugin exception : {0} at {1}.", e.Message, e.StackTrace);
            }
            return;
        }

        /// <summary>Create a new database using NOTEPAD.EXE and saves it under %LOCALAPPDATA%\idaplug</summary>
        /// <param name="savedDatabase">On return this parameter is updated with a descriptor of the saved
        /// database.</param>
        private void CreateNotepadDatabase(out FileInfo savedDatabase)
        {
            string inputFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "NOTEPAD.EXE");
            FileInfo inputFile = new FileInfo(inputFileName);

            if (!inputFile.Exists)
            {
                throw new ApplicationException(string.Format("File doesn't exists : '{0}'.", inputFile.FullName));
            }
            LoadFlags flags = LoadFlags.LoadAllSegmentsSilently | LoadFlags.CreateSegments | LoadFlags.CreateImportSegment;
            IdaDatabase database = IdaDatabase.Create(inputFile.FullName, flags);
            bridge.DisplayMessage(string.Format("Database successfully created for file '{0}'.", inputFileName));
            string outputFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"idaplug\NOTEPAD.IDB");
            savedDatabase = new FileInfo(outputFileName);
            if (savedDatabase.Exists) { savedDatabase.Delete(); }
            database.Save(savedDatabase.FullName, DatabaseFlags.CompressDatabase);
            bridge.DisplayMessage(string.Format("Database successfully saved to file '{0}'.", outputFileName));
            return;
        }
        #endregion

        #region FIELDS
        private BridgeToNative bridge;
        #endregion
    }
}
