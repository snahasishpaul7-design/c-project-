using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class AdminDashboard : Form
    {
        private static readonly Color SidebarBg = Color.FromArgb(30, 39, 46);
        private static readonly Color SidebarHover = Color.FromArgb(45, 57, 66);
        private static readonly Color SidebarText = Color.FromArgb(160, 174, 185);
        private static readonly Color AccentGreen = Color.FromArgb(0, 184, 148);
        private static readonly Color AccentRed = Color.FromArgb(214, 48, 49);
        private static readonly Color AccentBlue = Color.FromArgb(9, 132, 227);
        private static readonly Color AccentAmber = Color.FromArgb(253, 203, 110);
        private static readonly Color BgColor = Color.FromArgb(230, 235, 233);
        private static readonly Color CardBg = Color.White;
        private static readonly Color TextDark = Color.FromArgb(30, 35, 33);
        private static readonly Color TextGray = Color.FromArgb(120, 130, 125);


        private Panel pnlSidebar, pnlMain, pnlContent;
        public string AdminUsername { get; set; }

        public AdminDashboard(string username)
        {
            AdminUsername = username;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MediCare Pharmacy – Admin Panel";
            this.Size = new Size(1150, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Font;
            BuildSidebar();
            BuildMain();
            ShowHome();
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel { Size = new Size(220, this.ClientSize.Height), Location = new Point(0, 0), BackColor = SidebarBg };
            this.Controls.Add(pnlSidebar);

            var pnlLogo = new Panel { Size = new Size(220, 64), Location = new Point(0, 0), BackColor = Color.FromArgb(22, 30, 36) };
            var lblLogo = new Label { Text = "MediCare", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 12) };
            var lblSub = new Label { Text = "Admin Panel", Font = new Font("Segoe UI", 9), ForeColor = AccentGreen, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 38) };
            pnlLogo.Controls.Add(lblLogo);
            pnlLogo.Controls.Add(lblSub);
            pnlSidebar.Controls.Add(pnlLogo);

            var pnlAvatar = new Panel { Size = new Size(44, 44), Location = new Point(20, 80), BackColor = Color.Transparent };
            pnlAvatar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(AccentGreen), 0, 0, 44, 44);
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString("A", new Font("Segoe UI", 14, FontStyle.Bold), Brushes.White, new RectangleF(0, 0, 44, 44), sf);
            };
            pnlSidebar.Controls.Add(pnlAvatar);

            pnlSidebar.Controls.Add(new Label { Text = AdminUsername, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(72, 86) });
            pnlSidebar.Controls.Add(new Label { Text = "Administrator", Font = new Font("Segoe UI", 8), ForeColor = SidebarText, AutoSize = true, BackColor = Color.Transparent, Location = new Point(72, 106) });
            pnlSidebar.Controls.Add(new Panel { Size = new Size(180, 1), Location = new Point(20, 136), BackColor = Color.FromArgb(55, 65, 72) });

            string[] menus = { "🏠  Home", "💊  Medicines", "📦  Orders", "📋  Prescriptions", "👥  Users", "📊  Reports", "👤  My Profile" };
            int menuY = 148;
            foreach (var menu in menus) { pnlSidebar.Controls.Add(CreateMenuItem(menu, menuY)); menuY += 46; }

            pnlSidebar.Controls.Add(new Panel { Size = new Size(180, 1), Location = new Point(20, this.ClientSize.Height - 80), BackColor = Color.FromArgb(55, 65, 72) });

            var btnLogout = CreateMenuItem("🚪  Logout", this.ClientSize.Height - 62);
            btnLogout.Click += (s, e) => { this.Hide(); new LoginForm().Show(); };
            foreach (Control c in btnLogout.Controls) c.Click += (s, e) => { this.Hide(); new LoginForm().Show(); };
            pnlSidebar.Controls.Add(btnLogout);
        }

        private Panel CreateMenuItem(string text, int y)
        {
            var pnl = new Panel { Size = new Size(200, 38), Location = new Point(10, y), BackColor = Color.Transparent, Cursor = Cursors.Hand, Tag = text };
            var lbl = new Label { Text = text, Font = new Font("Segoe UI", 10), ForeColor = SidebarText, AutoSize = false, Size = new Size(200, 38), Location = new Point(0, 0), TextAlign = ContentAlignment.MiddleLeft, BackColor = Color.Transparent, Padding = new Padding(12, 0, 0, 0) };
            pnl.Controls.Add(lbl);
            pnl.MouseEnter += (s, e) => { pnl.BackColor = SidebarHover; lbl.ForeColor = Color.White; };
            pnl.MouseLeave += (s, e) => { pnl.BackColor = Color.Transparent; lbl.ForeColor = SidebarText; };
            lbl.MouseEnter += (s, e) => { pnl.BackColor = SidebarHover; lbl.ForeColor = Color.White; };
            lbl.MouseLeave += (s, e) => { pnl.BackColor = Color.Transparent; lbl.ForeColor = SidebarText; };
            pnl.Click += MenuItem_Click;
            lbl.Click += MenuItem_Click;
            return pnl;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            string tag = ctrl?.Tag?.ToString() ?? ctrl?.Parent?.Tag?.ToString() ?? "";
            if (tag.Contains("Home")) ShowHome();
            else if (tag.Contains("Medicine")) ShowMedicines();
            else if (tag.Contains("Order")) ShowOrders();
            else if (tag.Contains("Prescript")) ShowPrescriptions();
            else if (tag.Contains("User")) ShowUsers();
            else if (tag.Contains("Report")) ShowReports();
            else if (tag.Contains("Profile")) ShowProfile();
        }

        private void BuildMain()
        {
            pnlMain = new Panel { Size = new Size(this.ClientSize.Width - 220, this.ClientSize.Height), Location = new Point(220, 0), BackColor = BgColor };
            this.Controls.Add(pnlMain);

            var pnlTop = new Panel { Size = new Size(pnlMain.Width, 60), Location = new Point(0, 0), BackColor = CardBg };
            pnlTop.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 225, 222)), 0, 59, pnlTop.Width, 59);
            pnlMain.Controls.Add(pnlTop);
            pnlTop.Controls.Add(new Label { Text = "Dashboard Overview", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 10) });
            pnlTop.Controls.Add(new Label { Text = DateTime.Now.ToString("dddd, d MMMM yyyy"), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 34) });

            pnlContent = new Panel { Size = new Size(pnlMain.Width, pnlMain.Height - 60), Location = new Point(0, 60), BackColor = BgColor, AutoScroll = true };
            pnlMain.Controls.Add(pnlContent);
        }

        private void ClearContent() => pnlContent.Controls.Clear();

        private void ShowHome()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            int totalUsers = 0, totalMeds = 0, totalOrders = 0, lowStock = 0;
            decimal revenue = 0;
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    totalUsers = (int)new SqlCommand("SELECT COUNT(*) FROM Users", con).ExecuteScalar();
                    totalMeds = (int)new SqlCommand("SELECT COUNT(*) FROM Medicines", con).ExecuteScalar();
                    totalOrders = (int)new SqlCommand("SELECT COUNT(*) FROM OrderHeaders", con).ExecuteScalar();
                    lowStock = (int)new SqlCommand("SELECT COUNT(*) FROM Medicines WHERE Stock <= 20", con).ExecuteScalar();
                    revenue = Convert.ToDecimal(new SqlCommand("SELECT ISNULL(SUM(TotalPrice),0) FROM OrderHeaders WHERE Status='Delivered'", con).ExecuteScalar());
                }
            }
            catch { }

            string[] titles = { "Total Users", "Total Medicines", "Total Orders", "Revenue (৳)" };
            string[] values = { totalUsers.ToString(), totalMeds.ToString(), totalOrders.ToString(), revenue.ToString("N0") };
            string[] subs = { "Registered accounts", lowStock + " low stock", "All time", "From delivered orders" };
            Color[] accents = { AccentBlue, AccentGreen, AccentAmber, AccentGreen };

            int cardW = (w - 30) / 4;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var card = new Panel { Size = new Size(cardW, 110), Location = new Point(pad + i * (cardW + 10), pad), BackColor = CardBg };
                card.Paint += (s, e) => DrawCard(e.Graphics, card);
                card.Controls.Add(new Panel { Size = new Size(4, 110), Location = new Point(0, 0), BackColor = accents[idx] });
                card.Controls.Add(new Label { Text = titles[idx], Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });
                card.Controls.Add(new Label { Text = values[idx], Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 36) });
                card.Controls.Add(new Label { Text = subs[idx], Font = new Font("Segoe UI", 8), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 80) });
                pnlContent.Controls.Add(card);
            }

            int y2 = pad + 128;

            var pnlOrders = new Panel { Size = new Size((int)(w * 0.60), 230), Location = new Point(pad, y2), BackColor = CardBg };
            pnlOrders.Paint += (s, e) => DrawCard(e.Graphics, pnlOrders);
            pnlContent.Controls.Add(pnlOrders);
            pnlOrders.Controls.Add(new Label { Text = "Recent Orders", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });

            string[] hdr = { "Medicine", "User", "Qty", "Amount", "Status" };
            int[] hw = { 160, 110, 50, 80, 90 };
            int hx = 16;
            for (int i = 0; i < hdr.Length; i++) { pnlOrders.Controls.Add(new Label { Text = hdr[i], Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[i], 22), Location = new Point(hx, 42), BackColor = Color.Transparent }); hx += hw[i]; }

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 5 oi.MedicineName, u.FirstName+' '+u.LastName AS UserName, " +
                        "oi.Quantity, oi.TotalPrice, oh.Status " +
                        "FROM OrderItems oi " +
                        "JOIN OrderHeaders oh ON oi.OrderHeaderID = oh.OrderHeaderID " +
                        "JOIN Users u ON oh.UserID = u.UserID " +
                        "ORDER BY oh.OrderDate DESC", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            string st = rd["Status"].ToString();
                            Color sbg = st == "Delivered" ? Color.FromArgb(212, 237, 218) : st == "Processing" ? Color.FromArgb(210, 228, 248) : Color.FromArgb(255, 234, 167);
                            Color sfg = st == "Delivered" ? Color.FromArgb(21, 87, 36) : st == "Processing" ? Color.FromArgb(12, 62, 132) : Color.FromArgb(133, 77, 14);
                            int ry = 68 + row * 30;
                            int rx2 = 16;
                            string[] vals = { rd["MedicineName"].ToString(), rd["UserName"].ToString(), rd["Quantity"].ToString(), "৳" + Convert.ToDecimal(rd["TotalPrice"]).ToString("N0") };
                            for (int c2 = 0; c2 < 4; c2++) { pnlOrders.Controls.Add(new Label { Text = vals[c2], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c2], 26), Location = new Point(rx2, ry), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx2 += hw[c2]; }
                            pnlOrders.Controls.Add(new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = sfg, BackColor = sbg, AutoSize = true, Location = new Point(rx2, ry + 4), Padding = new Padding(5, 2, 5, 2) });
                            row++;
                        }
                    }
                }
            }
            catch { }

            int rx3 = pad + (int)(w * 0.60) + 10;
            var pnlStock = new Panel { Size = new Size(w - (int)(w * 0.60) - 10, 230), Location = new Point(rx3, y2), BackColor = CardBg };
            pnlStock.Paint += (s, e) => DrawCard(e.Graphics, pnlStock);
            pnlContent.Controls.Add(pnlStock);
            pnlStock.Controls.Add(new Label { Text = "Low Stock Alert", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT TOP 6 Name, Stock FROM Medicines ORDER BY Stock ASC", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            int stock = Convert.ToInt32(rd["Stock"]);
                            Color bc = stock <= 20 ? AccentRed : AccentGreen;
                            int ry = 44 + row * 28;
                            int numX = pnlStock.Width - 36;
                            int barWidth = numX - 6 - 114;
                            int filled = Math.Min(Math.Max((int)((stock / 200.0) * barWidth), 4), barWidth);

                            pnlStock.Controls.Add(new Label { Text = rd["Name"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(100, 22), Location = new Point(10, ry), BackColor = Color.Transparent });
                            pnlStock.Controls.Add(new Label { Text = stock.ToString(), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = bc, AutoSize = false, Size = new Size(30, 22), Location = new Point(numX, ry), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                            var barBg = new Panel { Size = new Size(barWidth, 8), Location = new Point(114, ry + 7), BackColor = Color.FromArgb(220, 225, 222) };
                            barBg.Controls.Add(new Panel { Size = new Size(filled, 8), Location = new Point(0, 0), BackColor = bc });
                            pnlStock.Controls.Add(barBg);
                            row++;
                        }
                    }
                }
            }
            catch { }

            int y3 = y2 + 248;

            var pnlUsers = new Panel { Size = new Size((int)(w * 0.45), 200), Location = new Point(pad, y3), BackColor = CardBg };
            pnlUsers.Paint += (s, e) => DrawCard(e.Graphics, pnlUsers);
            pnlContent.Controls.Add(pnlUsers);
            pnlUsers.Controls.Add(new Label { Text = "Recent Users", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });

            Color[] avatarColors = { AccentBlue, AccentGreen, AccentAmber, AccentRed, Color.FromArgb(162, 155, 254) };
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT TOP 5 FirstName, LastName, Role FROM Users ORDER BY UserID DESC", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            string fn = rd["FirstName"].ToString(), ln = rd["LastName"].ToString(), role = rd["Role"].ToString();
                            Color ac = avatarColors[row % avatarColors.Length];
                            string initials = (fn.Length > 0 ? fn[0].ToString() : "") + (ln.Length > 0 ? ln[0].ToString() : "");
                            int ry = 44 + row * 30;
                            var av = new Panel { Size = new Size(24, 24), Location = new Point(12, ry), BackColor = Color.Transparent };
                            Color avColor = ac;
                            av.Paint += (s2, e2) => { e2.Graphics.SmoothingMode = SmoothingMode.AntiAlias; e2.Graphics.FillEllipse(new SolidBrush(avColor), 0, 0, 24, 24); using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }) e2.Graphics.DrawString(initials, new Font("Segoe UI", 7, FontStyle.Bold), Brushes.White, new RectangleF(0, 0, 24, 24), sf); };
                            pnlUsers.Controls.Add(av);
                            pnlUsers.Controls.Add(new Label { Text = fn + " " + ln, Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(44, ry + 4) });
                            pnlUsers.Controls.Add(new Label { Text = role, Font = new Font("Segoe UI", 8), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(pnlUsers.Width - 90, ry + 4) });
                            row++;
                        }
                    }
                }
            }
            catch { }

            var pnlQuick = new Panel { Size = new Size(w - (int)(w * 0.45) - 10, 200), Location = new Point(pad + (int)(w * 0.45) + 10, y3), BackColor = CardBg };
            pnlQuick.Paint += (s, e) => DrawCard(e.Graphics, pnlQuick);
            pnlContent.Controls.Add(pnlQuick);
            pnlQuick.Controls.Add(new Label { Text = "Quick Actions", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });

            string[] actions = { "Add Medicine", "View Orders", "Manage Users", "View Reports" };
            Color[] actClr = { AccentGreen, AccentBlue, AccentAmber, AccentRed };
            for (int i = 0; i < actions.Length; i++)
            {
                int idx = i;
                var btn = new Button { Text = actions[idx], Size = new Size((pnlQuick.Width - 40) / 2, 38), Location = new Point(12 + (idx % 2) * ((pnlQuick.Width - 40) / 2 + 16), 44 + (idx / 2) * 52), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = actClr[idx], Cursor = Cursors.Hand };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => { if (actions[idx].Contains("Medicine")) ShowMedicines(); else if (actions[idx].Contains("Order")) ShowOrders(); else if (actions[idx].Contains("User")) ShowUsers(); else ShowReports(); };
                pnlQuick.Controls.Add(btn);
            }

            pnlContent.AutoScrollMinSize = new Size(0, y3 + 220);
        }

        private void ShowMedicines()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "💊  Medicine Management", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            var btnAddNew = new Button { Text = "+ Add Medicine", Size = new Size(140, 34), Location = new Point(pnlCard.Width - 160, 12), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += (s, e) => ShowAddMedicineForm(null);
            pnlCard.Controls.Add(btnAddNew);

            var txtSearch = new TextBox { Size = new Size(260, 30), Location = new Point(16, 54), Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, Text = "Search medicine..." };
            pnlCard.Controls.Add(txtSearch);

            string[] hdrs = { "Name", "Category", "Price (৳)", "Stock", "Actions" };
            int[] hw2 = { 220, 150, 100, 100, 180 };
            int hx2 = 16;
            for (int i = 0; i < hdrs.Length; i++) { pnlCard.Controls.Add(new Label { Text = hdrs[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw2[i], 26), Location = new Point(hx2, 96), BackColor = Color.FromArgb(245, 247, 246), TextAlign = ContentAlignment.MiddleLeft }); hx2 += hw2[i]; }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 140), Location = new Point(16, 126), BackColor = Color.Transparent, AutoScroll = true };
            pnlCard.Controls.Add(pnlList);

            Action<string> loadMeds = null;
            loadMeds = (search) =>
            {
                pnlList.Controls.Clear();
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        string sql = "SELECT MedicineID, Name, Category, Price, Stock, ISNULL(ImagePath,'') AS ImagePath FROM Medicines" +
                                     (string.IsNullOrEmpty(search) ? "" : " WHERE Name LIKE @s OR Category LIKE @s") + " ORDER BY Name";
                        using (var cmd = new SqlCommand(sql, con))
                        {
                            if (!string.IsNullOrEmpty(search)) cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                            using (var rd = cmd.ExecuteReader())
                            {
                                int row = 0;
                                while (rd.Read())
                                {
                                    int medID = Convert.ToInt32(rd["MedicineID"]);
                                    string name = rd["Name"].ToString();
                                    string cat = rd["Category"].ToString();
                                    decimal pr = Convert.ToDecimal(rd["Price"]);
                                    int stock = Convert.ToInt32(rd["Stock"]);
                                    string imagePath = rd["ImagePath"].ToString();
                                    Color sc = stock <= 20 ? AccentRed : AccentGreen;

                                    var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 50), Location = new Point(0, row * 56), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };


                                    var pic = new PictureBox { Size = new Size(40, 40), Location = new Point(5, 5), BackColor = Color.FromArgb(235, 240, 238), SizeMode = PictureBoxSizeMode.Zoom };
                                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                    { try { pic.Image = Image.FromFile(imagePath); } catch { } }
                                    if (pic.Image == null)
                                    { pic.Paint += (s2, e2) => { using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }) e2.Graphics.DrawString("💊", new Font("Segoe UI", 16), new SolidBrush(TextGray), new RectangleF(0, 0, pic.Width, pic.Height), sf); }; }
                                    rowPnl.Controls.Add(pic);

                                    int rx4 = 50;
                                    rowPnl.Controls.Add(new Label { Text = name, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(170, 50), Location = new Point(rx4, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(5, 0, 0, 0) }); rx4 += 170;
                                    rowPnl.Controls.Add(new Label { Text = cat, Font = new Font("Segoe UI", 10), ForeColor = TextGray, AutoSize = false, Size = new Size(150, 50), Location = new Point(rx4, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx4 += 150;
                                    rowPnl.Controls.Add(new Label { Text = pr.ToString("N2"), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(100, 50), Location = new Point(rx4, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx4 += 100;
                                    rowPnl.Controls.Add(new Label { Text = stock + " left", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = sc, AutoSize = false, Size = new Size(100, 50), Location = new Point(rx4, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx4 += 100;

                                    int capturedID = medID;
                                    string capturedName = name, capturedCat = cat;
                                    decimal capturedPr = pr;
                                    int capturedStock = stock;


                                    var btnImg = new Button { Text = "📷", Size = new Size(36, 28), Location = new Point(rx4, 11), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                                    btnImg.FlatAppearance.BorderSize = 0;
                                    btnImg.Click += (s2, e2) =>
                                    {
                                        using (OpenFileDialog ofd = new OpenFileDialog())
                                        {
                                            ofd.Title = "Select Medicine Image";
                                            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                                            if (ofd.ShowDialog() == DialogResult.OK)
                                            {
                                                string imgFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MedicineImages");
                                                if (!System.IO.Directory.Exists(imgFolder)) System.IO.Directory.CreateDirectory(imgFolder);
                                                string ext = System.IO.Path.GetExtension(ofd.FileName);
                                                string destPath = System.IO.Path.Combine(imgFolder, "med_" + capturedID + ext);
                                                System.IO.File.Copy(ofd.FileName, destPath, true);
                                                try
                                                {
                                                    using (var con2 = DBHelper.GetConnection())
                                                    {
                                                        con2.Open();
                                                        using (var cmd2 = new SqlCommand("UPDATE Medicines SET ImagePath=@img WHERE MedicineID=@id", con2))
                                                        {
                                                            cmd2.Parameters.AddWithValue("@img", destPath);
                                                            cmd2.Parameters.AddWithValue("@id", capturedID);
                                                            cmd2.ExecuteNonQuery();
                                                        }
                                                    }
                                                    MessageBox.Show("Image uploaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    ShowMedicines();
                                                }
                                                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                                            }
                                        }
                                    };
                                    rowPnl.Controls.Add(btnImg);


                                    var btnEdit = new Button { Text = "Edit", Size = new Size(55, 28), Location = new Point(rx4 + 42, 11), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentBlue, Cursor = Cursors.Hand };
                                    btnEdit.FlatAppearance.BorderSize = 0;
                                    btnEdit.Click += (s2, e2) => ShowAddMedicineForm(new object[] { capturedID, capturedName, capturedCat, capturedPr, capturedStock });
                                    rowPnl.Controls.Add(btnEdit);


                                    var btnDel = new Button { Text = "Delete", Size = new Size(55, 28), Location = new Point(rx4 + 103, 11), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                                    btnDel.FlatAppearance.BorderSize = 0;
                                    btnDel.Click += (s2, e2) =>
                                    {
                                        if (MessageBox.Show($"Delete {capturedName}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                        {
                                            try
                                            {
                                                using (var con2 = DBHelper.GetConnection()) { con2.Open(); new SqlCommand($"DELETE FROM Medicines WHERE MedicineID={capturedID}", con2).ExecuteNonQuery(); }
                                                loadMeds(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim());
                                            }
                                            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                                        }
                                    };
                                    rowPnl.Controls.Add(btnDel);
                                    pnlList.Controls.Add(rowPnl);
                                    row++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            };

            loadMeds("");
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search medicine...") txtSearch.Text = ""; };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrEmpty(txtSearch.Text)) txtSearch.Text = "Search medicine..."; };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) loadMeds(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim()); };
        }

        private void ShowAddMedicineForm(object[] data)
        {
            bool isEdit = data != null;
            var f = new Form { Text = isEdit ? "Edit Medicine" : "Add Medicine", Size = new Size(380, 320), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, BackColor = Color.White };

            string[] lbls = { "Medicine Name", "Category", "Price (৳)", "Stock Quantity" };
            var txts = new TextBox[4];
            for (int i = 0; i < lbls.Length; i++)
            {
                f.Controls.Add(new Label { Text = lbls[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, Location = new Point(20, 16 + i * 56) });
                txts[i] = new TextBox { Size = new Size(320, 28), Location = new Point(20, 34 + i * 56), Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
                if (isEdit) txts[i].Text = new string[] { data[1].ToString(), data[2].ToString(), data[3].ToString(), data[4].ToString() }[i];
                f.Controls.Add(txts[i]);
            }

            var btnSave = new Button { Text = isEdit ? "Update" : "Add Medicine", Size = new Size(150, 38), Location = new Point(20, 248), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txts[0].Text)) { MessageBox.Show("Enter medicine name."); return; }
                if (!decimal.TryParse(txts[2].Text, out decimal price)) { MessageBox.Show("Enter valid price."); return; }
                if (!int.TryParse(txts[3].Text, out int stock)) { MessageBox.Show("Enter valid stock."); return; }
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        if (isEdit)
                        {
                            using (var cmd = new SqlCommand("UPDATE Medicines SET Name=@n,Category=@c,Price=@p,Stock=@s WHERE MedicineID=@id", con))
                            { cmd.Parameters.AddWithValue("@n", txts[0].Text); cmd.Parameters.AddWithValue("@c", txts[1].Text); cmd.Parameters.AddWithValue("@p", price); cmd.Parameters.AddWithValue("@s", stock); cmd.Parameters.AddWithValue("@id", (int)data[0]); cmd.ExecuteNonQuery(); }
                        }
                        else
                        {
                            using (var cmd = new SqlCommand("INSERT INTO Medicines(Name,Category,Price,Stock) VALUES(@n,@c,@p,@s)", con))
                            { cmd.Parameters.AddWithValue("@n", txts[0].Text); cmd.Parameters.AddWithValue("@c", txts[1].Text); cmd.Parameters.AddWithValue("@p", price); cmd.Parameters.AddWithValue("@s", stock); cmd.ExecuteNonQuery(); }
                        }
                    }
                    MessageBox.Show(isEdit ? "Medicine updated!" : "Medicine added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    f.Close();
                    ShowMedicines();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            };
            f.Controls.Add(btnSave);
            f.ShowDialog(this);
        }

        private void ShowOrders()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "📦  Order Management", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            // Headers
            string[] hdrs = { "Order #", "Customer", "Total", "Date", "Status", "Action" };
            int[] hw = { 80, 180, 100, 120, 110, 140 };
            int hx = 16;
            for (int i = 0; i < hdrs.Length; i++)
            {
                pnlCard.Controls.Add(new Label { Text = hdrs[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[i], 26), Location = new Point(hx, 52), BackColor = Color.Transparent });
                hx += hw[i];
            }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 96), Location = new Point(16, 82), BackColor = Color.Transparent, AutoScroll = true };
            pnlCard.Controls.Add(pnlList);

            Action loadOrders = null;
            loadOrders = () =>
            {
                pnlList.Controls.Clear();
                int yPos = 0;

                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        using (var cmd = new SqlCommand(
                            "SELECT oh.OrderHeaderID, u.FirstName+' '+u.LastName AS Customer, " +
                            "oh.TotalPrice, oh.OrderDate, oh.Status " +
                            "FROM OrderHeaders oh " +
                            "JOIN Users u ON oh.UserID = u.UserID " +
                            "ORDER BY oh.OrderDate DESC", con))
                        using (var rd = cmd.ExecuteReader())
                        {
                            int row = 0;
                            while (rd.Read())
                            {
                                int orderHeaderID = Convert.ToInt32(rd["OrderHeaderID"]);
                                string st = rd["Status"].ToString();
                                Color sbg = st == "Delivered" ? Color.FromArgb(212, 237, 218) : st == "Processing" ? Color.FromArgb(210, 228, 248) : Color.FromArgb(255, 234, 167);
                                Color sfg = st == "Delivered" ? Color.FromArgb(21, 87, 36) : st == "Processing" ? Color.FromArgb(12, 62, 132) : Color.FromArgb(133, 77, 14);

                                // Order header row
                                var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 42), Location = new Point(0, yPos), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };

                                int rx = 0;
                                rowPnl.Controls.Add(new Label { Text = "#" + orderHeaderID, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = AccentGreen, AutoSize = false, Size = new Size(hw[0], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(5, 0, 0, 0) }); rx += hw[0];
                                rowPnl.Controls.Add(new Label { Text = rd["Customer"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[1], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[1];
                                rowPnl.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(rd["TotalPrice"]).ToString("N2"), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[2], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[2];
                                rowPnl.Controls.Add(new Label { Text = Convert.ToDateTime(rd["OrderDate"]).ToString("dd MMM yy"), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[3], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[3];
                                rowPnl.Controls.Add(new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = sfg, BackColor = sbg, AutoSize = true, Location = new Point(rx + 4, 12), Padding = new Padding(5, 2, 5, 2) }); rx += hw[4];

                                // Status dropdown
                                int capturedOrderID = orderHeaderID;
                                string capturedStatus = st;
                                var cbo = new ComboBox { Size = new Size(120, 24), Location = new Point(rx, 9), Font = new Font("Segoe UI", 8), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
                                cbo.Items.AddRange(new object[] { "Pending", "Processing", "Delivered", "Cancelled" });
                                cbo.SelectedItem = st;
                                cbo.SelectedIndexChanged += (s2, e2) =>
                                {
                                    try
                                    {
                                        using (var con2 = DBHelper.GetConnection())
                                        {
                                            con2.Open();
                                            using (var cmd2 = new SqlCommand("UPDATE OrderHeaders SET Status=@s WHERE OrderHeaderID=@id", con2))
                                            {
                                                cmd2.Parameters.AddWithValue("@s", cbo.SelectedItem.ToString());
                                                cmd2.Parameters.AddWithValue("@id", capturedOrderID);
                                                cmd2.ExecuteNonQuery();
                                            }
                                        }
                                        loadOrders();
                                    }
                                    catch { }
                                };
                                rowPnl.Controls.Add(cbo);
                                pnlList.Controls.Add(rowPnl);
                                yPos += 42;

                                // Order items
                                using (var con2 = DBHelper.GetConnection())
                                {
                                    con2.Open();
                                    using (var cmd2 = new SqlCommand("SELECT MedicineName, Quantity, UnitPrice, TotalPrice FROM OrderItems WHERE OrderHeaderID=@id", con2))
                                    {
                                        cmd2.Parameters.AddWithValue("@id", orderHeaderID);
                                        using (var rd2 = cmd2.ExecuteReader())
                                        {
                                            int itemRow = 0;
                                            while (rd2.Read())
                                            {
                                                var itemPnl = new Panel { Size = new Size(pnlList.Width - 20, 32), Location = new Point(0, yPos), BackColor = Color.FromArgb(240, 244, 242) };
                                                itemPnl.Controls.Add(new Label { Text = "     → " + rd2["MedicineName"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(260, 32), Location = new Point(hw[0], 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                                                itemPnl.Controls.Add(new Label { Text = "Qty: " + rd2["Quantity"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(100, 32), Location = new Point(hw[0] + 260, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                                                itemPnl.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(rd2["TotalPrice"]).ToString("N2"), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(100, 32), Location = new Point(hw[0] + 360, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                                                pnlList.Controls.Add(itemPnl);
                                                yPos += 32;
                                                itemRow++;
                                            }
                                        }
                                    }
                                }

                                // Spacer
                                yPos += 6;
                                row++;
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }

                pnlList.AutoScrollMinSize = new Size(0, yPos);
            };

            loadOrders();
        }

        private void ShowPrescriptions()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);
            pnlCard.Controls.Add(new Label { Text = "📋  Prescription Management", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            string[] hdrs = { "File Name", "Customer", "Upload Date", "Status", "Actions", "Delete" };
            int[] hw = { 200, 160, 130, 110, 160, 70 };
            int hx = 16;
            for (int i = 0; i < hdrs.Length; i++) { pnlCard.Controls.Add(new Label { Text = hdrs[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[i], 26), Location = new Point(hx, 52), BackColor = Color.Transparent }); hx += hw[i]; }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 96), Location = new Point(16, 82), BackColor = Color.Transparent, AutoScroll = true };
            pnlCard.Controls.Add(pnlList);

            Action loadPres = null;
            loadPres = () =>
            {
                pnlList.Controls.Clear();
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        using (var cmd = new SqlCommand("SELECT p.PrescriptionID, p.FileName, u.FirstName+' '+u.LastName AS Customer, p.UploadDate, p.Status FROM Prescriptions p JOIN Users u ON p.UserID=u.UserID ORDER BY p.UploadDate DESC", con))
                        using (var rd = cmd.ExecuteReader())
                        {
                            int row = 0;
                            while (rd.Read())
                            {
                                int presID = Convert.ToInt32(rd["PrescriptionID"]);
                                string st = rd["Status"].ToString();
                                Color sbg = st == "Approved" ? Color.FromArgb(212, 237, 218) : st == "Rejected" ? Color.FromArgb(250, 215, 215) : Color.FromArgb(255, 234, 167);
                                Color sfg = st == "Approved" ? Color.FromArgb(21, 87, 36) : st == "Rejected" ? Color.FromArgb(120, 30, 30) : Color.FromArgb(133, 77, 14);

                                var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 42), Location = new Point(0, row * 48), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };
                                int rx6 = 0;
                                string[] vals = { "📄 " + rd["FileName"].ToString(), rd["Customer"].ToString(), Convert.ToDateTime(rd["UploadDate"]).ToString("dd MMM yyyy") };
                                for (int c2 = 0; c2 < 3; c2++) { rowPnl.Controls.Add(new Label { Text = vals[c2], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c2], 42), Location = new Point(rx6, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(c2 == 0 ? 5 : 0, 0, 0, 0) }); rx6 += hw[c2]; }
                                rowPnl.Controls.Add(new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = sfg, BackColor = sbg, AutoSize = true, Location = new Point(rx6 + 4, 12), Padding = new Padding(5, 2, 5, 2) }); rx6 += hw[3];

                                int capturedID = presID;
                                var btnApprove = new Button { Text = "Approve", Size = new Size(70, 26), Location = new Point(rx6, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                                btnApprove.FlatAppearance.BorderSize = 0;
                                btnApprove.Click += (s2, e2) => { try { using (var con2 = DBHelper.GetConnection()) { con2.Open(); new SqlCommand($"UPDATE Prescriptions SET Status='Approved' WHERE PrescriptionID={capturedID}", con2).ExecuteNonQuery(); } loadPres(); } catch { } };

                                var btnReject = new Button { Text = "Reject", Size = new Size(65, 26), Location = new Point(rx6 + 76, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                                btnReject.FlatAppearance.BorderSize = 0;
                                btnReject.Click += (s2, e2) => { try { using (var con2 = DBHelper.GetConnection()) { con2.Open(); new SqlCommand($"UPDATE Prescriptions SET Status='Rejected' WHERE PrescriptionID={capturedID}", con2).ExecuteNonQuery(); } loadPres(); } catch { } };

                                rowPnl.Controls.Add(btnApprove);
                                rowPnl.Controls.Add(btnReject);


                                var btnDelPres = new Button { Text = "Delete", Size = new Size(62, 26), Location = new Point(rx6 + 147, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                                btnDelPres.FlatAppearance.BorderSize = 0;
                                btnDelPres.Click += (s2, e2) =>
                                {
                                    if (MessageBox.Show("Delete this prescription?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            using (var con2 = DBHelper.GetConnection()) { con2.Open(); new SqlCommand($"DELETE FROM Prescriptions WHERE PrescriptionID={capturedID}", con2).ExecuteNonQuery(); }
                                            loadPres();
                                        }
                                        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                                    }
                                };
                                rowPnl.Controls.Add(btnDelPres);
                                pnlList.Controls.Add(rowPnl);
                                row++;
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            };
            loadPres();
        }

        private void ShowUsers()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);
            pnlCard.Controls.Add(new Label { Text = "👥  User Management", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            string[] hdrs = { "Name", "Username", "Email", "Phone", "Role", "Action" };
            int[] hw = { 150, 120, 180, 120, 100, 90 };
            int hx = 16;
            for (int i = 0; i < hdrs.Length; i++) { pnlCard.Controls.Add(new Label { Text = hdrs[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[i], 26), Location = new Point(hx, 52), BackColor = Color.Transparent }); hx += hw[i]; }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 96), Location = new Point(16, 82), BackColor = Color.Transparent, AutoScroll = true };
            pnlCard.Controls.Add(pnlList);

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT UserID, FirstName, LastName, Username, Email, Phone, Role FROM Users ORDER BY UserID", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            int userID = Convert.ToInt32(rd["UserID"]);
                            string uname = rd["Username"].ToString();
                            var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 42), Location = new Point(0, row * 48), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };
                            int rx7 = 0;
                            string[] vals = { rd["FirstName"] + " " + rd["LastName"], uname, rd["Email"].ToString(), rd["Phone"].ToString(), rd["Role"].ToString() };
                            for (int c2 = 0; c2 < vals.Length; c2++) { rowPnl.Controls.Add(new Label { Text = vals[c2], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c2], 42), Location = new Point(rx7, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(c2 == 0 ? 5 : 0, 0, 0, 0) }); rx7 += hw[c2]; }

                            int capturedID = userID; string capturedUname = uname;
                            var btnDel = new Button { Text = "Delete", Size = new Size(76, 26), Location = new Point(rx7, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                            btnDel.FlatAppearance.BorderSize = 0;
                            btnDel.Click += (s2, e2) =>
                            {
                                if (capturedUname == AdminUsername) { MessageBox.Show("You cannot delete your own account!"); return; }
                                if (MessageBox.Show($"Delete user {capturedUname}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                { try { using (var con2 = DBHelper.GetConnection()) { con2.Open(); new SqlCommand($"DELETE FROM Users WHERE UserID={capturedID}", con2).ExecuteNonQuery(); } ShowUsers(); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); } }
                            };
                            rowPnl.Controls.Add(btnDel);
                            pnlList.Controls.Add(rowPnl);
                            row++;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
        }

        private void ShowReports()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);
            pnlCard.Controls.Add(new Label { Text = "📊  Reports & Analytics", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            // ── Stat cards ────────────────────────────────────────────────────
            string[] stTitles = { "Total Revenue", "Delivered Orders", "Pending Orders", "Total Medicines" };
            string[] stVals = { "৳0", "0", "0", "0" };

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();

                    // Total revenue — from OrderHeaders where Delivered
                    stVals[0] = "৳" + Convert.ToDecimal(
                        new SqlCommand(
                            "SELECT ISNULL(SUM(TotalPrice),0) FROM OrderHeaders WHERE Status='Delivered'",
                            con).ExecuteScalar()).ToString("N0");

                    // Delivered orders count
                    stVals[1] = new SqlCommand(
                        "SELECT COUNT(*) FROM OrderHeaders WHERE Status='Delivered'",
                        con).ExecuteScalar().ToString();

                    // Pending orders count
                    stVals[2] = new SqlCommand(
                        "SELECT COUNT(*) FROM OrderHeaders WHERE Status='Pending'",
                        con).ExecuteScalar().ToString();

                    // Total medicines
                    stVals[3] = new SqlCommand(
                        "SELECT COUNT(*) FROM Medicines",
                        con).ExecuteScalar().ToString();
                }
            }
            catch { }

            Color[] stColors = { AccentGreen, AccentBlue, AccentAmber, AccentRed };
            int stW = (w - 60) / 4;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var card = new Panel { Size = new Size(stW, 80), Location = new Point(16 + i * (stW + 12), 52), BackColor = Color.FromArgb(245, 248, 246) };
                card.Controls.Add(new Panel { Size = new Size(4, 80), Location = new Point(0, 0), BackColor = stColors[idx] });
                card.Controls.Add(new Label { Text = stTitles[idx], Font = new Font("Segoe UI", 8), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(14, 14) });
                card.Controls.Add(new Label { Text = stVals[idx], Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(14, 36) });
                pnlCard.Controls.Add(card);
            }

            // ── Top Selling Medicines header ──────────────────────────────────
            pnlCard.Controls.Add(new Label { Text = "Top Selling Medicines", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 148) });

            string[] hdrs2 = { "Medicine Name", "Total Orders", "Total Qty Sold", "Total Revenue" };
            int[] hw2 = { 260, 160, 160, 160 };
            int hx2 = 16;
            for (int i = 0; i < hdrs2.Length; i++)
            {
                pnlCard.Controls.Add(new Label
                {
                    Text = hdrs2[i],
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = TextGray,
                    AutoSize = false,
                    Size = new Size(hw2[i], 24),
                    Location = new Point(hx2, 176),
                    BackColor = Color.Transparent
                });
                hx2 += hw2[i];
            }

            // ── Top Selling rows — query from OrderItems + OrderHeaders ───────
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();

                    // Group by MedicineName from OrderItems
                    // Join OrderHeaders to count only non-Cancelled orders
                    string sql = @"
                SELECT
                    oi.MedicineName,
                    COUNT(DISTINCT oi.OrderHeaderID)  AS TotalOrders,
                    SUM(oi.Quantity)                  AS TotalQty,
                    SUM(oi.TotalPrice)                AS TotalRevenue
                FROM OrderItems oi
                INNER JOIN OrderHeaders oh ON oi.OrderHeaderID = oh.OrderHeaderID
                WHERE oh.Status != 'Cancelled'
                GROUP BY oi.MedicineName
                ORDER BY TotalRevenue DESC";

                    using (var cmd = new SqlCommand(sql, con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            int ry = 202 + row * 38;

                            string[] vals =
                            {
                        rd["MedicineName"].ToString(),
                        rd["TotalOrders"].ToString()  + " orders",
                        rd["TotalQty"].ToString()     + " units",
                        "৳" + Convert.ToDecimal(rd["TotalRevenue"]).ToString("N0")
                    };

                            int rx = 16;
                            Color rowBg = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg;

                            for (int c = 0; c < vals.Length; c++)
                            {
                                bool isRevenue = (c == 3);
                                pnlCard.Controls.Add(new Label
                                {
                                    Text = vals[c],
                                    Font = new Font("Segoe UI", 10, isRevenue ? FontStyle.Bold : FontStyle.Regular),
                                    ForeColor = isRevenue ? AccentGreen : TextDark,
                                    AutoSize = false,
                                    Size = new Size(hw2[c], 34),
                                    Location = new Point(rx, ry),
                                    BackColor = rowBg,
                                    TextAlign = ContentAlignment.MiddleLeft,
                                    Padding = new Padding(c == 0 ? 5 : 0, 0, 0, 0)
                                });
                                rx += hw2[c];
                            }
                            row++;
                        }

                        // Show message if no data
                        if (row == 0)
                        {
                            pnlCard.Controls.Add(new Label
                            {
                                Text = "No order data available yet.",
                                Font = new Font("Segoe UI", 10),
                                ForeColor = TextGray,
                                AutoSize = true,
                                BackColor = Color.Transparent,
                                Location = new Point(16, 210)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reports DB Error: " + ex.Message);
            }
        }

        private void ShowProfile()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel
            {
                Size = new Size(w, pnlContent.Height - pad * 2),
                Location = new Point(pad, pad),
                BackColor = CardBg,
                AutoScroll = true
            };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "👤  My Profile", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            Color BorderColor = Color.FromArgb(200, 210, 205);

            string[] labels = { "First Name", "Last Name", "Username", "Email", "Phone" };
            string[] values = { "", "", AdminUsername, "", "" };

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT FirstName, LastName, Username, Email, Phone FROM Users WHERE Username=@u", con))
                    {
                        cmd.Parameters.AddWithValue("@u", AdminUsername);
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                values[0] = rd["FirstName"].ToString();
                                values[1] = rd["LastName"].ToString();
                                values[2] = rd["Username"].ToString();
                                values[3] = rd["Email"].ToString();
                                values[4] = rd["Phone"].ToString();
                            }
                        }
                    }
                }
            }
            catch { }

            var txtBoxes = new TextBox[labels.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                pnlCard.Controls.Add(new Label { Text = labels[idx].ToUpper(), Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 58 + i * 62) });
                var txt = new TextBox { Text = values[idx], Font = new Font("Segoe UI", 11), Size = new Size(400, 32), Location = new Point(16, 78 + i * 62), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(245, 247, 246), ForeColor = TextDark };
                pnlCard.Controls.Add(new Panel { Size = new Size(400, 1), Location = new Point(16, 110 + i * 62), BackColor = BorderColor });
                txtBoxes[idx] = txt;
                pnlCard.Controls.Add(txt);
            }

            var btnSave = new Button { Text = "Save Changes", Size = new Size(160, 44), Location = new Point(16, 390), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                string newFirst = txtBoxes[0].Text.Trim();
                string newLast = txtBoxes[1].Text.Trim();
                string newUsername = txtBoxes[2].Text.Trim();
                string newEmail = txtBoxes[3].Text.Trim();
                string newPhone = txtBoxes[4].Text.Trim();

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    MessageBox.Show("Username cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        if (newUsername != AdminUsername)
                        {
                            using (var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", con))
                            {
                                check.Parameters.AddWithValue("@u", newUsername);
                                if ((int)check.ExecuteScalar() > 0)
                                {
                                    MessageBox.Show("Username already taken. Please choose another.", "Username Taken", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        using (var cmd = new SqlCommand("UPDATE Users SET FirstName=@fn, LastName=@ln, Username=@un, Email=@em, Phone=@ph WHERE Username=@old", con))
                        {
                            cmd.Parameters.AddWithValue("@fn", newFirst);
                            cmd.Parameters.AddWithValue("@ln", newLast);
                            cmd.Parameters.AddWithValue("@un", newUsername);
                            cmd.Parameters.AddWithValue("@em", newEmail);
                            cmd.Parameters.AddWithValue("@ph", newPhone);
                            cmd.Parameters.AddWithValue("@old", AdminUsername);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    AdminUsername = newUsername;


                    foreach (Control c in pnlSidebar.Controls)
                        if (c is Label lbl && lbl.Font.Bold && lbl.ForeColor == Color.White && !lbl.Text.Contains("MediCare"))
                        { lbl.Text = AdminUsername; break; }

                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlCard.Controls.Add(btnSave);

            var btnChangePw = new Button { Text = "Change Password", Size = new Size(160, 44), Location = new Point(190, 390), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, BackColor = SidebarBg, Cursor = Cursors.Hand };
            btnChangePw.FlatAppearance.BorderSize = 0;
            btnChangePw.Click += (s, e) =>
            {
                var pnlChangePw = new Panel { Size = new Size(w - 32, 230), Location = new Point(16, 450), BackColor = Color.FromArgb(245, 247, 246) };
                pnlCard.AutoScrollMinSize = new Size(0, 710);
                pnlCard.Controls.Add(pnlChangePw);

                var lblCurrent = new Label { Text = "CURRENT PASSWORD", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) };
                var txtCurrent = new TextBox { Size = new Size(400, 28), Location = new Point(10, 28), BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(245, 247, 246), PasswordChar = '•' };
                var lineCurrent = new Panel { Size = new Size(400, 1), Location = new Point(10, 58), BackColor = BorderColor };

                var lblNew = new Label { Text = "NEW PASSWORD", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 70) };
                var txtNew = new TextBox { Size = new Size(400, 28), Location = new Point(10, 88), BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(245, 247, 246), PasswordChar = '•' };
                var lineNew = new Panel { Size = new Size(400, 1), Location = new Point(10, 118), BackColor = BorderColor };

                var lblConfirm = new Label { Text = "CONFIRM NEW PASSWORD", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 130) };
                var txtConfirm = new TextBox { Size = new Size(400, 28), Location = new Point(10, 148), BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(245, 247, 246), PasswordChar = '•' };
                var lineConfirm = new Panel { Size = new Size(400, 1), Location = new Point(10, 178), BackColor = BorderColor };

                var btnUpdate = new Button { Text = "Update Password", Size = new Size(160, 36), Location = new Point(10, 186), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                btnUpdate.FlatAppearance.BorderSize = 0;
                btnUpdate.Click += (s2, e2) =>
                {
                    if (string.IsNullOrWhiteSpace(txtCurrent.Text) || string.IsNullOrWhiteSpace(txtNew.Text) || string.IsNullOrWhiteSpace(txtConfirm.Text))
                    { MessageBox.Show("Fill all password fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (txtNew.Text != txtConfirm.Text)
                    { MessageBox.Show("New passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    DataRow user = DBHelper.LoginUser(AdminUsername, txtCurrent.Text);
                    if (user == null) { MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                    int uid = DBHelper.GetUserID(AdminUsername);
                    bool ok = DBHelper.UpdatePassword(uid, txtNew.Text);
                    if (ok)
                    {
                        MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtCurrent.Text = ""; txtNew.Text = ""; txtConfirm.Text = "";
                    }
                };

                pnlChangePw.Controls.AddRange(new Control[] { lblCurrent, txtCurrent, lineCurrent, lblNew, txtNew, lineNew, lblConfirm, txtConfirm, lineConfirm, btnUpdate });
            };
            pnlCard.Controls.Add(btnChangePw);
        }

        private void DrawCard(Graphics g, Panel card)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 10))
            using (var b = new SolidBrush(card.BackColor))
            {
                g.FillPath(b, path);
                using (var p = new Pen(Color.FromArgb(220, 225, 222), 1))
                    g.DrawPath(p, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}