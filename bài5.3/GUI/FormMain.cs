using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CafePOS.DAL;
using CafePOS.BUS;

namespace CafePOS.GUI
{
    public class FormMain : Form
    {
        private FlowLayoutPanel flpBanAn;
        private ComboBox cboDanhMuc, cboMonAn;
        private NumericUpDown nudSoLuong;
        private Button btnThemMon, btnThanhToan, btnQuanLyMon;
        private DataGridView dgvHoaDon;
        private Label lblTongTien, lblBanDangChon;

        private BanAnDAL banAnDAL = new BanAnDAL();
        private HoaDonDAL hoaDonDAL = new HoaDonDAL();
        private MonAnBUS monAnBUS = new MonAnBUS();

        private int maBanDangChon = -1;
        private string tenBanDangChon = "";

        public FormMain()
        {
            InitializeComponentProgrammatically();
            LoadBanAn();
            LoadDanhMuc();
        }

        private void InitializeComponentProgrammatically()
        {
            this.Text = "Hệ thống Quản lý Bán hàng Cafe POS";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            flpBanAn = new FlowLayoutPanel() { Location = new Point(12, 12), Size = new Size(420, 580), AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(flpBanAn);

            lblBanDangChon = new Label() { Text = "Chưa chọn bàn nào", Font = new Font("Arial", 12, FontStyle.Bold), Location = new Point(450, 15), AutoSize = true };
            this.Controls.Add(lblBanDangChon);

            cboDanhMuc = new ComboBox() { Location = new Point(450, 50), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboDanhMuc.SelectedIndexChanged += cboDanhMuc_SelectedIndexChanged;
            this.Controls.Add(cboDanhMuc);

            cboMonAn = new ComboBox() { Location = new Point(610, 50), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cboMonAn);

            nudSoLuong = new NumericUpDown() { Location = new Point(770, 50), Width = 60, Minimum = 1, Value = 1 };
            this.Controls.Add(nudSoLuong);

            btnThemMon = new Button() { Text = "Thêm món", Location = new Point(840, 48), Size = new Size(90, 26) };
            btnThemMon.Click += btnThemMon_Click;
            this.Controls.Add(btnThemMon);

            dgvHoaDon = new DataGridView() { Location = new Point(450, 90), Size = new Size(510, 420), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };
            this.Controls.Add(dgvHoaDon);

            lblTongTien = new Label() { Text = "Tổng tiền: 0 VNĐ", Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Red, Location = new Point(450, 530), AutoSize = true };
            this.Controls.Add(lblTongTien);

            btnThanhToan = new Button() { Text = "Thanh toán", Location = new Point(730, 525), Size = new Size(110, 40), BackColor = Color.LightSkyBlue };
            btnThanhToan.Click += btnThanhToan_Click;
            this.Controls.Add(btnThanhToan);

            btnQuanLyMon = new Button() { Text = "Quản lý món", Location = new Point(850, 525), Size = new Size(110, 40) };
            btnQuanLyMon.Click += btnQuanLyMon_Click;
            this.Controls.Add(btnQuanLyMon);
        }

        private void LoadBanAn()
        {
            flpBanAn.Controls.Clear();
            DataTable dt = banAnDAL.LayDanhSachBanAn();

            foreach (DataRow row in dt.Rows)
            {
                Button btn = new Button();
                btn.Width = 110;
                btn.Height = 100;
                string tenBan = row["TenBan"].ToString();
                string trangThai = row["TrangThai"].ToString();

                btn.Text = $"{tenBan}\n\n({trangThai})";
                btn.Tag = row;
                btn.Font = new Font("Arial", 10, FontStyle.Bold);

                if (trangThai == "Có khách")
                {
                    btn.BackColor = Color.LightCoral;
                    btn.ForeColor = Color.DarkRed;
                }
                else
                {
                    btn.BackColor = Color.LightGreen;
                    btn.ForeColor = Color.DarkGreen;
                }

                btn.Click += BtnBanAn_Click;
                flpBanAn.Controls.Add(btn);
            }
        }

        private void BtnBanAn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            DataRow row = btn.Tag as DataRow;
            maBanDangChon = Convert.ToInt32(row["MaBan"]);
            tenBanDangChon = row["TenBan"].ToString();
            lblBanDangChon.Text = $"Đang chọn: {tenBanDangChon}";

            HienThiHoaDonBan(maBanDangChon);
        }

        private void HienThiHoaDonBan(int maBan)
        {
            int maHD = hoaDonDAL.LayMaHoaDonChuaThanhToanByBan(maBan);
            if (maHD != -1)
            {
                DataTable dt = hoaDonDAL.LayChiTietHoaDonByMaHD(maHD);
                dgvHoaDon.DataSource = dt;
                TinhTongTien(dt);
            }
            else
            {
                dgvHoaDon.DataSource = null;
                lblTongTien.Text = "Tổng tiền: 0 VNĐ";
                lblTongTien.Tag = 0;
            }
        }

        private void TinhTongTien(DataTable dt)
        {
            decimal tongTien = 0;
            foreach (DataRow r in dt.Rows)
            {
                tongTien += Convert.ToDecimal(r["ThanhTien"]);
            }
            lblTongTien.Text = $"Tổng tiền: {tongTien:N0} VNĐ";
            lblTongTien.Tag = tongTien;
        }

        private void LoadDanhMuc()
        {
            cboDanhMuc.DataSource = monAnBUS.LayDanhSachDanhMuc();
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDanhMuc.SelectedValue != null && int.TryParse(cboDanhMuc.SelectedValue.ToString(), out int maDM))
            {
                cboMonAn.DataSource = monAnBUS.LayMonAnTheoDanhMuc(maDM);
                cboMonAn.DisplayMember = "TenMon";
                cboMonAn.ValueMember = "MaMon";
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (maBanDangChon == -1)
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi gọi món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboMonAn.SelectedItem == null) return;

            int maMon = Convert.ToInt32(cboMonAn.SelectedValue);
            int soLuong = (int)nudSoLuong.Value;

            DataRowView rowMon = cboMonAn.SelectedItem as DataRowView;
            decimal donGia = Convert.ToDecimal(rowMon["DonGia"]);

            int maHD = hoaDonDAL.LayMaHoaDonChuaThanhToanByBan(maBanDangChon);
            if (maHD == -1)
            {
                maHD = hoaDonDAL.TaoHoaDonMoi(maBanDangChon);
                banAnDAL.CapNhatTrangThaiBan(maBanDangChon, "Có khách");
            }

            hoaDonDAL.ThemChiTietHoaDon(maHD, maMon, soLuong, donGia);
            LoadBanAn();
            HienThiHoaDonBan(maBanDangChon);
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (maBanDangChon == -1) return;

            int maHD = hoaDonDAL.LayMaHoaDonChuaThanhToanByBan(maBanDangChon);
            if (maHD == -1)
            {
                MessageBox.Show("Bàn này chưa có hóa đơn để thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal tongTien = lblTongTien.Tag != null ? Convert.ToDecimal(lblTongTien.Tag) : 0;

            DialogResult dr = MessageBox.Show($"Xác nhận thanh toán cho {tenBanDangChon} với tổng tiền: {tongTien:N0} VNĐ?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                bool thanhCong = hoaDonDAL.ThanhToanHoaDon(maHD, maBanDangChon, tongTien);
                if (thanhCong)
                {
                    MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBanAn();
                    HienThiHoaDonBan(maBanDangChon);
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra trong quá trình thanh toán!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnQuanLyMon_Click(object sender, EventArgs e)
        {
            FormQuanLyMon f = new FormQuanLyMon();
            f.ShowDialog();
            LoadDanhMuc();
        }
    }
}