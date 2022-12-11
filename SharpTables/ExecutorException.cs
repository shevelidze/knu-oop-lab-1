using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class ExecutorException : Exception
    {
        public ExecutorException(string message): base(message)
        {
        }

    }
}
