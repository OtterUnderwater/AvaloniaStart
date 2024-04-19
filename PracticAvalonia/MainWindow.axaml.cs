using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;
using System;
using System.Xml.Linq;
using System.IO;
using Avalonia.Controls.Shapes;
using System.Text;
using static System.Net.WebRequestMethods;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Avalonia.Layout;
using static System.Net.Mime.MediaTypeNames;

namespace PracticAvalonia
{
    public struct Users
    {
        public string surname;
        public string name;
        public string patronymic;
        public string gender;
        public string telephone;
        public string dateBirth;
        public string dayWeek;
        public string horoscope;
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Users user = new Users();

        private void WriteFile(object sender, RoutedEventArgs e)
        {
            CheckData(); //�������� ������������ ������
            if (tbSurname.Text != null && tbName.Text != null && tbPatronymic.Text != null && tbTelephone.Text != null)
            {
                user.surname = tbSurname.Text;
                user.name = tbName.Text;
                user.patronymic = tbPatronymic.Text;
                // � ����������� �� ����, ����� ����������� �������
                if (rbM.IsChecked == true) user.gender = "�������";
                if (rbG.IsChecked == true) user.gender = "�������";
                user.telephone = tbTelephone.Text;
                string month = "";
                switch (cbMonth.SelectedIndex)
                {
                    case 1:
                        month = "������";
                        break;
                    case 2:
                        month = "�������";
                        break;
                    case 3:
                        month = "����";
                        break;
                    case 4:
                        month = "������";
                        break;
                    case 5:
                        month = "���";
                        break;
                    case 6:
                        month = "����";
                        break;
                    case 7:
                        month = "����";
                        break;
                    case 8:
                        month = "������";
                        break;
                    case 9:
                        month = "��������";
                        break;
                    case 10:
                        month = "�������";
                        break;
                    case 11:
                        month = "������";
                        break;
                    case 12:
                        month = "�������";
                        break;
                }
                user.dateBirth = tbDay.Text + month + tbYear.Text;
                user.dayWeek = "";
                user.horoscope = "";
            }
            else
            {
                ShowMessage.Text = "��������� ��� ����!";
            }
            WriteFileCsv();
        }

        /// <summary>
        /// ������ � ����
        /// </summary>
        public void WriteFileCsv()
        {
            string path = "files/formUser.csv";
            Directory.CreateDirectory("files"); //������� �����
            using (StreamWriter write = new StreamWriter(path, true, Encoding.UTF8))
            {
                write.WriteLine(user.surname + ";" +
                    user.name + ";" +
                    user.patronymic + ";" +
                    user.gender + ";" +
                    user.telephone + ";" +
                    user.dateBirth + ";" +
                    user.dayWeek + ";" +
                    user.horoscope + ";" );
                ShowMessage.Text = "������ ���������";
            }
        }

        /// <summary>
        /// �������� ������������ ��������� ������
        /// </summary>
        /// <returns></returns>
        bool CheckData()
        {
            if (CheckFio())
            {
                if (CheckTelephone())
                {
                    ShowMessage.Text = "";
                    return true;       
                }
                else
                {
                    ShowMessage.Text = "������������ ������ ������ ��������";
                }
            }
            else
            {
                ShowMessage.Text = "����������� ������� ������ ���";
            }
            return false;
        }

        /// <summary>
        /// �������� ������������ ���
        /// </summary>
        /// <returns></returns>
        bool CheckFio()
        {
            Regex surname = new Regex(@"^[�-�][�-�]{0,}$");
            Regex name = new Regex(@"^[�-�].[�-�]{1,}$");
            if (tbSurname.Text != null && tbName.Text!= null)
            {
                if (surname.IsMatch(tbSurname.Text) && name.IsMatch(tbName.Text))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// �������� ������������ ��������
        /// </summary>
        /// <returns></returns>
        bool CheckTelephone()
        {
            bool check = false;
            Regex reg = new Regex(@"^8(\d{3})\d{3}-\d{2}-\d{2}$");
            if (tbTelephone.Text != null)
            {
                check = reg.IsMatch(tbTelephone.Text);
            }
            return check;
        }

    }
}