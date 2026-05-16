using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class PharmacistDashboard : Form
    {
        private static readonly Color SidebarBg = Color.FromArgb(22, 88, 76);
        private static readonly Color SidebarHover = Color.FromArgb(30, 110, 95);
        private static readonly Color SidebarText = Color.FromArgb(160, 210, 195);
        private static readonly Color AccentGreen = Color.FromArgb(0, 184, 148);
        private static readonly Color AccentRed = Color.FromArgb(214, 48, 49);
        private static readonly Color AccentBlue = Color.FromArgb(9, 132, 227);
        private static readonly Color AccentAmber = Color.FromArgb(253, 203, 110);
        private static readonly Color BgColor = Color.FromArgb(230, 240, 237);
        private static readonly Color CardBg = Color.White;
        private static readonly Color TextDark = Color.FromArgb(30, 35, 33);
        private static readonly Color TextGray = Color.FromArgb(120, 130, 125);


        private Panel pnlSidebar, pnlMain, pnlContent;
        public string LoggedInUsername { get; set; }
        public string LoggedInFirstName { get; set; }
        public string LoggedInLastName { get; set; }
        public int LoggedInUserID { get; set; }

        public PharmacistDashboard(string firstName, string lastName, string username)
        {
            LoggedInFirstName = firstName;
            LoggedInLastName = lastName;
            LoggedInUsername = username;
            LoggedInUserID = DBHelper.GetUserID(username);
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MediCare Pharmacy – Pharmacist";
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

            var pnlLogo = new Panel { Size = new Size(220, 64), Location = new Point(0, 0), BackColor = Color.FromArgb(15, 65, 56) };
            pnlLogo.Controls.Add(new Label { Text = "MediCare", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 12) });
            pnlLogo.Controls.Add(new Label { Text = "Pharmacist Panel", Font = new Font("Segoe UI", 9), ForeColor = AccentGreen, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 38) });
            pnlSidebar.Controls.Add(pnlLogo);

            var pnlAvatar = new Panel { Size = new Size(44, 44), Location = new Point(20, 80), BackColor = Color.Transparent };
            pnlAvatar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(AccentGreen), 0, 0, 44, 44);
                string initials = (LoggedInFirstName?.Length > 0 ? LoggedInFirstName[0].ToString().ToUpper() : "") +
                                  (LoggedInLastName?.Length > 0 ? LoggedInLastName[0].ToString().ToUpper() : "");
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(initials, new Font("Segoe UI", 13, FontStyle.Bold), Brushes.White, new RectangleF(0, 0, 44, 44), sf);
            };
            pnlSidebar.Controls.Add(pnlAvatar);
            pnlSidebar.Controls.Add(new Label { Text = LoggedInFirstName + " " + LoggedInLastName, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(72, 86) });
            pnlSidebar.Controls.Add(new Label { Text = "Pharmacist", Font = new Font("Segoe UI", 8), ForeColor = SidebarText, AutoSize = true, BackColor = Color.Transparent, Location = new Point(72, 106) });
            pnlSidebar.Controls.Add(new Panel { Size = new Size(180, 1), Location = new Point(20, 136), BackColor = Color.FromArgb(30, 110, 90) });

            string[] menus = { "🏠  Home", "💊  Inventory", "📦  Orders", "📋  Prescriptions", "👤  My Profile" };
            int menuY = 150;
            foreach (var menu in menus) { pnlSidebar.Controls.Add(CreateMenuItem(menu, menuY)); menuY += 46; }

            pnlSidebar.Controls.Add(new Panel { Size = new Size(180, 1), Location = new Point(20, this.ClientSize.Height - 80), BackColor = Color.FromArgb(30, 110, 90) });

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
            else if (tag.Contains("Inventory")) ShowInventory();
            else if (tag.Contains("Order")) ShowOrders();
            else if (tag.Contains("Prescript")) ShowPrescriptions();
            else if (tag.Contains("Profile")) ShowProfile();
        }

        private void BuildMain()
        {
            pnlMain = new Panel { Size = new Size(this.ClientSize.Width - 220, this.ClientSize.Height), Location = new Point(220, 0), BackColor = BgColor };
            this.Controls.Add(pnlMain);

            var pnlTop = new Panel { Size = new Size(pnlMain.Width, 60), Location = new Point(0, 0), BackColor = CardBg };
            pnlTop.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 225, 222)), 0, 59, pnlTop.Width, 59);
            pnlMain.Controls.Add(pnlTop);
            pnlTop.Controls.Add(new Label { Text = "Welcome, " + LoggedInFirstName + "! 👋", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 10) });
            pnlTop.Controls.Add(new Label { Text = DateTime.Now.ToString("dddd, d MMMM yyyy"), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(20, 34) });

            pnlContent = new Panel { Size = new Size(pnlMain.Width, pnlMain.Height - 60), Location = new Point(0, 60), BackColor = BgColor, AutoScroll = true };
            pnlMain.Controls.Add(pnlContent);
        }

        private void ClearContent() => pnlContent.Controls.Clear();

        // ── HOME ───────────────────────────────────────────────────────
        private void ShowHome()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            int totalMeds = 0, lowStock = 0, pendingOrders = 0, pendingPres = 0;
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    totalMeds = (int)new SqlCommand("SELECT COUNT(*) FROM Medicines", con).ExecuteScalar();
                    lowStock = (int)new SqlCommand("SELECT COUNT(*) FROM Medicines WHERE Stock <= 20", con).ExecuteScalar();
                    pendingOrders = (int)new SqlCommand("SELECT COUNT(*) FROM OrderHeaders WHERE Status='Pending'", con).ExecuteScalar();
                    pendingPres = (int)new SqlCommand("SELECT COUNT(*) FROM Prescriptions WHERE Status='Pending'", con).ExecuteScalar();
                }
            }
            catch { }

            string[] titles = { "Total Medicines", "Low Stock", "Pending Orders", "Pending Prescriptions" };
            string[] values = { totalMeds.ToString(), lowStock.ToString(), pendingOrders.ToString(), pendingPres.ToString() };
            Color[] accents = { AccentGreen, AccentRed, AccentAmber, AccentBlue };

            int cardW = (w - 30) / 4;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var card = new Panel { Size = new Size(cardW, 90), Location = new Point(pad + i * (cardW + 10), pad), BackColor = CardBg };
                card.Paint += (s, e) => DrawCard(e.Graphics, card);
                card.Controls.Add(new Panel { Size = new Size(4, 90), Location = new Point(0, 0), BackColor = accents[idx] });
                card.Controls.Add(new Label { Text = titles[idx], Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(14, 14) });
                card.Controls.Add(new Label { Text = values[idx], Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(14, 36) });
                pnlContent.Controls.Add(card);
            }

            int y2 = pad + 108;

            // ── FIX 1: Pending Orders panel — taller + scrollable ──────
            var pnlOrders = new Panel { Size = new Size((int)(w * 0.58), 260), Location = new Point(pad, y2), BackColor = CardBg };
            pnlOrders.Paint += (s, e) => DrawCard(e.Graphics, pnlOrders);
            pnlContent.Controls.Add(pnlOrders);
            pnlOrders.Controls.Add(new Label { Text = "📦  Pending Orders", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 12) });

            // Column headers
            string[] hdrs = { "Medicine", "Customer", "Qty", "Total" };
            int[] hw = { 160, 130, 50, 90 };
            int hx = 16;
            for (int i = 0; i < hdrs.Length; i++)
            {
                pnlOrders.Controls.Add(new Label { Text = hdrs[i], Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[i], 22), Location = new Point(hx, 40), BackColor = Color.Transparent });
                hx += hw[i];
            }

            // Scrollable list inside pending orders panel
            var pnlOrderList = new Panel { Size = new Size(pnlOrders.Width - 24, pnlOrders.Height - 70), Location = new Point(12, 66), BackColor = Color.Transparent, AutoScroll = true };
            pnlOrders.Controls.Add(pnlOrderList);

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT oi.MedicineName, u.FirstName+' '+u.LastName AS Cust, oi.Quantity, oi.TotalPrice " +
                        "FROM OrderItems oi " +
                        "JOIN OrderHeaders oh ON oi.OrderHeaderID = oh.OrderHeaderID " +
                        "JOIN Users u ON oh.UserID = u.UserID " +
                        "WHERE oh.Status='Pending' ORDER BY oh.OrderDate DESC", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            int ry = row * 38;
                            var rowPnl = new Panel { Size = new Size(pnlOrderList.Width - 4, 34), Location = new Point(0, ry), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };
                            int rx2 = 0;
                            string[] vals = { rd["MedicineName"].ToString(), rd["Cust"].ToString(), rd["Quantity"].ToString(), "৳" + Convert.ToDecimal(rd["TotalPrice"]).ToString("N0") };
                            for (int c = 0; c < 4; c++)
                            {
                                rowPnl.Controls.Add(new Label { Text = vals[c], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c], 34), Location = new Point(rx2, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(c == 0 ? 4 : 0, 0, 0, 0) });
                                rx2 += hw[c];
                            }
                            // Pending badge
                            rowPnl.Controls.Add(new Label { Text = "Pending", Font = new Font("Segoe UI", 7, FontStyle.Bold), ForeColor = Color.FromArgb(133, 77, 14), BackColor = Color.FromArgb(255, 234, 167), AutoSize = true, Location = new Point(rx2 + 4, 9), Padding = new Padding(4, 2, 4, 2) });
                            pnlOrderList.Controls.Add(rowPnl);
                            row++;
                        }

                        if (row == 0)
                            pnlOrderList.Controls.Add(new Label { Text = "No pending orders.", Font = new Font("Segoe UI", 10), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(4, 8) });

                        pnlOrderList.AutoScrollMinSize = new Size(0, row * 38);
                    }
                }
            }
            catch { }

            // Low Stock Alert
            var pnlStock = new Panel { Size = new Size(w - (int)(w * 0.58) - 10, 260), Location = new Point(pad + (int)(w * 0.58) + 10, y2), BackColor = CardBg };
            pnlStock.Paint += (s, e) => DrawCard(e.Graphics, pnlStock);
            pnlContent.Controls.Add(pnlStock);
            pnlStock.Controls.Add(new Label { Text = "Low Stock Alert 🚨", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) });

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT TOP 6 Name, Stock FROM Medicines WHERE Stock <= 20 ORDER BY Stock ASC", con))
                    using (var rd = cmd.ExecuteReader())
                    {
                        int row = 0;
                        while (rd.Read())
                        {
                            int stock = Convert.ToInt32(rd["Stock"]);
                            int ry = 44 + row * 30;
                            pnlStock.Controls.Add(new Label { Text = rd["Name"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(110, 24), Location = new Point(12, ry), BackColor = Color.Transparent });
                            var barBg = new Panel { Size = new Size(pnlStock.Width - 168, 8), Location = new Point(124, ry + 8), BackColor = Color.FromArgb(220, 225, 222) };
                            barBg.Controls.Add(new Panel { Size = new Size(Math.Max((int)((stock / 20.0) * (pnlStock.Width - 168)), 4), 8), Location = new Point(0, 0), BackColor = AccentRed });
                            pnlStock.Controls.Add(barBg);
                            pnlStock.Controls.Add(new Label { Text = stock.ToString(), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = AccentRed, AutoSize = true, BackColor = Color.Transparent, Location = new Point(pnlStock.Width - 32, ry + 2) });
                            row++;
                        }

                        if (row == 0)
                            pnlStock.Controls.Add(new Label { Text = "All medicines are well stocked! ✅", Font = new Font("Segoe UI", 10), ForeColor = AccentGreen, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 50) });
                    }
                }
            }
            catch { }

            // Quick Actions
            int y3 = y2 + 278;
            var pnlQuick = new Panel { Size = new Size(w, 106), Location = new Point(pad, y3), BackColor = CardBg };
            pnlQuick.Paint += (s, e) => DrawCard(e.Graphics, pnlQuick);
            pnlContent.Controls.Add(pnlQuick);
            pnlQuick.Controls.Add(new Label { Text = "⚡  Quick Actions", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(14, 12) });
            pnlQuick.Controls.Add(new Panel { Size = new Size(w - 28, 1), Location = new Point(14, 38), BackColor = Color.FromArgb(220, 225, 222) });

            string[] actions = { "View Inventory", "Process Orders", "Review Prescriptions", "My Profile" };
            Color[] actClr = { AccentGreen, AccentBlue, AccentAmber, Color.FromArgb(108, 92, 231) };
            for (int i = 0; i < actions.Length; i++)
            {
                int idx = i;
                var btn = new Button { Text = actions[idx], Size = new Size((w - 60) / 4, 44), Location = new Point(12 + idx * ((w - 60) / 4 + 12), 48), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = actClr[idx], Cursor = Cursors.Hand };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) =>
                {
                    if (actions[idx].Contains("Inventory")) ShowInventory();
                    else if (actions[idx].Contains("Orders")) ShowOrders();
                    else if (actions[idx].Contains("Prescription")) ShowPrescriptions();
                    else ShowProfile();
                };
                pnlQuick.Controls.Add(btn);
            }

            pnlContent.AutoScrollMinSize = new Size(0, y3 + 150);
        }

        // ── INVENTORY ──────────────────────────────────────────────────
        private void ShowInventory()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "💊  Medicine Inventory", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            var txtSearch = new TextBox { Size = new Size(260, 30), Location = new Point(16, 54), Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, Text = "Search medicine..." };
            pnlCard.Controls.Add(txtSearch);

            var btnSearch = new Button { Text = "Search", Size = new Size(80, 30), Location = new Point(284, 54), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnSearch.FlatAppearance.BorderSize = 0;
            pnlCard.Controls.Add(btnSearch);

            // ── FIX 2: Adjusted column widths so Delete button is fully visible ──
            // Total = 56+185+130+80+70+100+82+68 = 771 fits in ~800px list width
            string[] hdrs = { "Image", "Medicine Name", "Category", "Price (৳)", "Stock", "Status", "Action", "Delete" };
            int[] hw = { 56, 185, 130, 80, 70, 100, 82, 68 };
            int hx = 16;
            foreach (var (h, hwidth) in System.Linq.Enumerable.Zip(hdrs, hw, (a, b) => (a, b)))
            {
                pnlCard.Controls.Add(new Label { Text = h, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hwidth, 26), Location = new Point(hx, 96), BackColor = Color.Transparent });
                hx += hwidth;
            }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 140), Location = new Point(16, 128), BackColor = Color.Transparent, AutoScroll = true };
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
                                     (string.IsNullOrEmpty(search) ? "" : " WHERE Name LIKE @s OR Category LIKE @s") +
                                     " ORDER BY Stock ASC";
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
                                    decimal price = Convert.ToDecimal(rd["Price"]);
                                    int stock = Convert.ToInt32(rd["Stock"]);
                                    string imagePath = rd["ImagePath"].ToString();

                                    string statusTxt = stock == 0 ? "Out of Stock" : stock <= 20 ? "Low Stock" : "In Stock";
                                    Color statusBg = stock == 0 ? Color.FromArgb(250, 215, 215) : stock <= 20 ? Color.FromArgb(255, 234, 167) : Color.FromArgb(212, 237, 218);
                                    Color statusFg = stock == 0 ? Color.FromArgb(120, 30, 30) : stock <= 20 ? Color.FromArgb(133, 77, 14) : Color.FromArgb(21, 87, 36);
                                    Color stockClr = stock <= 20 ? AccentRed : AccentGreen;

                                    var rowPnl = new Panel { Size = new Size(pnlList.Width - 4, 52), Location = new Point(0, row * 58), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };
                                    int rx = 0;

                                    // Image
                                    var pic = new PictureBox { Size = new Size(44, 44), Location = new Point(rx + 6, 4), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(240, 245, 243) };
                                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                        try { pic.Image = Image.FromFile(imagePath); } catch { }
                                    if (pic.Image == null)
                                        pic.Paint += (s2, e2) =>
                                        {
                                            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                                                e2.Graphics.DrawString("💊", new Font("Segoe UI", 16), new SolidBrush(TextGray), new RectangleF(0, 0, pic.Width, pic.Height), sf);
                                        };
                                    rowPnl.Controls.Add(pic);
                                    rx += hw[0];

                                    rowPnl.Controls.Add(new Label { Text = name, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[1], 52), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(4, 0, 0, 0) }); rx += hw[1];
                                    rowPnl.Controls.Add(new Label { Text = cat, Font = new Font("Segoe UI", 10), ForeColor = TextGray, AutoSize = false, Size = new Size(hw[2], 52), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[2];
                                    rowPnl.Controls.Add(new Label { Text = price.ToString("N2"), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[3], 52), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[3];
                                    rowPnl.Controls.Add(new Label { Text = stock.ToString(), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = stockClr, AutoSize = false, Size = new Size(hw[4], 52), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft }); rx += hw[4];
                                    rowPnl.Controls.Add(new Label { Text = statusTxt, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = statusFg, BackColor = statusBg, AutoSize = true, Location = new Point(rx + 4, 17), Padding = new Padding(5, 2, 5, 2) }); rx += hw[5];

                                    int capturedID = medID; string capturedName = name; int capturedStock = stock;

                                    // Restock button
                                    var btnRestock = new Button { Text = "Restock", Size = new Size(74, 28), Location = new Point(rx, 12), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                                    btnRestock.FlatAppearance.BorderSize = 0;
                                    btnRestock.Click += (s, e) =>
                                    {
                                        var f = new Form { Text = "Restock - " + capturedName, Size = new Size(320, 210), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, BackColor = Color.White };
                                        f.Controls.Add(new Label { Text = $"Current stock: {capturedStock}", Font = new Font("Segoe UI", 10), Location = new Point(20, 16), AutoSize = true });
                                        f.Controls.Add(new Label { Text = "Add quantity:", Font = new Font("Segoe UI", 10), Location = new Point(20, 40), AutoSize = true });
                                        var numQ = new NumericUpDown { Minimum = 1, Maximum = 10000, Value = 10, Font = new Font("Segoe UI", 11), Size = new Size(100, 32), Location = new Point(20, 68) };
                                        f.Controls.Add(numQ);
                                        var btnOk = new Button { Text = "Add Stock", Size = new Size(130, 38), Location = new Point(20, 116), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                                        btnOk.FlatAppearance.BorderSize = 0;
                                        btnOk.Click += (s2, e2) =>
                                        {
                                            try
                                            {
                                                using (var con2 = DBHelper.GetConnection())
                                                {
                                                    con2.Open();
                                                    using (var cmd2 = new SqlCommand("UPDATE Medicines SET Stock = Stock + @qty WHERE MedicineID = @id", con2))
                                                    { cmd2.Parameters.AddWithValue("@qty", (int)numQ.Value); cmd2.Parameters.AddWithValue("@id", capturedID); cmd2.ExecuteNonQuery(); }
                                                }
                                                MessageBox.Show($"Added {numQ.Value} units to {capturedName}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                f.Close();
                                                loadMeds(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim());
                                            }
                                            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                                        };
                                        f.Controls.Add(btnOk);
                                        f.ShowDialog(this);
                                    };
                                    rowPnl.Controls.Add(btnRestock);
                                    rx += hw[6];

                                    // Delete button — fully visible now
                                    var btnDel = new Button { Text = "Delete", Size = new Size(60, 28), Location = new Point(rx, 12), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                                    btnDel.FlatAppearance.BorderSize = 0;
                                    btnDel.Click += (s, e) =>
                                    {
                                        if (MessageBox.Show($"Delete {capturedName}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                        {
                                            try
                                            {
                                                using (var con2 = DBHelper.GetConnection())
                                                { con2.Open(); new SqlCommand($"DELETE FROM Medicines WHERE MedicineID={capturedID}", con2).ExecuteNonQuery(); }
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
            btnSearch.Click += (s, e) => loadMeds(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim());
        }

        // ── ORDERS ─────────────────────────────────────────────────────
        private void ShowOrders()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "📦  Order Processing", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            string[] filters = { "All", "Pending", "Processing", "Delivered" };
            Color[] fClrs = { TextGray, Color.FromArgb(133, 77, 14), AccentBlue, AccentGreen };
            for (int i = 0; i < filters.Length; i++)
            {
                int idx = i;
                var btn = new Button { Text = filters[idx], Size = new Size(90, 28), Location = new Point(16 + idx * 100, 52), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9), ForeColor = Color.White, BackColor = fClrs[idx], Cursor = Cursors.Hand };
                btn.FlatAppearance.BorderSize = 0;
                pnlCard.Controls.Add(btn);
            }

            string[] hdrs = { "Medicine", "Customer", "Qty", "Total", "Date", "Status", "Update" };
            int[] hw = { 150, 130, 50, 80, 100, 100, 120 };
            int hx = 16;
            foreach (var (h, hwidth) in System.Linq.Enumerable.Zip(hdrs, hw, (a, b) => (a, b)))
            {
                pnlCard.Controls.Add(new Label { Text = h, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hwidth, 26), Location = new Point(hx, 92), BackColor = Color.Transparent });
                hx += hwidth;
            }

            var pnlList = new Panel { Size = new Size(w - 32, pnlCard.Height - 136), Location = new Point(16, 122), BackColor = Color.Transparent, AutoScroll = true };
            pnlCard.Controls.Add(pnlList);

            Action<string> loadOrders = null;
            loadOrders = (filter) =>
            {
                pnlList.Controls.Clear();
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        string sql = "SELECT oh.OrderHeaderID, oi.MedicineName, u.FirstName+' '+u.LastName AS Cust, oi.Quantity, oi.TotalPrice, oh.OrderDate, oh.Status " +
                                     "FROM OrderItems oi " +
                                     "JOIN OrderHeaders oh ON oi.OrderHeaderID = oh.OrderHeaderID " +
                                     "JOIN Users u ON oh.UserID = u.UserID" +
                                     (filter == "All" ? "" : " WHERE oh.Status=@f") +
                                     " ORDER BY oh.OrderDate DESC";
                        using (var cmd = new SqlCommand(sql, con))
                        {
                            if (filter != "All") cmd.Parameters.AddWithValue("@f", filter);
                            using (var rd = cmd.ExecuteReader())
                            {
                                int row = 0;
                                while (rd.Read())
                                {
                                    int orderHeaderID = Convert.ToInt32(rd["OrderHeaderID"]);
                                    string st = rd["Status"].ToString();
                                    Color sbg = st == "Delivered" ? Color.FromArgb(212, 237, 218) : st == "Processing" ? Color.FromArgb(210, 228, 248) : Color.FromArgb(255, 234, 167);
                                    Color sfg = st == "Delivered" ? Color.FromArgb(21, 87, 36) : st == "Processing" ? Color.FromArgb(12, 62, 132) : Color.FromArgb(133, 77, 14);

                                    var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 42), Location = new Point(0, row * 48), BackColor = row % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg };
                                    int rx = 0;
                                    string[] vals = { rd["MedicineName"].ToString(), rd["Cust"].ToString(), rd["Quantity"].ToString(), "৳" + Convert.ToDecimal(rd["TotalPrice"]).ToString("N0"), Convert.ToDateTime(rd["OrderDate"]).ToString("dd MMM yy") };
                                    for (int c = 0; c < 5; c++) { rowPnl.Controls.Add(new Label { Text = vals[c], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(c == 0 ? 5 : 0, 0, 0, 0) }); rx += hw[c]; }
                                    rowPnl.Controls.Add(new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = sfg, BackColor = sbg, AutoSize = true, Location = new Point(rx + 4, 12), Padding = new Padding(5, 2, 5, 2) }); rx += hw[5];

                                    int capturedOrderHeaderID = orderHeaderID;
                                    var cbo = new ComboBox { Size = new Size(110, 24), Location = new Point(rx, 9), Font = new Font("Segoe UI", 8), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
                                    cbo.Items.AddRange(new object[] { "Pending", "Processing", "Delivered" });
                                    cbo.SelectedItem = st;
                                    cbo.SelectedIndexChanged += (s2, e2) =>
                                    {
                                        try
                                        {
                                            using (var con2 = DBHelper.GetConnection())
                                            {
                                                con2.Open();
                                                using (var cmd2 = new SqlCommand("UPDATE OrderHeaders SET Status=@s WHERE OrderHeaderID=@id", con2))
                                                { cmd2.Parameters.AddWithValue("@s", cbo.SelectedItem.ToString()); cmd2.Parameters.AddWithValue("@id", capturedOrderHeaderID); cmd2.ExecuteNonQuery(); }
                                            }
                                            loadOrders(filter);
                                        }
                                        catch { }
                                    };
                                    rowPnl.Controls.Add(cbo);
                                    pnlList.Controls.Add(rowPnl);
                                    row++;
                                }

                                if (row == 0)
                                    pnlList.Controls.Add(new Label { Text = "No orders found.", Font = new Font("Segoe UI", 11), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) });
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            };

            loadOrders("All");

            foreach (Control c in pnlCard.Controls)
            {
                if (c is Button btn && System.Linq.Enumerable.Contains(new[] { "All", "Pending", "Processing", "Delivered" }, btn.Text))
                { string f = btn.Text; btn.Click += (s, e) => loadOrders(f); }
            }
        }

        // ── PRESCRIPTIONS ──────────────────────────────────────────────
        private void ShowPrescriptions()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "📋  Prescription Review", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            string[] hdrs = { "File Name", "Customer", "Upload Date", "Status", "Actions" };
            int[] hw = { 200, 160, 130, 110, 160 };
            int hx = 16;
            foreach (var (h, hwidth) in System.Linq.Enumerable.Zip(hdrs, hw, (a, b) => (a, b)))
            {
                pnlCard.Controls.Add(new Label { Text = h, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(hwidth, 26), Location = new Point(hx, 52), BackColor = Color.Transparent });
                hx += hwidth;
            }

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
                        using (var cmd = new SqlCommand(
                            "SELECT p.PrescriptionID, p.FileName, u.FirstName+' '+u.LastName AS Customer, p.UploadDate, p.Status " +
                            "FROM Prescriptions p JOIN Users u ON p.UserID=u.UserID ORDER BY p.UploadDate DESC", con))
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
                                int rx = 0;
                                string[] vals = { "📄 " + rd["FileName"].ToString(), rd["Customer"].ToString(), Convert.ToDateTime(rd["UploadDate"]).ToString("dd MMM yyyy") };
                                for (int c = 0; c < 3; c++) { rowPnl.Controls.Add(new Label { Text = vals[c], Font = new Font("Segoe UI", 9), ForeColor = TextDark, AutoSize = false, Size = new Size(hw[c], 42), Location = new Point(rx, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(c == 0 ? 5 : 0, 0, 0, 0) }); rx += hw[c]; }
                                rowPnl.Controls.Add(new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = sfg, BackColor = sbg, AutoSize = true, Location = new Point(rx + 4, 12), Padding = new Padding(5, 2, 5, 2) }); rx += hw[3];

                                int capturedID = presID;
                                var btnApprove = new Button { Text = "Approve", Size = new Size(70, 26), Location = new Point(rx, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                                btnApprove.FlatAppearance.BorderSize = 0;
                                btnApprove.Click += (s2, e2) => { try { using (var c2 = DBHelper.GetConnection()) { c2.Open(); new SqlCommand($"UPDATE Prescriptions SET Status='Approved' WHERE PrescriptionID={capturedID}", c2).ExecuteNonQuery(); } loadPres(); } catch { } };

                                var btnReject = new Button { Text = "Reject", Size = new Size(66, 26), Location = new Point(rx + 76, 8), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentRed, Cursor = Cursors.Hand };
                                btnReject.FlatAppearance.BorderSize = 0;
                                btnReject.Click += (s2, e2) => { try { using (var c2 = DBHelper.GetConnection()) { c2.Open(); new SqlCommand($"UPDATE Prescriptions SET Status='Rejected' WHERE PrescriptionID={capturedID}", c2).ExecuteNonQuery(); } loadPres(); } catch { } };

                                rowPnl.Controls.Add(btnApprove);
                                rowPnl.Controls.Add(btnReject);
                                pnlList.Controls.Add(rowPnl);
                                row++;
                            }

                            if (row == 0)
                                pnlList.Controls.Add(new Label { Text = "No prescriptions found.", Font = new Font("Segoe UI", 11), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            };
            loadPres();
        }

        // ── PROFILE ────────────────────────────────────────────────────
        private void ShowProfile()
        {
            ClearContent();
            int pad = 18, w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel { Size = new Size(w, pnlContent.Height - pad * 2), Location = new Point(pad, pad), BackColor = CardBg };
            pnlCard.Paint += (s, e) => DrawCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "👤  My Profile", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            string[] labels = { "First Name", "Last Name", "Username", "Email", "Phone" };
            string[] values = { "", "", LoggedInUsername, "", "" };

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT FirstName,LastName,Username,Email,Phone FROM Users WHERE Username=@u", con))
                    {
                        cmd.Parameters.AddWithValue("@u", LoggedInUsername);
                        using (var rd = cmd.ExecuteReader())
                            if (rd.Read()) { values[0] = rd["FirstName"].ToString(); values[1] = rd["LastName"].ToString(); values[2] = rd["Username"].ToString(); values[3] = rd["Email"].ToString(); values[4] = rd["Phone"].ToString(); }
                    }
                }
            }
            catch { }

            var txtBoxes = new TextBox[5];
            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                pnlCard.Controls.Add(new Label { Text = labels[idx].ToUpper(), Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 56 + idx * 60) });
                txtBoxes[idx] = new TextBox { Text = values[idx], Font = new Font("Segoe UI", 11), Size = new Size(400, 30), Location = new Point(16, 74 + idx * 60), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(245, 247, 246), ForeColor = TextDark, ReadOnly = idx == 2 };
                pnlCard.Controls.Add(txtBoxes[idx]);
                pnlCard.Controls.Add(new Panel { Size = new Size(400, 1), Location = new Point(16, 106 + idx * 60), BackColor = Color.FromArgb(200, 210, 205) });
            }

            var btnSave = new Button { Text = "Save Changes", Size = new Size(150, 42), Location = new Point(16, 380), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        using (var cmd = new SqlCommand("UPDATE Users SET FirstName=@fn,LastName=@ln,Email=@em,Phone=@ph WHERE Username=@u", con))
                        {
                            cmd.Parameters.AddWithValue("@fn", txtBoxes[0].Text);
                            cmd.Parameters.AddWithValue("@ln", txtBoxes[1].Text);
                            cmd.Parameters.AddWithValue("@em", txtBoxes[3].Text);
                            cmd.Parameters.AddWithValue("@ph", txtBoxes[4].Text);
                            cmd.Parameters.AddWithValue("@u", LoggedInUsername);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            };
            pnlCard.Controls.Add(btnSave);

            var btnPw = new Button { Text = "Change Password", Size = new Size(160, 42), Location = new Point(180, 380), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = SidebarBg, Cursor = Cursors.Hand };
            btnPw.FlatAppearance.BorderSize = 0;
            btnPw.Click += (s, e) =>
            {
                var f = new Form { Text = "Change Password", Size = new Size(360, 260), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, BackColor = Color.White };
                string[] pwLbls = { "Current Password", "New Password", "Confirm New Password" };
                var pwBoxes = new TextBox[3];
                for (int i = 0; i < 3; i++)
                {
                    f.Controls.Add(new Label { Text = pwLbls[i], Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, Location = new Point(20, 16 + i * 60) });
                    pwBoxes[i] = new TextBox { Size = new Size(300, 28), Location = new Point(20, 34 + i * 60), Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '•' };
                    f.Controls.Add(pwBoxes[i]);
                }
                var btnUpdate = new Button { Text = "Update Password", Size = new Size(150, 36), Location = new Point(20, 202), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                btnUpdate.FlatAppearance.BorderSize = 0;
                btnUpdate.Click += (s2, e2) =>
                {
                    if (string.IsNullOrWhiteSpace(pwBoxes[0].Text) || string.IsNullOrWhiteSpace(pwBoxes[1].Text) || string.IsNullOrWhiteSpace(pwBoxes[2].Text)) { MessageBox.Show("Fill all fields."); return; }
                    if (pwBoxes[1].Text != pwBoxes[2].Text) { MessageBox.Show("New passwords do not match."); return; }
                    if (DBHelper.LoginUser(LoggedInUsername, pwBoxes[0].Text) == null) { MessageBox.Show("Current password is incorrect."); return; }
                    if (DBHelper.UpdatePassword(LoggedInUserID, pwBoxes[1].Text))
                    { MessageBox.Show("Password updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); f.Close(); }
                };
                f.Controls.Add(btnUpdate);
                f.ShowDialog(this);
            };
            pnlCard.Controls.Add(btnPw);
        }

        // ── Helpers ────────────────────────────────────────────────────
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