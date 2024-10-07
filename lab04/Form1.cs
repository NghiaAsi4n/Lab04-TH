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
    public partial class frmQuanLySinhVien : Form
    {
        public frmQuanLySinhVien()
        {
            InitializeComponent();
        }

        private void frmQuanLySinhVien_Load(object sender, EventArgs e)
        {
            LoadFacultyToComboBox();
            rbFemale.Checked = true;
            LoadDataGridView();
            CapNhatSoLuongSinhVien();
        }

        private void LoadDataGridView()
        {
            dgvStudent.ColumnCount = 5;
            dgvStudent.Columns[0].Name = "Mã SV";
            dgvStudent.Columns[1].Name = "Họ và Tên";
            dgvStudent.Columns[2].Name = "Giới Tính";
            dgvStudent.Columns[3].Name = "Điểm TB";
            dgvStudent.Columns[4].Name = "Khoa";
            using (var context = new StudentContextDB())
            {
                var sinhVienList = (from sv in context.Students
                                    join faculty in context.Faculties on sv.FacultyID equals faculty.FacultyID
                                    select new
                                    {
                                        StudentID = sv.StudentID,
                                        FullName = sv.FullName,
                                        Gender = sv.Gender,
                                        AverageScore = sv.AverageScore,
                                        FacultyName = faculty.FacultyName
                                    }).ToList();
                dgvStudent.Rows.Clear();

                foreach (var sinhVien in sinhVienList)
                {
                    dgvStudent.Rows.Add(sinhVien.StudentID, sinhVien.FullName, sinhVien.Gender, sinhVien.AverageScore, sinhVien.FacultyName);
                }

                dgvStudent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void LoadFacultyToComboBox()
        {
            using (var context = new StudentContextDB())
            {
                var faculties = context.Faculties.ToList();
                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName";
                cmbKhoa.ValueMember = "FacultyID";
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private bool KiemTraThongTin()
        {
            err.Clear();
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                err.SetError(txtMaSV, "Vui lòng nhập mã sinh viên.");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtMaSV.Text, @"^\d{10}$"))
            {
                err.SetError(txtMaSV, "Mã sinh viên không hợp lệ. Mã SV phải là 10 ký tự số.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                err.SetError(txtHoTen, "Vui lòng nhập tên sinh viên.");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtHoTen.Text, @"^[\p{L}\s]{3,100}$"))
            {
                err.SetError(txtHoTen, "Tên sinh viên không hợp lệ. Tên chỉ chứa ký tự chữ và phải có độ dài từ 3 đến 100 ký tự.");
                isValid = false;
            }
            decimal diemTB;
            if (string.IsNullOrWhiteSpace(txtDTB.Text))
            {
                err.SetError(txtDTB, "Vui lòng nhập điểm trung bình.");
                isValid = false;
            }
            else if (!decimal.TryParse(txtDTB.Text, out diemTB) || diemTB < 0 || diemTB > 10)
            {
                err.SetError(txtDTB, "Điểm trung bình không hợp lệ. Điểm phải là số thập phân trong khoảng từ 0 đến 10.");
                isValid = false;
            } 
            return isValid;
        }

        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDTB.Clear();
            cmbKhoa.SelectedIndex = 0;
            rbFemale.Checked = true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return;
            }
            using (var context = new StudentContextDB())
            {
                var sinhVienTonTai = context.Students.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                if (sinhVienTonTai == null)
                {
                    var sinhVienMoi = new Student
                    {
                        StudentID = txtMaSV.Text,
                        FullName = txtHoTen.Text,
                        AverageScore = float.Parse(txtDTB.Text),
                        FacultyID = (int)cmbKhoa.SelectedValue,
                        Gender = rbMale.Checked ? "Male" : "Female"
                    };
                    context.Students.Add(sinhVienMoi);
                    context.SaveChanges();
                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sinhVienTonTai.FullName = txtHoTen.Text;
                    sinhVienTonTai.AverageScore = float.Parse(txtDTB.Text);
                    sinhVienTonTai.FacultyID = (int)cmbKhoa.SelectedValue;
                    sinhVienTonTai.Gender = rbMale.Checked ? "Male" : "Female";

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            LoadDataGridView();
            CapNhatSoLuongSinhVien();
            ResetForm();
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells["Mã SV"].Value.ToString();
                txtHoTen.Text = row.Cells["Họ và Tên"].Value.ToString();
                txtDTB.Text = row.Cells["Điểm TB"].Value.ToString();
                if (row.Cells["Giới Tính"].Value.ToString() == "Male")
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }
                cmbKhoa.SelectedIndex = cmbKhoa.FindStringExact(row.Cells["Khoa"].Value.ToString());
            }
        }

        private void CapNhatSoLuongSinhVien()
        {
            using (var context = new StudentContextDB())
            {
                int soLuongNam = context.Students.Count(sv => sv.Gender == "Male");
                int soLuongNu = context.Students.Count(sv => sv.Gender == "Female");
                txtTongNam.Text = soLuongNam.ToString();
                txtTongNu.Text = soLuongNu.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new StudentContextDB())
            {
                var sinhVienCanXoa = context.Students.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                if (sinhVienCanXoa == null)
                {
                    MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    context.Students.Remove(sinhVienCanXoa);
                    context.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataGridView();
                    CapNhatSoLuongSinhVien();
                    ResetForm();
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return;
            }
            using (var context = new StudentContextDB())
            {
                var sinhVienTonTai = context.Students.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                if (sinhVienTonTai == null)
                {
                    var sinhVienMoi = new Student
                    {
                        StudentID = txtMaSV.Text,
                        FullName = txtHoTen.Text,
                        AverageScore = float.Parse(txtDTB.Text),
                        FacultyID = (int)cmbKhoa.SelectedValue,
                        Gender = rbMale.Checked ? "Male" : "Female"
                    };

                    context.Students.Add(sinhVienMoi);
                    context.SaveChanges();
                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sinhVienTonTai.FullName = txtHoTen.Text;
                    sinhVienTonTai.AverageScore = float.Parse(txtDTB.Text);
                    sinhVienTonTai.FacultyID = (int)cmbKhoa.SelectedValue;
                    sinhVienTonTai.Gender = rbMale.Checked ? "Male" : "Female";
                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            LoadDataGridView();
            CapNhatSoLuongSinhVien();
            ResetForm();
        }
    }
}
