using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using InventoryPOS.BUS;
using InventoryPOS.DTO;

namespace InventoryPOS.GUI
{
    public class FormMain : Form
    {
        private TabControl tabControl;
        private TabPage tabVatTu, tabPhieuKho, tabBaoCao;

        // Controls Tab Vật tư
        private DataGridView dgvVatTu;
        private TextBox txtSearch, txtMaVT, txtTenVT, txtDVT;
        private NumericUpDown nudSoLuongTon;
        private Button btnThemVT, btnSuaVT, btnXoaVT;

        // Controls Tab Phiếu kho
        private RadioButton rdoNhap, rdoXuat;
        private ComboBox cboNCC, cboVatTuChon;
        private NumericUpDown nudSoLuongLap, nudDonGiaLap;
        private Button btnThemDong, btnLuuPhieu;
        private DataGridView dgvChiTietPhieu;
        private Label lblTongTienPhieu;
        private List<ChiTietPhieuKhoDTO> dsChiTiet = new List<ChiTietPhieuKhoDTO>();

        // Controls Tab Báo cáo
        private Button btnCanhBaoTon, btnTongGiaTri;
        private DataGridView dgvBaoCao;
        private Label lblKetQuaBaoCao;
        private NumericUpDown nudNguongAnToan;

        private InventoryBUS bus = new InventoryBUS();

        public FormMain()
        {
            InitializeComponentProgrammatically();
            LoadData();
        }

        private void InitializeComponentProgrammatically()
        {
            this.Text = "Hệ thống Quản lý Quy trình Xuất - Nhập Kho (InventoryPOS)";
            this.Size = new Size(1000, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl() { Dock = DockStyle.Fill };
            tabVatTu = new TabPage("1. Quản lý Vật tư & Tìm kiếm");
            tabPhieuKho = new TabPage("2. Lập Phiếu Nhập / Xuất (SqlTransaction)");
            tabBaoCao = new TabPage("3. Cảnh báo Tồn kho & Báo cáo");

            tabControl.TabPages.AddRange(new TabPage[] { tabVatTu, tabPhieuKho, tabBaoCao });
            this.Controls.Add(tabControl);

            SetupTabVatTu();
            SetupTabPhieuKho();
            SetupTabBaoCao();
        }

        private void SetupTabVatTu()
        {
            Label lblSearch = new Label() { Text = "Tìm kiếm (Chống SQL Injection):", Location = new Point(15, 15), AutoSize = true };
            txtSearch = new TextBox() { Location = new Point(200, 12), Width = 200 };
            txtSearch.TextChanged += (s, e) => { dgvVatTu.DataSource = bus.TimKiemVatTu(txtSearch.Text.Trim()); };

            dgvVatTu = new DataGridView() { Location = new Point(15, 45), Size = new Size(580, 520), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvVatTu.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvVatTu.Rows[e.RowIndex];
                    txtMaVT.Text = row.Cells["MaVatTu"].Value.ToString();
                    txtTenVT.Text = row.Cells["TenVatTu"].Value.ToString();
                    txtDVT.Text = row.Cells["DVT"].Value.ToString();
                    nudSoLuongTon.Value = Convert.ToDecimal(row.Cells["SoLuongTon"].Value);
                }
            };

            GroupBox gbInfo = new GroupBox() { Text = "Thông tin Vật tư", Location = new Point(610, 45), Size = new Size(350, 300) };
            Label l1 = new Label() { Text = "Mã VT:", Location = new Point(15, 30), AutoSize = true };
            txtMaVT = new TextBox() { Location = new Point(100, 27), Width = 220 };
            Label l2 = new Label() { Text = "Tên VT:", Location = new Point(15, 70), AutoSize = true };
            txtTenVT = new TextBox() { Location = new Point(100, 67), Width = 220 };
            Label l3 = new Label() { Text = "Đơn vị tính:", Location = new Point(15, 110), AutoSize = true };
            txtDVT = new TextBox() { Location = new Point(100, 107), Width = 220 };
            Label l4 = new Label() { Text = "Số lượng tồn:", Location = new Point(15, 150), AutoSize = true };
            nudSoLuongTon = new NumericUpDown() { Location = new Point(100, 147), Width = 220, Maximum = 100000 };

            btnThemVT = new Button() { Text = "Thêm", Location = new Point(15, 220), Width = 90, Height = 35 };
            btnThemVT.Click += BtnThemVT_Click;
            btnSuaVT = new Button() { Text = "Sửa", Location = new Point(125, 220), Width = 90, Height = 35 };
            btnSuaVT.Click += BtnSuaVT_Click;
            btnXoaVT = new Button() { Text = "Xóa", Location = new Point(235, 220), Width = 90, Height = 35 };
            btnXoaVT.Click += BtnXoaVT_Click;

            gbInfo.Controls.AddRange(new Control[] { l1, txtMaVT, l2, txtTenVT, l3, txtDVT, l4, nudSoLuongTon, btnThemVT, btnSuaVT, btnXoaVT });
            tabVatTu.Controls.AddRange(new Control[] { lblSearch, txtSearch, dgvVatTu, gbInfo });
        }

        private void SetupTabPhieuKho()
        {
            GroupBox gbPhieu = new GroupBox() { Text = "Tạo Phiếu Nhập / Xuất Kho", Location = new Point(15, 15), Size = new Size(940, 100) };
            rdoNhap = new RadioButton() { Text = "Phiếu NHẬP Kho", Location = new Point(20, 25), Checked = true, AutoSize = true };
            rdoXuat = new RadioButton() { Text = "Phiếu XUẤT Kho", Location = new Point(160, 25), AutoSize = true };

            Label lblNCC = new Label() { Text = "Nhà cung cấp:", Location = new Point(310, 27), AutoSize = true };
            cboNCC = new ComboBox() { Location = new Point(410, 24), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblVT = new Label() { Text = "Vật tư:", Location = new Point(20, 62), AutoSize = true };
            cboVatTuChon = new ComboBox() { Location = new Point(80, 59), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblSL = new Label() { Text = "Số lượng:", Location = new Point(280, 62), AutoSize = true };
            nudSoLuongLap = new NumericUpDown() { Location = new Point(350, 59), Width = 80, Minimum = 1, Maximum = 10000, Value = 1 };

            Label lblDG = new Label() { Text = "Đơn giá:", Location = new Point(450, 62), AutoSize = true };
            nudDonGiaLap = new NumericUpDown() { Location = new Point(510, 59), Width = 120, Maximum = 100000000, Value = 50000 };

            btnThemDong = new Button() { Text = "Thêm vào danh sách", Location = new Point(650, 56), Width = 150, Height = 30, BackColor = Color.LightYellow };
            btnThemDong.Click += BtnThemDong_Click;

            gbPhieu.Controls.AddRange(new Control[] { rdoNhap, rdoXuat, lblNCC, cboNCC, lblVT, cboVatTuChon, lblSL, nudSoLuongLap, lblDG, nudDonGiaLap, btnThemDong });

            dgvChiTietPhieu = new DataGridView() { Location = new Point(15, 125), Size = new Size(940, 380), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            lblTongTienPhieu = new Label() { Text = "Tổng tiền phiếu: 0 VNĐ", Location = new Point(15, 520), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), ForeColor = Color.Red };

            btnLuuPhieu = new Button() { Text = "LƯU PHIẾU KHO (SqlTransaction)", Location = new Point(650, 515), Size = new Size(305, 45), BackColor = Color.LightGreen, Font = new Font(this.Font, FontStyle.Bold) };
            btnLuuPhieu.Click += BtnLuuPhieu_Click;

            tabPhieuKho.Controls.AddRange(new Control[] { gbPhieu, dgvChiTietPhieu, lblTongTienPhieu, btnLuuPhieu });
        }

        private void SetupTabBaoCao()
        {
            Label lblNguong = new Label() { Text = "Ngưỡng tồn kho an toàn:", Location = new Point(20, 20), AutoSize = true };
            nudNguongAnToan = new NumericUpDown() { Location = new Point(180, 17), Width = 100, Value = 20, Maximum = 10000 };

            btnCanhBaoTon = new Button() { Text = "Lọc Cảnh Báo Tồn Thấp", Location = new Point(300, 15), Width = 180, Height = 30 };
            btnCanhBaoTon.Click += (s, e) =>
            {
                dgvBaoCao.DataSource = bus.LayVatTuCanhBao(nudNguongAnToan.Value);
                lblKetQuaBaoCao.Text = $"Danh sách các vật tư có số lượng tồn <= {nudNguongAnToan.Value}";
            };

            btnTongGiaTri = new Button() { Text = "Tính Tổng Giá Trị Kho (ExecuteScalar)", Location = new Point(500, 15), Width = 260, Height = 30, BackColor = Color.LightSkyBlue };
            btnTongGiaTri.Click += (s, e) =>
            {
                decimal tongVal = bus.TinhTongGiaTriTonKho();
                lblKetQuaBaoCao.Text = $"TỔNG GIÁ TRỊ TỒN KHO HIỆN TẠI (ExecuteScalar): {tongVal:N0} VNĐ";
            };

            lblKetQuaBaoCao = new Label() { Text = "Vui lòng chọn chức năng báo cáo...", Location = new Point(20, 60), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            dgvBaoCao = new DataGridView() { Location = new Point(20, 90), Size = new Size(930, 480), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            tabBaoCao.Controls.AddRange(new Control[] { lblNguong, nudNguongAnToan, btnCanhBaoTon, btnTongGiaTri, lblKetQuaBaoCao, dgvBaoCao });
        }

        private void LoadData()
        {
            dgvVatTu.DataSource = bus.LayDanhSachVatTu();

            cboNCC.DataSource = bus.LayDanhSachNCC();
            cboNCC.DisplayMember = "TenNCC";
            cboNCC.ValueMember = "MaNCC";

            cboVatTuChon.DataSource = bus.LayDanhSachVatTu();
            cboVatTuChon.DisplayMember = "TenVatTu";
            cboVatTuChon.ValueMember = "MaVatTu";
        }

        private void BtnThemVT_Click(object sender, EventArgs e)
        {
            var vt = new VatTuDTO { MaVatTu = txtMaVT.Text.Trim(), TenVatTu = txtTenVT.Text.Trim(), DVT = txtDVT.Text.Trim(), SoLuongTon = nudSoLuongTon.Value };
            if (bus.ThemVatTu(vt))
            {
                MessageBox.Show("Thêm vật tư thành công!", "Thông báo");
                LoadData();
            }
        }

        private void BtnSuaVT_Click(object sender, EventArgs e)
        {
            var vt = new VatTuDTO { MaVatTu = txtMaVT.Text.Trim(), TenVatTu = txtTenVT.Text.Trim(), DVT = txtDVT.Text.Trim(), SoLuongTon = nudSoLuongTon.Value };
            if (bus.SuaVatTu(vt))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
                LoadData();
            }
        }

        private void BtnXoaVT_Click(object sender, EventArgs e)
        {
            if (bus.XoaVatTu(txtMaVT.Text.Trim()))
            {
                MessageBox.Show("Đã xóa vật tư!", "Thông báo");
                LoadData();
            }
        }

        private void BtnThemDong_Click(object sender, EventArgs e)
        {
            if (cboVatTuChon.SelectedValue == null) return;
            string maVT = cboVatTuChon.SelectedValue.ToString();
            decimal sl = nudSoLuongLap.Value;
            decimal dg = nudDonGiaLap.Value;

            dsChiTiet.Add(new ChiTietPhieuKhoDTO { MaVatTu = maVT, SoLuong = sl, DonGia = dg });
            CapNhatGridChiTiet();
        }

        private void CapNhatGridChiTiet()
        {
            dgvChiTietPhieu.DataSource = null;
            dgvChiTietPhieu.DataSource = dsChiTiet;
            decimal tong = 0;
            foreach (var item in dsChiTiet) tong += item.ThanhTien;
            lblTongTienPhieu.Text = $"Tổng tiền phiếu: {tong:N0} VNĐ";
            lblTongTienPhieu.Tag = tong;
        }

        private void BtnLuuPhieu_Click(object sender, EventArgs e)
        {
            if (dsChiTiet.Count == 0)
            {
                MessageBox.Show("Danh sách vật tư lập phiếu trống!", "Cảnh báo");
                return;
            }

            var phieu = new PhieuKhoDTO
            {
                LoaiPhieu = rdoNhap.Checked ? "NHAP" : "XUAT",
                MaNCC = cboNCC.SelectedValue != null ? Convert.ToInt32(cboNCC.SelectedValue) : null,
                TongTien = lblTongTienPhieu.Tag != null ? (decimal)lblTongTienPhieu.Tag : 0
            };

            if (bus.LuuPhieuKho(phieu, dsChiTiet, out string err))
            {
                MessageBox.Show("Lưu phiếu kho & cập nhật tồn kho thành công! (SqlTransaction đã Commit)", "Thành công");
                dsChiTiet.Clear();
                CapNhatGridChiTiet();
                LoadData();
            }
            else
            {
                MessageBox.Show("Thao tác thất bại! SqlTransaction đã Rollback.\nChi tiết: " + err, "Lỗi Transaction", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}