using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public delegate ICellValue TableFunctionExecutor(List<ICellValue> arguments);

    class TableFunction
    {
        public TableFunction(string name, TableFunctionExecutor executor, int argumentsNumber = -1)
        {
            _executor = executor;
            this.Name = name;
            _argumentsNumber = argumentsNumber;
        }

        public ICellValue Execute(List<ICellValue> arguments)
        {
            if (_argumentsNumber >= 0 && arguments.Count != _argumentsNumber)
            {
                throw new ExecutorException(String.Format(
                    "Function {0} accepts {1} arguments, but got {2}.",
                    this.Name,
                    _argumentsNumber,
                    arguments.Count
                    ));
            }

            return _executor(arguments);
        }

        public string Name { get; private set; }

        private TableFunctionExecutor _executor;
        private int _argumentsNumber;
    }
}
