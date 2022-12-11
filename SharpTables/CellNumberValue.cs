using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public class CellNumberValue: ICellValue
    {
        public CellNumberValue(double value)
        {
            this.Value = value;
        }

        public double Value;
        public override string ToString()
        {
            return this.Value.ToString();
        }
        CellBooleanValue ICellValue.ToCellBooleanValue()
        {
            return new CellBooleanValue(this.Value != 0);
        }

        CellNumberValue ICellValue.ToCellNumberValue()
        {
            return this;
        }

        CellStringValue ICellValue.ToCellStringValue()
        {
            return new CellStringValue(this.ToString());
        }
    }
}
