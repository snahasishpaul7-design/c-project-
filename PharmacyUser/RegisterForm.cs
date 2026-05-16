using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class RegisterForm : Form
    {
        private static readonly Color Primary = Color.FromArgb(60, 140, 100);
        private static readonly Color PrimaryDark = Color.FromArgb(30, 100, 70);
        private static readonly Color TextDark = Color.FromArgb(30, 30, 30);
        private static readonly Color PlaceholderC = Color.FromArgb(130, 130, 130);
        private static readonly Color BorderColor = Color.FromArgb(180, 180, 180);
        private static readonly Color BgColor = Color.FromArgb(240, 240, 240);

        private Panel pnlCard;
        private TextBox txtFirstName, txtLastName;
        private TextBox txtUsername, txtEmail, txtPhone;
        private TextBox txtPassword, txtConfirmPassword;
        private ComboBox cboRole;
        private Button btnRegister;
        private Image bgImage;

        public RegisterForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MediCare Pharmacy – Register";
            this.Size = new Size(900, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            try { bgImage = Image.FromFile("background.jpg"); }
            catch { bgImage = null; }
            this.Paint += (s, e) => PaintBackground(e.Graphics);

            pnlCard = new Panel();
            pnlCard.Size = new Size(500, 620);
            pnlCard.Location = new Point(
                 (900 - 500) / 2,
                 (680 - 620) / 2
             );
            pnlCard.BackColor = Color.Transparent;
            pnlCard.Paint += PnlCard_Paint;
            this.Controls.Add(pnlCard);

            var lblPlus = new Label();
            lblPlus.Text = "+";
            lblPlus.Font = new Font("Segoe UI", 32, FontStyle.Regular);
            lblPlus.ForeColor = Primary;
            lblPlus.AutoSize = true;
            lblPlus.BackColor = Color.Transparent;
            lblPlus.Location = new Point(228, 15);
            pnlCard.Controls.Add(lblPlus);

            var lblTitle = new Label();
            lblTitle.Text = "MediCare Pharmacy";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = TextDark;
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(480, 28);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Location = new Point(10, 68);
            pnlCard.Controls.Add(lblTitle);

            var lblSub = new Label();
            lblSub.Text = "Create your account";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.FromArgb(100, 100, 100);
            lblSub.AutoSize = false;
            lblSub.Size = new Size(480, 22);
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            lblSub.BackColor = Color.Transparent;
            lblSub.Location = new Point(10, 96);
            pnlCard.Controls.Add(lblSub);

            int left = 20;
            int right = 260;
            int halfW = 210;
            int fullW = 450;
            int y = 130;
            int gap = 58;

            AddLabel("FIRST NAME", left, y);
            txtFirstName = AddField(left, y + 22, halfW);
            AddLabel("LAST NAME", right, y);
            txtLastName = AddField(right, y + 22, halfW);
            y += gap;

            AddLabel("USERNAME", left, y);
            txtUsername = AddField(left, y + 22, fullW);
            y += gap;

            AddLabel("EMAIL ADDRESS", left, y);
            txtEmail = AddField(left, y + 22, fullW);
            y += gap;

            AddLabel("PHONE NUMBER", left, y);
            txtPhone = AddField(left, y + 22, fullW);
            y += gap;

            AddLabel("PASSWORD", left, y);
            txtPassword = AddField(left, y + 22, halfW, isPassword: true);
            AddLabel("CONFIRM PASSWORD", right, y);
            txtConfirmPassword = AddField(right, y + 22, halfW, isPassword: true);
            y += gap;

            AddLabel("ROLE", left, y);
            cboRole = new ComboBox
            {
                Location = new Point(left, y + 22),
                Size = new Size(fullW, 36),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f),
                ForeColor = PlaceholderC,
                BackColor = BgColor,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboRole.Items.AddRange(new object[] { "Customer" });
            cboRole.SelectedIndex = 0;
            cboRole.SelectedIndexChanged += (s, e) =>
                cboRole.ForeColor = cboRole.SelectedIndex == 0 ? PlaceholderC : TextDark;
            pnlCard.Controls.Add(cboRole);
            y += gap;

            btnRegister = new Button
            {
                Text = "REGISTER",
                Location = new Point(left, y),
                Size = new Size(fullW, 46),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextDark,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Paint += BtnPaint;
            btnRegister.MouseEnter += (s, e) => { btnRegister.BackColor = Color.FromArgb(60, 60, 60); btnRegister.Refresh(); };
            btnRegister.MouseLeave += (s, e) => { btnRegister.BackColor = TextDark; btnRegister.Refresh(); };
            btnRegister.Click += BtnRegister_Click;
            pnlCard.Controls.Add(btnRegister);
            y += 58;

            var pnlLogin = new FlowLayoutPanel
            {
                Size = new Size(fullW, 24),
                Location = new Point((pnlCard.Width - fullW) / 2, y),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                WrapContents = false
            };
            var lblAlready = new Label
            {
                Text = "Already have an account?",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 3, 4, 0)
            };
            var lnkLogin = new LinkLabel
            {
                Text = "Login here →",
                Font = new Font("Segoe UI", 9f),
                LinkColor = Primary,
                ActiveLinkColor = PrimaryDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 3, 0, 0)
            };
            lnkLogin.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkLogin.LinkClicked += (s, e) => { this.Hide(); new LoginForm().Show(); };
            pnlLogin.Controls.Add(lblAlready);
            pnlLogin.Controls.Add(lnkLogin);
            pnlCard.Controls.Add(pnlLogin);
            y += 30;

            var lblFooter = new Label
            {
                Text = "— MediCare Pharmacy © 2026 —",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = false,
                Size = new Size(480, 20),
                Location = new Point(10, y),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(lblFooter);

            this.ClientSize = new Size(900, 680);
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(x, y)
            };
            pnlCard.Controls.Add(lbl);
        }

        private TextBox AddField(int x, int y, int w, bool isPassword = false)
        {
            var pnl = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, 36),
                BackColor = BgColor
            };

            var tb = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11f),
                ForeColor = TextDark,
                BackColor = BgColor,
                Size = new Size(w - 10, 28),
                Location = new Point(5, 5)
            };
            if (isPassword) tb.PasswordChar = '•';
            pnl.Controls.Add(tb);
            pnlCard.Controls.Add(pnl);
            return tb;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                cboRole.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill in all fields and select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = DBHelper.RegisterUser(
                txtFirstName.Text.Trim(),
                txtLastName.Text.Trim(),
                txtUsername.Text.Trim(),
                txtEmail.Text.Trim(),
                txtPhone.Text.Trim(),
                txtPassword.Text.Trim(),
                cboRole.SelectedItem.ToString()
            );

            if (success)
            {
                MessageBox.Show("Account created successfully! Please login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                new LoginForm().Show();
            }
        }

        private void PaintBackground(Graphics g)
        {
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

        private void BtnPaint(object sender, PaintEventArgs e)
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