using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class CellNumberValue: ICellValue
    {
        CellNumberValue(double value)
        {
            this.Value = value;
        }

        public double Value;
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
