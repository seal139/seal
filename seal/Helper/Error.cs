using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    /// <summary>
    /// Error container
    /// </summary>
    public sealed class Error
    {
        private static Error instance;
        private static bool initialized = false;
        //Get instance of Error Class
        public static Error GetInstance()
        {
            if (!initialized)
            {
                instance = new Error();
                initialized = true;
            }

            return instance;
        }

        private List<string> msg;
        private bool err;
        private Error() 
        {
            msg = new List<string>();
            err = false;
        }

        /// <summary>
        /// Get error status
        /// </summary>
        public bool HasError
        {
            get
            {
                return err;
            }
        }
        
        /// <summary>
        /// Clear current error message <br/>
        /// This method should be called after every transaction is finished
        /// </summary>
        public void Clear()
        {
            msg.Clear();
            err = false;
        }

        /// <summary>
        /// Get error message list
        /// </summary>
        public List<string> MessageList
        {
            get
            {
                return msg;
            }
        }

        /// <summary>
        /// Add error message
        /// </summary>
        /// <param name="message"></param>
        public void Append(string message)
        {
            msg.Add(message);
            err = true;
        }
    }
}
