using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PharmacyUser
{
    public class DBHelper
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connStr);
        }

        public static bool RegisterUser(string firstName, string lastName, string username, string email, string phone, string password, string role)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Users (FirstName, LastName, Username, Email, Phone, Password, Role) VALUES (@FirstName, @LastName, @Username, @Email, @Phone, @Password, @Role)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static DataRow LoginUser(string username, string password)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public static bool ResetPassword(string username, string newPassword)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password = @Password WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", newPassword);
                        cmd.Parameters.AddWithValue("@Username", username);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable GetMedicines(string search = "")
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Medicines";
                    if (!string.IsNullOrEmpty(search))
                        query += " WHERE Name LIKE @Search OR Category LIKE @Search";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(search))
                            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public static bool AddToCart(int userID, int medicineID, int quantity)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"IF EXISTS (SELECT * FROM Cart WHERE UserID = @UserID AND MedicineID = @MedicineID)
                                        UPDATE Cart SET Quantity = Quantity + @Quantity WHERE UserID = @UserID AND MedicineID = @MedicineID
                                    ELSE
                                        INSERT INTO Cart (UserID, MedicineID, Quantity) VALUES (@UserID, @MedicineID, @Quantity)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@MedicineID", medicineID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable GetCart(int userID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT c.CartID, m.Name, m.Price, c.Quantity, 
                                    (m.Price * c.Quantity) AS Total, m.MedicineID
                                    FROM Cart c 
                                    JOIN Medicines m ON c.MedicineID = m.MedicineID
                                    WHERE c.UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public static bool RemoveFromCart(int cartID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Cart WHERE CartID = @CartID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartID", cartID);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool ClearCart(int userID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Cart WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool PlaceOrder(int userID, decimal finalTotal)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();

                    // Get cart items
                    string cartQuery = @"SELECT m.Name, c.Quantity, m.Price,
                                (m.Price * c.Quantity) AS Total
                                FROM Cart c
                                JOIN Medicines m ON c.MedicineID = m.MedicineID
                                WHERE c.UserID = @UserID";
                    DataTable cartItems = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(cartQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(cartItems);
                    }

                    if (cartItems.Rows.Count == 0) return false;

                    // Insert one order header
                    int orderHeaderID;
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO OrderHeaders (UserID, TotalPrice, Status, OrderDate) " +
                        "VALUES (@uid, @total, 'Pending', GETDATE()); SELECT SCOPE_IDENTITY();", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Parameters.AddWithValue("@total", finalTotal);
                        orderHeaderID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insert each cart item as order item
                    foreach (DataRow row in cartItems.Rows)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO OrderItems (OrderHeaderID, MedicineName, Quantity, UnitPrice, TotalPrice) " +
                            "VALUES (@hid, @med, @qty, @unit, @total)", conn))
                        {
                            cmd.Parameters.AddWithValue("@hid", orderHeaderID);
                            cmd.Parameters.AddWithValue("@med", row["Name"].ToString());
                            cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(row["Quantity"]));
                            cmd.Parameters.AddWithValue("@unit", Convert.ToDecimal(row["Price"]));
                            cmd.Parameters.AddWithValue("@total", Convert.ToDecimal(row["Total"]));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Clear cart
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Cart WHERE UserID = @UserID", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error placing order: " + ex.Message);
                return false;
            }
        }

        public static DataTable GetOrderHeaders(int userID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT OrderHeaderID, TotalPrice, Status, OrderDate
                            FROM OrderHeaders
                            WHERE UserID = @UserID
                            ORDER BY OrderDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public static DataTable GetOrderItems(int orderHeaderID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT MedicineName, Quantity, UnitPrice, TotalPrice
                            FROM OrderItems
                            WHERE OrderHeaderID = @hid";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hid", orderHeaderID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public static int GetUserID(string username)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT UserID FROM Users WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return -1;
            }
        }

        public static bool UploadPrescription(int userID, string fileName, string filePath)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Prescriptions (UserID, FileName, FilePath) VALUES (@UserID, @FileName, @FilePath)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@FileName", fileName);
                        cmd.Parameters.AddWithValue("@FilePath", filePath);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable GetPrescriptions(int userID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Prescriptions WHERE UserID = @UserID ORDER BY UploadDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }


        public static DataTable GetOrders(int userID)
        {
            // Kept for backwards compatibility — delegates to GetOrderHeaders
            return GetOrderHeaders(userID);
        }

        public static bool UpdatePassword(int userID, string newPassword)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", newPassword);
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
    }
}