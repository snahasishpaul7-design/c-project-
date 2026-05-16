using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class LoginForm : Form
    {
        TextBox txtUsername, txtPassword;
        Button btnLogin, btnRegister;
        Label lblForgotPassword, lblCopyright;
        Panel pnlCard;
        Image bgImage;

        public LoginForm()
        {
            InitializeComponent();
            this.panelHeader.Visible = false;
            this.Size = new Size(900, 580);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "MediCare Pharmacy";

            try
            {
                bgImage = Image.FromFile("background.jpg");
            }
            catch { bgImage = null; }

            this.Paint += LoginForm_Paint;
            BuildCard();
        }

        private void BuildCard()
        {
            pnlCard = new Panel();
            pnlCard.Size = new Size(340, 420);
            pnlCard.Location = new Point(
                (this.ClientSize.Width - 340) / 2,
                (this.ClientSize.Height - 420) / 2
            );
            pnlCard.BackColor = Color.Transparent;
            pnlCard.Paint += PnlCard_Paint;
            this.Controls.Add(pnlCard);

            
            var lblPlus = new Label();
            lblPlus.Text = "+";
            lblPlus.Font = new Font("Segoe UI", 32, FontStyle.Regular);
            lblPlus.ForeColor = Color.FromArgb(60, 140, 100);
            lblPlus.AutoSize = true;
            lblPlus.BackColor = Color.Transparent;
            lblPlus.Location = new Point(148, 20);
            pnlCard.Controls.Add(lblPlus);

            
            var lblTitle = new Label();
            lblTitle.Text = "MediCare Pharmacy";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(320, 28);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Location = new Point(10, 80);
            pnlCard.Controls.Add(lblTitle);

            
            var lblWelcome = new Label();
            lblWelcome.Text = "Welcome back!";
            lblWelcome.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblWelcome.ForeColor = Color.FromArgb(100, 100, 100);
            lblWelcome.AutoSize = false;
            lblWelcome.Size = new Size(320, 22);
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            lblWelcome.BackColor = Color.Transparent;
            lblWelcome.Location = new Point(10, 108);
            pnlCard.Controls.Add(lblWelcome);

           
            var pnlUser = CreateField("👤", 20, 148, 300);
            txtUsername = (TextBox)pnlUser.Controls[1];
            pnlCard.Controls.Add(pnlUser);

            
            var pnlPass = CreateField("🔒", 20, 208, 300);
            txtPassword = (TextBox)pnlPass.Controls[1];
            txtPassword.PasswordChar = '•';
            pnlCard.Controls.Add(pnlPass);

            
            lblForgotPassword = new Label();
            lblForgotPassword.Text = "Forgot Password?";
            lblForgotPassword.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblForgotPassword.ForeColor = Color.FromArgb(80, 80, 80);
            lblForgotPassword.AutoSize = true;
            lblForgotPassword.BackColor = Color.Transparent;
            lblForgotPassword.Location = new Point(185, 272);
            lblForgotPassword.Cursor = Cursors.Hand;
            lblForgotPassword.Click += (s, e) => { this.Hide(); new ForgotPasswordForm().Show(); };
            pnlCard.Controls.Add(lblForgotPassword);

            
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(300, 46);
            btnLogin.Location = new Point(20, 300);
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = Color.FromArgb(30, 30, 30);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Paint += BtnLogin_Paint;
            btnLogin.Click += (s, e) =>
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                System.Data.DataRow user = DBHelper.LoginUser(username, password);

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string role = user["Role"].ToString();

                if (role == "Customer")
                {
                    new CustomerDashboard(
                      user["FirstName"].ToString(),
                      user["LastName"].ToString(),
                      user["Username"].ToString()
                      ).Show();
                    this.Hide();
                }
                else if (role == "Admin")
                {
                    new AdminDashboard(
                        user["Username"].ToString()
                    ).Show();
                    this.Hide();
                }
                else if (role == "Pharmacist")
                {
                    new PharmacistDashboard(
                        user["FirstName"].ToString(),
                        user["LastName"].ToString(),
                        user["Username"].ToString()
                    ).Show();
                    this.Hide();
                }
            };
            pnlCard.Controls.Add(btnLogin);

            
            var pnlRegister = new FlowLayoutPanel();
            pnlRegister.Size = new Size(300, 24);
            pnlRegister.Location = new Point(20, 358);
            pnlRegister.FlowDirection = FlowDirection.LeftToRight;
            pnlRegister.BackColor = Color.Transparent;
            pnlRegister.WrapContents = false;

            var lblDontHave = new Label();
            lblDontHave.Text = "Don't have an account?";
            lblDontHave.Font = new Font("Segoe UI", 9);
            lblDontHave.ForeColor = Color.FromArgb(80, 80, 80);
            lblDontHave.AutoSize = true;
            lblDontHave.BackColor = Color.Transparent;
            lblDontHave.Margin = new Padding(0, 3, 4, 0);

            var lnkCreate = new LinkLabel();
            lnkCreate.Text = "Create one →";
            lnkCreate.Font = new Font("Segoe UI", 9);
            lnkCreate.LinkColor = Color.FromArgb(60, 140, 100);
            lnkCreate.ActiveLinkColor = Color.FromArgb(30, 100, 70);
            lnkCreate.AutoSize = true;
            lnkCreate.BackColor = Color.Transparent;
            lnkCreate.Margin = new Padding(0, 3, 0, 0);
            lnkCreate.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkCreate.LinkClicked += (s, e) => { this.Hide(); new RegisterForm().Show(); };

            pnlRegister.Controls.Add(lblDontHave);
            pnlRegister.Controls.Add(lnkCreate);
            pnlCard.Controls.Add(pnlRegister);

            
            lblCopyright = new Label();
            lblCopyright.Text = "— MediCare Pharmacy © 2026 —";
            lblCopyright.Font = new Font("Segoe UI", 8);
            lblCopyright.ForeColor = Color.FromArgb(150, 150, 150);
            lblCopyright.AutoSize = false;
            lblCopyright.Size = new Size(320, 20);
            lblCopyright.TextAlign = ContentAlignment.MiddleCenter;
            lblCopyright.BackColor = Color.Transparent;
            lblCopyright.Location = new Point(10, 390);
            pnlCard.Controls.Add(lblCopyright);
        }

        private Panel CreateField(string icon, int x, int y, int width)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(width, 46);
            pnl.Location = new Point(x, y);
            pnl.BackColor = Color.FromArgb(240, 240, 240);

            var lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 11);
            lblIcon.Size = new Size(36, 40);
            lblIcon.Location = new Point(6, 3);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            lblIcon.BackColor = Color.Transparent;
            pnl.Controls.Add(lblIcon);

            var txt = new TextBox();
            txt.BorderStyle = BorderStyle.None;
            txt.Font = new Font("Segoe UI", 11);
            txt.BackColor = Color.FromArgb(240, 240, 240);
            txt.ForeColor = Color.FromArgb(60, 60, 60);
            txt.Size = new Size(width - 50, 28);
            txt.Location = new Point(44, 10);
            pnl.Controls.Add(txt);

            return pnl;
        }

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (bgImage != null)
            {
                g.DrawImage(bgImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                using (SolidBrush overlay = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                    g.FillRectangle(overlay, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                using (LinearGradientBrush bg = new LinearGradientBrush(
                    new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height),
                    Color.FromArgb(200, 210, 200),
                    Color.FromArgb(230, 235, 230),
                    LinearGradientMode.ForwardDiagonal))
                    g.FillRectangle(bg, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
        }

        private void PnlCard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, pnlCard.Width, pnlCard.Height);

            using (GraphicsPath path = RoundedRect(rect, 16))
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(210, 255, 255, 255)))
                    g.FillPath(brush, path);

                using (Pen pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
                    g.DrawPath(pen, path);
            }
        }

        private void BtnLogin_Paint(object sender, PaintEventArgs e)
        {
            var btn = (Button)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, btn.Width, btn.Height), 8))
            using (SolidBrush brush = new SolidBrush(btn.BackColor))
            {
                g.FillPath(brush, path);
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(btn.Text, btn.Font, Brushes.White, btn.ClientRectangle, sf);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}