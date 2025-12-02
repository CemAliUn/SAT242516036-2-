USE SAT242516036;
GO

/* ============================
   1) TABLO SİLME (VARSA)
============================ */
IF OBJECT_ID('dbo.Transactions', 'U') IS NOT NULL DROP TABLE dbo.Transactions;
IF OBJECT_ID('dbo.LoyaltyPoints', 'U') IS NOT NULL DROP TABLE dbo.LoyaltyPoints;
IF OBJECT_ID('dbo.Campaigns', 'U') IS NOT NULL DROP TABLE dbo.Campaigns;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
GO

/* ============================
   2) TABLO OLUŞTURMA
============================ */
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20),
    JoinDate DATE DEFAULT GETDATE()
);
GO

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Points INT NOT NULL
);
GO

CREATE TABLE Transactions (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    TransactionDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE LoyaltyPoints (
    PointID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    PointsEarned INT NOT NULL,
    PointsUsed INT DEFAULT 0,
    Date DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Campaigns (
    CampaignID INT IDENTITY(1,1) PRIMARY KEY,
    CampaignName NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    BonusPoints INT NOT NULL
);
GO

/* ============================
   3) VIEW
============================ */
CREATE VIEW vw_CustomerPointsSummary AS
SELECT 
    c.CustomerID,
    c.FirstName,
    c.LastName,
    c.Email,
    c.Phone,
    COALESCE(SUM(lp.PointsEarned - lp.PointsUsed), 0) AS TotalPoints
FROM Customers c
LEFT JOIN LoyaltyPoints lp ON c.CustomerID = lp.CustomerID
GROUP BY c.CustomerID, c.FirstName, c.LastName, c.Email, c.Phone;
GO

/* ============================
   4) STORED PROCEDURES
============================ */
-- Satış → İşlem + Kazanılan Puan
CREATE PROCEDURE sp_AddTransaction
    @CustomerID INT,
    @ProductID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Price DECIMAL(10,2), @Points INT, @Total DECIMAL(10,2);

    SELECT @Price = Price, @Points = Points FROM Products WHERE ProductID = @ProductID;
    SET @Total = @Price * @Quantity;

    INSERT INTO Transactions (CustomerID, ProductID, Quantity, TotalAmount)
    VALUES (@CustomerID, @ProductID, @Quantity, @Total);

    INSERT INTO LoyaltyPoints (CustomerID, PointsEarned)
    VALUES (@CustomerID, @Points * @Quantity);
END;
GO

-- Kampanya Bonus Puanı
CREATE PROCEDURE sp_ApplyCampaignBonus
    @CustomerID INT,
    @CampaignID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BonusPoints INT;
    SELECT @BonusPoints = BonusPoints FROM Campaigns WHERE CampaignID = @CampaignID;

    INSERT INTO LoyaltyPoints (CustomerID, PointsEarned)
    VALUES (@CustomerID, @BonusPoints);
END;
GO

/* ============================
   5) TABLE TYPE + TOPLU İŞLEM SP
============================ */
CREATE TYPE TransactionListType AS TABLE 
(
    CustomerID INT,
    ProductID INT,
    Quantity INT
);
GO

CREATE PROCEDURE sp_AddBulkTransactions
    @Transactions TransactionListType READONLY
AS
BEGIN
    INSERT INTO Transactions (CustomerID, ProductID, Quantity, TotalAmount)
    SELECT 
        t.CustomerID,
        t.ProductID,
        t.Quantity,
        t.Quantity * p.Price
    FROM @Transactions t
    INNER JOIN Products p ON t.ProductID = p.ProductID;

    INSERT INTO LoyaltyPoints (CustomerID, PointsEarned)
    SELECT 
        t.CustomerID,
        t.Quantity * p.Points
    FROM @Transactions t
    INNER JOIN Products p ON t.ProductID = p.ProductID;
END;
GO

/* ============================
   6) INDEX'LER
============================ */
CREATE INDEX IX_Transactions_CustomerID ON Transactions(CustomerID);
CREATE INDEX IX_Transactions_ProductID ON Transactions(ProductID);
CREATE INDEX IX_LoyaltyPoints_CustomerID ON LoyaltyPoints(CustomerID);
CREATE INDEX IX_Customers_Email ON Customers(Email);
GO
