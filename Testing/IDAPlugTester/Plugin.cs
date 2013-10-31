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
            BridgeToNative bridge = BridgeToNative.get_Singleton();

            try {
                // YOUR CODE TO BE WRITEN HERE;
            } catch(Exception e) {
                WriteDebugMessage(".Net plugin exception : {0} at {1}.", e.Message, e.StackTrace);
            }
            return;
        }
        #endregion
    }
}
