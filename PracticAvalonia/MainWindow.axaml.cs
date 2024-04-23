using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using Avalonia.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

		string path = "files/formUser.csv";

		List<Users> users = new List<Users>();

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Событие кнопки для записи в файл
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
					/* Вариант для получения названия месяца
					 * ComboBoxItem selectedMonth = (ComboBoxItem)cbMonth.SelectedItem;
					 * name = selectedMonth.Content.ToString();*/
					string dateBirth = $"{tbDay.Text:00}.{cbMonth.SelectedIndex:00}.{tbYear.Text:0000}";
					user.dateBirth = dateBirth.ToString();
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
					//Очищаем поля
					ClearTextBox();
				}
			}
			catch (Exception except)
			{
				Trace.WriteLine($"Ошибка: {except.GetType().Name}");
				Trace.WriteLine($"Строка: {except.StackTrace}");
				Trace.WriteLine($"Описание: {except.Message}");
			}
		}

		/// <summary>
		/// Событие кнопки для чтения из файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReadFile(object sender, RoutedEventArgs e)
		{
			if (File.Exists(path))
			{
				ReadPanel.IsVisible = true;
				ReadPanelControl.IsVisible = true;
				WritePanel.IsVisible = false;
				ReadFileCsvToList();
				//Сортировка
				users = users.OrderBy(x => x.surname).ThenBy(x => x.name).ToList();
				//"Биндинг" всех элементов в ReadPanel
				users.ForEach(user =>
				{
					StackPanel spUsers = new();
					spUsers.Children.Add(GetTextBlock(user.surname + " " + user.name));
					spUsers.Children.Add(GetTextBlock(user.patronymic));
					spUsers.Children.Add(GetTextBlock(user.gender));
					spUsers.Children.Add(GetTextBlock(user.telephone));
					spUsers.Children.Add(GetTextBlock(user.dateBirth));
					spUsers.Children.Add(GetTextBlock(user.dayWeek));
					spUsers.Children.Add(GetTextBlock(user.zodiac));
					Border border = new()
					{
						Background = GetColor(user.gender),
						BorderBrush = GetColorBorder(user.gender), //цвет границы
						BorderThickness = new Avalonia.Thickness(3), //размер границы
						CornerRadius = new Avalonia.CornerRadius(30), //скругление границы
						Margin = new Avalonia.Thickness(10),
						Padding = new Avalonia.Thickness(20),
						Child = spUsers
					};
					ReadPanel.Children.Add(border);
				});
			}
			else
			{
				ShowMessage.Foreground = Brushes.Red;
				ShowMessage.Text = "Файла не существует!";
			}
		}

		/// <summary>
		/// Вернуться назад
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Back(object sender, RoutedEventArgs e)
		{
			ReadPanel.Children.Clear(); //Удаляем детей
			ReadPanel.IsVisible = false;
			ReadPanelControl.IsVisible = false;
			WritePanel.IsVisible = true;
		}

		/// <summary>
		/// Возвращает цвет элемента в зависимости от пола
		/// </summary>
		/// <param name="gender"></param>
		private SolidColorBrush GetColor(string gender)
		{
			switch (gender)
			{
				case "Мужской": return new SolidColorBrush(Color.FromRgb(165, 235, 255));
				case "Женский": return new SolidColorBrush(Color.FromRgb(255, 202, 220));
				default: return new SolidColorBrush(Color.FromRgb(228, 226, 227));
			}
		}

		/// <summary>
		/// Возвращает цвет рамки
		/// </summary>
		/// <param name="gender"></param>
		private IBrush GetColorBorder(string gender)
		{
			switch (gender)
			{
				case "Мужской": return (IBrush)Brushes.DodgerBlue;
				case "Женский": return (IBrush)Brushes.HotPink;
				default: return (IBrush) Brushes.Lime;
			}
		}

		/// <summary>
		/// Возвращает новый текст блок
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		private TextBlock GetTextBlock(string date)
		{
			return new TextBlock()
			{
				Text = date,
				Foreground = Brushes.Black,
				FontSize = 18
			};
		}

		/// <summary>
		///  Функция чтения из файла в лист
		/// </summary>
		private bool ReadFileCsvToList()
		{
			users.Clear(); //Очищаем лист прежде чем считывать
			using (StreamReader read = new StreamReader(path))
			{
				string line = "";
				while (!read.EndOfStream)
				{
					line = read.ReadLine();
					string[] lineArray = line.Split(';');
					Users user = new Users()
					{
						surname = lineArray[0],
						name = lineArray[1],
						patronymic = lineArray[2],
						gender = lineArray[3],
						telephone = lineArray[4],
						dateBirth = lineArray[5],
						dayWeek = lineArray[6],
						zodiac = lineArray[7]
					};
					users.Add(user);
				}
			}
			return true;
		}

		/// <summary>
		/// Запись в файл
		/// </summary>
		private void WriteFileCsv()
		{
			try
			{
				Directory.CreateDirectory("files"); //создаем папку
				using (StreamWriter write = new StreamWriter(path, true, Encoding.UTF8))
				{
					/*string userData = String.Join(";", users.ToArray());*/
					string userData = String.Join(";", user.surname, user.name, user.patronymic, user.gender, user.telephone, user.dateBirth, user.dayWeek, user.zodiac);
					write.WriteLine(userData);
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
		private string GetZodiacHoroscope(int day, int month)
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
		private string GetDayWeek(DateTime date)
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
		private bool CheckData()
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
						if (CheckDate())
						{
							ShowMessage.Text = "";
							return true;
						}
						else
						{
							ShowMessage.Text = "Такой даты не существует";
						}
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
		/// Проверка существования даты
		/// </summary>
		/// <returns></returns>
		private bool CheckDate()
		{
			try
			{
				DateOnly dateBirth = new DateOnly(Convert.ToInt32(tbYear.Text), cbMonth.SelectedIndex, Convert.ToInt32(tbDay.Text));
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		/// <summary>
		/// Проверка корректности фио
		/// </summary>
		/// <returns></returns>
		private bool CheckFio()
		{
			Regex surname = new Regex(@"^[А-Я][а-я]{0,}$");
			Regex name = new Regex(@"^[А-Я][а-я]{1,}$");
			bool check = surname.IsMatch(tbSurname.Text) && name.IsMatch(tbName.Text) ? true : false;
			return check;
		}

		/// <summary>
		/// Проверка корректности телефона
		/// </summary>
		/// <returns></returns>
		private bool CheckTelephone()
		{
			Regex reg = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");
			bool check = reg.IsMatch(tbTelephone.Text);
			return check;
		}

		/// <summary>
		/// Сброс полей
		/// </summary>
		/// <returns></returns>
		private void ClearTextBox()
		{
			tbSurname.Text = null;
			tbName.Text = null;
			tbPatronymic.Text = null;
			tbTelephone.Text = null;
			rbM.IsChecked = false;
			rbG.IsChecked = false;
			cbMonth.SelectedIndex = 0;
			tbDay.Text = null;
			tbYear.Text = null;
		}
	}
}