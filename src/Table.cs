using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class Table
    {
        public Table()
        {
            _cellsExpressions = new Dictionary<string, string>();
        }

        public Dictionary<string, ICellValue> SetCellExpression(int rowIndex, int columnIndex, string expression)
        {
            if (expression.Length == 0)
            {
                this._cellsExpressions.Remove(Table.IndexesToCellId(rowIndex, columnIndex));
            }
            else
            {
                this._cellsExpressions[Table.IndexesToCellId(rowIndex, columnIndex)] = expression;
            }

            return this.Calculate();
        }

        public string GetCellExpression(int rowIndex, int columnIndex)
        {
            var cellId = Table.IndexesToCellId(rowIndex, columnIndex);
            
            return _cellsExpressions.ContainsKey(cellId) ? _cellsExpressions[cellId] : "";
        }

        public Dictionary<string, ICellValue> Calculate()
        {
            Dictionary<string, ICellValue> calculatedValues = new Dictionary<string, ICellValue>();

            foreach (var entry in _cellsExpressions)
            {
                if (!calculatedValues.ContainsKey(entry.Key))
                {
                    ExpressionExecutor executor = new ExpressionExecutor(
                        entry.Value,
                        this._cellsExpressions,
                        calculatedValues,
                        0
                    );
                    calculatedValues.Add(entry.Key, executor.Execute());
                }
            }

            return calculatedValues;
        }
        
        public static string ColumnIndexToString(int columnIndex)
        {

            if (columnIndex >= 52)
            {
                var result = "";

                columnIndex -= 26;

                while (columnIndex > 0)
                {
                    result = (char)(65 + columnIndex % 26) + result;
                    columnIndex /= 26;
                }

                return result;
            }
            else if (columnIndex >= 26)
            {
                return "A" + (char)(65 + columnIndex - 26);
            }
            else
            {
                return "" + (char)(65 + columnIndex);
            }
        }

        public static int StringToColumnIndex(string columnIndexString)
        {
            if (columnIndexString.Length == 1)
            {
                return (int)(columnIndexString[0]) - 65;
            }
            else if (columnIndexString.Length == 2 && columnIndexString[0] == 'A')
            {
                return (int)(columnIndexString[1]) - 65 + 26;
            }
            else
            {
                var result = 26;

                for (var i = 0; i < columnIndexString.Length; i++)
                {
                    result += (int)Math.Pow(26, i) * ((int)(columnIndexString[columnIndexString.Length - i - 1]) - 65);
                }

                return result;
            }
        }

        public static Tuple<int, int> ParseCellId(string cellId)
        {
            var currentIndex = 0;
            var columnId = "";

            while (Char.IsLetter(cellId[currentIndex]))
            {
                columnId += Char.ToUpper(cellId[currentIndex]);
                currentIndex++;
            }

            return new Tuple<int, int>(
                Int32.Parse(cellId.Substring(currentIndex)) - 1,
                Table.StringToColumnIndex(columnId)
            );
        }

        public static string IndexesToCellId(int rowIndex, int columnIndex)
        {
            return Table.ColumnIndexToString(columnIndex) + (rowIndex + 1).ToString();
        }

        private Dictionary<string, string> _cellsExpressions;
    }
}
