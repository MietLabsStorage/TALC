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

        public EMarkException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}
