using lab04.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab04
{
    public partial class frmQuanLyKhoa : Form
    {
        public frmQuanLyKhoa()
        {
            InitializeComponent();
        }

        private void LoadDataGridView()
        {
            using (var context = new StudentContextDB())
            {
                var danhSachKhoa = context.Faculties.OrderBy(khoa => khoa.TotalProfessor).ToList();
                dgvQuanLyKhoa.Rows.Clear();
                foreach (var khoa in danhSachKhoa)
                {
                    dgvQuanLyKhoa.Rows.Add(khoa.FacultyID, khoa.FacultyName, khoa.TotalProfessor);
                }
            }
        }

        private void frmQuanLyKhoa_Load(object sender, EventArgs e)
        {
            cmbSapXep.Items.Add("Tăng dần");
            cmbSapXep.Items.Add("Giảm dần");
            cmbSapXep.SelectedIndex = 0;
            LoadDataGridView();
            CapNhatTongSoGS();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                var danhSachKhoa = context.Faculties.ToList();

                if (cmbSapXep.SelectedItem.ToString() == "Tăng dần")
                {
                    danhSachKhoa = danhSachKhoa.OrderBy(khoa => khoa.TotalProfessor).ToList();
                }
                else if (cmbSapXep.SelectedItem.ToString() == "Giảm dần")
                {
                    danhSachKhoa = danhSachKhoa.OrderByDescending(khoa => khoa.TotalProfessor).ToList();
                }

                dgvQuanLyKhoa.Rows.Clear();
                foreach (var khoa in danhSachKhoa)
                {
                    dgvQuanLyKhoa.Rows.Add(khoa.FacultyID, khoa.FacultyName, khoa.TotalProfessor);
                }
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool KiemTraThongTinKhoa()
        {
            err.Clear();
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text))
            {
                err.SetError(txtMaKhoa, "Vui lòng nhập mã khoa.");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtMaKhoa.Text, @"^[a-zA-Z0-9]{10}$"))
            {
                err.SetError(txtMaKhoa, "Mã khoa không hợp lệ. Mã khoa phải là chuỗi gồm 10 ký tự và không chứa ký tự đặc biệt.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtTenKhoa.Text))
            {
                err.SetError(txtTenKhoa, "Vui lòng nhập tên khoa.");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtTenKhoa.Text, @"^[\p{L}\s]{3,100}$"))
            {
                err.SetError(txtTenKhoa, "Tên khoa không hợp lệ. Tên khoa chỉ chứa chữ cái và có độ dài từ 3 đến 100 ký tự.");
                isValid = false;
            }
            int tongSoGS;
            if (string.IsNullOrWhiteSpace(txtTongSoGS.Text))
            {
                err.SetError(txtTongSoGS, "Vui lòng nhập tổng số giáo sư.");
                isValid = false;
            }
            else if (!int.TryParse(txtTongSoGS.Text, out tongSoGS) || tongSoGS < 0 || tongSoGS > 15)
            {
                err.SetError(txtTongSoGS, "Tổng số giáo sư không hợp lệ. Số lượng giáo sư phải nằm trong khoảng từ 0 đến 15.");
                isValid = false;
            }
            return isValid;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTinKhoa())
            {
                return;
            }
            int facultyID;
            int totalProfessor; 
            if (!int.TryParse(txtMaKhoa.Text, out facultyID))
            {
                MessageBox.Show("Mã khoa không hợp lệ. Vui lòng nhập một số nguyên hợp lệ cho mã khoa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtTongSoGS.Text, out totalProfessor))
            {
                MessageBox.Show("Tổng số giáo sư không hợp lệ. Vui lòng nhập một số nguyên hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool khoaDaTonTai = false;
            using (var context = new StudentContextDB())
            {         
                var khoaTonTai = context.Faculties.FirstOrDefault(f => f.FacultyID == facultyID);

                if (khoaTonTai != null)
                {
                    khoaTonTai.FacultyName = txtTenKhoa.Text;
                    khoaTonTai.TotalProfessor = totalProfessor;
                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    khoaDaTonTai = true;
                }
                else
                {
                    var khoaMoi = new Faculty
                    {
                        FacultyID = facultyID,
                        FacultyName = txtTenKhoa.Text,
                        TotalProfessor = totalProfessor
                    };

                    context.Faculties.Add(khoaMoi); 
                    context.SaveChanges();

                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }   
            LoadDataGridView();
            CapNhatTongSoGS();
            ResetForm();
        }

        private void ResetForm()
        {
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtTongSoGS.Clear();
        }

        private void dgvQuanLyKhoa_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvQuanLyKhoa.Rows[e.RowIndex];

                txtMaKhoa.Text = row.Cells["FacultyID"].Value.ToString();
                txtTenKhoa.Text = row.Cells["FacultyName"].Value.ToString();
                txtTongSoGS.Text = row.Cells["TotalProfessor"].Value.ToString();
            }
        }

        private void CapNhatTongSoGS()
        {
            int tongSoGS = 0;
            using (var context = new StudentContextDB())
            {
                tongSoGS = context.Faculties.Sum(khoa => khoa.TotalProfessor) ?? 0;
            }
            txtTongSoGS.Text = tongSoGS.ToString();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập mã khoa để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int facultyID;
            if (!int.TryParse(txtMaKhoa.Text, out facultyID))
            {
                MessageBox.Show("Mã khoa không hợp lệ. Vui lòng nhập mã khoa hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool khoaTonTai = false;
            foreach (DataGridViewRow row in dgvQuanLyKhoa.Rows)
            {
                if (row.Cells["FacultyID"].Value != null && row.Cells["FacultyID"].Value.ToString() == facultyID.ToString())
                {
                    khoaTonTai = true;
                    DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khoa này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        dgvQuanLyKhoa.Rows.RemoveAt(row.Index);
                        MessageBox.Show("Xóa khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CapNhatTongSoGS();
                        ResetForm();
                    }
                    break;
                }
            }
            if (!khoaTonTai)
            {
                MessageBox.Show("Mã khoa không tồn tại trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTinKhoa())
            {
                return;
            }
            int facultyID;
            int totalProfessor;
            if (!int.TryParse(txtMaKhoa.Text, out facultyID))
            {
                MessageBox.Show("Mã khoa không hợp lệ. Vui lòng nhập một số nguyên hợp lệ cho mã khoa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtTongSoGS.Text, out totalProfessor))
            {
                MessageBox.Show("Tổng số giáo sư không hợp lệ. Vui lòng nhập một số nguyên hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool khoaDaTonTai = false;
            foreach (DataGridViewRow row in dgvQuanLyKhoa.Rows)
            {
                if (row.Cells["FacultyID"].Value != null && row.Cells["FacultyID"].Value.ToString() == facultyID.ToString())
                {
                    row.Cells["FacultyName"].Value = txtTenKhoa.Text;
                    row.Cells["TotalProfessor"].Value = totalProfessor;
                    khoaDaTonTai = true;
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
            }
            if (!khoaDaTonTai)
            {
                dgvQuanLyKhoa.Rows.Add(facultyID, txtTenKhoa.Text, totalProfessor);
                MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            CapNhatTongSoGS();
            ResetForm();
        }
    }
}
