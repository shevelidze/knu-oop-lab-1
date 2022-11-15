using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class ExpressionExecutor
    {
        public ExpressionExecutor(
            string expression,
            Dictionary<string, string> cellsExpressions,
            Dictionary<string, ICellValue> calculatedValues
        )
        {
            _expression = expression;
            _cellsExpressions = cellsExpressions;
        }

        public ICellValue Execute()
        {
            return new CellNumberValue(777);
        }

        private string _expression;
        private Dictionary<string, string> _cellsExpressions;
    }
}
