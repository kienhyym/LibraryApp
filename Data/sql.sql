/*
==========================================================
LIBRARY MANAGEMENT SYSTEM
Database Script
Version : 2.0
Author  : Nguyễn Đình Kiên
==========================================================
*/

-- =============================================
-- XÓA DATABASE NẾU ĐÃ TỒN TẠI
-- =============================================
USE master;
GO
SELECT @@VERSION;
IF EXISTS (
    SELECT 1
FROM sys.databases
WHERE name = N'LibDB'
)
BEGIN
    ALTER DATABASE LibDB
    SET SINGLE_USER
    WITH ROLLBACK IMMEDIATE;

    DROP DATABASE LibDB;
END
GO

-- =============================================
-- TẠO DATABASE
-- =============================================
CREATE DATABASE LibDB;
GO

USE LibDB;
GO



-- =============================================
-- BẢNG ACCOUNTS
-- =============================================
CREATE TABLE ACCOUNTS
(
    AccountId INT IDENTITY(1,1),

    Email VARCHAR(100) NOT NULL,

    PasswordHash NVARCHAR(500) NOT NULL,

    -- 1 = Admin
    -- 2 = Staff(NhanVien)
    -- 3 = Resident(CuDan)
    AccountRole INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_ACCOUNTS_IsActive DEFAULT (1),

    IsEmailVerified BIT NOT NULL
        CONSTRAINT DF_ACCOUNTS_IsEmailVerified DEFAULT (0),

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_ACCOUNTS_CreatedAt DEFAULT (GETDATE()),

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_ACCOUNTS
        PRIMARY KEY (AccountId),

    -- =========================
    -- UNIQUE
    -- =========================
    CONSTRAINT UQ_ACCOUNTS_Email
        UNIQUE (Email),

    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_ACCOUNTS_AccountRole
        CHECK (AccountRole IN (1,2,3))
);
GO

-- =============================================
-- BẢNG RESIDENTS (CƯ DÂN)
-- =============================================
CREATE TABLE RESIDENTS
(
    ResidentId INT IDENTITY(1,1),

    AccountId INT NOT NULL,

    FullName NVARCHAR(100) NOT NULL,

    DateOfBirth DATE NULL,

    -- 1 = Male(Nam)
    -- 2 = Female(Nu)
    -- 3 = Other(Khac)
    Gender INT NULL,

    PhoneNumber VARCHAR(15) NULL,

    ApartmentNumber NVARCHAR(20) NULL,

    PermanentAddress NVARCHAR(255) NULL,

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_RESIDENTS
        PRIMARY KEY (ResidentId),

    -- =========================
    -- UNIQUE
    -- Mỗi tài khoản chỉ thuộc một cư dân
    -- =========================
    CONSTRAINT UQ_RESIDENTS_AccountId
        UNIQUE (AccountId),

    -- =========================
    -- FOREIGN KEY
    -- =========================
    CONSTRAINT FK_RESIDENTS_ACCOUNTS
        FOREIGN KEY (AccountId)
        REFERENCES ACCOUNTS(AccountId),

    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_RESIDENTS_Gender
        CHECK (Gender IS NULL OR Gender IN (1,2,3))
);
GO

-- #################################################
-- =============================================
-- BẢNG PERSONNEL (NHANVIEN)
-- =============================================
CREATE TABLE PERSONNEL
(
    PersonnelId INT IDENTITY(1,1),

    AccountId INT NOT NULL,

    FullName NVARCHAR(100) NOT NULL,

    DateOfBirth DATE NULL,

    -- 1 = Male(Nam)
    -- 2 = Female(Nu)
    -- 3 = Other(Khac)
    Gender INT NULL,

    PhoneNumber VARCHAR(15) NULL,

    PersonnelAddress NVARCHAR(255) NULL,

    Position NVARCHAR(50) NULL,

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_PERSONNEL
        PRIMARY KEY (PersonnelId),

    -- =========================
    -- UNIQUE
    -- Mỗi tài khoản chỉ thuộc một nhân viên
    -- =========================
    CONSTRAINT UQ_PERSONNEL_AccountId
        UNIQUE (AccountId),

    -- =========================
    -- FOREIGN KEY
    -- =========================
    CONSTRAINT FK_PERSONNEL_ACCOUNTS
        FOREIGN KEY (AccountId)
        REFERENCES ACCOUNTS(AccountId),

    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_PERSONNEL_Gender
        CHECK (Gender IS NULL OR Gender IN (1,2,3))
);
GO

-- #################################################

-- =============================================
-- BẢNG CATEGORIES (THELOAI)
-- =============================================
CREATE TABLE CATEGORIES
(
    CategoryId INT IDENTITY(1,1),

    CategoryName NVARCHAR(100) NOT NULL,

    CategoryDescription NVARCHAR(255) NULL,

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_CATEGORIES_CreatedAt DEFAULT (GETDATE()),
    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_CATEGORIES
        PRIMARY KEY
    (CategoryId),
    CONSTRAINT UQ_CATEGORIES_CategoryName
    UNIQUE (CategoryName)

);
GO

-- #################################################

-- =============================================
-- BẢNG AUTHORS (TACGIA)
-- =============================================
CREATE TABLE AUTHORS
(
    AuthorId INT IDENTITY(1,1),

    AuthorName NVARCHAR(100) NOT NULL,

    Nationality NVARCHAR(50) NULL,

    Notes NVARCHAR(255) NULL,

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_AUTHORS_CreatedAt DEFAULT (GETDATE()),

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_AUTHORS
        PRIMARY KEY (AuthorId),

    -- =========================
    -- UNIQUE
    -- =========================
    CONSTRAINT UQ_AUTHORS_AuthorName
        UNIQUE (AuthorName)

);
GO


-- #################################################
-- =============================================
-- BẢNG BOOKS (SACH)
-- =============================================
CREATE TABLE BOOKS
(
    BookId INT IDENTITY(1,1),

    Title NVARCHAR(200) NOT NULL,

    CategoryId INT NOT NULL,

    AuthorId INT NOT NULL,

    Publisher NVARCHAR(150) NULL,

    PublicationYear INT NULL,

    Quantity INT NOT NULL
        CONSTRAINT DF_BOOKS_Quantity DEFAULT (0),

    AvailableQuantity INT NOT NULL
        CONSTRAINT DF_BOOKS_AvailableQuantity DEFAULT (0),

    BookDescription NVARCHAR(MAX) NULL,

    CoverImage NVARCHAR(255) NULL,

    IsAvailable BIT NOT NULL
        CONSTRAINT DF_BOOKS_IsAvailable DEFAULT (1),

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_BOOKS_CreatedAt DEFAULT (GETDATE()),

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_BOOKS
        PRIMARY KEY (BookId),

    -- =========================
    -- FOREIGN KEY
    -- =========================
    CONSTRAINT FK_BOOKS_CATEGORIES
        FOREIGN KEY (CategoryId)
        REFERENCES CATEGORIES(CategoryId),

    CONSTRAINT FK_BOOKS_AUTHORS
        FOREIGN KEY (AuthorId)
        REFERENCES AUTHORS(AuthorId),


    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_BOOKS_Quantity
        CHECK
        (
            Quantity >= 0
        AND AvailableQuantity >= 0
        AND AvailableQuantity <= Quantity
        ),

    CONSTRAINT CK_BOOKS_PublicationYear
        CHECK
        (
            PublicationYear IS NULL
        OR
        (
                PublicationYear >= 1000
        AND PublicationYear <= YEAR(GETDATE())
            )
        )
);
GO

-- #################################################


-- =============================================
-- BẢNG BORROWRECORDS (PHIEUMUON)
-- =============================================
CREATE TABLE BORROWRECORDS
(
    BorrowRecordId INT IDENTITY(1,1),

    ResidentId INT NOT NULL,

    PersonnelId INT NOT NULL,

    BorrowDate DATETIME NOT NULL
        CONSTRAINT DF_BORROWRECORDS_BorrowDate DEFAULT (GETDATE()),

    DueDate DATETIME NOT NULL,

    -- 1 = Borrowing (DangMuon)
    -- 2 = Returned (DaTra)
    -- 3 = Overdue (QuaHan)
    -- 4 = Lost (MatSach)
    BorrowRecordStatus INT NOT NULL
        CONSTRAINT DF_BORROWRECORDS_BorrowRecordStatus DEFAULT (1),

    Notes NVARCHAR(250) NULL,

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_BORROWRECORDS
        PRIMARY KEY (BorrowRecordId),

    -- =========================
    -- FOREIGN KEY
    -- =========================
    CONSTRAINT FK_BORROWRECORDS_RESIDENTS
        FOREIGN KEY (ResidentId)
        REFERENCES RESIDENTS(ResidentId),

    CONSTRAINT FK_BORROWRECORDS_PERSONNEL
        FOREIGN KEY (PersonnelId)
        REFERENCES PERSONNEL(PersonnelId),


    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_BORROWRECORDS_BorrowRecordStatus
        CHECK (BorrowRecordStatus IN (1,2,3,4)),

    CONSTRAINT CK_BORROWRECORDS_DueDate
        CHECK (DueDate >= BorrowDate)
);
GO

-- #################################################


-- =============================================
-- BẢNG BORROWRECORDDETAILS (CHITIETPHIEUMUON)
-- =============================================
CREATE TABLE BORROWRECORDDETAILS
(
    BorrowRecordDetailId INT IDENTITY(1,1),

    BorrowRecordId INT NOT NULL,

    BookId INT NOT NULL,

    Quantity INT NOT NULL
        CONSTRAINT DF_BORROWRECORDDETAILS_Quantity DEFAULT (1),

    ReturnDate DATETIME NULL,

    ReturnCondition NVARCHAR(100) NULL,

    -- =========================
    -- PRIMARY KEY
    -- =========================
    CONSTRAINT PK_BORROWRECORDDETAILS
        PRIMARY KEY (BorrowRecordDetailId),

    -- =========================
    -- FOREIGN KEY
    -- =========================
    CONSTRAINT FK_BORROWRECORDDETAILS_BORROWRECORDS
        FOREIGN KEY (BorrowRecordId)
        REFERENCES BORROWRECORDS(BorrowRecordId),

    CONSTRAINT FK_BORROWRECORDDETAILS_BOOKS
        FOREIGN KEY (BookId)
        REFERENCES BOOKS(BookId),


    -- =========================
    -- CHECK
    -- =========================
    CONSTRAINT CK_BORROWRECORDDETAILS_Quantity
        CHECK (Quantity > 0),
    CONSTRAINT UQ_BORROWRECORDDETAILS_BorrowRecordId_BookId
    UNIQUE (BorrowRecordId, BookId)

);
GO