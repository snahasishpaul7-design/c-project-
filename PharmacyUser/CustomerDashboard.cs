using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PharmacyUser
{
    public partial class CustomerDashboard : Form
    {
        private static readonly Color SidebarBg = Color.FromArgb(45, 52, 54);
        private static readonly Color SidebarActive = Color.FromArgb(60, 70, 72);
        private static readonly Color SidebarText = Color.FromArgb(180, 190, 185);
        private static readonly Color AccentGreen = Color.FromArgb(80, 140, 110);
        private static readonly Color BgColor = Color.FromArgb(235, 240, 238);
        private static readonly Color CardBg = Color.White;
        private static readonly Color TextDark = Color.FromArgb(30, 35, 33);
        private static readonly Color TextGray = Color.FromArgb(120, 130, 125);


        private Panel pnlSidebar, pnlMain, pnlContent;
        public string LoggedInFirstName { get; set; }
        public string LoggedInLastName { get; set; }
        public string LoggedInUsername { get; set; }

        public int LoggedInUserID { get; set; }

        public CustomerDashboard(string firstName, string lastName, string username)
        {
            LoggedInFirstName = firstName;
            LoggedInLastName = lastName;
            LoggedInUsername = username;
            LoggedInUserID = DBHelper.GetUserID(username);
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MediCare Pharmacy – Dashboard";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            BuildSidebar();
            BuildMain();
            ShowHome();
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel
            {
                Size = new Size(220, this.ClientSize.Height),
                Location = new Point(0, 0),
                BackColor = SidebarBg
            };
            this.Controls.Add(pnlSidebar);


            var pnlAvatar = new Panel
            {
                Size = new Size(52, 52),
                Location = new Point(20, 24),
                BackColor = Color.Transparent
            };
            pnlAvatar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(Brushes.White, 0, 0, 52, 52);
                string initials = (LoggedInFirstName?.Length > 0 ? LoggedInFirstName[0].ToString().ToUpper() : "") +
                  (LoggedInLastName?.Length > 0 ? LoggedInLastName[0].ToString().ToUpper() : "");
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(initials, new Font("Segoe UI", 14, FontStyle.Bold),
                        new SolidBrush(SidebarBg), new RectangleF(0, 0, 52, 52), sf);
            };
            pnlSidebar.Controls.Add(pnlAvatar);


            var lblName = new Label
            {
                Text = LoggedInFirstName + " " + LoggedInLastName,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(20, 84)
            };
            pnlSidebar.Controls.Add(lblName);

            var lblRole = new Label
            {
                Text = "Customer",
                Font = new Font("Segoe UI", 9),
                ForeColor = SidebarText,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(20, 106)
            };
            pnlSidebar.Controls.Add(lblRole);


            var div = new Panel
            {
                Size = new Size(180, 1),
                Location = new Point(20, 132),
                BackColor = Color.FromArgb(70, 80, 78)
            };
            pnlSidebar.Controls.Add(div);


            string[] menus = { "🏠  Home", "💊  Medicines", "🛒  My Cart", "📦  My Orders", "🧾  Invoices", "📋  Prescriptions", "👤  My Profile" };
            int menuY = 148;
            foreach (var menu in menus)
            {
                var item = CreateMenuItem(menu, menuY);
                pnlSidebar.Controls.Add(item);
                menuY += 48;
            }


            var div2 = new Panel
            {
                Size = new Size(180, 1),
                Location = new Point(20, this.ClientSize.Height - 80),
                BackColor = Color.FromArgb(70, 80, 78)
            };
            pnlSidebar.Controls.Add(div2);


            var btnLogout = CreateMenuItem("🚪  Logout", this.ClientSize.Height - 64);
            btnLogout.Click += (s, e) => { this.Hide(); new LoginForm().Show(); };
            foreach (Control c in btnLogout.Controls)
            {
                c.Click += (s, e) => { this.Hide(); new LoginForm().Show(); };
            }
            pnlSidebar.Controls.Add(btnLogout);
        }

        private Panel CreateMenuItem(string text, int y)
        {
            var pnl = new Panel
            {
                Size = new Size(190, 40),
                Location = new Point(15, y),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = text
            };

            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                ForeColor = SidebarText,
                AutoSize = false,
                Size = new Size(190, 40),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 0, 0, 0)
            };

            pnl.Controls.Add(lbl);

            pnl.MouseEnter += (s, e) => { pnl.BackColor = Color.FromArgb(60, 70, 68); lbl.ForeColor = Color.White; };
            pnl.MouseLeave += (s, e) => { pnl.BackColor = Color.Transparent; lbl.ForeColor = SidebarText; };
            lbl.MouseEnter += (s, e) => { pnl.BackColor = Color.FromArgb(60, 70, 68); lbl.ForeColor = Color.White; };
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
            else if (tag.Contains("Medicines")) ShowMedicines();
            else if (tag.Contains("Cart")) ShowCart();
            else if (tag.Contains("Orders")) ShowOrders();
            else if (tag.Contains("Invoices")) ShowInvoices();
            else if (tag.Contains("Prescriptions")) ShowPrescriptions();
            else if (tag.Contains("Profile")) ShowProfile();
        }

        private void BuildMain()
        {
            pnlMain = new Panel
            {
                Size = new Size(this.ClientSize.Width - 220, this.ClientSize.Height),
                Location = new Point(220, 0),
                BackColor = BgColor
            };
            this.Controls.Add(pnlMain);


            var pnlTop = new Panel
            {
                Size = new Size(pnlMain.Width, 64),
                Location = new Point(0, 0),
                BackColor = CardBg
            };
            pnlTop.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 225, 222)), 0, 63, pnlTop.Width, 63);
            };
            pnlMain.Controls.Add(pnlTop);

            var lblWelcome = new Label
            {
                Text = "Welcome back, " + LoggedInFirstName + "! 👋",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(24, 12)
            };
            pnlTop.Controls.Add(lblWelcome);

            var lblDate = new Label
            {
                Text = DateTime.Now.ToString("dddd, d MMMM yyyy"),
                Font = new Font("Segoe UI", 9),
                ForeColor = TextGray,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(24, 36)
            };
            pnlTop.Controls.Add(lblDate);


            pnlContent = new Panel
            {
                Size = new Size(pnlMain.Width, pnlMain.Height - 64),
                Location = new Point(0, 64),
                BackColor = BgColor,
                AutoScroll = true
            };
            pnlMain.Controls.Add(pnlContent);
        }

        private void ClearContent()
        {
            pnlContent.Controls.Clear();
        }

        private void ShowHome()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;


            int totalOrders = 0, pendingOrders = 0;
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM OrderHeaders WHERE UserID=@uid", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", LoggedInUserID);
                        totalOrders = (int)cmd.ExecuteScalar();
                    }
                    using (var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM OrderHeaders WHERE UserID=@uid AND Status='Pending'", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", LoggedInUserID);
                        pendingOrders = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch { }

            string[] titles = { "Total Orders", "Pending", "Cart Items", "Prescriptions" };
            DataTable dtCart = DBHelper.GetCart(LoggedInUserID);
            DataTable dtPrescriptions = DBHelper.GetPrescriptions(LoggedInUserID);
            int cartItems = dtCart != null ? dtCart.Rows.Count : 0;
            int prescriptions = dtPrescriptions != null ? dtPrescriptions.Rows.Count : 0;

            string[] values = { totalOrders.ToString(), pendingOrders.ToString(), cartItems.ToString(), prescriptions.ToString() };
            int cardW = (w - 30) / 4;

            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var card = new Panel
                {
                    Size = new Size(cardW, 90),
                    Location = new Point(pad + i * (cardW + 10), pad),
                    BackColor = CardBg
                };
                card.Paint += (s, e) => DrawRoundedCard(e.Graphics, card);

                var lblT = new Label { Text = titles[idx], Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) };
                var lblV = new Label { Text = values[idx], Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 38) };
                card.Controls.Add(lblT);
                card.Controls.Add(lblV);
                pnlContent.Controls.Add(card);
            }


            var pnlOrders = new Panel
            {
                Size = new Size((int)(w * 0.58), 220),
                Location = new Point(pad, pad + 110),
                BackColor = CardBg
            };
            pnlOrders.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlOrders);
            pnlContent.Controls.Add(pnlOrders);

            var lblOrders = new Label { Text = "Recent orders", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) };
            pnlOrders.Controls.Add(lblOrders);

            Color[] statusBg = { Color.FromArgb(220, 240, 228), Color.FromArgb(250, 230, 210), Color.FromArgb(210, 228, 248) };
            Color[] statusFg = { Color.FromArgb(40, 100, 70), Color.FromArgb(160, 80, 20), Color.FromArgb(20, 70, 150) };

            var recentOrders = new System.Collections.Generic.List<(string name, string detail, string status)>();
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 3 oh.OrderHeaderID, oh.TotalPrice, oh.Status, " +
                        "(SELECT TOP 1 MedicineName FROM OrderItems WHERE OrderHeaderID = oh.OrderHeaderID) AS FirstMedicine, " +
                        "(SELECT COUNT(*) FROM OrderItems WHERE OrderHeaderID = oh.OrderHeaderID) AS ItemCount " +
                        "FROM OrderHeaders oh WHERE oh.UserID=@uid ORDER BY oh.OrderDate DESC", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", LoggedInUserID);
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                int itemCount = Convert.ToInt32(rd["ItemCount"]);
                                string firstName = rd["FirstMedicine"] != DBNull.Value ? rd["FirstMedicine"].ToString() : "Order";
                                string displayName = itemCount > 1 ? $"{firstName} + {itemCount - 1} more" : firstName;
                                recentOrders.Add((
                                    displayName,
                                    $"Order #{rd["OrderHeaderID"]} · ৳{Convert.ToDecimal(rd["TotalPrice"]):F2}",
                                    rd["Status"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch { }

            if (recentOrders.Count == 0)
            {
                var lblEmpty = new Label { Text = "No orders yet. Browse medicines to place your first order!", Font = new Font("Segoe UI", 10), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 60) };
                pnlOrders.Controls.Add(lblEmpty);
            }
            else
            {
                for (int i = 0; i < recentOrders.Count; i++)
                {
                    int idx = i;
                    var row = new Panel { Size = new Size(pnlOrders.Width - 32, 46), Location = new Point(16, 44 + i * 54), BackColor = Color.FromArgb(245, 248, 246) };
                    var lblMed = new Label { Text = recentOrders[idx].name, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 6) };
                    var lblDet = new Label { Text = recentOrders[idx].detail, Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 26) };
                    string st = recentOrders[idx].status;
                    Color bg = st == "Delivered" ? statusBg[0] : st == "Pending" ? statusBg[1] : statusBg[2];
                    Color fg = st == "Delivered" ? statusFg[0] : st == "Pending" ? statusFg[1] : statusFg[2];
                    var lblStatus = new Label { Text = st, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = fg, BackColor = bg, AutoSize = true, Location = new Point(row.Width - 90, 14), Padding = new Padding(6, 2, 6, 2) };
                    row.Controls.Add(lblMed);
                    row.Controls.Add(lblDet);
                    row.Controls.Add(lblStatus);
                    pnlOrders.Controls.Add(row);
                }
            }


            var pnlFeatured = new Panel
            {
                Size = new Size(w - (int)(w * 0.58) - 10, 220),
                Location = new Point(pad + (int)(w * 0.58) + 10, pad + 110),
                BackColor = CardBg
            };
            pnlFeatured.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlFeatured);
            pnlContent.Controls.Add(pnlFeatured);

            var lblFeat = new Label { Text = "Featured medicines", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 14) };
            pnlFeatured.Controls.Add(lblFeat);

            string[] meds = { "Paracetamol", "Vitamin D3", "Zinc Tablets", "Omeprazole" };
            string[] prices = { "৳60", "৳150", "৳95", "৳75" };
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var lblM = new Label { Text = meds[idx], Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 44 + i * 30) };
                var lblP = new Label { Text = prices[idx], Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };
                lblP.Location = new Point(pnlFeatured.Width - lblP.PreferredWidth - 20, 44 + i * 30);
                pnlFeatured.Controls.Add(lblM);
                pnlFeatured.Controls.Add(lblP);
            }

            var btnBrowse = new Button { Text = "Browse all medicines", Size = new Size(pnlFeatured.Width - 32, 36), Location = new Point(16, 170), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click += (s, e) => ShowMedicines();
            pnlFeatured.Controls.Add(btnBrowse);
        }

        private void ShowMedicines()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;

            var lblTitle = new Label
            {
                Text = "💊  Browse Medicines",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(pad, pad)
            };
            pnlContent.Controls.Add(lblTitle);

            var txtSearch = new TextBox
            {
                Size = new Size(280, 32),
                Location = new Point(pad, pad + 36),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Search medicine..."
            };
            pnlContent.Controls.Add(txtSearch);

            var btnSearch = new Button
            {
                Text = "Search",
                Size = new Size(80, 32),
                Location = new Point(pad + 288, pad + 36),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = AccentGreen,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            pnlContent.Controls.Add(btnSearch);

            var pnlCards = new Panel
            {
                Location = new Point(pad, pad + 80),
                Size = new Size(w, pnlContent.Height - 120),
                BackColor = BgColor,
                AutoScroll = true
            };
            pnlContent.Controls.Add(pnlCards);

            int cardW = 200, cardH = 260, cardPad = 16;
            int cols = w / (cardW + cardPad);

            Action<string> loadMedicines = null;
            loadMedicines = (search) =>
            {
                pnlCards.Controls.Clear();
                DataTable dt = DBHelper.GetMedicines(search);
                if (dt == null || dt.Rows.Count == 0)
                {
                    pnlCards.Controls.Add(new Label { Text = "No medicines found.", Font = new Font("Segoe UI", 11), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) });
                    return;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    int col = i % cols, rowNum = i / cols;
                    int x = col * (cardW + cardPad), y = rowNum * (cardH + cardPad);
                    int stockQty = Convert.ToInt32(row["Stock"]);
                    string imagePath = row["ImagePath"] != DBNull.Value ? row["ImagePath"].ToString() : "";

                    var card = new Panel { Size = new Size(cardW, cardH), Location = new Point(x, y), BackColor = CardBg, Cursor = Cursors.Default };
                    card.Paint += (s, e) => DrawRoundedCard(e.Graphics, card);
                    pnlCards.Controls.Add(card);

                    var picBox = new PictureBox { Size = new Size(cardW - 2, 110), Location = new Point(1, 1), BackColor = Color.FromArgb(240, 245, 242), SizeMode = PictureBoxSizeMode.Zoom };
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                        try { picBox.Image = Image.FromFile(imagePath); } catch { picBox.Image = null; }
                    if (picBox.Image == null)
                        picBox.Paint += (s, e) =>
                        {
                            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                                e.Graphics.DrawString("💊", new Font("Segoe UI", 32), new SolidBrush(Color.FromArgb(180, 190, 185)), new RectangleF(0, 0, picBox.Width, picBox.Height), sf);
                        };
                    card.Controls.Add(picBox);

                    card.Controls.Add(new Label { Text = row["Name"].ToString(), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(cardW - 16, 36), Location = new Point(8, 116), BackColor = Color.Transparent, TextAlign = ContentAlignment.TopLeft });
                    card.Controls.Add(new Label { Text = row["Category"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(8, 152) });
                    card.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(row["Price"]).ToString("F2"), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(8, 172) });

                    bool isLow = stockQty <= 30;
                    card.Controls.Add(new Label { Text = stockQty + " left", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = isLow ? Color.FromArgb(160, 80, 20) : Color.FromArgb(40, 100, 70), BackColor = isLow ? Color.FromArgb(250, 230, 210) : Color.FromArgb(220, 240, 228), AutoSize = true, Location = new Point(8, 196), Padding = new Padding(4, 2, 4, 2) });

                    var btnAdd = new Button { Text = "Add to Cart", Size = new Size(cardW - 16, 30), Location = new Point(8, 222), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                    btnAdd.FlatAppearance.BorderSize = 0;

                    var capturedRow = row;
                    btnAdd.Click += (s, e) =>
                    {
                        int medicineID = Convert.ToInt32(capturedRow["MedicineID"]);
                        int stockAvailable = Convert.ToInt32(capturedRow["Stock"]);

                        if (stockAvailable == 0)
                        {
                            MessageBox.Show("This medicine is out of stock!", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Form qtyForm = new Form { Text = "Select Quantity", Size = new Size(300, 160), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.White };
                        var lblQ = new Label { Text = $"Quantity (max {stockAvailable}):", Font = new Font("Segoe UI", 10), Location = new Point(20, 20), AutoSize = true };
                        var numQ = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1, Font = new Font("Segoe UI", 11), Size = new Size(80, 30), Location = new Point(20, 46) };
                        var btnConfirm = new Button { Text = "Add to Cart", Size = new Size(120, 36), Location = new Point(20, 86), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                        btnConfirm.FlatAppearance.BorderSize = 0;
                        btnConfirm.Click += (s2, e2) =>
                        {
                            int qty = (int)numQ.Value;
                            if (qty <= 0) { MessageBox.Show("Please enter a valid quantity.", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                            if (qty > stockAvailable) { MessageBox.Show($"Only {stockAvailable} units available!", "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning); numQ.Value = stockAvailable; return; }

                            using (var con = DBHelper.GetConnection())
                            {
                                con.Open();
                                using (var cmd = new SqlCommand("UPDATE Medicines SET Stock = Stock - @qty WHERE MedicineID = @id", con))
                                {
                                    cmd.Parameters.AddWithValue("@qty", qty);
                                    cmd.Parameters.AddWithValue("@id", medicineID);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            bool success = DBHelper.AddToCart(LoggedInUserID, medicineID, qty);
                            if (success)
                            {
                                MessageBox.Show($"{qty}x {capturedRow["Name"]} added to cart!", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                qtyForm.Close();
                                loadMedicines(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim());
                            }
                        };
                        qtyForm.Controls.Add(lblQ);
                        qtyForm.Controls.Add(numQ);
                        qtyForm.Controls.Add(btnConfirm);
                        qtyForm.ShowDialog(this);
                    };
                    card.Controls.Add(btnAdd);
                }

                int totalRows = (int)Math.Ceiling((double)dt.Rows.Count / cols);
                pnlCards.AutoScrollMinSize = new Size(0, totalRows * (cardH + cardPad));
            };

            loadMedicines("");
            btnSearch.Click += (s, e) => loadMedicines(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim());
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) loadMedicines(txtSearch.Text == "Search medicine..." ? "" : txtSearch.Text.Trim()); };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search medicine...") txtSearch.Text = ""; };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrEmpty(txtSearch.Text)) txtSearch.Text = "Search medicine..."; };
        }

        private void ShowCart()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;
            int full = w - 32;

            var pnlCard = new Panel
            {
                Size = new Size(w, pnlContent.Height - pad * 2),
                Location = new Point(pad, pad),
                BackColor = CardBg
            };
            pnlCard.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            var lblTitle = new Label
            {
                Text = "🛒  My Cart",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(16, 16)
            };
            pnlCard.Controls.Add(lblTitle);


            var lblDiscountInfo = new Label
            {
                Text = "🎁  Spend ৳500+ get 5% | ৳1000+ get 10% | ৳2000+ get 15% discount!",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 62, 132),
                BackColor = Color.FromArgb(210, 228, 248),
                AutoSize = false,
                Size = new Size(full, 28),
                Location = new Point(16, 46),
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(5, 0, 0, 0)
            };
            pnlCard.Controls.Add(lblDiscountInfo);

            var pnlList = new Panel
            {
                Size = new Size(full, pnlCard.Height - 210),
                Location = new Point(16, 82),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            pnlCard.Controls.Add(pnlList);

            var lblSubtotal = new Label
            {
                Text = "Subtotal: ৳0.00",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextGray,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(16, pnlCard.Height - 126)
            };
            pnlCard.Controls.Add(lblSubtotal);

            var lblDiscountAmt = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(21, 87, 36),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(16, pnlCard.Height - 106)
            };
            pnlCard.Controls.Add(lblDiscountAmt);

            var lblTotal = new Label
            {
                Text = "Total: ৳0.00",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(16, pnlCard.Height - 82)
            };
            pnlCard.Controls.Add(lblTotal);

            var lblDiscountBadge = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(21, 87, 36),
                BackColor = Color.FromArgb(212, 237, 218),
                AutoSize = true,
                Location = new Point(200, pnlCard.Height - 78),
                Padding = new Padding(6, 3, 6, 3),
                Visible = false
            };
            pnlCard.Controls.Add(lblDiscountBadge);

            var btnOrder = new Button
            {
                Text = "Place Order",
                Size = new Size(160, 44),
                Location = new Point(16, pnlCard.Height - 56),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = AccentGreen,
                Cursor = Cursors.Hand
            };
            btnOrder.FlatAppearance.BorderSize = 0;
            pnlCard.Controls.Add(btnOrder);

            decimal finalOrderTotal = 0;

            Action loadCart = null;
            loadCart = () =>
            {
                pnlList.Controls.Clear();
                DataTable dt = DBHelper.GetCart(LoggedInUserID);

                if (dt == null || dt.Rows.Count == 0)
                {
                    pnlList.Controls.Add(new Label { Text = "Your cart is empty!", Font = new Font("Segoe UI", 12), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(0, 10) });
                    lblSubtotal.Text = "Subtotal: ৳0.00";
                    lblDiscountAmt.Text = "";
                    lblTotal.Text = "Total: ৳0.00";
                    lblDiscountBadge.Visible = false;
                    finalOrderTotal = 0;
                    return;
                }

                decimal subTotal = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    decimal rowTotal = Convert.ToDecimal(row["Total"]);
                    subTotal += rowTotal;

                    var rowPnl = new Panel { Size = new Size(pnlList.Width - 20, 50), Location = new Point(0, i * 60), BackColor = Color.FromArgb(248, 250, 249) };
                    rowPnl.Controls.Add(new Label { Text = row["Name"].ToString(), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 8) });
                    rowPnl.Controls.Add(new Label { Text = "Qty: " + row["Quantity"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 28) });

                    var lblPrice = new Label { Text = "৳" + rowTotal.ToString("F2"), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };
                    lblPrice.Location = new Point(rowPnl.Width - 180, 15);
                    rowPnl.Controls.Add(lblPrice);

                    var btnRemove = new Button { Text = "Remove", Size = new Size(80, 28), Location = new Point(rowPnl.Width - 90, 11), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8), ForeColor = Color.FromArgb(180, 40, 40), BackColor = Color.FromArgb(250, 220, 220), Cursor = Cursors.Hand, Tag = row["CartID"].ToString() };
                    btnRemove.FlatAppearance.BorderSize = 0;
                    btnRemove.Click += (s, e) =>
                    {
                        int cartID = Convert.ToInt32(btnRemove.Tag);
                        int cartQty = Convert.ToInt32(row["Quantity"]);
                        int removedMedID = Convert.ToInt32(row["MedicineID"]);

                        Form removeForm = new Form { Text = "Remove from Cart", Size = new Size(300, 170), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.White };
                        var lblQ = new Label { Text = $"How many to remove? (max {cartQty}):", Font = new Font("Segoe UI", 10), Location = new Point(20, 20), AutoSize = true };
                        var numQ = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1, Font = new Font("Segoe UI", 11), Size = new Size(80, 30), Location = new Point(20, 46) };
                        var btnConfirm = new Button { Text = "Remove", Size = new Size(120, 36), Location = new Point(20, 90), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(180, 40, 40), Cursor = Cursors.Hand };
                        btnConfirm.FlatAppearance.BorderSize = 0;
                        btnConfirm.Click += (s2, e2) =>
                        {
                            int qtyToRemove = (int)numQ.Value;
                            if (qtyToRemove <= 0) { MessageBox.Show("Please enter a valid quantity.", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                            if (qtyToRemove > cartQty) { MessageBox.Show($"You only have {cartQty} of this item in your cart.", "Exceeds Cart Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning); numQ.Value = cartQty; return; }

                            using (var con = DBHelper.GetConnection())
                            {
                                con.Open();
                                using (var cmd = new SqlCommand("UPDATE Medicines SET Stock = Stock + @qty WHERE MedicineID = @id", con))
                                { cmd.Parameters.AddWithValue("@qty", qtyToRemove); cmd.Parameters.AddWithValue("@id", removedMedID); cmd.ExecuteNonQuery(); }

                                if (qtyToRemove == cartQty)
                                    using (var cmd = new SqlCommand("DELETE FROM Cart WHERE CartID = @id", con))
                                    { cmd.Parameters.AddWithValue("@id", cartID); cmd.ExecuteNonQuery(); }
                                else
                                    using (var cmd = new SqlCommand("UPDATE Cart SET Quantity = Quantity - @qty WHERE CartID = @id", con))
                                    { cmd.Parameters.AddWithValue("@qty", qtyToRemove); cmd.Parameters.AddWithValue("@id", cartID); cmd.ExecuteNonQuery(); }
                            }
                            removeForm.Close();
                            loadCart();
                        };
                        removeForm.Controls.Add(lblQ);
                        removeForm.Controls.Add(numQ);
                        removeForm.Controls.Add(btnConfirm);
                        removeForm.ShowDialog(this);
                    };
                    rowPnl.Controls.Add(btnRemove);
                    pnlList.Controls.Add(rowPnl);
                }


                decimal discountPercent = 0;
                string discountLabel = "";
                string badgeText = "";

                if (subTotal >= 2000) { discountPercent = 15; discountLabel = "🎉 Discount (15%):"; badgeText = "15% OFF Applied!"; }
                else if (subTotal >= 1000) { discountPercent = 10; discountLabel = "😊 Discount (10%):"; badgeText = "10% OFF Applied!"; }
                else if (subTotal >= 500) { discountPercent = 5; discountLabel = "👍 Discount (5%):"; badgeText = "5% OFF Applied!"; }

                decimal discountAmount = subTotal * (discountPercent / 100);
                decimal finalTotal = subTotal - discountAmount;
                finalOrderTotal = finalTotal;

                lblSubtotal.Text = $"Subtotal: ৳{subTotal:F2}";

                if (discountPercent > 0)
                {
                    lblDiscountAmt.Text = $"{discountLabel}  -৳{discountAmount:F2}";
                    lblDiscountAmt.ForeColor = Color.FromArgb(21, 87, 36);
                    lblDiscountBadge.Text = badgeText;
                    lblDiscountBadge.Visible = true;
                }
                else
                {
                    decimal needed = subTotal < 500 ? 500 - subTotal : subTotal < 1000 ? 1000 - subTotal : 2000 - subTotal;
                    lblDiscountAmt.Text = $"💡 Spend ৳{needed:F0} more to get {(subTotal < 500 ? 5 : subTotal < 1000 ? 10 : 15)}% discount!";
                    lblDiscountAmt.ForeColor = Color.FromArgb(133, 77, 14);
                    lblDiscountBadge.Visible = false;
                }

                lblTotal.Text = $"Total: ৳{finalTotal:F2}";
            };

            loadCart();

            btnOrder.Click += (s, e) =>
            {
                DataTable dt = DBHelper.GetCart(LoggedInUserID);
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Your cart is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool success = DBHelper.PlaceOrder(LoggedInUserID, finalOrderTotal);
                if (success)
                {
                    MessageBox.Show($"Order placed successfully!\nTotal paid: ৳{finalOrderTotal:F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadCart();
                    ShowHome();
                }
            };
        }

        private void ShowOrders()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel
            {
                Size = new Size(w, pnlContent.Height - pad * 2),
                Location = new Point(pad, pad),
                BackColor = CardBg
            };
            pnlCard.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "📦  My Orders", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            var pnlList = new Panel
            {
                Size = new Size(w - 32, pnlCard.Height - 60),
                Location = new Point(16, 52),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            pnlCard.Controls.Add(pnlList);

            DataTable dt = DBHelper.GetOrderHeaders(LoggedInUserID);

            if (dt == null || dt.Rows.Count == 0)
            {
                pnlList.Controls.Add(new Label { Text = "No orders yet. Browse medicines to place your first order!", Font = new Font("Segoe UI", 11), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) });
                return;
            }

            int yPos = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                int orderHeaderID = Convert.ToInt32(row["OrderHeaderID"]);
                string status = row["Status"].ToString();
                decimal total = Convert.ToDecimal(row["TotalPrice"]);
                string date = Convert.ToDateTime(row["OrderDate"]).ToString("dd MMM yyyy");

                Color statusBg = status == "Delivered" ? Color.FromArgb(220, 240, 228) : status == "Processing" ? Color.FromArgb(210, 228, 248) : Color.FromArgb(250, 230, 210);
                Color statusFg = status == "Delivered" ? Color.FromArgb(40, 100, 70) : status == "Processing" ? Color.FromArgb(20, 70, 150) : Color.FromArgb(160, 80, 20);

                // Order header row
                var pnlOrderHeader = new Panel
                {
                    Size = new Size(pnlList.Width - 4, 46),
                    Location = new Point(0, yPos),
                    BackColor = SidebarBg
                };

                pnlOrderHeader.Controls.Add(new Label { Text = "Order #" + orderHeaderID, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(160, 46), Location = new Point(10, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                pnlOrderHeader.Controls.Add(new Label { Text = date, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(180, 190, 185), AutoSize = false, Size = new Size(140, 46), Location = new Point(170, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                pnlOrderHeader.Controls.Add(new Label { Text = "৳" + total.ToString("F2"), Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = AccentGreen, AutoSize = false, Size = new Size(120, 46), Location = new Point(310, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                pnlOrderHeader.Controls.Add(new Label { Text = status, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = statusFg, BackColor = statusBg, AutoSize = true, Location = new Point(440, 14), Padding = new Padding(6, 3, 6, 3) });

                pnlList.Controls.Add(pnlOrderHeader);
                yPos += 46;

                // Order items
                DataTable items = DBHelper.GetOrderItems(orderHeaderID);
                if (items != null)
                {
                    for (int j = 0; j < items.Rows.Count; j++)
                    {
                        var item = items.Rows[j];
                        var pnlItem = new Panel
                        {
                            Size = new Size(pnlList.Width - 4, 40),
                            Location = new Point(0, yPos),
                            BackColor = j % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg
                        };

                        pnlItem.Controls.Add(new Label { Text = "     " + item["MedicineName"].ToString(), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(240, 40), Location = new Point(10, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                        pnlItem.Controls.Add(new Label { Text = "Qty: " + item["Quantity"].ToString(), Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(80, 40), Location = new Point(255, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                        pnlItem.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(item["UnitPrice"]).ToString("F2") + " each", Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(120, 40), Location = new Point(335, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                        pnlItem.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(item["TotalPrice"]).ToString("F2"), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(100, 40), Location = new Point(455, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });

                        pnlList.Controls.Add(pnlItem);
                        yPos += 40;
                    }
                }

                // Spacer between orders
                yPos += 10;
            }

            pnlList.AutoScrollMinSize = new Size(0, yPos);
        }

        private void ShowPrescriptions()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel
            {
                Size = new Size(w, pnlContent.Height - pad * 2),
                Location = new Point(pad, pad),
                BackColor = CardBg
            };
            pnlCard.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            var lblTitle = new Label
            {
                Text = "📋  My Prescriptions",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(16, 16)
            };
            pnlCard.Controls.Add(lblTitle);


            var btnUpload = new Button
            {
                Text = "📎  Upload Prescription",
                Size = new Size(220, 42),
                Location = new Point(16, 52),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = SidebarBg,
                Cursor = Cursors.Hand
            };
            btnUpload.FlatAppearance.BorderSize = 0;
            pnlCard.Controls.Add(btnUpload);


            var pnlList = new Panel
            {
                Size = new Size(w - 32, pnlCard.Height - 120),
                Location = new Point(16, 108),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            pnlCard.Controls.Add(pnlList);


            var pnlHeader = new Panel
            {
                Size = new Size(pnlList.Width - 20, 30),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(240, 242, 241)
            };
            var lblHName = new Label { Text = "File Name", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(280, 30), Location = new Point(10, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft };
            var lblHDate = new Label { Text = "Upload Date", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(180, 30), Location = new Point(290, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft };
            var lblHStatus = new Label { Text = "Status", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(100, 30), Location = new Point(470, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft };
            pnlHeader.Controls.Add(lblHName);
            pnlHeader.Controls.Add(lblHDate);
            pnlHeader.Controls.Add(lblHStatus);
            pnlList.Controls.Add(pnlHeader);

            Action loadPrescriptions = null;
            loadPrescriptions = () =>
            {

                for (int i = pnlList.Controls.Count - 1; i >= 0; i--)
                {
                    if (pnlList.Controls[i] != pnlHeader)
                        pnlList.Controls.RemoveAt(i);
                }

                DataTable dt = DBHelper.GetPrescriptions(LoggedInUserID);
                if (dt == null || dt.Rows.Count == 0)
                {
                    var lblEmpty = new Label
                    {
                        Text = "No prescriptions uploaded yet!",
                        Font = new Font("Segoe UI", 11),
                        ForeColor = TextGray,
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Location = new Point(10, 40)
                    };
                    pnlList.Controls.Add(lblEmpty);
                    return;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    string status = row["Status"].ToString();
                    Color statusBg = status == "Approved" ? Color.FromArgb(220, 240, 228) :
                                     status == "Rejected" ? Color.FromArgb(250, 220, 220) :
                                     Color.FromArgb(250, 230, 210);
                    Color statusFg = status == "Approved" ? Color.FromArgb(40, 100, 70) :
                                     status == "Rejected" ? Color.FromArgb(180, 40, 40) :
                                     Color.FromArgb(160, 80, 20);

                    var rowPnl = new Panel
                    {
                        Size = new Size(pnlList.Width - 20, 46),
                        Location = new Point(0, 36 + i * 54),
                        BackColor = i % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg
                    };

                    var lblName = new Label
                    {
                        Text = "📄 " + row["FileName"].ToString(),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = TextDark,
                        AutoSize = false,
                        Size = new Size(280, 46),
                        Location = new Point(10, 0),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var lblDate = new Label
                    {
                        Text = Convert.ToDateTime(row["UploadDate"]).ToString("dd MMM yyyy"),
                        Font = new Font("Segoe UI", 10),
                        ForeColor = TextGray,
                        AutoSize = false,
                        Size = new Size(180, 46),
                        Location = new Point(290, 0),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var lblStatus = new Label
                    {
                        Text = status,
                        Font = new Font("Segoe UI", 8, FontStyle.Bold),
                        ForeColor = statusFg,
                        BackColor = statusBg,
                        AutoSize = true,
                        Location = new Point(470, 13),
                        Padding = new Padding(6, 3, 6, 3)
                    };

                    rowPnl.Controls.Add(lblName);
                    rowPnl.Controls.Add(lblDate);
                    rowPnl.Controls.Add(lblStatus);
                    pnlList.Controls.Add(rowPnl);
                }
            };

            loadPrescriptions();


            btnUpload.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select Prescription";
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png|PDF Files|*.pdf|All Files|*.*";
                    ofd.Multiselect = false;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;
                        string fileName = System.IO.Path.GetFileName(filePath);

                        bool success = DBHelper.UploadPrescription(LoggedInUserID, fileName, filePath);
                        if (success)
                        {
                            MessageBox.Show("Prescription uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadPrescriptions();
                        }
                    }
                }
            };
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
                BackColor = CardBg
            };
            pnlCard.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            var lblTitle = new Label { Text = "👤  My Profile", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) };
            pnlCard.Controls.Add(lblTitle);
            Color BorderColor = Color.FromArgb(200, 210, 205);


            string[] labels = { "First Name", "Last Name", "Username", "Email", "Phone" };
            string[] values = { "", "", LoggedInUsername, "", "" };

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT FirstName, LastName, Username, Email, Phone FROM Users WHERE Username=@u", con))
                    {
                        cmd.Parameters.AddWithValue("@u", LoggedInUsername);
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
                var lbl = new Label { Text = labels[idx].ToUpper(), Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 58 + i * 62) };
                var txt = new TextBox { Text = values[idx], Font = new Font("Segoe UI", 11), Size = new Size(400, 32), Location = new Point(16, 78 + i * 62), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(245, 247, 246), ForeColor = TextDark };
                var line = new Panel { Size = new Size(400, 1), Location = new Point(16, 110 + i * 62), BackColor = Color.FromArgb(200, 210, 205) };
                txtBoxes[idx] = txt;
                pnlCard.Controls.Add(lbl);
                pnlCard.Controls.Add(txt);
                pnlCard.Controls.Add(line);
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


                        if (newUsername != LoggedInUsername)
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


                        using (var cmd = new SqlCommand(
                            "UPDATE Users SET FirstName=@fn, LastName=@ln, Username=@un, Email=@em, Phone=@ph WHERE Username=@old", con))
                        {
                            cmd.Parameters.AddWithValue("@fn", newFirst);
                            cmd.Parameters.AddWithValue("@ln", newLast);
                            cmd.Parameters.AddWithValue("@un", newUsername);
                            cmd.Parameters.AddWithValue("@em", newEmail);
                            cmd.Parameters.AddWithValue("@ph", newPhone);
                            cmd.Parameters.AddWithValue("@old", LoggedInUsername);
                            cmd.ExecuteNonQuery();
                        }
                    }


                    LoggedInFirstName = newFirst;
                    LoggedInLastName = newLast;
                    LoggedInUsername = newUsername;


                    foreach (Control c in pnlSidebar.Controls)
                        if (c is Label lbl && lbl.Font.Bold && lbl.ForeColor == Color.White)
                        { lbl.Text = LoggedInFirstName + " " + LoggedInLastName; break; }

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

                var pnlChangePw = new Panel
                {
                    Size = new Size(w - 32, 220),
                    Location = new Point(16, 450),
                    BackColor = Color.FromArgb(245, 247, 246)
                };
                pnlCard.AutoScrollMinSize = new Size(0, 700);
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

                var btnUpdate = new Button
                {
                    Text = "Update Password",
                    Size = new Size(160, 36),
                    Location = new Point(10, 186),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = AccentGreen,
                    Cursor = Cursors.Hand
                };
                btnUpdate.FlatAppearance.BorderSize = 0;
                btnUpdate.Click += (s2, e2) =>
                {
                    if (string.IsNullOrWhiteSpace(txtCurrent.Text) ||
                        string.IsNullOrWhiteSpace(txtNew.Text) ||
                        string.IsNullOrWhiteSpace(txtConfirm.Text))
                    {
                        MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (txtNew.Text != txtConfirm.Text)
                    {
                        MessageBox.Show("New passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    DataRow user = DBHelper.LoginUser(LoggedInUsername, txtCurrent.Text);
                    if (user == null)
                    {
                        MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    bool success = DBHelper.UpdatePassword(LoggedInUserID, txtNew.Text);
                    if (success)
                    {
                        MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pnlCard.Controls.Remove(pnlChangePw);
                    }
                };

                pnlChangePw.Controls.Add(lblCurrent);
                pnlChangePw.Controls.Add(txtCurrent);
                pnlChangePw.Controls.Add(lineCurrent);
                pnlChangePw.Controls.Add(lblNew);
                pnlChangePw.Controls.Add(txtNew);
                pnlChangePw.Controls.Add(lineNew);
                pnlChangePw.Controls.Add(lblConfirm);
                pnlChangePw.Controls.Add(txtConfirm);
                pnlChangePw.Controls.Add(lineConfirm);
                pnlChangePw.Controls.Add(btnUpdate);
            };
            pnlCard.Controls.Add(btnChangePw);
        }

        private void ShowInvoices()
        {
            ClearContent();
            int pad = 20;
            int w = pnlContent.Width - pad * 2;

            var pnlCard = new Panel
            {
                Size = new Size(w, pnlContent.Height - pad * 2),
                Location = new Point(pad, pad),
                BackColor = CardBg
            };
            pnlCard.Paint += (s, e) => DrawRoundedCard(e.Graphics, pnlCard);
            pnlContent.Controls.Add(pnlCard);

            pnlCard.Controls.Add(new Label { Text = "🧾  My Invoices", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(16, 16) });

            var pnlList = new Panel
            {
                Size = new Size(w - 32, pnlCard.Height - 60),
                Location = new Point(16, 52),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            pnlCard.Controls.Add(pnlList);

            DataTable dt = DBHelper.GetOrderHeaders(LoggedInUserID);

            if (dt == null || dt.Rows.Count == 0)
            {
                pnlList.Controls.Add(new Label { Text = "No invoices yet. Place an order to generate invoices!", Font = new Font("Segoe UI", 11), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(10, 10) });
                return;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                int orderHeaderID = Convert.ToInt32(row["OrderHeaderID"]);
                string status = row["Status"].ToString();
                decimal total = Convert.ToDecimal(row["TotalPrice"]);
                string date = Convert.ToDateTime(row["OrderDate"]).ToString("dd MMM yyyy");

                Color statusBg = status == "Delivered" ? Color.FromArgb(220, 240, 228) : status == "Processing" ? Color.FromArgb(210, 228, 248) : Color.FromArgb(250, 230, 210);
                Color statusFg = status == "Delivered" ? Color.FromArgb(40, 100, 70) : status == "Processing" ? Color.FromArgb(20, 70, 150) : Color.FromArgb(160, 80, 20);

                var rowPnl = new Panel
                {
                    Size = new Size(pnlList.Width - 4, 50),
                    Location = new Point(0, i * 58),
                    BackColor = i % 2 == 0 ? Color.FromArgb(248, 250, 249) : CardBg
                };

                rowPnl.Controls.Add(new Label { Text = "#" + orderHeaderID, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = AccentGreen, AutoSize = false, Size = new Size(90, 50), Location = new Point(10, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                rowPnl.Controls.Add(new Label { Text = date, Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = false, Size = new Size(140, 50), Location = new Point(110, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                rowPnl.Controls.Add(new Label { Text = "৳" + total.ToString("F2"), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(120, 50), Location = new Point(260, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                rowPnl.Controls.Add(new Label { Text = status, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = statusFg, BackColor = statusBg, AutoSize = true, Location = new Point(390, 16), Padding = new Padding(6, 3, 6, 3) });

                var capturedRow = row;
                var btnView = new Button { Text = "View", Size = new Size(64, 30), Location = new Point(520, 10), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, BackColor = AccentGreen, Cursor = Cursors.Hand };
                btnView.FlatAppearance.BorderSize = 0;
                btnView.Click += (s, e) => ShowInvoiceDetail(orderHeaderID, total, status, date);
                rowPnl.Controls.Add(btnView);
                pnlList.Controls.Add(rowPnl);
            }
            pnlList.AutoScrollMinSize = new Size(0, dt.Rows.Count * 58);
        }
        // ═══════════════════════════════════════════════════════════════════════
        //  UPDATED: ShowInvoiceDetail — with AutoScroll + Up/Down scroll buttons
        //  Replace your existing ShowInvoiceDetail method with this one.
        // ═══════════════════════════════════════════════════════════════════════

        private void ShowInvoiceDetail(int orderHeaderID, decimal totalPrice, string status, string orderDate)
        {
            DataTable items = DBHelper.GetOrderItems(orderHeaderID);

            var invoiceForm = new Form
            {
                Text = "Invoice #" + orderHeaderID,
                Size = new Size(600, 660),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            // ── Scrollable panel (mouse wheel works automatically) ────────────
            var pnlInvoice = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true      // ← this is all you need for mouse scroll
            };
            invoiceForm.Controls.Add(pnlInvoice);

            // ── Header ────────────────────────────────────────────────────────
            var pnlHeader = new Panel { Size = new Size(580, 88), Location = new Point(0, 0), BackColor = SidebarBg };
            pnlHeader.Controls.Add(new Label { Text = "💊  MediCare Pharmacy", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(24, 14) });
            pnlHeader.Controls.Add(new Label { Text = "INVOICE", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = AccentGreen, AutoSize = true, BackColor = Color.Transparent, Location = new Point(26, 50) });
            pnlHeader.Controls.Add(new Label { Text = "Invoice #" + orderHeaderID, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Location = new Point(370, 18) });
            pnlHeader.Controls.Add(new Label { Text = "Date: " + orderDate, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(180, 190, 185), AutoSize = true, BackColor = Color.Transparent, Location = new Point(370, 44) });
            pnlInvoice.Controls.Add(pnlHeader);

            // ── Bill To ───────────────────────────────────────────────────────
            pnlInvoice.Controls.Add(new Label { Text = "BILL TO", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(24, 106) });
            pnlInvoice.Controls.Add(new Label { Text = LoggedInFirstName + " " + LoggedInLastName, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent, Location = new Point(24, 122) });
            pnlInvoice.Controls.Add(new Label { Text = "@" + LoggedInUsername, Font = new Font("Segoe UI", 9), ForeColor = TextGray, AutoSize = true, BackColor = Color.Transparent, Location = new Point(24, 148) });

            Color sBg = status == "Delivered" ? Color.FromArgb(220, 240, 228) :
                        status == "Processing" ? Color.FromArgb(210, 228, 248) :
                                                 Color.FromArgb(250, 230, 210);
            Color sFg = status == "Delivered" ? Color.FromArgb(40, 100, 70) :
                        status == "Processing" ? Color.FromArgb(20, 70, 150) :
                                                 Color.FromArgb(160, 80, 20);
            pnlInvoice.Controls.Add(new Label { Text = status, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = sFg, BackColor = sBg, AutoSize = true, Location = new Point(390, 126), Padding = new Padding(10, 5, 10, 5) });

            pnlInvoice.Controls.Add(new Panel { Size = new Size(524, 1), Location = new Point(24, 176), BackColor = Color.FromArgb(220, 225, 222) });

            // ── Table header ──────────────────────────────────────────────────
            var pnlTH = new Panel { Size = new Size(524, 32), Location = new Point(24, 184), BackColor = SidebarBg };
            pnlTH.Controls.Add(new Label { Text = "Medicine", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(200, 32), Location = new Point(8, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
            pnlTH.Controls.Add(new Label { Text = "Qty", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(60, 32), Location = new Point(212, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
            pnlTH.Controls.Add(new Label { Text = "Unit Price", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(120, 32), Location = new Point(278, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
            pnlTH.Controls.Add(new Label { Text = "Total", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(94, 32), Location = new Point(408, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
            pnlInvoice.Controls.Add(pnlTH);

            // ── Item rows ─────────────────────────────────────────────────────
            int rowY = 216;
            if (items != null)
            {
                for (int i = 0; i < items.Rows.Count; i++)
                {
                    var item = items.Rows[i];
                    var pnlRow = new Panel
                    {
                        Size = new Size(524, 44),
                        Location = new Point(24, rowY),
                        BackColor = i % 2 == 0 ? Color.FromArgb(248, 250, 249) : Color.White
                    };
                    pnlRow.Controls.Add(new Label { Text = item["MedicineName"].ToString(), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(200, 44), Location = new Point(8, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
                    pnlRow.Controls.Add(new Label { Text = item["Quantity"].ToString(), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(60, 44), Location = new Point(212, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                    pnlRow.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(item["UnitPrice"]).ToString("F2"), Font = new Font("Segoe UI", 10), ForeColor = TextDark, AutoSize = false, Size = new Size(120, 44), Location = new Point(278, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                    pnlRow.Controls.Add(new Label { Text = "৳" + Convert.ToDecimal(item["TotalPrice"]).ToString("F2"), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, AutoSize = false, Size = new Size(94, 44), Location = new Point(408, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                    pnlInvoice.Controls.Add(pnlRow);
                    rowY += 44;
                }
            }

            // ── Grand Total ───────────────────────────────────────────────────
            pnlInvoice.Controls.Add(new Panel { Size = new Size(524, 1), Location = new Point(24, rowY), BackColor = Color.FromArgb(220, 225, 222) });
            rowY += 8;

            var pnlGT = new Panel { Size = new Size(524, 52), Location = new Point(24, rowY), BackColor = Color.White };
            pnlGT.Controls.Add(new Label { Text = "GRAND TOTAL", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextGray, AutoSize = false, Size = new Size(260, 52), Location = new Point(8, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft });
            pnlGT.Controls.Add(new Label { Text = "৳" + totalPrice.ToString("F2"), Font = new Font("Segoe UI", 17, FontStyle.Bold), ForeColor = AccentGreen, AutoSize = false, Size = new Size(250, 52), Location = new Point(270, 0), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
            pnlInvoice.Controls.Add(pnlGT);
            rowY += 60;

            pnlInvoice.Controls.Add(new Panel { Size = new Size(524, 1), Location = new Point(24, rowY), BackColor = Color.FromArgb(220, 225, 222) });
            rowY += 12;

            // ── Thank-you note ────────────────────────────────────────────────
            pnlInvoice.Controls.Add(new Label
            {
                Text = "Thank you for choosing MediCare Pharmacy! 💊\nFor queries, contact support at medicare@pharmacy.com",
                Font = new Font("Segoe UI", 9),
                ForeColor = TextGray,
                AutoSize = false,
                Size = new Size(524, 46),
                Location = new Point(24, rowY),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            });
            rowY += 66;

            // ── Print + Close buttons ─────────────────────────────────────────
            var btnPrint = new Button
            {
                Text = "🖨️  Print Invoice",
                Size = new Size(180, 42),
                Location = new Point(24, rowY),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = SidebarBg,
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            pnlInvoice.Controls.Add(btnPrint);

            var btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 42),
                Location = new Point(216, rowY),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextDark,
                BackColor = Color.FromArgb(235, 240, 238),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            pnlInvoice.Controls.Add(btnClose);
            rowY += 60;

            // ── Set scroll height based on actual content ─────────────────────
            pnlInvoice.AutoScrollMinSize = new Size(0, rowY);

            // ── Print logic ───────────────────────────────────────────────────
            btnPrint.Click += (s, e) =>
            {
                var pd = new System.Drawing.Printing.PrintDocument();
                pd.PrintPage += (ps, pe) =>
                {
                    Graphics g = pe.Graphics;
                    float y = 30f;
                    var titleF = new Font("Segoe UI", 16, FontStyle.Bold);
                    var headF = new Font("Segoe UI", 11, FontStyle.Bold);
                    var bodyF = new Font("Segoe UI", 10);
                    var smallF = new Font("Segoe UI", 9);
                    var greenBr = new SolidBrush(AccentGreen);
                    var darkBr = new SolidBrush(TextDark);
                    var grayBr = new SolidBrush(TextGray);

                    g.DrawString("MediCare Pharmacy", titleF, darkBr, 40, y); y += 30;
                    g.DrawString("INVOICE", headF, greenBr, 40, y); y += 26;
                    g.DrawString("Invoice #" + orderHeaderID + "    Date: " + orderDate, smallF, grayBr, 40, y); y += 32;
                    g.DrawLine(Pens.LightGray, 40, y, 560, y); y += 12;
                    g.DrawString("Bill To: " + LoggedInFirstName + " " + LoggedInLastName + "  (@" + LoggedInUsername + ")", bodyF, darkBr, 40, y); y += 24;
                    g.DrawString("Status:  " + status, bodyF, darkBr, 40, y); y += 32;
                    g.DrawLine(Pens.LightGray, 40, y, 560, y); y += 12;
                    g.DrawString("Medicine", headF, darkBr, 40, y);
                    g.DrawString("Qty", headF, darkBr, 270, y);
                    g.DrawString("Unit Price", headF, darkBr, 360, y);
                    g.DrawString("Total", headF, darkBr, 470, y); y += 26;
                    g.DrawLine(Pens.LightGray, 40, y, 560, y); y += 10;

                    if (items != null)
                        foreach (DataRow item in items.Rows)
                        {
                            g.DrawString(item["MedicineName"].ToString(), bodyF, darkBr, 40, y);
                            g.DrawString(item["Quantity"].ToString(), bodyF, darkBr, 270, y);
                            g.DrawString("৳" + Convert.ToDecimal(item["UnitPrice"]).ToString("F2"), bodyF, darkBr, 360, y);
                            g.DrawString("৳" + Convert.ToDecimal(item["TotalPrice"]).ToString("F2"), bodyF, darkBr, 470, y);
                            y += 26;
                        }

                    y += 10;
                    g.DrawLine(Pens.LightGray, 40, y, 560, y); y += 12;
                    g.DrawString("Grand Total:  ৳" + totalPrice.ToString("F2"), new Font("Segoe UI", 13, FontStyle.Bold), greenBr, 40, y); y += 44;
                    g.DrawString("Thank you for choosing MediCare Pharmacy!", smallF, grayBr, 40, y);

                    titleF.Dispose(); headF.Dispose(); bodyF.Dispose(); smallF.Dispose();
                    greenBr.Dispose(); darkBr.Dispose(); grayBr.Dispose();
                };
                var preview = new System.Windows.Forms.PrintPreviewDialog { Document = pd, WindowState = FormWindowState.Maximized };
                preview.ShowDialog();
            };

            btnClose.Click += (s, e) => invoiceForm.Close();

            invoiceForm.ShowDialog(this);
        }

        private void DrawRoundedCard(Graphics g, Panel card)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 12))
            using (SolidBrush brush = new SolidBrush(card.BackColor))
            {
                g.FillPath(brush, path);
                using (Pen pen = new Pen(Color.FromArgb(220, 225, 222), 1))
                    g.DrawPath(pen, path);
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