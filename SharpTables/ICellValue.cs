using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public interface ICellValue
    {
        public string ToString();
        public CellStringValue ToCellStringValue();
        public CellNumberValue ToCellNumberValue();
        public CellBooleanValue ToCellBooleanValue();
    }
}
