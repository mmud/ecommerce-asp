CREATE DATABASE ECommerce;
GO

USE ECommerce;
GO

-- Customer Table
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
	role VARCHAR(255),
	passwordHash VARCHAR(512),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);


-- Product Table
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Quantity INT NOT NULL,
    Description NVARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
	ImagePath NVARCHAR(MAX) NULL,
	AdditionalImage1 NVARCHAR(MAX),
    AdditionalImage2 NVARCHAR(MAX),
    AdditionalImage3 NVARCHAR(MAX),
    CONSTRAINT UQ_Products_Name UNIQUE (Name)
);
GO


-- Orders Table
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    OrderDate DATETIME NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

-- OrderItems Table
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    PriceAtOrder DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO
