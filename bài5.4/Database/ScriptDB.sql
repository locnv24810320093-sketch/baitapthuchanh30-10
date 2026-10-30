CREATE DATABASE HotelDB;
GO
USE HotelDB;
GO

-- 1. Bảng Phòng
CREATE TABLE Phong (
    SoPhong VARCHAR(10) PRIMARY KEY,
    LoaiPhong NVARCHAR(50) NOT NULL,
    GiaTheoGio DECIMAL(18,2) NOT NULL,
    TrangThai NVARCHAR(30) NOT NULL -- N'Trống', N'Đang ở', N'Đang dọn dẹp'
);

-- 2. Bảng Khách Hàng
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    CCCD VARCHAR(20) UNIQUE NOT NULL,
    SoDienThoai VARCHAR(15)
);

-- 3. Bảng Dịch Vụ
CREATE TABLE DichVu (
    MaDV INT IDENTITY(1,1) PRIMARY KEY,
    TenDV NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL
);

-- 4. Bảng Phiếu Đặt Phòng
CREATE TABLE PhieuDatPhong (
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    SoPhong VARCHAR(10) FOREIGN KEY REFERENCES Phong(SoPhong),
    MaKH INT FOREIGN KEY REFERENCES KhachHang(MaKH),
    NgayNhanPhong DATETIME NOT NULL DEFAULT GETDATE(),
    NgayTraPhong DATETIME NULL,
    TongTienThanhToan DECIMAL(18,2) NULL
);

-- 5. Bảng Sử Dụng Dịch Vụ
CREATE TABLE SuDungDichVu (
    MaPhieu INT FOREIGN KEY REFERENCES PhieuDatPhong(MaPhieu),
    MaDV INT FOREIGN KEY REFERENCES DichVu(MaDV),
    SoLuong INT NOT NULL,
    ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (MaPhieu, MaDV, ThoiGian)
);

-- Dữ liệu mẫu
INSERT INTO Phong VALUES 
('P101', N'Đơn', 50000, N'Trống'),
('P102', N'Đôi', 80000, N'Trống'),
('P201', N'VIP', 150000, N'Trống'),
('P202', N'VIP', 150000, N'Đang dọn dẹp');

INSERT INTO DichVu VALUES 
(N'Nước suối', 10000),
(N'Mì ly', 20000),
(N'Giặt ủi', 50000),
(N'Bia Tiger', 25000);