using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    class ExpressionExecutor
    {
        public ExpressionExecutor(
            string expression,
            Dictionary<string, string> cellsExpressions,
            Dictionary<string, ICellValue> calculatedValues
        )
        {
            _expression = expression;
            _cellsExpressions = cellsExpressions;
        }

        public ICellValue Execute()
        {
            return new CellNumberValue(777);
        }

        private List<Token> tokenize()
        {
            return new List<Token>();
        }

        private Token readOperator(int startIndex)
        {
            // The operators must be sorted by their length
            string[] validOperators = { "==", "!=", "**", ">=", "<=", ">", "<",  "+", "-", "*", "/"};

            foreach (var validOperator in validOperators)
            {
                try
                {
                    if (_expression.Substring(startIndex, validOperator.Length) == validOperator)
                    {
                        return new Token(_expression, startIndex, startIndex + validOperator.Length, TokenType.Operator);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    continue;
                }
            }

            throw new TokenizerException(
                String.Format("Invalid operator {0} on a position {1}.", _expression[startIndex], startIndex)
                );
        }

        private Token readFunctionName(int startIndex)
        {
            if (!Char.IsLetter(_expression[startIndex]))
            {
                throw new TokenizerException("Expected a letter on the begin of the function name.");
            }

            var tokenEnd = startIndex + 1;

            while (Char.IsLetterOrDigit(_expression[tokenEnd]) && tokenEnd < _expression.Length)
            {
                tokenEnd++;
            }

            return new Token(_expression, startIndex, tokenEnd, TokenType.FunctionName);
        }

        private Token readBoolean(int startIndex)
        {
            const string errorMessage = "Invalid boolen value. Expected \"True\" or \"False\".";

            try
            {
                if (_expression.Substring(startIndex, 4) == "True")
                {
                    return new Token(_expression, startIndex, startIndex + 4, TokenType.BooleanValue);
                }
                else if (_expression.Substring(startIndex, 5) == "False")
                {
                    return new Token(_expression, startIndex, startIndex + 5, TokenType.BooleanValue);
                }
                else
                {
                    throw new TokenizerException(errorMessage);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new TokenizerException(errorMessage);
            }
        }

        private Token readNumber(int startIndex)
        {
            if (!Char.IsDigit(_expression[startIndex]))
            {
                throw new TokenizerException("Expected a digit on the begin of the number");
            }

            var tokenEnd = startIndex;

            while (
                (Char.IsDigit(_expression[tokenEnd]) || _expression[tokenEnd] == '.') &&
                tokenEnd < _expression.Length
                )
            {
                tokenEnd++;
            }

            if (_expression[tokenEnd - 1] == '.')
            {
                throw new TokenizerException("Expected a digit on the end of the number");
            }

            return new Token(_expression, startIndex, tokenEnd, TokenType.NumberValue);
        }

        private Token readCellReference(int startIndex)
        {
            if (_expression[startIndex] != '$')
            {
                throw new TokenizerException("Expected $ on the begin of the cell refernce.");
            }

            var tokenEnd = startIndex + 1;

            while (!Char.IsDigit(_expression[tokenEnd]) && tokenEnd < _expression.Length)
            {
                if ((int)_expression[tokenEnd] < 65 || (int)_expression[tokenEnd] > 90)
                {
                    throw new TokenizerException("Invalid column index. Expected a combination of A-Z letters.");
                }
                tokenEnd++;
            }

            if (tokenEnd == startIndex + 1)
            {
                throw new TokenizerException("Expected a column index before the row index.");
            }

            if (!Char.IsDigit(_expression[tokenEnd]))
            {
                throw new TokenizerException("Expeced a row index after the column index.");
            }

            while (Char.IsDigit(_expression[tokenEnd]) && tokenEnd < _expression.Length)
            {
                tokenEnd++;
            }

            return new Token(_expression, startIndex + 1, tokenEnd, TokenType.CellReference);
        }

        private Token readString(int startIndex)
        {
            if (_expression[startIndex] != '\"')
            {
                throw new TokenizerException("Expected \" on the begin of the string.");
            }

            var tokenEnd = startIndex + 1;

            while(_expression[tokenEnd] != '\"')
            {
                tokenEnd++;
                if (tokenEnd >= _expression.Length)
                {
                    throw new TokenizerException("Expected \" on the end of the string.");
                }
            }

            return new Token(_expression, startIndex + 1, tokenEnd - 1, TokenType.StringValue);
        }

        private string _expression;
        private Dictionary<string, string> _cellsExpressions;
    }
}
