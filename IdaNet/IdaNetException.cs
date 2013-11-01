using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdaNet
{
    public class IdaNetException : ApplicationException
    {
        #region CONSTRUCTORS
        internal IdaNetException()
        {
            return;
        }

        internal IdaNetException(string message)
            : base(message)
        {
            return;
        }

        internal IdaNetException(string message, params object[] parameters)
            : base(FormatMessage(message, parameters))
        {
            return;
        }

        internal IdaNetException(string message, Exception innerException)
            : base(message, innerException)
        {
            return;
        }

        internal IdaNetException(string message, Exception innerException, params object[] parameters)
            : base(FormatMessage(message, parameters), innerException)
        {
            return;
        }
        #endregion

        #region METHODS
        private static string FormatMessage(string message, params object[] parameters)
        {
            try { return string.Format(message, parameters); }
            catch { return message ?? "NO MESSAGE"; }
        }
        #endregion
    }
}