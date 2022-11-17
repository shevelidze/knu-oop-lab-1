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
            var tokens = this.tokenize();

            return new CellNumberValue(777);
        }

        private List<Token> tokenize()
        {
            var tokens = new List<Token>();
            var currentIndex = 0;

            while (currentIndex < _expression.Length)
            {
                if (Char.IsWhiteSpace(_expression[currentIndex]))
                {
                    currentIndex++;
                    continue;
                }

                if (Char.IsDigit(_expression[currentIndex]))
                {
                    tokens.Add(_readNumber(currentIndex));
                }
                else if (_expression[currentIndex] == '"')
                {
                    tokens.Add(_readString(currentIndex));
                }
                else if (_expression[currentIndex] == '$') {
                    tokens.Add(_readCellReference(currentIndex));
                }
                else
                {
                    var tokenRead = false;

                    try
                    {
                        tokens.Add(_readBoolean(currentIndex));
                        tokenRead = true;
                    }
                    catch (TokenizerException) { 
                    }

                    if (!tokenRead)
                    {
                        try
                        {
                            tokens.Add(_readOperator(currentIndex));
                            tokenRead = true;
                        }
                        catch (TokenizerException) { }
                    }


                    if (!tokenRead)
                    {
                        tokens.Add(_readFunctionName(currentIndex));
                    }
                }

                currentIndex = tokens[tokens.Count - 1].TokenEnd;
            }

            return tokens;
        }

        private Token _readOperator(int startIndex)
        {
            // The operators must be sorted by their length
            string[] validOperators = { "==", "!=", "**", ">=", "<=", ">", "<",  "+", "-", "*", "/", "(", ")"};

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

        private Token _readFunctionName(int startIndex)
        {
            if (!Char.IsLetter(_expression[startIndex]))
            {
                throw new TokenizerException("Expected a letter on the begin of the function name.");
            }

            var tokenEnd = startIndex + 1;

            while (tokenEnd < _expression.Length && Char.IsLetterOrDigit(_expression[tokenEnd]))
            {
                tokenEnd++;
            }

            return new Token(_expression, startIndex, tokenEnd, TokenType.FunctionName);
        }

        private Token _readBoolean(int startIndex)
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

        private Token _readNumber(int startIndex)
        {
            if (!Char.IsDigit(_expression[startIndex]))
            {
                throw new TokenizerException("Expected a digit on the begin of the number");
            }

            var tokenEnd = startIndex;

            while (
                tokenEnd < _expression.Length &&
                (Char.IsDigit(_expression[tokenEnd]) || _expression[tokenEnd] == '.')
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

        private Token _readCellReference(int startIndex)
        {
            if (_expression[startIndex] != '$')
            {
                throw new TokenizerException("Expected $ on the begin of the cell refernce.");
            }

            var tokenEnd = startIndex + 1;

            while (tokenEnd < _expression.Length && !Char.IsDigit(_expression[tokenEnd]))
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

            while (tokenEnd < _expression.Length && Char.IsDigit(_expression[tokenEnd]))
            {
                tokenEnd++;
            }

            return new Token(_expression, startIndex, tokenEnd, TokenType.CellReference);
        }

        private Token _readString(int startIndex)
        {
            if (_expression[startIndex] != '\"')
            {
                throw new TokenizerException("Expected \" on the begin of the string.");
            }

            var tokenEnd = startIndex + 1;

            while(tokenEnd < _expression.Length && _expression[tokenEnd] != '\"')
            {
                tokenEnd++;
            }

            if (tokenEnd >= _expression.Length || _expression[tokenEnd] != '\"')
            {
                throw new TokenizerException("Expected \" on the end of the string.");
            }

            return new Token(_expression, startIndex, tokenEnd + 1, TokenType.StringValue);
        }

        private string _expression;
        private Dictionary<string, string> _cellsExpressions;
    }
}
