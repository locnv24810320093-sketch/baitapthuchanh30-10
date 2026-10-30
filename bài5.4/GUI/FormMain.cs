using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HotelPOS.BUS;
using HotelPOS.DTO;

namespace HotelPOS.GUI
{
    public class FormMain : Form
    {
        private DataGridView dgvPhong;
        private ComboBox cboLoaiPhongFilter, cboTrangThaiFilter;
        private TextBox txtCCCD, txtHoTen, txtSDT;
        private Button btnCheckIn, btnCheckOut, btnGoiDV, btnDondepDone;
        private Label lblSelectedPhong, lblTongTien;
        private DataGridView dgvDichVu;
        private ComboBox cboDichVu;
        private NumericUpDown nudSoLuongDV;

        private HotelBUS bus = new HotelBUS();
        private DataTable dtPhongMaster;
        private DataView dvPhongView;

        private string currentSoPhong = "";
        private int currentMaPhieu = -1;

        public FormMain()
        {
            InitializeComponentProgrammatically();
            LoadData();
        }

        private void InitializeComponentProgrammatically()
        {
            this.Text = "Hệ thống Quản lý Khách sạn & Dịch vụ (HotelPOS)";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Group 1: Filter Ngắt kết nối (DataView)
            GroupBox gbFilter = new GroupBox() { Text = "Lọc & Tìm kiếm phòng (Disconnected)", Location = new Point(15, 10), Size = new Size(550, 60) };
            Label lblLP = new Label() { Text = "Loại:", Location = new Point(15, 25), AutoSize = true };
            cboLoaiPhongFilter = new ComboBox() { Location = new Point(55, 22), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiPhongFilter.Items.AddRange(new string[] { "Tất cả", "Đơn", "Đôi", "VIP" });
            cboLoaiPhongFilter.SelectedIndex = 0;
            cboLoaiPhongFilter.SelectedIndexChanged += FilterRooms;

            Label lblTT = new Label() { Text = "Trạng thái:", Location = new Point(200, 25), AutoSize = true };
            cboTrangThaiFilter = new ComboBox() { Location = new Point(270, 22), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThaiFilter.Items.AddRange(new string[] { "Tất cả", "Trống", "Đang ở", "Đang dọn dẹp" });
            cboTrangThaiFilter.SelectedIndex = 0;
            cboTrangThaiFilter.SelectedIndexChanged += FilterRooms;

            gbFilter.Controls.AddRange(new Control[] { lblLP, cboLoaiPhongFilter, lblTT, cboTrangThaiFilter });

            // GridView Danh sách Phòng
            dgvPhong = new DataGridView() { Location = new Point(15, 80), Size = new Size(550, 480), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvPhong.CellClick += DgvPhong_CellClick;

            // Group 2: Khách hàng & Check-in
            GroupBox gbCheckIn = new GroupBox() { Text = "Thông tin Khách / Check-in", Location = new Point(580, 10), Size = new Size(380, 180) };
            Label lblCCCD = new Label() { Text = "CCCD:", Location = new Point(15, 25), AutoSize = true };
            txtCCCD = new TextBox() { Location = new Point(90, 22), Width = 160 };
            txtCCCD.Leave += TxtCCCD_Leave; // Tra cứu tự động

            Label lblHoTen = new Label() { Text = "Họ tên:", Location = new Point(15, 60), AutoSize = true };
            txtHoTen = new TextBox() { Location = new Point(90, 57), Width = 260 };

            Label lblSDT = new Label() { Text = "SĐT:", Location = new Point(15, 95), AutoSize = true };
            txtSDT = new TextBox() { Location = new Point(90, 92), Width = 260 };

            lblSelectedPhong = new Label() { Text = "Phòng chọn: Chưa chọn", Location = new Point(15, 135), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            btnCheckIn = new Button() { Text = "Check-in", Location = new Point(260, 130), Width = 90, Height = 30, BackColor = Color.LightGreen };
            btnCheckIn.Click += BtnCheckIn_Click;

            gbCheckIn.Controls.AddRange(new Control[] { lblCCCD, txtCCCD, lblHoTen, txtHoTen, lblSDT, txtSDT, lblSelectedPhong, btnCheckIn });

            // Group 3: Gọi Dịch vụ & Check-out
            GroupBox gbService = new GroupBox() { Text = "Dịch vụ & Thanh toán Check-out", Location = new Point(580, 200), Size = new Size(380, 360) };
            
            cboDichVu = new ComboBox() { Location = new Point(15, 25), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            nudSoLuongDV = new NumericUpDown() { Location = new Point(175, 25), Width = 60, Minimum = 1, Value = 1 };
            btnGoiDV = new Button() { Text = "Thêm DV", Location = new Point(245, 23), Width = 110 };
            btnGoiDV.Click += BtnGoiDV_Click;

            dgvDichVu = new DataGridView() { Location = new Point(15, 60), Size = new Size(340, 180), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            lblTongTien = new Label() { Text = "Tổng tiền: 0 VNĐ", Location = new Point(15, 260), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), ForeColor = Color.Red };
            btnCheckOut = new Button() { Text = "Check-out (Trả phòng)", Location = new Point(15, 295), Width = 200, Height = 40, BackColor = Color.LightSkyBlue };
            btnCheckOut.Click += BtnCheckOut_Click;

            btnDondepDone = new Button() { Text = "Đã dọn xong", Location = new Point(225, 295), Width = 130, Height = 40 };
            btnDondepDone.Click += BtnDondepDone_Click;

            gbService.Controls.AddRange(new Control[] { cboDichVu, nudSoLuongDV, btnGoiDV, dgvDichVu, lblTongTien, btnCheckOut, btnDondepDone });

            this.Controls.AddRange(new Control[] { gbFilter, dgvPhong, gbCheckIn, gbService });
        }

        private void LoadData()
        {
            dtPhongMaster = bus.LayDanhSachPhong();
            dvPhongView = new DataView(dtPhongMaster);
            dgvPhong.DataSource = dvPhongView;

            // Load danh mục dịch vụ mẫu
            DataTable dtDV = new DataTable();
            dtDV.Columns.Add("MaDV", typeof(int));
            dtDV.Columns.Add("TenDV", typeof(string));
            dtDV.Rows.Add(1, "Nước suối - 10k");
            dtDV.Rows.Add(2, "Mì ly - 20k");
            dtDV.Rows.Add(3, "Giặt ủi - 50k");
            dtDV.Rows.Add(4, "Bia Tiger - 25k");
            cboDichVu.DataSource = dtDV;
            cboDichVu.DisplayMember = "TenDV";
            cboDichVu.ValueMember = "MaDV";
        }

        // Lọc ngắt kết nối bằng DataView
        private void FilterRooms(object sender, EventArgs e)
        {
            string filterLP = cboLoaiPhongFilter.SelectedItem.ToString();
            string filterTT = cboTrangThaiFilter.SelectedItem.ToString();

            string expression = "";
            if (filterLP != "Tất cả") expression += $"LoaiPhong = '{filterLP}'";
            if (filterTT != "Tất cả")
            {
                if (expression.Length > 0) expression += " AND ";
                expression += $"TrangThai = '{filterTT}'";
            }

            dvPhongView.RowFilter = expression;
        }

        private void DgvPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhong.Rows[e.RowIndex];
                currentSoPhong = row.Cells["SoPhong"].Value.ToString();
                string trangThai = row.Cells["TrangThai"].Value.ToString();
                lblSelectedPhong.Text = $"Phòng chọn: {currentSoPhong} ({trangThai})";

                if (trangThai == "Đang ở")
                {
                    DataRow phieu = bus.LayThongTinThuePhong(currentSoPhong);
                    if (phieu != null)
                    {
                        currentMaPhieu = Convert.ToInt32(phieu["MaPhieu"]);
                        txtHoTen.Text = phieu["HoTen"].ToString();
                        txtCCCD.Text = phieu["CCCD"].ToString();
                        LoadDichVuAndTinhTien(phieu);
                    }
                }
                else
                {
                    currentMaPhieu = -1;
                    dgvDichVu.DataSource = null;
                    lblTongTien.Text = "Tổng tiền: 0 VNĐ";
                }
            }
        }

        private void TxtCCCD_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCCCD.Text))
            {
                KhachHangDTO kh = bus.TimKhachHangByCCCD(txtCCCD.Text.Trim());
                if (kh != null)
                {
                    txtHoTen.Text = kh.HoTen;
                    txtSDT.Text = kh.SoDienThoai;
                }
            }
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSoPhong))
            {
                MessageBox.Show("Vui lòng chọn 1 phòng!", "Cảnh báo");
                return;
            }

            if (bus.CheckIn(currentSoPhong, txtHoTen.Text, txtCCCD.Text, txtSDT.Text, out string err))
            {
                MessageBox.Show("Check-in thành công!", "Thông báo");
                LoadData();
            }
            else
            {
                MessageBox.Show(err, "Lỗi");
            }
        }

        private void BtnGoiDV_Click(object sender, EventArgs e)
        {
            if (currentMaPhieu == -1)
            {
                MessageBox.Show("Phòng hiện tại chưa có phiếu đặt!", "Cảnh báo");
                return;
            }

            int maDV = Convert.ToInt32(cboDichVu.SelectedValue);
            int soLuong = (int)nudSoLuongDV.Value;
            bus.GoiDichVu(currentMaPhieu, maDV, soLuong);

            DataRow phieu = bus.LayThongTinThuePhong(currentSoPhong);
            LoadDichVuAndTinhTien(phieu);
        }

        private void LoadDichVuAndTinhTien(DataRow phieu)
        {
            DataTable dtDV = bus.LayDichVuDaDung(currentMaPhieu);
            dgvDichVu.DataSource = dtDV;

            decimal tiềnDV = 0;
            foreach (DataRow r in dtDV.Rows) tiềnDV += Convert.ToDecimal(r["ThanhTien"]);

            DateTime ngayNhan = Convert.ToDateTime(phieu["NgayNhanPhong"]);
            decimal giaTheoGio = Convert.ToDecimal(phieu["GiaTheoGio"]);
            int soGio = (int)Math.Ceiling((DateTime.Now - ngayNhan).TotalHours);
            if (soGio < 1) soGio = 1;

            decimal tiềnPhong = soGio * giaTheoGio;
            decimal tongTien = tiềnPhong + tiềnDV;

            lblTongTien.Text = $"Tổng: {tongTien:N0} VNĐ ({soGio}h phòng + DV)";
            lblTongTien.Tag = tongTien;
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (currentMaPhieu == -1 || lblTongTien.Tag == null) return;

            decimal tongTien = (decimal)lblTongTien.Tag;
            if (MessageBox.Show($"Xác nhận thanh toán {tongTien:N0} VNĐ cho phòng {currentSoPhong}?", "Check-out", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (bus.CheckOut(currentMaPhieu, currentSoPhong, tongTien))
                {
                    MessageBox.Show("Thanh toán thành công! Phòng chuyển sang 'Đang dọn dẹp'.", "Thông báo");
                    LoadData();
                }
            }
        }

        private void BtnDondepDone_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentSoPhong))
            {
                bus.ChuyenTrangThaiPhong(currentSoPhong, "Trống");
                LoadData();
            }
        }
    }
}