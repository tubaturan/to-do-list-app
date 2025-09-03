using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace To_Do_List_App
{
    public partial class SignUpForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public SignUpForm()
        {
            InitializeComponent();
        }

        private void SignUpForm_Load(object sender, EventArgs e)
        {
            this.Text = "To Do List - Sign Up";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(400, 500);

            // ===== Ortadaki beyaz panel =====
            Panel panel = new Panel();
            panel.Size = new Size(300, 400);
            panel.BackColor = Color.White;
            panel.Location = new Point((this.ClientSize.Width - panel.Width) / 2,
                                       (this.ClientSize.Height - panel.Height) / 2);
            MakeRoundedPanel(panel, 25);
            this.Controls.Add(panel);

            // ===== Başlık =====
            Label title = new Label();
            title.Text = "Kaydol";
            title.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(35, 37, 65);
            title.AutoSize = false;
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Size = new Size(280, 30);
            title.Location = new Point(10, 40);
            panel.Controls.Add(title);

            // ===== Username TextBox =====
            Panel usernamePanel = CreateRoundedPanel();
            TextBox usernameBox = CreateTextBox("Kullanıcı Adı");
            usernamePanel.Controls.Add(usernameBox);
            usernamePanel.Location = new Point(30, 100);
            panel.Controls.Add(usernamePanel);

            // ===== Password TextBox =====
            Panel passwordPanel = CreateRoundedPanel();
            TextBox passwordBox = CreateTextBox("Şifre", true);
            passwordPanel.Controls.Add(passwordBox);
            passwordPanel.Location = new Point(30, 160);
            panel.Controls.Add(passwordPanel);

            // ===== Confirm Password TextBox =====
            Panel confirmPanel = CreateRoundedPanel();
            TextBox confirmBox = CreateTextBox("Şifre Tekrar", true);
            confirmPanel.Controls.Add(confirmBox);
            confirmPanel.Location = new Point(30, 220);
            panel.Controls.Add(confirmPanel);

            // ===== Sign Up Button =====
            Button signUpBtn = new Button();
            signUpBtn.Text = "Kaydol";
            signUpBtn.Width = 230;
            signUpBtn.Height = 40;
            signUpBtn.Location = new Point((panel.Width - signUpBtn.Width) / 2, 280);
            signUpBtn.BackColor = Color.FromArgb(88, 101, 242);
            signUpBtn.ForeColor = Color.White;
            signUpBtn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            MakeRoundedButton(signUpBtn, 20);
            panel.Controls.Add(signUpBtn);

            signUpBtn.Click += (s, ev) =>
            {
                string username = usernameBox.Text.Trim();
                string password = passwordBox.Text.Trim();
                string confirm = confirmBox.Text.Trim();

                if (username == "" || password == "" || confirm == "")
                {
                    MessageBox.Show("Alanlar boş bırakılamaz!");
                    return;
                }

                if (password != confirm)
                {
                    MessageBox.Show("Şifreler uyuşmuyor!");
                    return;
                }

                if (CreateUser(username, password))
                {
                    MessageBox.Show("Kullanıcı oluşturuldu!");
                    this.Close(); // Kaydolduktan sonra SignUpForm'u kapat
                }
                else
                {
                    MessageBox.Show("Bir hata oluştu!");
                }
            };
        }

        private Panel CreateRoundedPanel()
        {
            Panel p = new Panel();
            p.Size = new Size(235, 35);
            p.BackColor = Color.FromArgb(240, 240, 240);
            p.Padding = new Padding(5);
            MakeRoundedPanel(p, 15);
            return p;
        }

        private TextBox CreateTextBox(string placeholder, bool isPassword = false)
        {
            TextBox tb = new TextBox();
            tb.BorderStyle = BorderStyle.None;
            tb.Font = new Font("Segoe UI", 10);
            tb.ForeColor = Color.Black;
            tb.BackColor = Color.FromArgb(240, 240, 240);
            tb.Location = new Point(5, 5);
            tb.Width = 225;
            tb.UseSystemPasswordChar = isPassword;

            // Handle oluştuğunda placeholder ekle
            tb.HandleCreated += (s, e) =>
            {
                SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);
            };

            return tb;
        }

        private bool CreateUser(string username, string password)
        {
            string connectionString = "Data Source=localhost:1521/XEPDB1;User Id=SYS;Password=St123!;DBA Privilege=SYSDBA;";
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO POSMRC.Users (Username, Password) VALUES (:username, :password)";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("username", username));
                        cmd.Parameters.Add(new OracleParameter("password", password));
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB Hatası: " + ex.Message);
                    return false;
                }
            }
        }

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

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(35, 37, 65), Color.FromArgb(60, 70, 120), 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }
    }
}
