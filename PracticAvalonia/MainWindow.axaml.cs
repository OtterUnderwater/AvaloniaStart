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
	/// ��������� �������������
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
		/// ������� ������ ��� ������ � ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WriteFile(object sender, RoutedEventArgs e)
		{
			try
			{
				if (CheckData()) //�������� ������������ ������
				{
					//���������� ���
					user.surname = tbSurname.Text;
					user.name = tbName.Text;
					user.patronymic = tbPatronymic.Text;
					//���������� ��� �� �����������
					user.gender = (rbM.IsChecked == true) ? rbM.Content.ToString() : user.gender = rbG.Content.ToString();
					//���������� �������
					user.telephone = tbTelephone.Text;
					//���������� ����� � ������� ���������� ���������� ��������
					/* ������� ��� ��������� �������� ������
					 * ComboBoxItem selectedMonth = (ComboBoxItem)cbMonth.SelectedItem;
					 * name = selectedMonth.Content.ToString();*/
					string dateBirth = $"{tbDay.Text:00}.{cbMonth.SelectedIndex:00}.{tbYear.Text:0000}";
					user.dateBirth = dateBirth.ToString();
					//���������� � ����� ���� ������ ������� ������������
					int day = Convert.ToInt32(tbDay.Text);
					int month = cbMonth.SelectedIndex;
					int year = Convert.ToInt32(tbYear.Text);
					DateTime date = new DateTime(year, month, day);
					user.dayWeek = GetDayWeek(date);
					//���������� ��� ������������ �� ���������
					user.zodiac = GetZodiacHoroscope(date.Day, date.Month);
					//���������� � ����
					WriteFileCsv();
					//������� ����
					ClearTextBox();
				}
			}
			catch (Exception except)
			{
				Trace.WriteLine($"������: {except.GetType().Name}");
				Trace.WriteLine($"������: {except.StackTrace}");
				Trace.WriteLine($"��������: {except.Message}");
			}
		}

		/// <summary>
		/// ������� ������ ��� ������ �� �����
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
				//����������
				users = users.OrderBy(x => x.surname).ThenBy(x => x.name).ToList();
				//"�������" ���� ��������� � ReadPanel
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
						BorderBrush = GetColorBorder(user.gender), //���� �������
						BorderThickness = new Avalonia.Thickness(3), //������ �������
						CornerRadius = new Avalonia.CornerRadius(30), //���������� �������
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
				ShowMessage.Text = "����� �� ����������!";
			}
		}

		/// <summary>
		/// ��������� �����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Back(object sender, RoutedEventArgs e)
		{
			ReadPanel.Children.Clear(); //������� �����
			ReadPanel.IsVisible = false;
			ReadPanelControl.IsVisible = false;
			WritePanel.IsVisible = true;
		}

		/// <summary>
		/// ���������� ���� �������� � ����������� �� ����
		/// </summary>
		/// <param name="gender"></param>
		private SolidColorBrush GetColor(string gender)
		{
			switch (gender)
			{
				case "�������": return new SolidColorBrush(Color.FromRgb(165, 235, 255));
				case "�������": return new SolidColorBrush(Color.FromRgb(255, 202, 220));
				default: return new SolidColorBrush(Color.FromRgb(228, 226, 227));
			}
		}

		/// <summary>
		/// ���������� ���� �����
		/// </summary>
		/// <param name="gender"></param>
		private IBrush GetColorBorder(string gender)
		{
			switch (gender)
			{
				case "�������": return (IBrush)Brushes.DodgerBlue;
				case "�������": return (IBrush)Brushes.HotPink;
				default: return (IBrush) Brushes.Lime;
			}
		}

		/// <summary>
		/// ���������� ����� ����� ����
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
		///  ������� ������ �� ����� � ����
		/// </summary>
		private bool ReadFileCsvToList()
		{
			users.Clear(); //������� ���� ������ ��� ���������
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
		/// ������ � ����
		/// </summary>
		private void WriteFileCsv()
		{
			try
			{
				Directory.CreateDirectory("files"); //������� �����
				using (StreamWriter write = new StreamWriter(path, true, Encoding.UTF8))
				{
					/*string userData = String.Join(";", users.ToArray());*/
					string userData = String.Join(";", user.surname, user.name, user.patronymic, user.gender, user.telephone, user.dateBirth, user.dayWeek, user.zodiac);
					write.WriteLine(userData);
				}
				ShowMessage.Foreground = Brushes.Green;
				ShowMessage.Text = "������ ���������";
			}
			catch
			{
				ShowMessage.Foreground = Brushes.Red;
				ShowMessage.Text = "���-�� ����� �� ���. ���� �� �������!";
			}
		}

		/// <summary>
		/// ���������� ������������ �� ����������������� ���������
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		private string GetZodiacHoroscope(int day, int month)
		{
			if ((month == 12 && day >= 24) || (month == 1 && day <= 30))
				return "�����";
			else if ((month == 1 && day == 31) || (month == 2 && day <= 29))
				return "�����";
			else if (month == 3)
				return "������";
			else if (month == 4)
				return "����";
			else if (month == 5 && day >= 1 && day <= 14)
				return "�����";
			else if ((month == 5 && day >= 15) || (month == 6 && day <= 2))
				return "����";
			else if (month == 6 && day >= 3 && day <= 12)
				return "��������";
			else if (!(month == 6 && day == 24) && ((month == 6 && day >= 13) || (month == 7 && day <= 6)))
				return "������";
			else if (month == 6 && day == 24)
				return "���� ������";
			else if (month == 7 && day >= 7 && day <= 31)
				return "����";
			else if (month == 8 && day >= 1 && day <= 28)
				return "�����";
			else if ((month == 8 && day >= 29) || (month == 9 && day <= 13))
				return "����";
			else if (month == 9 && day >= 14 && day <= 27)
				return "��������";
			else if ((month == 9 && day >= 28) || (month == 10 && day <= 15))
				return "���������";
			else if ((month == 10 && day >= 16) || (month == 11 && day <= 8))
				return "������";
			else if (month == 11 && day >= 9 && day <= 28)
				return "����";
			else if ((month == 11 && day >= 29) || (month == 12 && day <= 23))
				return "�������";
			else
				return "����������";
		}

		/// <summary>
		/// ���������� ����, � ������� ������� ������������
		/// </summary>
		/// <returns></returns>
		private string GetDayWeek(DateTime date)
		{
			return (int)date.DayOfWeek switch
			{
				0 => "�����������",
				1 => "�����������",
				2 => "�������",
				3 => "�����",
				4 => "�������",
				5 => "�������",
				6 => "�������"
			};
		}

		/// <summary>
		/// �������� ������������ ��������� ������
		/// </summary>
		/// <returns></returns>
		private bool CheckData()
		{
			ShowMessage.Foreground = Brushes.Red;
			//�������� ��� ��� ���� ����������
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
							ShowMessage.Text = "����� ���� �� ����������";
						}
					}
					else
					{
						ShowMessage.Text = "������������ ������ ������ ��������";
					}
				}
				else
				{
					ShowMessage.Text = "����������� ������� ������ ���.\n��� ������ ���� �� ������� � ��������� �����";
				}
			}
			else
			{
				ShowMessage.Text = "��������� ��� ����!";
			}
			return false;
		}

		/// <summary>
		/// �������� ������������� ����
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
		/// �������� ������������ ���
		/// </summary>
		/// <returns></returns>
		private bool CheckFio()
		{
			Regex surname = new Regex(@"^[�-�][�-�]{0,}$");
			Regex name = new Regex(@"^[�-�][�-�]{1,}$");
			bool check = surname.IsMatch(tbSurname.Text) && name.IsMatch(tbName.Text) ? true : false;
			return check;
		}

		/// <summary>
		/// �������� ������������ ��������
		/// </summary>
		/// <returns></returns>
		private bool CheckTelephone()
		{
			Regex reg = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");
			bool check = reg.IsMatch(tbTelephone.Text);
			return check;
		}

		/// <summary>
		/// ����� �����
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