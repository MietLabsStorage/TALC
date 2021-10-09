using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    class EMarkException: Exception
    {
        private string _message;

        public EMarkException(string element, string message = "")
        {
            _message = string.IsNullOrEmpty(message) ? $"Error in emark in element {element}" : message;
        }

        public override string Message => _message;
    }
}
