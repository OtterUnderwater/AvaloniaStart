using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using Avalonia.Media;

namespace PracticAvalonia
{
	/// <summary>
	/// Строктура пользователей
	/// </summary>
	public struct Users
	{
		public string surname;
		public string name;
		public string patronymic;
		public string gender;
		public string telephone;
		public string dateBirth;
		public string dayWeek;
		public string zodiac;
	}

	public partial class MainWindow : Window
	{
		Users user = new Users();
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Функция для записи в файл
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WriteFile(object sender, RoutedEventArgs e)
		{
			try
			{
				if (CheckData()) //Проверка корректности данных
				{
					//Записываем ФИО
					user.surname = tbSurname.Text;
					user.name = tbName.Text;
					user.patronymic = tbPatronymic.Text;
					//Записываем пол из радиокнопки
					user.gender = (rbM.IsChecked == true) ? rbM.Content.ToString() : user.gender = rbG.Content.ToString();
					//Записываем телефон
					user.telephone = tbTelephone.Text;
					//Записываем месяц с помощью приведения выбранного элемента
					ComboBoxItem selectedMonth = (ComboBoxItem)cbMonth.SelectedItem;
					user.dateBirth = tbDay.Text + selectedMonth.Content.ToString() + tbYear.Text;
					//Определяем в какой день недели родился пользователь
					int day = Convert.ToInt32(tbDay.Text);
					int month = cbMonth.SelectedIndex;
					int year = Convert.ToInt32(tbYear.Text);
					DateTime date = new DateTime(year, month, day);
					user.dayWeek = GetDayWeek(date);
					//Определяем кто пользователь по гороскопу
					user.zodiac = GetZodiacHoroscope(date.Day, date.Month);
					//Записываем в файл
					WriteFileCsv();
				}
			}
			catch (Exception except)
			{
				Console.WriteLine("Ошибка: " + except.GetType().Name);
				Console.WriteLine("Строка: " + except.StackTrace);
				Console.WriteLine("Описание: " + except.Message);
			}
		}

		/// <summary>
		/// Запись в файл
		/// </summary>
		public void WriteFileCsv()
		{
			try
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
						user.zodiac + ";");
				}
				ShowMessage.Foreground = Brushes.Green;
				ShowMessage.Text = "Запись добавлена";
			}
			catch
			{
				ShowMessage.Foreground = Brushes.Red;
				ShowMessage.Text = "Что-то пошло не так. Файл не записан!";
			}
		}

		/// <summary>
		/// Определяем пользователь по древнеславянскому гороскопу
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		string GetZodiacHoroscope(int day, int month)
		{
			if ((month == 12 && day >= 24) || (month == 1 && day <= 30))
				return "Мороз";
			else if ((month == 1 && day == 31) || (month == 2 && day <= 29))
				return "Велес";
			else if (month == 3)
				return "Макошь";
			else if (month == 4)
				return "Жива";
			else if (month == 5 && day >= 1 && day <= 14)
				return "Ярила";
			else if ((month == 5 && day >= 15) || (month == 6 && day <= 2))
				return "Леля";
			else if (month == 6 && day >= 3 && day <= 12)
				return "Кострома";
			else if (!(month == 6 && day == 24) && ((month == 6 && day >= 13) || (month == 7 && day <= 6)))
				return "Додола";
			else if (month == 6 && day == 24)
				return "Иван Купала";
			else if (month == 7 && day >= 7 && day <= 31)
				return "Лада";
			else if (month == 8 && day >= 1 && day <= 28)
				return "Перун";
			else if ((month == 8 && day >= 29) || (month == 9 && day <= 13))
				return "Сева";
			else if (month == 9 && day >= 14 && day <= 27)
				return "Рожаница";
			else if ((month == 9 && day >= 28) || (month == 10 && day <= 15))
				return "Сварожичи";
			else if ((month == 10 && day >= 16) || (month == 11 && day <= 8))
				return "Морена";
			else if (month == 11 && day >= 9 && day <= 28)
				return "Зима";
			else if ((month == 11 && day >= 29) || (month == 12 && day <= 23))
				return "Карачун";
			else
				return "Неизвестен";
		}

		/// <summary>
		/// Определяем день, в который родился пользователь
		/// </summary>
		/// <returns></returns>
		string GetDayWeek(DateTime date)
		{
			return (int)date.DayOfWeek switch
			{
				0 => "Воскресенье",
				1 => "Понедельник",
				2 => "Вторник",
				3 => "Среда",
				4 => "Четверг",
				5 => "Пятница",
				6 => "Суббота"
			};
		}

		/// <summary>
		/// Проверка корректности введенных данных
		/// </summary>
		/// <returns></returns>
		bool CheckData()
		{
			ShowMessage.Foreground = Brushes.Red;
			//Проверка что все поля заполненны
			if (tbSurname.Text != null && tbName.Text != null && tbPatronymic.Text != null &&
				tbTelephone.Text != null && (rbM.IsChecked != false || rbG.IsChecked != false)
				&& tbDay.Text != null && cbMonth.SelectedIndex != 0 && tbYear.Text != null)
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
					ShowMessage.Text = "Неправильно введены данные ФИО.\nФИо должно быть на русском с заглавной буквы";
				}
			}
			else
			{
				ShowMessage.Text = "Заполните все поля!";
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
			Regex name = new Regex(@"^[А-Я][а-я]{1,}$");
			bool check = (surname.IsMatch(tbSurname.Text) && name.IsMatch(tbName.Text)) ? true : false;
			return check;
		}

		/// <summary>
		/// Проверка корректности телефона
		/// </summary>
		/// <returns></returns>
		bool CheckTelephone()
		{
			Regex reg = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");
			bool check = reg.IsMatch(tbTelephone.Text);
			return check;
		}

	}
}