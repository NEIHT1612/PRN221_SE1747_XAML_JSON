using CRUD_WPF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRUD_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            load();
            loadDepart();
            cbSearch.ItemsSource = elements;
            loadDeptRadio();
        }
        string[] elements = { "Id", "Name", "Gender", "DepartmentName", "Dob", "Gpa" };

        private void loadDeptRadio(string id = "SE")
        {
            spnDept.Children.Clear();
            foreach (var d in PRN221_DemoContext.Instance.Departments)
            {
                RadioButton rd = new RadioButton()
                {
                    Content = d.Name,
                    Name = d.Id,
                    IsChecked = d.Id.Equals(id),
                };
                rd.Click += Rd_Click;
                spnDept.Children.Add(rd);
            }
        }

        private void Rd_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            spnDept.Tag = rd.Name;
        }

        private void load()
        {
            var student = PRN221_DemoContext.Instance.Students.Include(x => x.Depart)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Gender = x.Gender ? "Male" : "Female",
                    DepartId = x.Depart.Name,
                    Dob = x.Dob,
                    Gpa = x.Gpa,
                })
                .ToList();
            dgvDisplay.ItemsSource = student;
        }

        private void loadDepart()
        {
            var dept = PRN221_DemoContext.Instance.Departments.Select(x => x.Name).ToList();
            dept.Insert(0, "All");
            cbDepartFilter.ItemsSource = dept;
            cbDepartFilter.SelectedIndex = 0;
            cbDepartment.ItemsSource = dept;
            cbDepartFilter.SelectedIndex = 0;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            filter();

            //if (dept.Equals("All"))
            //{
            //    var st = PRN221_DemoContext.Instance.Students.ToList();
            //    dgvDisplay.ItemsSource = st;
            //}
            //else
            //{
            //    string deptId = PRN221_DemoContext.Instance.Departments.FirstOrDefault(x => x.Name.Equals(dept)).Id;
            //    var st = PRN221_DemoContext.Instance.Students.Where(x => x.DepartId.Equals(deptId)).ToList();
            //    dgvDisplay.ItemsSource = st;
            //}
        }

        private void filter()
        {
            string dept = cbDepartFilter.SelectedValue.ToString();
            var st1 = PRN221_DemoContext.Instance.Students.Include(x => x.Depart).Where(x => dept.Equals("All")
                      ? true : x.Depart.Name.Equals(dept)).ToList();
            dgvDisplay.ItemsSource = st1;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void search()
        {
            string txt = txtSearch.Text;
            if (cbSearch.SelectedItem.ToString().Equals(elements[1]))
            {
                dgvDisplay.ItemsSource = PRN221_DemoContext.Instance.Students.Include(x => x.Depart).
                    Where(x => string.IsNullOrEmpty(txt) ? true : x.Name.Contains(txt)).ToList();
            }
        }

        private void cbDepartFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            search();
        }

        private Student getStudent()
        {
            try
            {
                int id = int.Parse(txtId.Text);
                string name = txtName.Text;
                bool gender = rdbMale.IsChecked == true;
                //bool gender = (bool)rdbMale.IsChecked;
                //bool gender = rdbMale.IsChecked.Value;
                //string deptId = PRN221_DemoContext.Instance.Departments.FirstOrDefault(x => x.Name.Equals(cbDepartment.SelectedValue.ToString())).Id;
                string deptId = spnDept.Tag.ToString();
                DateTime dob = dtpDob.SelectedDate.Value;
                float gpa = float.Parse(txtGpa.Text);
                return new Student()
                {
                    Id = id,
                    Name = name,
                    Gender = gender,
                    DepartId = deptId,
                    Dob = dob,
                    Gpa = gpa
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void clearForm()
        {
            txtId.Text = "";
            txtName.Text = "";
            rdbFemale.IsChecked = true;
            rdbMale.IsChecked = false;
            cbDepartment.SelectedIndex = 0;
            dtpDob.SelectedDate = null;
            txtGpa.Text = "";
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            Student student = getStudent();
            if (student == null)
            {
                clearForm();
                return;
            }
            var x = PRN221_DemoContext.Instance.Students.Find(student.Id);
            if(x == null)
            {
                PRN221_DemoContext.Instance.Add(student);
                PRN221_DemoContext.Instance.SaveChanges();
                load();
            }
            else
            {
                //update
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Student student = getStudent();
            if (student == null)
            {
                clearForm();
                return;
            }
            var x = PRN221_DemoContext.Instance.Students.Find(student.Id);
            if (x != null)
            {
                x.Id = student.Id;
                x.Name = student.Name;
                x.Gender = student.Gender;
                x.DepartId = student.DepartId;
                x.Dob = student.Dob;
                x.Gpa = student.Gpa;
                PRN221_DemoContext.Instance.Students.Update(x);
                PRN221_DemoContext.Instance.SaveChanges();
                load();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this student?",
                                              "Delete Confirmation",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var x = PRN221_DemoContext.Instance.Students.Find(int.Parse(txtId.Text));
                    if (x != null)
                    {
                        PRN221_DemoContext.Instance.Students.Remove(x);
                        PRN221_DemoContext.Instance.SaveChanges();
                        load();
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private void dgvDisplay_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //Student student = dgvDisplay.SelectedItem as Student;
            //if (student != null) {
            //    txtId.Text = student.Id.ToString();
            //    txtName.Text = student.Name.ToString();
            //    rdbMale.IsChecked = student.Gender;
            //    rdbFemale.IsChecked = !student.Gender;
            //    cbDepartment.SelectedValue = student.Depart.Name;
            //    dtpDob.SelectedDate = student.Dob;
            //    txtGpa.Text = student.Gpa.ToString();
            //}

            Student st = dgvDisplay.SelectedItem as Student;
            if (st != null)
            {
                loadDeptRadio(st.DepartId);
            }
        }
    }
}
//BTVN Dinh dang lai dob theo dd/mm/yyyy          done
//Thay the datagrid => listView                   done
//Filter theo gender (Male/Female/All)            done
//va ket hop gender and/or voi department         
// radiobutton, checkbox
//them messagebox de confirm xoa hay khong        done
//dung calendar thay vi datepicker                done 
//BTVN checkbox depart, day len tag               done (WPF1)