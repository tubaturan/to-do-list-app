using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace To_Do_List_App
{
    public partial class ForgotPasswordForm : Form
    {
        public ForgotPasswordForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "To Do List - Şifremi Unuttum";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(400, 450);

            // ===== Gradient arka plan =====
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                    Color.FromArgb(35, 37, 65), Color.FromArgb(60, 70, 120), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // === Ortadaki panel ===
            Panel panel = new Panel();
            panel.Size = new Size(300, 330);
            panel.BackColor = Color.White;
            panel.Location = new Point((this.ClientSize.Width - panel.Width) / 2,
                                       (this.ClientSize.Height - panel.Height) / 2);
            MakeRoundedPanel(panel, 25);
            this.Controls.Add(panel);

            // === Başlık ===
            Label title = new Label();
            title.Text = "Şifremi Unuttum";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(35, 37, 65);
            title.AutoSize = true;
            title.Location = new Point((panel.Width - title.PreferredWidth) / 2, 20);
            panel.Controls.Add(title);

            // === Kullanıcı Adı TextBox ===
            TextBox txtUsername = CreateTextBox("Kullanıcı Adı");
            txtUsername.Location = new Point(30, 70);
            panel.Controls.Add(txtUsername);

            // === Yeni Şifre TextBox ===
            TextBox txtPassword = CreatePasswordTextBox("Yeni Şifre");
            txtPassword.Location = new Point(30, 120);
            panel.Controls.Add(txtPassword);

            // === Yeni Şifre Tekrar TextBox ===
            TextBox txtPasswordRepeat = CreatePasswordTextBox("Yeni Şifre (Tekrar)");
            txtPasswordRepeat.Location = new Point(30, 170);
            panel.Controls.Add(txtPasswordRepeat);

            // === Güncelle Butonu ===
            Button btnUpdate = new Button();
            btnUpdate.Text = "Şifreyi Güncelle";
            btnUpdate.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.BackColor = Color.FromArgb(88, 101, 242);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Size = new Size(220, 40);
            btnUpdate.Location = new Point((panel.Width - btnUpdate.Width) / 2, 230);
            MakeRoundedButton(btnUpdate, 20);
            panel.Controls.Add(btnUpdate);

            // ===== Buton Click =====
            btnUpdate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtPasswordRepeat.Text) ||
                    txtUsername.Text == "Kullanıcı Adı" ||
                    txtPassword.Text == "Yeni Şifre" ||
                    txtPasswordRepeat.Text == "Yeni Şifre (Tekrar)")
                {
                    MessageBox.Show("Tüm alanları doldurun!");
                    return;
                }

                if (txtPassword.Text != txtPasswordRepeat.Text)
                {
                    MessageBox.Show("Şifreler aynı değil!");
                    return;
                }

                bool success = UpdatePassword(txtUsername.Text.Trim(), txtPassword.Text.Trim());
                if (success)
                {
                    MessageBox.Show("Şifreniz başarıyla güncellendi ✅");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kullanıcı bulunamadı ❌");
                }
            };
        }

        // ===== TextBox =====
        private TextBox CreateTextBox(string placeholder)
        {
            TextBox tb = new TextBox();
            tb.Width = 230;
            tb.Height = 30;
            tb.Font = new Font("Segoe UI", 10);
            tb.ForeColor = Color.Gray;
            tb.Text = placeholder;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                }
            };
            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;
                }
            };

            return tb;
        }

        private TextBox CreatePasswordTextBox(string placeholder)
        {
            TextBox tb = new TextBox();
            tb.Width = 230;
            tb.Height = 30;
            tb.Font = new Font("Segoe UI", 10);
            tb.ForeColor = Color.Gray;
            tb.Text = placeholder;
            tb.UseSystemPasswordChar = false;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                    tb.UseSystemPasswordChar = true;
                }
            };
            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;
                    tb.UseSystemPasswordChar = false;
                }
            };

            return tb;
        }

        // ===== DB Güncelle =====
        private bool UpdatePassword(string username, string newPassword)
        {
            string connectionString = "Data Source=localhost:1521/XEPDB1;User Id=SYS;Password=St123!;DBA Privilege=SYSDBA;";
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE POSMRC.Users SET Password = :password WHERE Username = :username";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("password", newPassword));
                        cmd.Parameters.Add(new OracleParameter("username", username));
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB Hatası: " + ex.Message);
                    return false;
                }
            }
        }

        // ===== Oval Panel =====
        private void MakeRoundedPanel(Panel panel, int radius)
        {
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.StartFigure();
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    panel.Region = new Region(path);
                }
            };
        }

        // ===== Oval Button =====
        private void MakeRoundedButton(Button button, int radius)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                button.Region = new Region(path);
            }
        }
    }
}
