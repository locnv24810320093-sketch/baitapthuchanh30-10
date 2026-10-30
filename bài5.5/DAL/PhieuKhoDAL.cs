using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryPOS.DTO;

namespace InventoryPOS.DAL
{
    public class PhieuKhoDAL
    {
        public bool LuuPhieuKhoTransaction(PhieuKhoDTO phieu, List<ChiTietPhieuKhoDTO> chiTietList, out string error)
        {
            error = string.Empty;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    // 1. Chèn vào bảng PhieuKho
                    string sqlPhieu = @"INSERT INTO PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien) 
                                       OUTPUT INSERTED.MaPhieu 
                                       VALUES (@LoaiPhieu, GETDATE(), @MaNCC, @TongTien)";
                    SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@LoaiPhieu", phieu.LoaiPhieu);
                    cmdPhieu.Parameters.AddWithValue("@MaNCC", (object)phieu.MaNCC ?? DBNull.Value);
                    cmdPhieu.Parameters.AddWithValue("@TongTien", phieu.TongTien);
                    int maPhieuGenerated = (int)cmdPhieu.ExecuteScalar();

                    // 2 & 3. Chèn ChiTietPhieuKho & Cập nhật SoLuongTon trong VatTu
                    foreach (var item in chiTietList)
                    {
                        // Chèn chi tiết
                        string sqlCT = "INSERT INTO ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia) VALUES (@MaPhieu, @MaVT, @SL, @DG)";
                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                        cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieuGenerated);
                        cmdCT.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                        cmdCT.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DG", item.DonGia);
                        cmdCT.ExecuteNonQuery();

                        // Cập nhật tồn kho theo loại phiếu
                        if (phieu.LoaiPhieu == "NHAP")
                        {
                            string sqlUpdateTon = "UPDATE VatTu SET SoLuongTon = SoLuongTon + @SL WHERE MaVatTu = @MaVT";
                            SqlCommand cmdUpdate = new SqlCommand(sqlUpdateTon, conn, trans);
                            cmdUpdate.Parameters.AddWithValue("@SL", item.SoLuong);
                            cmdUpdate.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                            cmdUpdate.ExecuteNonQuery();
                        }
                        else if (phieu.LoaiPhieu == "XUAT")
                        {
                            // Kiểm tra tồn kho trước khi trừ
                            string sqlCheck = "SELECT SoLuongTon FROM VatTu WHERE MaVatTu = @MaVT";
                            SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, trans);
                            cmdCheck.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                            decimal currentTon = Convert.ToDecimal(cmdCheck.ExecuteScalar());

                            if (currentTon < item.SoLuong)
                            {
                                throw new Exception($"Không đủ hàng trong kho cho vật tư '{item.MaVatTu}'! (Hiện tồn: {currentTon}, Yêu cầu xuất: {item.SoLuong})");
                            }

                            string sqlUpdateTon = "UPDATE VatTu SET SoLuongTon = SoLuongTon - @SL WHERE MaVatTu = @MaVT";
                            SqlCommand cmdUpdate = new SqlCommand(sqlUpdateTon, conn, trans);
                            cmdUpdate.Parameters.AddWithValue("@SL", item.SoLuong);
                            cmdUpdate.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }

                    // 4. Commit khi tất cả các bước thành công
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback khi gặp bất kỳ lỗi nào
                    trans.Rollback();
                    error = ex.Message;
                    return false;
                }
            }
        }
    }
}