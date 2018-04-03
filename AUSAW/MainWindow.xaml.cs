using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AUSAW
{
	public partial class MainWindow : Window
	{
		#region Private

		// Liczba wykladow dodanych do wydarzenia
		private int lectureCount = 0;

		// Sciezki do plikow potrzebne do zapisu i odczytu
		private string pathForLec;
		private string pathForLecInfo;

		#endregion

		#region Public

		// Lista wykladow dodanych do wydarzenia
		public List<Lecture> lectures = new List<Lecture>();

		#endregion

		#region Constructor

		public MainWindow()
		{
			preparingFiles();

			InitializeComponent();

			writeFromfile();

			// Podczas uruchomienia programu po raz pierwszy dodaje przykladowy wyklad
			if (lectureCount == 0)
			{
				RowDefinition rowDef = new RowDefinition();
				Lecture lecture = new Lecture();

				rowDef.Height = new GridLength(80);
				gridForLectures.RowDefinitions.Add(rowDef);
				lecture.hourTbx1.Text = "G";
				lecture.minuteTbx1.Text = "M";
				lecture.hourTbx2.Text = "G";
				lecture.minuteTbx2.Text = "M";
				lecture.descriptionTbx.Text = "Nazwa i opis wykładu";
				lecture.speakerTbx.Text = "Mówca";
				gridForLectures.Children.Add(lecture.mainStackPanel);
				Grid.SetRow(lecture.mainStackPanel, lectureCount);
				lectures.Add(lecture);
				lectureCount++;
			}
		}

		#endregion

		#region Buttons events, validators and other functions 

		// Sprawdzenie i przygotowanie plikow potrzebnych do dzialania programu 
		private void preparingFiles()
		{
			pathForLec = System.AppDomain.CurrentDomain.BaseDirectory + "lectures.txt";
			pathForLecInfo = System.AppDomain.CurrentDomain.BaseDirectory + "lecturesInfo.txt";

			if (!File.Exists(pathForLec))
			{
				StreamWriter sw = File.CreateText(pathForLec);
			}
			if (!File.Exists(pathForLecInfo))
			{
				StreamWriter sw = File.CreateText(pathForLecInfo);
			}
		}

		// Funkcja wczytujaca wyklady z plikow uzywana przy starcie programu oraz przy usuwaniu wykladow w celu reorganizacji siatki 
		private void writeFromfile()
		{
			string tempInfo;
			// Kazdy wyklad jest zapisany w pliku "lectures.text" w osobnej lini 
			List<string> lines = File.ReadAllLines(pathForLec).ToList();

			foreach (string line in lines)
			{
				// Kolejne informacje o wykladzie i wydarzeniu sa oddzielone "^"
				// UWAGA jesli uzytkownik doda znak "^" wypelnienie textboxow sie wysypie ale program nie 
				string[] arr = line.Split('^');
				RowDefinition rowDef = new RowDefinition();
				Lecture lecture = new Lecture();

				rowDef.Height = new GridLength(80);
				gridForLectures.RowDefinitions.Add(rowDef);

				if (arr.Length == 6)
				{
					lecture.hourTbx1.Text = arr[0];
					lecture.minuteTbx1.Text = arr[1];
					lecture.hourTbx2.Text = arr[2];
					lecture.minuteTbx2.Text = arr[3];
					lecture.descriptionTbx.Text = arr[4];
					lecture.speakerTbx.Text = arr[5];
				}
				gridForLectures.Children.Add(lecture.mainStackPanel);
				Grid.SetRow(lecture.mainStackPanel, lectureCount);
				lectures.Add(lecture);

				lectureCount++;
			}
			tempInfo = File.ReadAllText(pathForLecInfo);
			string[] arr2 = tempInfo.Split('^');
			if (arr2.Length == 5)
			{
				dayTbx.Text = arr2[0];
				monthTbx.Text = arr2[1];
				yearTbx.Text = arr2[2];
				placeTbx.Text = arr2[3];
				nameAndDescTbx.Text = arr2[4];
			}
		}

		// Funkcja zapisujaca do PDF przycisk w menu PDF
		private void ToPdf_Click(object sender, RoutedEventArgs e)
		{
			// Wyskakujace okienko inforujace o utworzeniu pdf
			MessageBoxResult pdf = MessageBox.Show("Zapisano do PDF", "Zapis do PDF", MessageBoxButton.OK);

			// font - czcionka dla nazwy eventu i opisu,
			// font2 - czcionka dla godziny i miejsca eventu, 
			// font3 - czcionka dla wykladow
			Font font = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\tahoma.ttf", BaseFont.CP1250, true));
			font.Size = 16;
			Font font2 = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\tahoma.ttf", BaseFont.CP1250, true));
			font2.Size = 14;
			Font font3 = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\tahoma.ttf", BaseFont.CP1250, true));
			font3.Size = 12;

			float[] width = new float[] { 18, 57, 25 };

			// Tabela na dane
			PdfPTable table = new PdfPTable(width);

			Document doc = new Document();
			FileStream fs = new FileStream("Agenda.pdf", FileMode.Create);

			using (fs)
			{
				PdfWriter.GetInstance(doc, fs);
				doc.Open();
		
				// Oznacza ze tabela bedzie na 90% strony
				table.WidthPercentage = 90;

				// Dodanie nazwy i opisu wydarzenia
				PdfPCell cell = new PdfPCell(new Phrase($"{nameAndDescTbx.Text}\n\n", font));
				cell.Colspan = 3;
				cell.BorderWidth = 0;
				table.AddCell(cell);

				// Dodanie daty i miejsca wydarzenia
				cell = new PdfPCell(new Phrase($"{dayTbx.Text}-{monthTbx.Text}-{yearTbx.Text}   {placeTbx.Text}\n", font2));
				cell.Colspan = 3;
				cell.PaddingBottom = 30;
				cell.BorderWidth = 0;
				table.AddCell(cell);

				// Kazdy wyklad zostaje dodany do komorki a ta po dostosowaniu zostaje dodana do tabeli
				foreach (Lecture lecture in lectures)
				{
					cell = new PdfPCell(new Phrase($"{lecture.hourTbx1.Text}:{lecture.minuteTbx1.Text} - {lecture.hourTbx2.Text}:{lecture.minuteTbx1.Text}", font3));
					cell.BorderWidthLeft = cell.BorderWidthTop = cell.BorderWidthRight = 0;
					cell.PaddingBottom = 10;
					cell.BorderColor = new BaseColor(208, 208, 225);
					table.AddCell(cell);

					cell = new PdfPCell(new Phrase(lecture.descriptionTbx.Text, font3));
					cell.BorderWidthLeft = cell.BorderWidthTop = cell.BorderWidthRight = 0;
					cell.PaddingBottom = 10;
					cell.BorderColor = new BaseColor(208, 208, 225);
					table.AddCell(cell);

					cell = new PdfPCell(new Phrase(lecture.speakerTbx.Text, font3));
					cell.BorderWidthLeft = cell.BorderWidthTop = cell.BorderWidthRight = 0;
					cell.PaddingBottom = 10;
					cell.BorderColor = new BaseColor(208, 208, 225);
					table.AddCell(cell);
				}
				doc.Add(table);
				doc.Close();
			}
		}

		// Wyswietlenie informacji w menu Informacje
		private void Info_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult info = MessageBox.Show("UWAGA! używanie znaku ^ w wypelnianu danych spowoduje problem z zapisem danych\nAplikacja stworzona przez Paweł Kosik", "Informacje", MessageBoxButton.OK);
		}

		// Zapis do pliku przycisk Zapisz
		private void editSaveBtn_Click(object sender, RoutedEventArgs e)
		{
			saveToFile();

			MessageBoxResult save = MessageBox.Show("Zapisano poprawnie", "Zapis", MessageBoxButton.OK);
		}

		// Funkcja zapisujaca do pliku
		private void saveToFile()
		{
			string tempString;
			List<string> lines = new List<string>();

			foreach (Lecture lecture in lectures)
			{
				tempString = lecture.hourTbx1.Text + "^" + lecture.minuteTbx1.Text + "^" + lecture.hourTbx2.Text + "^" + lecture.minuteTbx2.Text
					+ "^" + lecture.descriptionTbx.Text + "^" + lecture.speakerTbx.Text;
				lines.Add(tempString);
			}

			File.WriteAllLines(pathForLec, lines);
			tempString = dayTbx.Text + "^" + monthTbx.Text + "^" + yearTbx.Text + "^" + placeTbx.Text + "^" + nameAndDescTbx.Text;
			File.WriteAllText(pathForLecInfo, tempString);
		}

		// Dodanie wykladu przycisk Dodaj Wyklad
		private void addLectureBtn_Click(object sender, RoutedEventArgs e)
		{
			RowDefinition rowDef = new RowDefinition();
			Lecture lecture = new Lecture();

			rowDef.Height = new GridLength(80);
			gridForLectures.RowDefinitions.Add(rowDef);
			gridForLectures.Children.Add(lecture.mainStackPanel);
			Grid.SetRow(lecture.mainStackPanel, lectureCount);
			lectures.Add(lecture);

			lectureCount++;
		}

		// Usuniecie zaznaczonych wykladow przycisk Usun
		private void deleteLecturesbtn_Click(object sender, RoutedEventArgs e)
		{
			// Zapamietanie ile bedzie do usuniecia wierszy w siatce na wyklady
			int tempLectureCount = lectureCount;

			// Wyzerowanie ilosci wykladow bo bedziemy je jeszcze raz wczytywac po selekcji
			lectureCount = 0;

			foreach (Lecture lecture in lectures)
			{
				// Usuniecie zaznaczonych wykladow z siatki na wyklady
				if (lecture.toDeleteFlag)
					gridForLectures.Children.Remove(lecture.mainStackPanel);
			}

			// Usuniecie zaznaczonych wykladow z listy
			lectures.RemoveAll(item => item.toDeleteFlag == true);

			// Zapis do pliku wykladow ktore nie zostaly usuniete
			saveToFile();

			// Usuniecie pozostalych wykladow z siatki
			foreach (Lecture lecture in lectures)
				gridForLectures.Children.Remove(lecture.mainStackPanel);

			// Wyczyszczenie listy wykladow
			lectures.Clear();

			// Usuniecie wierszy z siatki
			gridForLectures.RowDefinitions.RemoveRange(0, tempLectureCount);

			// Wczytanie wykladow ktore nie byly zaznaczone
			writeFromfile();
		}

		// Walidacja sprawdzajaca czy wpisywane sa liczby
		private void digitValidation(object sender, KeyEventArgs e)
		{
			string tmpString = e.Key.ToString();

			if (tmpString.Length < 2)
			{
				e.Handled = true;
				return;
			}

			tmpString = tmpString.Substring(1, 1);
			char tmpChar = tmpString[0];

			if (!char.IsDigit(tmpChar))
				e.Handled = true;
		}

		// Sprawdzenie poprawnosci wpisanego dnia
		private void dayTbx_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			int day;

			if (int.TryParse(dayTbx.Text, out day))
			{
				if (day > 31 || day < 0)
					dayTbx.Text = string.Empty;

				else if (day < 10 && tbx.Text.Length < 2)
					dayTbx.Text = "0" + dayTbx.Text;
			}

			if (tbx.Text.Length > 2)
				tbx.Text = "00";
		}

		// Sprawdzenie poprawnosci wpisanego miesiaca
		private void monthTbx_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			int month;

			if (int.TryParse(monthTbx.Text, out month))
			{
				if (month > 12 || month < 0)
					monthTbx.Text = string.Empty;

				else if (month < 10 && tbx.Text.Length < 2)
					monthTbx.Text = "0" + monthTbx.Text;
			}

			if (tbx.Text.Length > 2)
				tbx.Text = "00";
		}

		// Sprawdzenie poprawnosci wpisanego roku
		private void yearTbx_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			int year;

			if (int.TryParse(yearTbx.Text, out year))
			{
				if (year < 2018)
					yearTbx.Text = string.Empty;
			}

			if (tbx.Text.Length > 4)
				tbx.Text = "0000";
		}

		#endregion
	}
}
