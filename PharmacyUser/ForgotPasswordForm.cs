using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class ForgotPasswordForm : Form
    {
        private static readonly Color Primary = Color.FromArgb(30, 100, 200);
        private static readonly Color PrimaryDark = Color.FromArgb(20, 70, 160);
        private static readonly Color TextDark = Color.FromArgb(30, 30, 30);
        private static readonly Color PlaceholderC = Color.FromArgb(130, 130, 130);
        private static readonly Color BgColor = Color.FromArgb(240, 240, 240);

        private Panel pnlCard;
        private TextBox txtUsername, txtNewPassword, txtConfirmPassword;
        private Button btnReset;
        private Image bgImage;

        public ForgotPasswordForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MediCare Pharmacy – Forgot Password";
            this.Size = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            try { bgImage = Image.FromFile("background.jpg"); }
            catch { bgImage = null; }
            this.Paint += (s, e) => PaintBackground(e.Graphics);

            pnlCard = new Panel();
            pnlCard.Size = new Size(420, 480);
            pnlCard.Location = new Point(
                (900 - 420) / 2,
                (620 - 480) / 2
            );
            pnlCard.BackColor = Color.Transparent;
            pnlCard.Paint += PnlCard_Paint;
            this.Controls.Add(pnlCard);

            
            var lblKey = new Label();
            lblKey.Text = "🔑";
            lblKey.Font = new Font("Segoe UI", 28);
            lblKey.AutoSize = true;
            lblKey.BackColor = Color.Transparent;
            lblKey.Location = new Point(178, 18);
            pnlCard.Controls.Add(lblKey);

            
            var lblTitle = new Label();
            lblTitle.Text = "Forgot Password?";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = TextDark;
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(400, 28);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Location = new Point(10, 72);
            pnlCard.Controls.Add(lblTitle);

            
            var lblSub = new Label();
            lblSub.Text = "Reset your account password";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.FromArgb(100, 100, 100);
            lblSub.AutoSize = false;
            lblSub.Size = new Size(400, 22);
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            lblSub.BackColor = Color.Transparent;
            lblSub.Location = new Point(10, 100);
            pnlCard.Controls.Add(lblSub);

            int left = 20;
            int fullW = 375;
            int y = 138;
            int gap = 62;

            AddLabel("USERNAME", left, y);
            txtUsername = AddField(left, y + 22, fullW);
            y += gap;

            AddLabel("NEW PASSWORD", left, y);
            txtNewPassword = AddField(left, y + 22, fullW, isPassword: true);
            y += gap;

            AddLabel("CONFIRM NEW PASSWORD", left, y);
            txtConfirmPassword = AddField(left, y + 22, fullW, isPassword: true);
            y += gap;

            btnReset = new Button
            {
                Text = "RESET PASSWORD",
                Location = new Point(left, y),
                Size = new Size(fullW, 46),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextDark,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Paint += BtnPaint;
            btnReset.MouseEnter += (s, e) => { btnReset.BackColor = Color.FromArgb(60, 60, 60); btnReset.Refresh(); };
            btnReset.MouseLeave += (s, e) => { btnReset.BackColor = TextDark; btnReset.Refresh(); };
            btnReset.Click += BtnReset_Click;
            pnlCard.Controls.Add(btnReset);
            y += 58;

            var lblBack = new Label
            {
                Text = "← Back to Login",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Primary,
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblBack.Click += (s, e) => { this.Hide(); new LoginForm().Show(); };
            pnlCard.Controls.Add(lblBack);
            lblBack.Location = new Point((pnlCard.Width - lblBack.PreferredWidth) / 2, y);
            y += 30;

            var lblFooter = new Label
            {
                Text = "— MediCare Pharmacy © 2026 —",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = false,
                Size = new Size(400, 20),
                Location = new Point(10, y),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(lblFooter);
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

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = DBHelper.ResetPassword(
                           txtUsername.Text.Trim(),
                           txtNewPassword.Text.Trim()
 );

            if (success)
            {
                MessageBox.Show("Password reset successfully! Please login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                new LoginForm().Show();
            }
            else
            {
                MessageBox.Show("Username or email not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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