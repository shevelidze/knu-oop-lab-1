using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    enum TokenType
    {
        NumberValue,
        BooleanValue,
        StringValue,
        Operator,
        CellReference,
        FunctionName
    }

    class Token
    {
        Token(string exrpession, int tokenBegin, int tokenEnd, TokenType type)
        {
            this.Expression = exrpession;
            this.TokenBegin = tokenBegin;
            this.TokenEnd = tokenEnd;
            this.TokenType = type;
        }
        public string Expression { get; private set; }
        public string Value {
            get
            {
                return this.Expression.Substring(TokenBegin, TokenEnd - TokenBegin);
            }
        }
        public int TokenBegin { get; private set; }
        public int TokenEnd { get; private set; }
        public TokenType TokenType { get; private set; }
    }
}
