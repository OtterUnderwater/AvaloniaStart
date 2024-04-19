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
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// ������� ��� ������ � ����
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
					ComboBoxItem selectedMonth = (ComboBoxItem)cbMonth.SelectedItem;
					user.dateBirth = tbDay.Text + selectedMonth.Content.ToString() + tbYear.Text;
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
				}
			}
			catch (Exception except)
			{
				Console.WriteLine("������: " + except.GetType().Name);
				Console.WriteLine("������: " + except.StackTrace);
				Console.WriteLine("��������: " + except.Message);
			}
		}

		/// <summary>
		/// ������ � ����
		/// </summary>
		public void WriteFileCsv()
		{
			try
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
						user.zodiac + ";");
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
		string GetZodiacHoroscope(int day, int month)
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
		string GetDayWeek(DateTime date)
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
		bool CheckData()
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
		/// �������� ������������ ���
		/// </summary>
		/// <returns></returns>
		bool CheckFio()
		{
			Regex surname = new Regex(@"^[�-�][�-�]{0,}$");
			Regex name = new Regex(@"^[�-�][�-�]{1,}$");
			bool check = (surname.IsMatch(tbSurname.Text) && name.IsMatch(tbName.Text)) ? true : false;
			return check;
		}

		/// <summary>
		/// �������� ������������ ��������
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