using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Lab_Mvc.Contest
{
    public static class DbInitializer
    {
        public static void Initialize(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // 1. Create Tables
            CreateTables(connection);
        }

        private static void CreateTables(IDbConnection connection)
        {
            // Table AppUser
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_AppUser]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_AppUser] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [username] NVARCHAR(100) NOT NULL UNIQUE,
                        [password] NVARCHAR(255) NOT NULL,
                        [role] NVARCHAR(50) NOT NULL
                    );
                    -- Insert default admin:admin password
                    -- Password here: admin (SHA256 hashed)
                    INSERT INTO [dbo].[Market_AppUser] ([username], [password], [role]) 
                    VALUES (N'admin', N'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Admin');
                END
            ");

            // Table Hotel
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_Hotel]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_Hotel] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [hotel_name] NVARCHAR(200) NOT NULL,
                        [address] NVARCHAR(MAX) NULL,
                        [contact_number] NVARCHAR(50) NULL
                    );
                END
            ");

            // Table Vegetable
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_Vegetable]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_Vegetable] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Eng_vegetable_name] NVARCHAR(200) NOT NULL UNIQUE,
                        [Mar_vegetable_name] NVARCHAR(200) NULL
                    );
                END
            ");

            // Table PurchaseEntry
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_PurchaseEntry]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_PurchaseEntry] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [hotel_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Market_Hotel]([id]) ON DELETE CASCADE,
                        [date] DATETIME NOT NULL,
                        [payment_method] NVARCHAR(50) NOT NULL,
                        [paid_amount] DECIMAL(18,2) NOT NULL,
                        [payment_image] NVARCHAR(MAX) NULL,
                        [grand_total] DECIMAL(18,2) NOT NULL,
                        [notes] NVARCHAR(MAX) NULL,
                        [show_marathi] BIT NOT NULL DEFAULT 0
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Market_PurchaseEntry]') AND name = 'show_marathi')
                    BEGIN
                        ALTER TABLE [dbo].[Market_PurchaseEntry] ADD [show_marathi] BIT NOT NULL DEFAULT 0;
                    END
                END
            ");

            // Table PurchasePdf (Separate storage)
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_PurchasePdf]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_PurchasePdf] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [purchase_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Market_PurchaseEntry]([id]) ON DELETE CASCADE UNIQUE,
                        [pdf_file] VARBINARY(MAX) NOT NULL
                    );
                END
            ");

            // Table PurchaseItem
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Market_PurchaseItem]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Market_PurchaseItem] (
                        [id] INT IDENTITY(1,1) PRIMARY KEY,
                        [purchase_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Market_PurchaseEntry]([id]) ON DELETE CASCADE,
                        [vegetable_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Market_Vegetable]([id]),
                        [quantity] DECIMAL(18,2) NOT NULL,
                        [price_per_kg] DECIMAL(18,2) NOT NULL,
                        [total] DECIMAL(18,2) NOT NULL
                    );
                END
            ");
        }
    }
}
