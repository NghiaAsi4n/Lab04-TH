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
    public partial class frmTimKiem : Form
    {
        public frmTimKiem()
        {
            InitializeComponent();
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            rbFemale.Checked = true;
            LoadFacultyToComboBox();
            LoadDataGridView();
        }

        private void LoadFacultyToComboBox()
        {
            using (var context = new StudentContextDB())
            {
                var faculties = context.Faculties.ToList();
                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName";
                cmbKhoa.ValueMember = "FacultyID";
                cmbKhoa.SelectedIndex = -1;
            }
        }

        private void LoadDataGridView()
        {
            using (var context = new StudentContextDB())
            {
                var danhSachSinhVien = (from sv in context.Students
                                        join faculty in context.Faculties on sv.FacultyID equals faculty.FacultyID
                                        select new
                                        {
                                            MaSV = sv.StudentID,
                                            HoTen = sv.FullName,
                                            GioiTinh = sv.Gender,
                                            TenKhoa = faculty.FacultyName,
                                            DiemTB = sv.AverageScore
                                        }).ToList();
                dgvTimKiem.Rows.Clear();
                foreach (var sv in danhSachSinhVien)
                {
                    dgvTimKiem.Rows.Add(sv.MaSV, sv.HoTen, sv.GioiTinh, sv.TenKhoa, sv.DiemTB);
                }

                txtKetQuaTimKiem.Text = danhSachSinhVien.Count.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool maSinhVienTonTai = false;
            foreach (DataGridViewRow row in dgvTimKiem.Rows)
            {
                if (row.Cells["MaSV"].Value != null && row.Cells["MaSV"].Value.ToString() == txtMaSV.Text)
                {
                    maSinhVienTonTai = true;
                    break;
                }
            }
            if (!maSinhVienTonTai)
            {
                MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (var context = new StudentContextDB())
                {
                    var sinhVienCanXoa = context.Students.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                    if (sinhVienCanXoa != null)
                    {
                        context.Students.Remove(sinhVienCanXoa);
                        context.SaveChanges();
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataGridView();
                        ResetForm();
                    }
                }
            }
        }

        private void btnTroVe_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            cmbKhoa.SelectedIndex = -1;
            rbFemale.Checked = true;
        }
    }
}
