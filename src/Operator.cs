using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTables
{
    public delegate ICellValue BinaryOperatorExecutor(ICellValue left, ICellValue right);
    public delegate ICellValue UnaryOperatorExecutor(ICellValue operand);

    class Operator
    {
        public Operator(string text, BinaryOperatorExecutor binaryExecutor, UnaryOperatorExecutor unaryExecutor)
        {
            this.Text = text;
            _binaryExecutor = binaryExecutor;
            _unaryExecutor = unaryExecutor;
        }

        public string Text { get; private set; }

        public ICellValue Execute(ICellValue operand)
        {
            if (_unaryExecutor == null)
            {
                throw new ExecutorException(String.Format("Operator {0} can\'t be unary.", this.Text));
            }

            return _unaryExecutor(operand);
        }

        public ICellValue Execute(ICellValue left, ICellValue right)
        {
            if (_binaryExecutor == null)
            {
                throw new ExecutorException(String.Format("Operator {0} can\'t be binary.", this.Text));
            }

            return _binaryExecutor(left, right);
        }

        public bool IsUnary {
            get
            {
                return _unaryExecutor != null;
            }
        }

        public bool IsBinary
        {
            get
            {
                return _binaryExecutor != null;
            }
        }

        private BinaryOperatorExecutor _binaryExecutor;
        private UnaryOperatorExecutor _unaryExecutor;
    }
}
