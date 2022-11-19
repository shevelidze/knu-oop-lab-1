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
            Dictionary<string, ICellValue> calculatedValues,
            int stackSize
        )
        {
            _expression = expression;
            _cellsExpressions = cellsExpressions;
            _stackSize = stackSize;
            _calculatedValues = calculatedValues;
        }

        public ICellValue Execute()
        {
            _tokenize();
            return _executeExpression(0, 0).Item1;
        }

        // sorted in the priority order
        public static Operator[] Operators = {
            new Operator("||", (left, right) => new CellBooleanValue(
                left.ToCellBooleanValue().Value || right.ToCellBooleanValue().Value
                ), null),
            new Operator("&&", (left, right) => new CellBooleanValue(
                left.ToCellBooleanValue().Value && right.ToCellBooleanValue().Value
                ), null),
            new Operator("^", (left, right) => new CellBooleanValue(
                left.ToCellBooleanValue().Value ^ right.ToCellBooleanValue().Value
                ), null),
            new Operator("==", (left, right) => new CellBooleanValue(
                left.ToCellStringValue().Value == right.ToCellStringValue().Value
                ), null),
            new Operator("<=", (left, right) => new CellBooleanValue(
                left.ToCellNumberValue().Value <= right.ToCellNumberValue().Value),
                null),
            new Operator(">=", (left, right) => new CellBooleanValue(
                left.ToCellNumberValue().Value >= right.ToCellNumberValue().Value),
                null),
            new Operator("<", (left, right) => new CellBooleanValue(
                left.ToCellNumberValue().Value < right.ToCellNumberValue().Value),
                null),
            new Operator(">", (left, right) => new CellBooleanValue(
                left.ToCellNumberValue().Value > right.ToCellNumberValue().Value),
                null),
            new Operator("-",
                (left, right) => new CellNumberValue(
                    left.ToCellNumberValue().Value - right.ToCellNumberValue().Value),
                null),
            new Operator("+", (left, right) => {
                if (left is CellStringValue || right is CellStringValue)
                {
                    return new CellStringValue(left.ToCellStringValue().Value + right.ToCellStringValue().Value);
                }

                return new CellNumberValue(
                    left.ToCellNumberValue().Value + right.ToCellNumberValue().Value);
                },
                null),
            new Operator("/", (left, right) => new CellNumberValue(
                left.ToCellNumberValue().Value / right.ToCellNumberValue().Value),
                null),
            new Operator("*", (left, right) => new CellNumberValue(
                left.ToCellNumberValue().Value * right.ToCellNumberValue().Value),
                null),
            new Operator("%", (left, right) => new CellNumberValue(
                left.ToCellNumberValue().Value % right.ToCellNumberValue().Value),
                null),
            new Operator("**", (left, right) => new CellNumberValue(
                Math.Pow(left.ToCellNumberValue().Value, right.ToCellNumberValue().Value)),
                null),
            new Operator("-",
                null,
                (operand) => new CellNumberValue(-operand.ToCellNumberValue().Value)),
            new Operator("!", null, (operand) => new CellBooleanValue(!(operand.ToCellBooleanValue()).Value)),
        };

        public static TableFunction[] Functions =
        {
            new TableFunction("now", (arguments) =>
            {
                return new CellStringValue(DateTime.Now.ToString());
            })
        };

        private ICellValue _resolveCellReference(string cellId)
        {
            if (_stackSize > 20)
            {
                throw new ExecutorException("Circular cell reference detected.");
            }

            if (!_cellsExpressions.ContainsKey(cellId))
            {
                throw new ExecutorException("Reference to the non-existing cell.");
            }

            if (_calculatedValues.ContainsKey(cellId))
            {
                return _calculatedValues[cellId];
            }

            var executor = new ExpressionExecutor(
                _cellsExpressions[cellId],
                _cellsExpressions,
                _calculatedValues,
                _stackSize + 1
            );

            var referenceValue = executor.Execute();
            _calculatedValues[cellId] = referenceValue;

            return referenceValue;
        }

        private Tuple<ICellValue, int> _executeExpression(
            int tokenIndex,
            int minPriority,
            bool allowClosingBracket = false,
            bool allowComma = false
            )
        {
            if (tokenIndex >= _tokens.Count)
            {
                return new Tuple<ICellValue, int>(new CellNumberValue(0), tokenIndex);
            }

            if (minPriority >= ExpressionExecutor.Operators.Length)
            {
                var currentToken = _tokens[tokenIndex];
                ICellValue value;

                if (currentToken.TokenType == TokenType.NumberValue)
                {
                    value = new CellNumberValue(Double.Parse(currentToken.Value));
                }
                else if (currentToken.TokenType == TokenType.BooleanValue)
                {
                    value = new CellBooleanValue(currentToken.Value == "True");
                }
                else if (currentToken.TokenType == TokenType.StringValue)
                {
                    value = new CellStringValue(currentToken.Value.Substring(1, currentToken.Value.Length - 2));
                }
                else if (currentToken.TokenType == TokenType.CellReference)
                {
                    value = _resolveCellReference(currentToken.Value.Substring(1));
                }
                else
                {
                    throw new ExecutorException("Expected a value expression.");
                }

                return new Tuple<ICellValue, int>(value, tokenIndex + 1);
            }

            ICellValue leftValue;
            int nextTokenIndex;

            if (_tokens[tokenIndex].Value == "(")
            {
                (leftValue, nextTokenIndex) = _executeExpression(tokenIndex + 1, 0, true);

                if (_tokens.Count <= nextTokenIndex || _tokens[nextTokenIndex].Value != ")")
                {
                    throw new ExecutorException("Expected ) after (.");
                }

                nextTokenIndex++;
            }
            else if (_tokens[tokenIndex].TokenType == TokenType.Operator)
            {
                var operatorObject = _findUnaryOperator(_tokens[tokenIndex].Value);
                var operatorIndex = Array.FindIndex(
                        ExpressionExecutor.Operators,
                        possibleOperator => possibleOperator == operatorObject
                    );
                ICellValue operand;
                (operand, nextTokenIndex) = _executeExpression(tokenIndex + 1, operatorIndex, allowClosingBracket, allowComma);
                leftValue = operatorObject.Execute(operand);
            }
            else if (_tokens[tokenIndex].TokenType == TokenType.FunctionName)
            {
                (leftValue, nextTokenIndex) = _executeFunctionExpression(tokenIndex);
            }
            else
            {
                (leftValue, nextTokenIndex) = _executeExpression(
                    tokenIndex,
                    minPriority + 1,
                    allowClosingBracket,
                    allowComma);
            }

            while (nextTokenIndex < _tokens.Count)
            {
                if (_tokens[nextTokenIndex].TokenType != TokenType.Operator)
                {
                    throw new ExecutorException("Expected an operator after the value.");
                }

                var operatorToken = _tokens[nextTokenIndex];

                var operatorIndex = Array.FindIndex(
                        ExpressionExecutor.Operators,
                        operatorValue => operatorToken.Value == operatorValue.Text
                    );

                if (
                    allowClosingBracket && operatorToken.Value == ")" ||
                    allowComma && operatorToken.Value == "," ||
                    operatorIndex >= 0 && operatorIndex < minPriority
                    )
                {
                    break;
                }

                var operatorObject = _findBinaryOperator(operatorToken.Value);

                if (nextTokenIndex == _tokens.Count - 1)
                {
                    throw new ExecutorException("Expected a value after the operator.");
                }


                var (rightValue, newNextTokenIndex) = _executeExpression(
                    nextTokenIndex + 1,
                    minPriority + 1,
                    allowClosingBracket,
                    allowComma
                );

                leftValue = operatorObject.Execute(leftValue, rightValue);

                nextTokenIndex = newNextTokenIndex;
            }

            return new Tuple<ICellValue, int>(leftValue, nextTokenIndex);
        }

        private Tuple<ICellValue, int> _executeFunctionExpression(int tokenIndex)
        {
            var function = _findFunction(_tokens[tokenIndex].Value);

            tokenIndex++;

            if (tokenIndex >= _tokens.Count || _tokens[tokenIndex].Value != "(")
            {
                throw new ExecutorException("Expected ( after the function name.");
            }

            tokenIndex++;

            var arguments = new List<ICellValue>();

            while (tokenIndex < _tokens.Count && _tokens[tokenIndex].Value != ")")
            {
                var (value, nextTokenIndex) = _executeExpression(tokenIndex, 0, true, true);

                arguments.Add(value);

                tokenIndex = nextTokenIndex;

                var token = _tokens[tokenIndex];

                if (token.Value == ",")
                {
                    tokenIndex++;
                }
                else if (token.Value != ")")
                {
                    throw new ExecutorException(String.Format("Expected , or ), but got {0}.", token.Value));
                }
            }
            
            if (tokenIndex >= _tokens.Count || _tokens[tokenIndex].Value != ")")
            {
                throw new ExecutorException("Expected ) in the end of the function call.");
            }

            return new Tuple<ICellValue, int>(function.Execute(arguments), tokenIndex + 1);
        }

        private TableFunction _findFunction(string functionName)
        {
            var function = Array.Find(
                ExpressionExecutor.Functions,
                function => function.Name == functionName
                );
            
            if (function == null)
            {
                throw new ExecutorException(String.Format("Unknown function {0}", functionName));
            }

            return function;

        }

        private List<Token> _tokenize()
        {
            _tokens = new List<Token>();
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
                    _tokens.Add(_readNumber(currentIndex));
                }
                else if (_expression[currentIndex] == '"')
                {
                    _tokens.Add(_readString(currentIndex));
                }
                else if (_expression[currentIndex] == '$') {
                    _tokens.Add(_readCellReference(currentIndex));
                }
                else
                {
                    var tokenRead = false;

                    try
                    {
                        _tokens.Add(_readBoolean(currentIndex));
                        tokenRead = true;
                    }
                    catch (TokenizerException) { 
                    }

                    if (!tokenRead)
                    {
                        try
                        {
                            _tokens.Add(_readOperator(currentIndex));
                            tokenRead = true;
                        }
                        catch (TokenizerException) { }
                    }


                    if (!tokenRead)
                    {
                        _tokens.Add(_readFunctionName(currentIndex));
                    }
                }

                currentIndex = _tokens[_tokens.Count - 1].TokenEnd;
            }

            return _tokens;
        }

        private Operator _findBinaryOperator(string operatorText)
        {
            var operatorObject = Array.Find(
                ExpressionExecutor.Operators,
                operatorObject => operatorObject.Text == operatorText && operatorObject.IsBinary
                );
            
            if (operatorObject == null)
            {
                throw new ExecutorException(String.Format("Unknown binary operator {0}", operatorText));
            }

            return operatorObject;
        }
        private Operator _findUnaryOperator(string operatorText)
        {
            var operatorObject = Array.Find(
                ExpressionExecutor.Operators,
                operatorObject => operatorObject.Text == operatorText && operatorObject.IsUnary
                );
            
            if (operatorObject == null)
            {
                throw new ExecutorException(String.Format("Unknown unary operator {0}", operatorText));
            }

            return operatorObject;
        }


        private Token _readOperator(int startIndex)
        {
            string[] validOperators = new string[ExpressionExecutor.Operators.Length + 3];

            for (var operatorIndex = 0; operatorIndex < ExpressionExecutor.Operators.Length; operatorIndex++)
            {
                validOperators[operatorIndex] = ExpressionExecutor.Operators[operatorIndex].Text;
            }

            validOperators[ExpressionExecutor.Operators.Length] = "(";
            validOperators[ExpressionExecutor.Operators.Length + 1] = ")";
            validOperators[ExpressionExecutor.Operators.Length + 2] = ",";

            Array.Sort(validOperators, (left, right) => right.Length - left.Length);

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

            if (tokenEnd >= _expression.Length || !Char.IsDigit(_expression[tokenEnd]))
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
        private Dictionary<string, ICellValue> _calculatedValues;
        private List<Token> _tokens;
        private int _stackSize;
    }
}
