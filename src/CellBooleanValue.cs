﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class CellBooleanValue: ICellValue
    {
        CellBooleanValue(bool value)
        {
            this.Value = value;
        }

        public bool Value;
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
