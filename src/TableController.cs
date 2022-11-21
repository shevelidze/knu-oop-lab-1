using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        var values = table.SetCellExpression(rowIndex, columnIndex, text);

                        form.Clear();

                        foreach (var entry in values)
                        {
                            var cellIndexes = Table.ParseCellId(entry.Key);
                            form.SetCellText(cellIndexes.Item1, cellIndexes.Item2, entry.Value.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        if (_calculationErrorHandler != null && (e is TokenizerException || e is ExecutorException))
                        {
                            _calculationErrorHandler((e is TokenizerException ? "Tokenizer exception: " :
                                "Executor exception: ") + e.Message);
                            form.SetCellText(rowIndex, columnIndex, table.GetCellExpression(rowIndex, columnIndex));
                        }
                    }
                },
            (rowIndex, columnIndex, form) =>
            {
                form.SetCellText(rowIndex, columnIndex, table.GetCellExpression(rowIndex, columnIndex));
            },
            form =>
            {
                table.Undo();

                var values = table.Calculate();

                form.Clear();

                foreach (var entry in values)
                {
                    var cellIndexes = Table.ParseCellId(entry.Key);
                    form.SetCellText(cellIndexes.Item1, cellIndexes.Item2, entry.Value.ToString());
                }
            },
            form =>
            {
                table.Redo();

                var values = table.Calculate();

                form.Clear();

                foreach (var entry in values)
                {
                    var cellIndexes = Table.ParseCellId(entry.Key);
                    form.SetCellText(cellIndexes.Item1, cellIndexes.Item2, entry.Value.ToString());
                }
            }
            );
        }

        public TableForm Form { get; private set; }

        private Table _table;
        private CalculationErrorHandler _calculationErrorHandler;
    }
}
