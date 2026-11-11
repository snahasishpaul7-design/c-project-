<p align="center">
  <h1 align="center">💊 MediCare Pharmacy Management System</h1>
  <p align="center">
    <img src="https://img.shields.io/badge/Platform-Windows-blue?style=for-the-badge&logo=windows" />
    <img src="https://img.shields.io/badge/.NET_Framework-4.7.2-purple?style=for-the-badge&logo=dotnet" />
    <img src="https://img.shields.io/badge/Database-SQL_Server-red?style=for-the-badge&logo=microsoftsqlserver" />
    <img src="https://img.shields.io/badge/Language-C%23-green?style=for-the-badge&logo=csharp" />
    <img src="https://img.shields.io/badge/License-Educational-orange?style=for-the-badge" />
  </p>
</p>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#️-tech-stack)
- [Database Schema](#️-database-schema)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Default Login Credentials](#-default-login-credentials)
- [Contributors](#-contributors)

---

## 🏥 Overview

**MediCare** is a role-based pharmacy desktop application designed to streamline the complete pharmacy workflow — from customer medicine orders to pharmacist inventory management and admin-level oversight.

> 🎯 **Three roles. One system. Zero chaos.**

| 👤 Customer | 💊 Pharmacist | 🛡️ Admin |
|---|---|---|
| Browse & order medicines | Manage inventory | Full system control |
| Upload prescriptions | Process orders | Analytics & reports |
| Track order status | Approve prescriptions | User management |

---

## ✨ Features

### 👤 Customer
- ✅ Register and login with role-based access
- 🔍 Browse and search medicines with real-time stock display
- 🛒 Add medicines to cart with quantity validation
- ❌ Remove items from cart (stock automatically restocked)
- 📦 Place orders and track order status
- 📄 Upload and view prescriptions
- ✏️ Edit profile and change password
- 🔐 Forgot password / reset via email

### 💊 Pharmacist
- 📋 View and search medicine inventory
- 📦 Restock medicines with quantity popup
- 🔄 Process and update order status (`Pending → Processing → Delivered`)
- 🔎 Filter orders by status
- ✅ Approve or reject customer prescriptions
- ✏️ Edit profile and change password

### 🛡️ Admin
- 📊 Dashboard with real-time stats (users, medicines, orders, revenue)
- ➕ Add, edit, and delete medicines
- 📋 View and update all orders with status management
- 📄 Approve or reject prescriptions
- 👥 Manage all users (view, delete)
- 📈 Reports & analytics — top selling medicines, revenue stats
- ⚙️ Settings — change admin password

---

## 🛠️ Tech Stack

| Technology | Usage |
|---|---|
| ⚙️ C# (.NET Framework 4.7.2) | Application logic |
| 🖥️ Windows Forms | UI / Frontend |
| 🗄️ SQL Server (LocalDB) | Database |
| 🔌 ADO.NET | Database connectivity |
| 🎨 System.Drawing | Custom UI rendering |

---

## 🗄️ Database Schema

### 👥 Users
| Column | Type | Description |
|---|---|---|
| UserID | INT (PK) | Auto-increment primary key |
| FirstName | NVARCHAR(50) | User's first name |
| LastName | NVARCHAR(50) | User's last name |
| Username | NVARCHAR(50) | Unique username |
| Email | NVARCHAR(100) | Email address |
| Phone | NVARCHAR(20) | Phone number |
| Password | NVARCHAR(255) | User password |
| Role | NVARCHAR(20) | Customer / Pharmacist / Admin |

### 💊 Medicines
| Column | Type | Description |
|---|---|---|
| MedicineID | INT (PK) | Auto-increment primary key |
| Name | NVARCHAR(100) | Medicine name |
| Category | NVARCHAR(50) | Medicine category |
| Price | DECIMAL(10,2) | Price per unit |
| Stock | INT | Available stock quantity |
| ImagePath | NVARCHAR(500) | Optional medicine image path |

### 🛒 Cart
| Column | Type | Description |
|---|---|---|
| CartID | INT (PK) | Auto-increment primary key |
| UserID | INT (FK) | References Users |
| MedicineID | INT (FK) | References Medicines |
| Quantity | INT | Quantity added to cart |

### 📦 OrderHeaders
| Column | Type | Description |
|---|---|---|
| OrderHeaderID | INT (PK) | Auto-increment order ID |
| UserID | INT (FK) | Customer who placed the order |
| TotalPrice | DECIMAL(10,2) | Total order amount |
| Status | NVARCHAR(20) | Pending / Processing / Delivered |
| OrderDate | DATETIME | Date of the order |

### 📦 OrderItems
| Column | Type | Description |
|---|---|---|
| OrderItemID | INT (PK) | Auto-increment line ID |
| OrderHeaderID | INT (FK) | Parent order |
| MedicineName | NVARCHAR(100) | Ordered medicine |
| Quantity | INT | Quantity ordered |
| UnitPrice | DECIMAL(10,2) | Price per unit |
| TotalPrice | DECIMAL(10,2) | Line total |

### 📄 Prescriptions
| Column | Type | Description |
|---|---|---|
| PrescriptionID | INT (PK) | Auto-increment primary key |
| UserID | INT (FK) | References Users |
| FileName | NVARCHAR(255) | Uploaded file name |
| FilePath | NVARCHAR(500) | File path on disk |
| UploadDate | DATETIME | Date uploaded |
| Status | NVARCHAR(20) | Pending / Approved / Rejected |

---

## 📁 Project Structure

```
.
├── PharmacyUser.sln
└── PharmacyUser/
    ├── Program.cs
    ├── DBHelper.cs
    ├── LoginForm.cs
    ├── RegisterForm.cs
    ├── ForgotPasswordForm.cs
    ├── CustomerDashboard.cs
    ├── PharmacistDashboard.cs
    ├── AdminDashboard.cs
    └── App.config
```

---

## 🚀 Getting Started

### Prerequisites

Before you begin, make sure you have the following installed:

- 🖥️ [Visual Studio 2019 or later](https://visualstudio.microsoft.com/)
- ⚙️ [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework)
- 🗄️ [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

### ⚡ Installation

**Step 1 — Clone the repository**
```bash
git clone https://github.com/snahasishpaul7-design/c-project-.git
cd c-project-
```

**Step 2 — Set up the database**

Open SSMS, connect to `(localdb)\MSSQLLocalDB` and run:

```sql
CREATE DATABASE PharmacyDB;
USE PharmacyDB;

CREATE TABLE Users (
    UserID    INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50)  NOT NULL,
    LastName  NVARCHAR(50)  NOT NULL,
    Username  NVARCHAR(50)  NOT NULL UNIQUE,
    Email     NVARCHAR(100) NOT NULL,
    Phone     NVARCHAR(20),
    Password  NVARCHAR(255) NOT NULL,
    Role      NVARCHAR(20)  NOT NULL DEFAULT 'Customer'
);

CREATE TABLE Medicines (
    MedicineID INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    Category   NVARCHAR(50),
    Price      DECIMAL(10,2) NOT NULL,
    Stock      INT NOT NULL DEFAULT 0,
    ImagePath  NVARCHAR(500) NULL
);

CREATE TABLE Cart (
    CartID     INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT NOT NULL,
    MedicineID INT NOT NULL,
    Quantity   INT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (MedicineID) REFERENCES Medicines(MedicineID)
);

CREATE TABLE OrderHeaders (
    OrderHeaderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID        INT NOT NULL,
    TotalPrice    DECIMAL(10,2) NOT NULL,
    Status        NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    OrderDate     DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE OrderItems (
    OrderItemID   INT IDENTITY(1,1) PRIMARY KEY,
    OrderHeaderID INT NOT NULL,
    MedicineName  NVARCHAR(100) NOT NULL,
    Quantity      INT NOT NULL,
    UnitPrice     DECIMAL(10,2) NOT NULL,
    TotalPrice    DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderHeaderID) REFERENCES OrderHeaders(OrderHeaderID)
);

CREATE TABLE Prescriptions (
    PrescriptionID INT IDENTITY(1,1) PRIMARY KEY,
    UserID         INT NOT NULL,
    FileName       NVARCHAR(255) NOT NULL,
    FilePath       NVARCHAR(500),
    UploadDate     DATETIME NOT NULL DEFAULT GETDATE(),
    Status         NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- 🌱 Seed: Sample Medicines
INSERT INTO Medicines (Name, Category, Price, Stock) VALUES
('Paracetamol',  'Pain Relief',  60.00,  100),
('Vitamin D3',   'Supplements',  150.00,  80),
('Zinc Tablets', 'Supplements',   95.00,  60),
('Omeprazole',   'Gastric',       75.00,  90),
('Amoxicillin',  'Antibiotic',   120.00,  50),
('Metformin',    'Diabetes',      85.00,  70),
('Aspirin',      'Pain Relief',   45.00, 110),
('Cetirizine',   'Allergy',       55.00,  65);

-- 🌱 Seed: Default Admin
INSERT INTO Users (FirstName, LastName, Username, Email, Phone, Password, Role)
VALUES ('Admin', 'User', 'arnob', 'admin@medicare.com', '01700000000', '1', 'Admin');
```

**Step 3 — Open the project**

Open `PharmacyUser.sln` from the repository root in Visual Studio.

**Step 4 — Check the connection string**

Make sure `App.config` has:

```xml
<connectionStrings>
  <add name="PharmacyDB"
       connectionString="Server=(localdb)\MSSQLLocalDB;Database=PharmacyDB;Trusted_Connection=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Step 5 — Build and run**

Press `Ctrl + Shift + B` to build, then `F5` to run. 🎉

---

## 🔑 Default Login Credentials

| Role | Username | Password |
|---|---|---|
| 🛡️ Admin | `arnob` | `1` |

> 💡 New **Customer** and **Pharmacist** accounts can be created via the Register screen.

---

## 👥 Contributors

> 🙌 Built with teamwork, late nights, and plenty of coffee.

| # | Name | Contribution |
|---|---|---|
| 🛡️ | **Arnob Das** | Admin Dashboard |
| 👤 | **Istiauque Hossain Akib** | Customer Dashboard, Forgot Password |
| 💊 | **Adreta Mallick** | Pharmacist Dashboard |
| 🗄️ | **Snahasish Paul Tonmoy** | Database Connection, Register, Login, README |

---

## 📄 License

> 🎓 This project is developed for **educational purposes** as part of an academic assignment.  
> Feel free to learn from it — but please don't forget to credit the team!

---

<p align="center">
  Made with ❤️ by the MediCare Team
</p>