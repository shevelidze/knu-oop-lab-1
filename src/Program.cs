using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpTables
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Table table = new Table();
            Application.Run(new MainForm((rowIndex, columnIndex, text, form) => {
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
                    if (e is TokenizerException || e is ExecutorException)
                    {
                        MessageBox.Show((e is TokenizerException ? "Tokenizer exception: " :
                            "Executor exception: ") + e.Message);
                        form.SetCellText(rowIndex, columnIndex, table.GetCellExpression(rowIndex, columnIndex));
                    }
                }
             },
            (rowIndex, columnIndex, form) => {
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
            )
            );
        }
    }
}
