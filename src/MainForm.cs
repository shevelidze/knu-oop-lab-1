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
    public delegate void CellChnageHandler(int rowIndex, int colIndex, string text, MainForm form);
    public delegate void CellFocusHandler(int rowIndex, int colIndex, MainForm form);

    public delegate void MainFormVoidHandler(MainForm form);

    public partial class MainForm : Form
    {
        public MainForm(
            CellChnageHandler cellChnageHandler,
            CellFocusHandler cellFocusHandler,
            MainFormVoidHandler undoHandler,
            MainFormVoidHandler redoHandler
            )
        {
            InitializeComponent();

            _cellChnageHandler = cellChnageHandler;
            _cellFocusHandler = cellFocusHandler;
            _undoHandler = undoHandler;
            _redoHandler = redoHandler;

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
            this.mainGridView.Columns.Add(Table.ColumnIndexToString(index), Table.ColumnIndexToString(index));
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

        public void SetCellText(int rowIndex, int columnIndex, string text)
        {
            this.mainGridView.Rows[rowIndex].Cells[columnIndex].Value = text;
        }

        private CellChnageHandler _cellChnageHandler;
        private CellFocusHandler _cellFocusHandler;
        private MainFormVoidHandler _undoHandler;
        private MainFormVoidHandler _redoHandler;

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

            _cellChnageHandler(
                e.RowIndex,
                e.ColumnIndex,
                cellValue == null ? "" : cellValue.ToString(),
                this
            );
        }

        private void mainGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _cellFocusHandler(e.RowIndex, e.ColumnIndex, this);
        }

        private void undoMenuItem_Click(object sender, EventArgs e)
        {
            _undoHandler(this);
        }

        private void redoMenuItem_Click(object sender, EventArgs e)
        {
            _redoHandler(this);
        }
    }
}
