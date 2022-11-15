using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class CellStringValue : ICellValue
    {
        CellStringValue(string value)
        {
            this.Value = value;
        }

        public string Value;
        public override string ToString()
        {
            return this.Value;
        }
    }
}
