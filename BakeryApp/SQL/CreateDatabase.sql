-- ============================================================
--  BakeryApp — Скрипт создания базы данных
--  Запустите этот скрипт в SQL Server Management Studio
--  или в другом клиенте, подключённом к вашему SQL Server.
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BakeryDB')
BEGIN
    CREATE DATABASE BakeryDB;
    PRINT 'База данных BakeryDB создана.';
END
ELSE
BEGIN
    PRINT 'База данных BakeryDB уже существует.';
END
GO

USE BakeryDB;
GO

-- ============================================================
--  Ингредиенты
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Ingredients' AND xtype = 'U')
BEGIN
    CREATE TABLE Ingredients
    (
        Id            INT           IDENTITY(1,1) PRIMARY KEY,
        Name          NVARCHAR(200) NOT NULL,
        Unit          NVARCHAR(50)  NOT NULL,
        Quantity      DECIMAL(18,3) NOT NULL DEFAULT 0,
        PurchasePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreatedAt     DATETIME2     NOT NULL DEFAULT GETDATE(),
        UpdatedAt     DATETIME2     NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Таблица Ingredients создана.';
END
GO

-- ============================================================
--  Справочник готовой продукции
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Products' AND xtype = 'U')
BEGIN
    CREATE TABLE Products
    (
        Id        INT           IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(200) NOT NULL,
        Type      NVARCHAR(100) NOT NULL,
        SalePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
        Notes     NVARCHAR(500) NULL,
        CreatedAt DATETIME2     NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Таблица Products создана.';
END
GO

-- ============================================================
--  Журнал производства
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ProductionRecords' AND xtype = 'U')
BEGIN
    CREATE TABLE ProductionRecords
    (
        Id          INT           IDENTITY(1,1) PRIMARY KEY,
        RecordDate  DATE          NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Quantity    DECIMAL(18,3) NOT NULL,
        Comment     NVARCHAR(500) NULL,
        CreatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Таблица ProductionRecords создана.';
END
GO

-- ============================================================
--  Журнал продаж
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'SaleRecords' AND xtype = 'U')
BEGIN
    CREATE TABLE SaleRecords
    (
        Id          INT           IDENTITY(1,1) PRIMARY KEY,
        RecordDate  DATE          NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Quantity    DECIMAL(18,3) NOT NULL,
        UnitPrice   DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        Buyer       NVARCHAR(200) NULL,
        CreatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Таблица SaleRecords создана.';
END
GO

PRINT '';
PRINT '=== BakeryDB успешно инициализирована ===';
GO
