using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpTables
{

    public delegate void CalculationErrorHandler(string message);

    class TableController
    {
        public TableController(Table table, CalculationErrorHandler calculationErrorHandler = null)
        {
            _table = table;
            _calculationErrorHandler = calculationErrorHandler;
            _openedTableFilePath = null;

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
                form => {
                    _table.Clear();
                    _updateCells();
                },
                form => {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = TableController.FilesFilter;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            _table.LoadFromFile(openFileDialog.FileName);
                            _updateCells();
                            _openedTableFilePath = openFileDialog.FileName;
                        }
                    }

                },
                form => {
                    if (_openedTableFilePath != null)
                    {
                        _table.SaveToFile(_openedTableFilePath);
                    }
                },
                form => {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = TableController.FilesFilter;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _table.SaveToFile(saveFileDialog.FileName);
                        _openedTableFilePath = saveFileDialog.FileName;
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
        private string _openedTableFilePath;

        public const string FilesFilter = "SharpTables table (*.stt)|*.stt|All files (*.*)|*.*";
    }
}
