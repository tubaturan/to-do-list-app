using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace To_Do_List_App
{
    public partial class ToDoList : Form
    {
        string connectionString = "Data Source=localhost:1521/XEPDB1;User Id=SYS;Password=St123!;DBA Privilege=SYSDBA;";
        DataTable toDoList = new DataTable();
        bool isEditing = false;
        int selectedId = -1;

        public ToDoList()
        {
            InitializeComponent();
        }

        private void ToDoList_Load(object sender, EventArgs e)
        {
            // DataTable kolonları
            toDoList.Columns.Add("ID");
            toDoList.Columns.Add("TITLE");
            toDoList.Columns.Add("DESCRIPTION");

            toDoListView.DataSource = toDoList;

            if (toDoListView.Columns["ID"] != null)
                toDoListView.Columns["ID"].Visible = false;

              // Form açılır açılmaz verileri yükle
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();

                if (isEditing && selectedId != -1)
                {
                    // UPDATE işlemi paket üzerinden
                    using (OracleCommand cmd = new OracleCommand("POSMRC.todo_pkg.update_todo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = selectedId;
                        cmd.Parameters.Add("p_title", OracleDbType.Varchar2).Value = titleTextBox.Text;
                        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = descriptionTextBox.Text;
                        cmd.ExecuteNonQuery();
                    }

                    // DataTable'da güncelle
                    DataRow row = toDoList.Rows.Cast<DataRow>()
                                        .FirstOrDefault(r => r["ID"] != DBNull.Value && Convert.ToInt32(r["ID"]) == selectedId);
                    if (row != null)
                    {
                        row["TITLE"] = titleTextBox.Text;
                        row["DESCRIPTION"] = descriptionTextBox.Text;
                    }
                }
                else
                {
                    // INSERT işlemi paket üzerinden
                    using (OracleCommand cmd = new OracleCommand("POSMRC.todo_pkg.insert_todo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_title", OracleDbType.Varchar2).Value = titleTextBox.Text;
                        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = descriptionTextBox.Text;

                        OracleParameter idParam = new OracleParameter("p_id", OracleDbType.Int32);
                        idParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(idParam);

                        cmd.ExecuteNonQuery();

                        int newId = ((Oracle.ManagedDataAccess.Types.OracleDecimal)idParam.Value).ToInt32();
                        toDoList.Rows.Add(newId, titleTextBox.Text, descriptionTextBox.Text);
                    }
                }
            }

            ClearForm();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Silmek için bir kayıt seçin");
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();
                
                using (OracleCommand cmd = new OracleCommand("POSMRC.todo_pkg.delete_todo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = selectedId;
                    cmd.ExecuteNonQuery();
                }
            }

            LoadData();
            ClearForm();
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
            toDoList.Clear();
        }

        private void toDoListView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = Convert.ToInt32(toDoListView.Rows[e.RowIndex].Cells["ID"].Value);
                titleTextBox.Text = toDoListView.Rows[e.RowIndex].Cells["TITLE"].Value.ToString();
                descriptionTextBox.Text = toDoListView.Rows[e.RowIndex].Cells["DESCRIPTION"].Value.ToString();
                isEditing = true;
            }
        }

        private void LoadData()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand("POSMRC.todo_pkg.select_all_todos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter refCursor = new OracleParameter
                    {
                        ParameterName = "p_cursor", // paket parametre adı ile birebir olmalı
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(refCursor);

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    toDoList.Clear();
                    da.Fill(toDoList);

                    if (toDoListView.Columns["ID"] != null)
                        toDoListView.Columns["ID"].Visible = false;
                }
            }
        }

        private void ClearForm()
        {
            titleTextBox.Clear();
            descriptionTextBox.Clear();
            isEditing = false;
            selectedId = -1;
        }
    }
}
