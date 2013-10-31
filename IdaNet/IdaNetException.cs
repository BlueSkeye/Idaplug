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

        internal IdaNetException(string message, Exception innerException)
            : base(message, innerException)
        {
            return;
        }
        #endregion
    }
}