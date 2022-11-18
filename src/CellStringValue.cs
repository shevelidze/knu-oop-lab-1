using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public class CellStringValue : ICellValue
    {
        public CellStringValue(string value)
        {
            this.Value = value;
        }

        public string Value;
        public override string ToString()
        {
            return this.Value;
        }

        CellBooleanValue ICellValue.ToCellBooleanValue()
        {
            return new CellBooleanValue(this.Value.Length > 0);
        }

        CellNumberValue ICellValue.ToCellNumberValue()
        {
            return new CellNumberValue(this.Value.Length);
        }

        CellStringValue ICellValue.ToCellStringValue()
        {
            return this;
        }
    }
}
