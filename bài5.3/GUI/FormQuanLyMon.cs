using System;
using System.Drawing;
using System.Windows.Forms;
using CafePOS.BUS;
using CafePOS.DTO;

namespace CafePOS.GUI
{
    public class FormQuanLyMon : Form
    {
        private DataGridView dgvMonAn;
        private TextBox txtTenMon, txtDonGia;
        private ComboBox cboDanhMuc;
        private Button btnThem, btnSua, btnXoa;

        private MonAnBUS monAnBUS = new MonAnBUS();
        private int maMonChon = -1;

        public FormQuanLyMon()
        {
            InitializeComponentProgrammatically();
            LoadData();
        }

        private void InitializeComponentProgrammatically()
        {
            this.Text = "Quản lý Thực đơn Món ăn";
            this.Size = new Size(650, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTenMon = new Label() { Text = "Tên món:", Location = new Point(20, 20), AutoSize = true };
            txtTenMon = new TextBox() { Location = new Point(90, 17), Width = 180 };

            Label lblDanhMuc = new Label() { Text = "Danh mục:", Location = new Point(300, 20), AutoSize = true };
            cboDanhMuc = new ComboBox() { Location = new Point(380, 17), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblDonGia = new Label() { Text = "Đơn giá:", Location = new Point(20, 60), AutoSize = true };
            txtDonGia = new TextBox() { Location = new Point(90, 57), Width = 180 };

            btnThem = new Button() { Text = "Thêm", Location = new Point(300, 55), Width = 80 };
            btnSua = new Button() { Text = "Sửa", Location = new Point(390, 55), Width = 80 };
            btnXoa = new Button() { Text = "Xóa", Location = new Point(480, 55), Width = 80 };

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;

            dgvMonAn = new DataGridView() { Location = new Point(20, 100), Size = new Size(590, 280), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };
            dgvMonAn.CellClick += dgvMonAn_CellClick;

            this.Controls.Add(lblTenMon);
            this.Controls.Add(txtTenMon);
            this.Controls.Add(lblDanhMuc);
            this.Controls.Add(cboDanhMuc);
            this.Controls.Add(lblDonGia);
            this.Controls.Add(txtDonGia);
            this.Controls.Add(btnThem);
            this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa);
            this.Controls.Add(dgvMonAn);
        }

        private void LoadData()
        {
            dgvMonAn.DataSource = monAnBUS.LayTatCaMonAn();
            cboDanhMuc.DataSource = monAnBUS.LayDanhSachDanhMuc();
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";
        }

        private void dgvMonAn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMonAn.Rows[e.RowIndex];
                maMonChon = Convert.ToInt32(row.Cells["MaMon"].Value);
                txtTenMon.Text = row.Cells["TenMon"].Value.ToString();
                cboDanhMuc.SelectedValue = row.Cells["MaDanhMuc"].Value;
                txtDonGia.Text = row.Cells["DonGia"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MonAnDTO mon = new MonAnDTO()
            {
                TenMon = txtTenMon.Text,
                MaDanhMuc = Convert.ToInt32(cboDanhMuc.SelectedValue),
                DonGia = donGia
            };

            if (monAnBUS.ThemMon(mon, out string error))
            {
                MessageBox.Show("Thêm món ăn thành công!", "Thông báo");
                LoadData();
            }
            else
            {
                MessageBox.Show(error, "Cảnh báo Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (maMonChon == -1)
            {
                MessageBox.Show("Vui lòng chọn món cần sửa!", "Cảnh báo");
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ!", "Cảnh báo");
                return;
            }

            MonAnDTO mon = new MonAnDTO()
            {
                MaMon = maMonChon,
                TenMon = txtTenMon.Text,
                MaDanhMuc = Convert.ToInt32(cboDanhMuc.SelectedValue),
                DonGia = donGia
            };

            if (monAnBUS.SuaMon(mon, out string error))
            {
                MessageBox.Show("Sửa thông tin món thành công!", "Thông báo");
                LoadData();
            }
            else
            {
                MessageBox.Show(error, "Cảnh báo Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (maMonChon == -1) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa món này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (monAnBUS.XoaMon(maMonChon))
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    LoadData();
                }
            }
        }
    }
}