// Data/DatabaseHelper.cs
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace BakeryApp.Data
{
    /// <summary>
    /// Вспомогательный класс для работы с подключением к SQL Server.
    /// Строка подключения читается из App.config (ключ "BakeryDB").
    /// </summary>
    public static class DatabaseHelper
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    var cs = ConfigurationManager.ConnectionStrings["BakeryDB"];
                    if (cs == null)
                        throw new InvalidOperationException(
                            "Строка подключения 'BakeryDB' не найдена в App.config.");
                    _connectionString = cs.ConnectionString;
                }
                return _connectionString;
            }
        }

        /// <summary>Возвращает открытое подключение к БД.</summary>
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Проверяет подключение и создаёт таблицы, если они ещё не существуют.
        /// Вызывается один раз при старте приложения.
        /// </summary>
        public static bool EnsureDatabase()
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Ingredients' AND xtype='U')
CREATE TABLE Ingredients (
    Id            INT           IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(200) NOT NULL,
    Unit          NVARCHAR(50)  NOT NULL,
    Quantity      DECIMAL(18,3) NOT NULL DEFAULT 0,
    PurchasePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt     DATETIME2     NOT NULL DEFAULT GETDATE(),
    UpdatedAt     DATETIME2     NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
CREATE TABLE Products (
    Id        INT           IDENTITY(1,1) PRIMARY KEY,
    Name      NVARCHAR(200) NOT NULL,
    Type      NVARCHAR(100) NOT NULL,
    SalePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    Notes     NVARCHAR(500) NULL,
    CreatedAt DATETIME2     NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProductionRecords' AND xtype='U')
CREATE TABLE ProductionRecords (
    Id          INT           IDENTITY(1,1) PRIMARY KEY,
    RecordDate  DATE          NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity    DECIMAL(18,3) NOT NULL,
    Comment     NVARCHAR(500) NULL,
    CreatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SaleRecords' AND xtype='U')
CREATE TABLE SaleRecords (
    Id          INT           IDENTITY(1,1) PRIMARY KEY,
    RecordDate  DATE          NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity    DECIMAL(18,3) NOT NULL,
    UnitPrice   DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Buyer       NVARCHAR(200) NULL,
    CreatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
);";
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось подключиться к базе данных.\n\n{ex.Message}\n\n" +
                    "Проверьте строку подключения в App.config.",
                    "Ошибка подключения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }
    }
}
