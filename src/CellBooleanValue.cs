using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public class CellBooleanValue: ICellValue
    {
        public CellBooleanValue(bool value)
        {
            this.Value = value;
        }

        public bool Value;
        public override string ToString()
        {
            return this.Value.ToString();
        }

        CellBooleanValue ICellValue.ToCellBooleanValue()
        {
            return this;
        }

        CellNumberValue ICellValue.ToCellNumberValue()
        {
            return new CellNumberValue(this.Value ? 1 : 0);
        }

        CellStringValue ICellValue.ToCellStringValue()
        {
            return new CellStringValue(this.ToString());
        }
    }
}
