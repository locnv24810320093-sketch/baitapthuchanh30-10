CREATE DATABASE InventoryDB;
GO
USE InventoryDB;
GO

-- 1. Bảng Nhà Cung Cấp
CREATE TABLE NhaCungCap (
    MaNCC INT IDENTITY(1,1) PRIMARY KEY,
    TenNCC NVARCHAR(150) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(255)
);

-- 2. Bảng Vật Tư
CREATE TABLE VatTu (
    MaVatTu VARCHAR(20) PRIMARY KEY,
    TenVatTu NVARCHAR(150) NOT NULL,
    DVT NVARCHAR(20),
    SoLuongTon DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0)
);

-- 3. Bảng Phiếu Kho (Nhập / Xuất)
CREATE TABLE PhieuKho (
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    LoaiPhieu NVARCHAR(10) NOT NULL CHECK (LoaiPhieu IN (N'NHAP', N'XUAT')),
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    MaNCC INT NULL FOREIGN KEY REFERENCES NhaCungCap(MaNCC),
    TongTien DECIMAL(18,2) DEFAULT 0
);

-- 4. Bảng Chi Tiết Phiếu Kho
CREATE TABLE ChiTietPhieuKho (
    MaPhieu INT FOREIGN KEY REFERENCES PhieuKho(MaPhieu),
    MaVatTu VARCHAR(20) FOREIGN KEY REFERENCES VatTu(MaVatTu),
    SoLuong DECIMAL(18,2) NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    PRIMARY KEY (MaPhieu, MaVatTu)
);

-- Dữ liệu mẫu
INSERT INTO NhaCungCap (TenNCC, DienThoai, DiaChi) VALUES
(N'Công ty Xi Măng Hà Tiên', '0901234567', N'TP. Hồ Chí Minh'),
(N'Tập đoàn Hòa Phát', '0912345678', N'Hà Nội'),
(N'Nhà máy Sắt Thép Việt Nhật', '0923456789', N'Đà Nẵng');

INSERT INTO VatTu (MaVatTu, TenVatTu, DVT, SoLuongTon) VALUES
('VT01', N'Xi măng PCB40', N'Bao', 500),
('VT02', N'Thép Phi 12', N'Cây', 120),
('VT03', N'Cát vàng xây dựng', N'Khối', 15),
('VT04', N'Sơn Nước KOVA', N'Thùng', 5);