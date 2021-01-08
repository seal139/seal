using System;

namespace seal.Helper
{
    class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}
