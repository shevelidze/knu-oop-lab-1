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
        public TableFunction(string name, TableFunctionExecutor executor)
        {
            _executor = executor;
            this.Name = name;
        }

        public ICellValue Execute(List<ICellValue> arguments)
        {
            return _executor(arguments);
        }

        public string Name { get; private set; }

        private TableFunctionExecutor _executor;
    }
}
