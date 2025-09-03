using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Oracle.ManagedDataAccess.Client;

namespace To_Do_List_App
{
    public partial class LoginForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            if (ValidateUser(username, password))
            {
                ToDoList mainForm = new ToDoList();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre!");
            }
        }

        // Oracle DB bağlantısı ve kontrol
        private bool ValidateUser(string username, string password)
        {
            // Oracle connection string
            string connectionString = "Data Source=localhost:1521/XEPDB1;User Id=SYS;Password=St123!;DBA Privilege=SYSDBA;";
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM POSMRC.Users WHERE Username=:username AND Password=:password";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("username", username));
                        cmd.Parameters.Add(new OracleParameter("password", password));

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DB Hatası: " + ex.Message);
                    return false;
                }
            }
        }
        private void signUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUpForm form = new SignUpForm();
            form.ShowDialog();
        }
        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPasswordForm forgotPasswordForm = new ForgotPasswordForm();
            forgotPasswordForm.ShowDialog();  
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

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Text = "To Do List - Login";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(400, 500);

            // ===== Ortadaki beyaz panel =====
            Panel loginPanel = new Panel();
            loginPanel.Size = new Size(300, 400);
            loginPanel.BackColor = Color.White;
            loginPanel.Location = new Point((this.ClientSize.Width - loginPanel.Width) / 2,
                                            (this.ClientSize.Height - loginPanel.Height) / 2);
            MakeRoundedPanel(loginPanel, 25);
            this.Controls.Add(loginPanel);

            // ===== Başlık ve ikon =====
            PictureBox titleIcon = new PictureBox();
            titleIcon.Image = Properties.Resources.login;
            titleIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            titleIcon.Size = new Size(48, 48);
            titleIcon.Location = new Point((loginPanel.Width - titleIcon.Width) / 2, 20);
            loginPanel.Controls.Add(titleIcon);

            Label welcomeLabel = new Label();
            welcomeLabel.Text = "To Do List";
            welcomeLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.FromArgb(35, 37, 65);
            welcomeLabel.AutoSize = false;
            welcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
            welcomeLabel.Size = new Size(280, 30);
            welcomeLabel.Location = new Point(10, 80);
            loginPanel.Controls.Add(welcomeLabel);

            // ===== Username Panel =====
            Panel usernamePanel = new Panel();
            usernamePanel.Size = new Size(235, 35);
            usernamePanel.BackColor = Color.FromArgb(240, 240, 240);
            usernamePanel.Location = new Point(30, 130);
            usernamePanel.Padding = new Padding(5);
            MakeRoundedPanel(usernamePanel, 15);

            PictureBox userIcon = new PictureBox();
            userIcon.Image = Properties.Resources.user;
            userIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            userIcon.Size = new Size(18, 18);
            userIcon.Location = new Point(5, 5);
            usernamePanel.Controls.Add(userIcon);

            usernameTextBox.BorderStyle = BorderStyle.None;
            usernameTextBox.Font = new Font("Segoe UI", 10);
            usernameTextBox.ForeColor = Color.Black;
            usernameTextBox.BackColor = Color.FromArgb(240, 240, 240);
            usernameTextBox.Location = new Point(35, 5);
            usernameTextBox.Width = 195;
            SendMessage(usernameTextBox.Handle, EM_SETCUEBANNER, (IntPtr)1, "Kullanıcı Adı");
            usernamePanel.Controls.Add(usernameTextBox);

            loginPanel.Controls.Add(usernamePanel);

            // ===== Password Panel =====
            Panel passwordPanel = new Panel();
            passwordPanel.Size = new Size(235, 35);
            passwordPanel.BackColor = Color.FromArgb(240, 240, 240);
            passwordPanel.Location = new Point(30, 180);
            passwordPanel.Padding = new Padding(5);
            MakeRoundedPanel(passwordPanel, 15);

            PictureBox lockIcon = new PictureBox();
            lockIcon.Image = Properties.Resources._lock;
            lockIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            lockIcon.Size = new Size(18, 18);
            lockIcon.Location = new Point(5, 5);
            passwordPanel.Controls.Add(lockIcon);

            passwordTextBox.BorderStyle = BorderStyle.None;
            passwordTextBox.Font = new Font("Segoe UI", 10);
            passwordTextBox.ForeColor = Color.Black;
            passwordTextBox.BackColor = Color.FromArgb(240, 240, 240);
            passwordTextBox.Location = new Point(35, 5);
            passwordTextBox.Width = 195;
            passwordTextBox.UseSystemPasswordChar = true;
            SendMessage(passwordTextBox.Handle, EM_SETCUEBANNER, (IntPtr)1, "Şifre");
            passwordPanel.Controls.Add(passwordTextBox);

            loginPanel.Controls.Add(passwordPanel);

            // ===== Login Button =====
            loginButton.Text = "Giriş Yap";
            loginButton.Width = 230;
            loginButton.Height = 40;
            loginButton.Location = new Point((loginPanel.Width - loginButton.Width) / 2, 230);
            loginButton.BackColor = Color.FromArgb(88, 101, 242);
            loginButton.ForeColor = Color.White;
            loginButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            MakeRoundedButton(loginButton, 20);
            loginButton.MouseEnter += (s, ev) => loginButton.BackColor = Color.FromArgb(71, 82, 196);
            loginButton.MouseLeave += (s, ev) => loginButton.BackColor = Color.FromArgb(88, 101, 242);
            loginPanel.Controls.Add(loginButton);

            // ===== Alt Linkler =====
            LinkLabel forgotPassword = new LinkLabel();
            forgotPassword.Text = "Şifremi Unuttum?";
            forgotPassword.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            forgotPassword.LinkColor = Color.FromArgb(35,37,65);
            forgotPassword.AutoSize = true;
            forgotPassword.Location = new Point(95, 300);
            forgotPassword.LinkClicked += forgotPassword_LinkClicked;
            loginPanel.Controls.Add(forgotPassword);

            LinkLabel signUp = new LinkLabel();
            signUp.Text = "Hesabın yok mu? Kaydol";
            signUp.Font = new Font("Segoe UI", 9);
            signUp.LinkColor = Color.FromArgb(88, 101, 242);
            signUp.AutoSize = true;
            signUp.Location = new Point((loginPanel.Width - loginButton.Width) / 2, 340);
            signUp.LinkClicked += signUp_LinkClicked;
            loginPanel.Controls.Add(signUp);

            // Eski label'ları gizle
            label1.Visible = false;
            label2.Visible = false;
        }

        // ===== Gradient arka plan =====
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
