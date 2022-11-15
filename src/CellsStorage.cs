using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class CellsStorage
    {
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
                Int32.Parse(cellId.Substring(currentIndex)),
                CellsStorage.StringToColumnIndex(columnId)
            );
        }

        public Cell GetCell(string id)
        {
            return this.cells[id];
        }

        public Cell SetCellExpression(string cellId, string expression)
        {
            if (this.cells.ContainsKey(cellId))
            {
                return this.cells[cellId];
            }
            else
            {
                var newCell = new Cell(expression);
                this.cells.Add(cellId, newCell);

                return newCell;
            }
        }

        private Dictionary<string, Cell> cells;
    }
}
