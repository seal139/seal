﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}