create database OnlineShopDB;

use OnlineShopDB

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(MAX) NOT NULL,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    PhoneNumber VARCHAR(20),
	UserRole VARCHAR(20) DEFAULT 'Cliente',
    CreatedAt DATETIME DEFAULT GETDATE()
);

--Este insert agrega una contraseña de 12345678 pero como esta encriptado puede estallar el sistema, EJECUTAR ESTE INSERT DE PRIMERO
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, UserRole)
VALUES ('admin', 'admin@mail.com', 'AQAAAAIAAYagAAAAEDON3oYdoXk9k3SHRK2WaTPuqwU+URTp2o41EBH2GslnTphCZToReMV9zeRrjnWJXQ==', 'Admin', 'S', '88888888', 'Admin');

select * from users

CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    CategoryId INT NOT NULL,
    Stock INT DEFAULT 0,
	ImagePath VARCHAR(255), -- Ruta relativa de la imagen
    ImageFileName VARCHAR(100), -- Nombre del archivo
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL UNIQUE
);

select * from categories

CREATE TABLE Cart (
    CartId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

select * from cart

CREATE TABLE CartItems (
    CartItemId INT PRIMARY KEY IDENTITY(1,1),
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (CartId) REFERENCES Cart(CartId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

select * from CartItems

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status VARCHAR(20) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

select * from Orders

CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

select * from OrderDetails

