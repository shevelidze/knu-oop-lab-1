using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SharpTables
{
    public delegate void CalculationErrorHandler(string message);

    class TableController
    {
        public TableController(Table table, CalculationErrorHandler calculationErrorHandler = null)
        {
            _table = table;
            _calculationErrorHandler = calculationErrorHandler;

            this.Form = new TableForm(
                (rowIndex, columnIndex, text, form) =>
                {
                    try
                    {
                        _updateCells(_table.SetCellExpression(rowIndex, columnIndex, text));
                    }
                    catch (Exception e)
                    {
                        if (_calculationErrorHandler != null && (e is TokenizerException || e is ExecutorException))
                        {
                            _calculationErrorHandler((e is TokenizerException ? "Tokenizer exception: " :
                                "Executor exception: ") + e.Message);
                            form.SetCellText(rowIndex, columnIndex, _table.GetCellExpression(rowIndex, columnIndex));
                        }
                    }
                },
                (rowIndex, columnIndex, form) =>
                {
                    form.SetCellText(rowIndex, columnIndex, _table.GetCellExpression(rowIndex, columnIndex));
                },
                form =>
                {
                    _table.Undo();
                    _updateCells();
                },
                form =>
                {
                    _table.Redo();
                    _updateCells();
                },
                form => { },
                form => { },
                form => { },
                form => {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = "SharpTables table (*.stt)|*.stt|All files (*.*)|*.*";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _table.SaveToFile(Path.GetFullPath(saveFileDialog.FileName));
                    }
                }
            );
        }

        private void _updateCells(Dictionary<string, ICellValue> values = null)
        {
            if (values == null)
            {
                values = _table.Calculate();
            }
            
            this.Form.Clear();
            
            foreach (var entry in values)
            {
                var cellIndexes = Table.ParseCellId(entry.Key);
                this.Form.SetCellText(cellIndexes.Item1, cellIndexes.Item2, entry.Value.ToString());
            }

        }

        public TableForm Form { get; private set; }

        private Table _table;
        private CalculationErrorHandler _calculationErrorHandler;
    }
}
