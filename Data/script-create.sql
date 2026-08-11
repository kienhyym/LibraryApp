/*
==========================================================
LIBRARY MANAGEMENT SYSTEM
Database Script
Version : 1.0
Author  : Nguyễn Đình Kiên
Database: LibDB
==========================================================

MÔ TẢ:

1. ACCOUNTS
   - Quản lý tài khoản Admin / Staff / Resident
   - AccountRole:
       1 = Admin
       2 = Staff
       3 = Resident

2. RESIDENTS
   - Thông tin cư dân

3. PERSONNEL
   - Thông tin nhân viên

4. CATEGORIES
   - Thể loại sách

5. AUTHORS
   - Tác giả

6. BOOKS
   - Sách

7. BORROWRECORDS
   - Phiếu mượn
   - BorrowRecordStatus:
       1 = Borrowing
       2 = Returned
       3 = Overdue

8. BORROWRECORDDETAILS
   - Chi tiết phiếu mượn
   - ReturnStatus:
       1 = Good
       2 = Damaged
       3 = Lost

9. EMAIL_VERIFICATIONS
   - OTP xác thực email

10. FAVORITEBOOK
    - Sách yêu thích của cư dân
==========================================================
*/


-- ======================================================
-- 1. XÓA DATABASE CŨ NẾU ĐÃ TỒN TẠI
-- ======================================================

USE master;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.databases
    WHERE name = N'LibDB'
)
BEGIN

    ALTER DATABASE LibDB
    SET SINGLE_USER
    WITH ROLLBACK IMMEDIATE;

    DROP DATABASE LibDB;

END;
GO


-- ======================================================
-- 2. TẠO DATABASE
-- ======================================================

CREATE DATABASE LibDB;
GO

USE LibDB;
GO


-- ======================================================
-- 3. BẢNG ACCOUNTS
-- ======================================================

CREATE TABLE ACCOUNTS
(
    AccountId INT IDENTITY(1,1) NOT NULL,

    Email VARCHAR(100) NOT NULL,

    PasswordHash NVARCHAR(500) NOT NULL,

    /*
        1 = Admin
        2 = Staff
        3 = Resident
    */
    AccountRole INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_ACCOUNTS_IsActive
        DEFAULT (1),

    IsEmailVerified BIT NOT NULL
        CONSTRAINT DF_ACCOUNTS_IsEmailVerified
        DEFAULT (0),

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_ACCOUNTS_CreatedAt
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_ACCOUNTS
        PRIMARY KEY (AccountId),


    -- UNIQUE
    CONSTRAINT UQ_ACCOUNTS_Email
        UNIQUE (Email),


    -- CHECK
    CONSTRAINT CK_ACCOUNTS_AccountRole
        CHECK (AccountRole IN (1,2,3))
);
GO


-- ======================================================
-- 4. BẢNG RESIDENTS
-- ======================================================

CREATE TABLE RESIDENTS
(
    ResidentId INT IDENTITY(1,1) NOT NULL,

    AccountId INT NOT NULL,

    FullName NVARCHAR(100) NOT NULL,

    DateOfBirth DATE NULL,

    /*
        1 = Male
        2 = Female
        3 = Other
    */
    Gender INT NULL,

    PhoneNumber VARCHAR(15) NULL,

    ApartmentNumber NVARCHAR(20) NULL,

    PermanentAddress NVARCHAR(255) NULL,


    -- PRIMARY KEY
    CONSTRAINT PK_RESIDENTS
        PRIMARY KEY (ResidentId),


    -- Mỗi tài khoản chỉ thuộc một cư dân
    CONSTRAINT UQ_RESIDENTS_AccountId
        UNIQUE (AccountId),


    -- FOREIGN KEY
    CONSTRAINT FK_RESIDENTS_ACCOUNTS
        FOREIGN KEY (AccountId)
        REFERENCES ACCOUNTS(AccountId),


    -- CHECK
    CONSTRAINT CK_RESIDENTS_Gender
        CHECK
        (
            Gender IS NULL
            OR Gender IN (1,2,3)
        )
);
GO


-- ======================================================
-- 5. BẢNG PERSONNEL
-- ======================================================

CREATE TABLE PERSONNEL
(
    PersonnelId INT IDENTITY(1,1) NOT NULL,

    AccountId INT NOT NULL,

    FullName NVARCHAR(100) NOT NULL,

    DateOfBirth DATE NULL,

    /*
        1 = Male
        2 = Female
        3 = Other
    */
    Gender INT NULL,

    PhoneNumber VARCHAR(15) NULL,

    PersonnelAddress NVARCHAR(255) NULL,

    -- PRIMARY KEY
    CONSTRAINT PK_PERSONNEL
        PRIMARY KEY (PersonnelId),

    -- Mỗi tài khoản chỉ thuộc một nhân viên
    CONSTRAINT UQ_PERSONNEL_AccountId
        UNIQUE (AccountId),

    -- FOREIGN KEY
    CONSTRAINT FK_PERSONNEL_ACCOUNTS
        FOREIGN KEY (AccountId)
        REFERENCES ACCOUNTS(AccountId),

    -- CHECK
    CONSTRAINT CK_PERSONNEL_Gender
        CHECK
        (
            Gender IS NULL
            OR Gender IN (1,2,3)
        )
);
GO


-- ======================================================
-- 6. BẢNG CATEGORIES
-- ======================================================

CREATE TABLE CATEGORIES
(
    CategoryId INT IDENTITY(1,1) NOT NULL,

    CategoryName NVARCHAR(100) NOT NULL,

    CategoryDescription NVARCHAR(255) NULL,

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_CATEGORIES_CreatedAt
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_CATEGORIES
        PRIMARY KEY (CategoryId),


    -- UNIQUE
    CONSTRAINT UQ_CATEGORIES_CategoryName
        UNIQUE (CategoryName)
);
GO


-- ======================================================
-- 7. BẢNG AUTHORS
-- ======================================================

CREATE TABLE AUTHORS
(
    AuthorId INT IDENTITY(1,1) NOT NULL,

    AuthorName NVARCHAR(100) NOT NULL,

    Nationality NVARCHAR(50) NULL,

    Notes NVARCHAR(255) NULL,

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_AUTHORS_CreatedAt
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_AUTHORS
        PRIMARY KEY (AuthorId),


    -- UNIQUE
    CONSTRAINT UQ_AUTHORS_AuthorName
        UNIQUE (AuthorName)
);
GO


-- ======================================================
-- 8. BẢNG BOOKS
-- ======================================================

CREATE TABLE BOOKS
(
    BookId INT IDENTITY(1,1) NOT NULL,

    Title NVARCHAR(200) NOT NULL,

    CategoryId INT NOT NULL,

    AuthorId INT NOT NULL,

    Publisher NVARCHAR(150) NULL,

    PublicationYear INT NULL,

    Quantity INT NOT NULL
        CONSTRAINT DF_BOOKS_Quantity
        DEFAULT (0),

    AvailableQuantity INT NOT NULL
        CONSTRAINT DF_BOOKS_AvailableQuantity
        DEFAULT (0),

    BookDescription NVARCHAR(MAX) NULL,

    CoverImage NVARCHAR(255) NULL,

    IsAvailable BIT NOT NULL
        CONSTRAINT DF_BOOKS_IsAvailable
        DEFAULT (1),

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_BOOKS_CreatedAt
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_BOOKS
        PRIMARY KEY (BookId),


    -- FOREIGN KEY
    CONSTRAINT FK_BOOKS_CATEGORIES
        FOREIGN KEY (CategoryId)
        REFERENCES CATEGORIES(CategoryId),

    CONSTRAINT FK_BOOKS_AUTHORS
        FOREIGN KEY (AuthorId)
        REFERENCES AUTHORS(AuthorId),


    -- CHECK số lượng
    CONSTRAINT CK_BOOKS_Quantity
        CHECK
        (
            Quantity >= 0
            AND AvailableQuantity >= 0
            AND AvailableQuantity <= Quantity
        ),


    -- CHECK năm xuất bản
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


-- ======================================================
-- 9. BẢNG BORROWRECORDS
-- ======================================================

CREATE TABLE BORROWRECORDS
(
    BorrowRecordId INT IDENTITY(1,1) NOT NULL,

    ResidentId INT NOT NULL,

    PersonnelId INT NOT NULL,

    BorrowDate DATETIME NOT NULL
        CONSTRAINT DF_BORROWRECORDS_BorrowDate
        DEFAULT (GETDATE()),

    DueDate DATETIME NOT NULL,

    /*
        1 = Borrowing
        2 = Returned
        3 = Overdue
    */
    BorrowRecordStatus INT NOT NULL
        CONSTRAINT DF_BORROWRECORDS_BorrowRecordStatus
        DEFAULT (1),

    Notes NVARCHAR(250) NULL,


    -- PRIMARY KEY
    CONSTRAINT PK_BORROWRECORDS
        PRIMARY KEY (BorrowRecordId),


    -- FOREIGN KEY
    CONSTRAINT FK_BORROWRECORDS_RESIDENTS
        FOREIGN KEY (ResidentId)
        REFERENCES RESIDENTS(ResidentId),

    CONSTRAINT FK_BORROWRECORDS_PERSONNEL
        FOREIGN KEY (PersonnelId)
        REFERENCES PERSONNEL(PersonnelId),


    -- CHECK trạng thái
    CONSTRAINT CK_BORROWRECORDS_BorrowRecordStatus
        CHECK
        (
            BorrowRecordStatus IN (1,2,3)
        ),


    -- CHECK ngày trả
    CONSTRAINT CK_BORROWRECORDS_DueDate
        CHECK
        (
            DueDate >= BorrowDate
        )
);
GO


-- ======================================================
-- 10. BẢNG BORROWRECORDDETAILS
-- ======================================================

CREATE TABLE BORROWRECORDDETAILS
(
    BorrowRecordDetailId INT IDENTITY(1,1) NOT NULL,

    BorrowRecordId INT NOT NULL,

    BookId INT NOT NULL,

    ReturnDate DATETIME NULL,

    /*
        ReturnStatus:
            NULL = Chưa trả
            1    = Tốt
            2    = Hư hỏng
            3    = Mất
    */
    ReturnStatus INT NULL,

    ReturnNote NVARCHAR(500) NULL,

    /*
        Tiền phạt của cuốn sách
        Đơn vị: VNĐ
    */
    Penalty DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_BORROWRECORDDETAILS_Penalty
        DEFAULT (0),

    -- PRIMARY KEY
    CONSTRAINT PK_BORROWRECORDDETAILS
        PRIMARY KEY (BorrowRecordDetailId),

    -- FOREIGN KEY
    CONSTRAINT FK_BORROWRECORDDETAILS_BORROWRECORDS
        FOREIGN KEY (BorrowRecordId)
        REFERENCES BORROWRECORDS(BorrowRecordId),

    CONSTRAINT FK_BORROWRECORDDETAILS_BOOKS
        FOREIGN KEY (BookId)
        REFERENCES BOOKS(BookId),

    -- CHECK trạng thái trả
    CONSTRAINT CK_BORROWRECORDDETAILS_ReturnStatus
        CHECK
        (
            ReturnStatus IS NULL
            OR ReturnStatus IN (1,2,3)
        ),

    -- CHECK tiền phạt
    CONSTRAINT CK_BORROWRECORDDETAILS_Penalty
        CHECK
        (
            Penalty >= 0
        ),

    -- Một sách chỉ xuất hiện một lần
    -- trong cùng một phiếu mượn
    CONSTRAINT UQ_BORROWRECORDDETAILS_BorrowRecordId_BookId
        UNIQUE (BorrowRecordId, BookId)
);
GO


-- ======================================================
-- 11. BẢNG EMAIL_VERIFICATIONS
-- ======================================================

CREATE TABLE EMAIL_VERIFICATIONS
(
    EmailVerificationId INT IDENTITY(1,1) NOT NULL,

    Email VARCHAR(100) NOT NULL,

    OtpCode CHAR(6) NOT NULL,

    ExpiredAt DATETIME NOT NULL,

    IsVerified BIT NOT NULL
        CONSTRAINT DF_EMAIL_VERIFICATIONS_IsVerified
        DEFAULT (0),

    CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_EMAIL_VERIFICATIONS_CreatedAt
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_EMAIL_VERIFICATIONS
        PRIMARY KEY (EmailVerificationId)
);
GO


-- ======================================================
-- 12. BẢNG FAVORITEBOOK
-- ======================================================

CREATE TABLE FAVORITEBOOK
(
    FavoriteBookId INT IDENTITY(1,1) NOT NULL,

    ResidentId INT NOT NULL,

    BookId INT NOT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_FAVORITEBOOK_CreatedDate
        DEFAULT (GETDATE()),


    -- PRIMARY KEY
    CONSTRAINT PK_FAVORITEBOOK
        PRIMARY KEY (FavoriteBookId),


    -- FOREIGN KEY
    CONSTRAINT FK_FAVORITEBOOK_RESIDENT
        FOREIGN KEY (ResidentId)
        REFERENCES RESIDENTS(ResidentId),

    CONSTRAINT FK_FAVORITEBOOK_BOOK
        FOREIGN KEY (BookId)
        REFERENCES BOOKS(BookId),


    -- Một cư dân không thể yêu thích
    -- cùng một cuốn sách nhiều lần
    CONSTRAINT UQ_FAVORITEBOOK
        UNIQUE (ResidentId, BookId)
);
GO


-- ======================================================
-- 13. HOÀN TẤT
-- ======================================================

PRINT '==============================================';
PRINT 'LibDB database created successfully.';
PRINT 'All tables and constraints have been created.';
PRINT '==============================================';
GO