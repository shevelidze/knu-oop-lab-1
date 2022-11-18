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
                catch (TokenizerException e)
                {
                    MessageBox.Show("Tokenizer exception: " + e.Message);
                    table.SetCellExpression(rowIndex, columnIndex, "");
                    form.SetCellText(rowIndex, columnIndex, "");
                }
                catch (ExecutorException e)
                {
                    MessageBox.Show("Executor exception: " + e.Message);
                    table.SetCellExpression(rowIndex, columnIndex, "");
                    form.SetCellText(rowIndex, columnIndex, "");
                }
             },
            (rowIndex, columnIndex, form) => {
                form.SetCellText(rowIndex, columnIndex, table.GetCellExpression(rowIndex, columnIndex));
            }));
        }
    }
}
