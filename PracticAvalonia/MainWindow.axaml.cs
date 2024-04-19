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
            CheckData(); //Проверка корректности данных
            if (tbSurname.Text != null && tbName.Text != null && tbPatronymic.Text != null && tbTelephone.Text != null)
            {
                user.surname = tbSurname.Text;
                user.name = tbName.Text;
                user.patronymic = tbPatronymic.Text;
                // в зависимости от того, какая радиокнопка активна
                if (rbM.IsChecked == true) user.gender = "Мужской";
                if (rbG.IsChecked == true) user.gender = "Женский";
                user.telephone = tbTelephone.Text;
                string month = "";
                switch (cbMonth.SelectedIndex)
                {
                    case 1:
                        month = "Январь";
                        break;
                    case 2:
                        month = "Февраль";
                        break;
                    case 3:
                        month = "Март";
                        break;
                    case 4:
                        month = "Апрель";
                        break;
                    case 5:
                        month = "Май";
                        break;
                    case 6:
                        month = "Июнь";
                        break;
                    case 7:
                        month = "Июль";
                        break;
                    case 8:
                        month = "Август";
                        break;
                    case 9:
                        month = "Сентябрь";
                        break;
                    case 10:
                        month = "Октябрь";
                        break;
                    case 11:
                        month = "Ноябрь";
                        break;
                    case 12:
                        month = "Декабрь";
                        break;
                }
                user.dateBirth = tbDay.Text + month + tbYear.Text;
                user.dayWeek = "";
                user.horoscope = "";
            }
            else
            {
                ShowMessage.Text = "Заполните все поля!";
            }
            WriteFileCsv();
        }

        /// <summary>
        /// Запись в файл
        /// </summary>
        public void WriteFileCsv()
        {
            string path = "files/formUser.csv";
            Directory.CreateDirectory("files"); //создаем папку
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
                ShowMessage.Text = "Запись добавлена";
            }
        }

        /// <summary>
        /// Проверка корректности введенных данных
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
                    ShowMessage.Text = "Неправильный формат номера телефона";
                }
            }
            else
            {
                ShowMessage.Text = "Неправильно введены данные ФИО";
            }
            return false;
        }

        /// <summary>
        /// Проверка корректности фио
        /// </summary>
        /// <returns></returns>
        bool CheckFio()
        {
            Regex surname = new Regex(@"^[А-Я][а-я]{0,}$");
            Regex name = new Regex(@"^[А-Я].[а-я]{1,}$");
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
        /// Проверка корректности телефона
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