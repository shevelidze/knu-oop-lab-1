using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class TokenizerException : Exception
    {
        TokenizerException(string message): base(message)
        {
        }
    }
}
