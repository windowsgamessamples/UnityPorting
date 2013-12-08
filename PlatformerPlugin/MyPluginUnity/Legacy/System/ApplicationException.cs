using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LegacySystem
{

    public class ApplicationException : Exception
    {
        private Exception _ex;

        // Summary:
        //     Initializes a new instance of the System.ApplicationException class.
        public ApplicationException()
        {
            _ex = new Exception();
        }

        public override Exception GetBaseException()
        {
            return _ex.GetBaseException();
        }

        public override string HelpLink
        {
            get
            {
                return _ex.HelpLink;
            }
            set
            {
                _ex.HelpLink = value;
            }
        }

        public override string Source
        {
            get
            {
                return _ex.Source;
            }
            set
            {
                _ex.Source = value;
            }
        }

        public override string StackTrace
        {
            get
            {
                return _ex.StackTrace;
            }
        }

        public override System.Collections.IDictionary Data
        {
            get
            {
                return _ex.Data;
            }
        }

        public override string Message
        {
            get
            {
                return _ex.Message;
            }
        }
       
        public ApplicationException(string message)
        {
            _ex = new Exception(message);
        }
      
        public ApplicationException(string message, Exception innerException)
        {
            _ex = new Exception(message, innerException);
        }
    }
}
