using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Filters
{
    public class SessionTimeoutException : Exception
    {
        public SessionTimeoutException() : base() { }
        public SessionTimeoutException(string message) : base(message) { }
    }
}