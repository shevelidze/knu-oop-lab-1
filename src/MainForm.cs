using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpTables
{
    public delegate void TableChnageHandler(int rowIndex, int colIndex, string text);
    public partial class MainForm : Form
    {
        public MainForm(TableChnageHandler tableChnageHandler)
        {
            InitializeComponent();

            this.tableChnageHandler = tableChnageHandler;

            const int initialRowsNumber = 10;
            const int initialColumnsNumber = 5;

            for (var i = 0; i < initialColumnsNumber; i++)
            {
                this.AddColumn();
            }

            for (var i = 0; i < initialRowsNumber; i++)
            {
                this.AddRow();
            }

            this.mainGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
        }

        public void AddColumn()
        {
            var index = this.mainGridView.Columns.Count;
            this.mainGridView.Columns.Add(CellsStorage.ColumnIndexToString(index), CellsStorage.ColumnIndexToString(index));
        }

        public void AddRow()
        {
            if (this.mainGridView.Columns.Count == 0)
            {
                this.AddColumn();
            }

            var index = this.mainGridView.Rows.Add();
            this.mainGridView.Rows[index].HeaderCell.Value = (index + 1).ToString();
        }

        public void Clear()
        {
            for (var colIndex = 0; colIndex < this.mainGridView.Columns.Count; colIndex++)
            {
                for (var rowIndex = 0; rowIndex < this.mainGridView.Rows.Count; rowIndex++)
                {
                    this.mainGridView.Rows[rowIndex].Cells[colIndex].Value = null;
                }
            }
        }

        public void SetCellText(string cellId, string text)
        {
            var indexes = CellsStorage.ParseCellId(cellId);

            this.mainGridView.Rows[indexes.Item1].Cells[indexes.Item2].Value = text;
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddColumn();
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AddRow();
        }

        private void mainGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cellValue = this.mainGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            this.tableChnageHandler(
                e.RowIndex,
                e.ColumnIndex,
                cellValue == null ? "" : cellValue.ToString()
            );
        }

        private TableChnageHandler tableChnageHandler;
    }
}
