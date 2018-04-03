using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AUSAW
{
	public class Lecture
	{
		#region Private

		// Znaki do wklejenia 
		private Label labelColon1 = new Label();
		private Label labelColon2 = new Label();
		private Label labelDash = new Label();

		// Dockpanel na godzine rozpoczenia i zakonczenia
		private DockPanel dockPanelForTime = new DockPanel();

		#endregion

		#region Public

		// Wskazuje czy wyklad jest do usuniecia
		public bool toDeleteFlag { get; set; } = false;

		// Glowny stackpanel na wyklad
		public StackPanel mainStackPanel = new StackPanel();

		// Godziny zaczecia
		public TextBox hourTbx1 = new TextBox();

		// Godziny zakonczenia
		public TextBox hourTbx2 = new TextBox();

		// Minuty zaczencia
		public TextBox minuteTbx1 = new TextBox();

		// Minuty zakonczenia
		public TextBox minuteTbx2 = new TextBox();

		// Textbox na opis wykladu
		public TextBox descriptionTbx = new TextBox();

		// Textbox na informacje o mowcy
		public TextBox speakerTbx = new TextBox();

		// Przycisk usuwajacy wyklad
		public Button deleteLectureBtn = new Button();

		#endregion

		public Lecture()
		{
			#region Adding styles and events

			// Dodanie styli jak we wzorcu + eventy
			mainStackPanel.Margin = new Thickness(5);
			mainStackPanel.Orientation = Orientation.Horizontal;

			hourTbx1.Width = hourTbx2.Width = minuteTbx1.Width = minuteTbx2.Width = 25;
			hourTbx1.Height = hourTbx2.Height = minuteTbx1.Height = minuteTbx2.Height = 25;
			hourTbx1.FontSize = hourTbx2.FontSize = minuteTbx1.FontSize = minuteTbx2.FontSize = 16;

			hourTbx1.KeyDown += hourValidation;
			hourTbx2.KeyDown += hourValidation;
			minuteTbx1.KeyDown += hourValidation;
			minuteTbx2.KeyDown += hourValidation;

			hourTbx1.LostFocus += timeHour_LostFocus;
			minuteTbx1.LostFocus += timeMinute_LostFocus;
			hourTbx2.LostFocus += timeHour_LostFocus;
			minuteTbx2.LostFocus += timeMinute_LostFocus;

			hourTbx1.MaxLength = hourTbx2.MaxLength = minuteTbx1.MaxLength = minuteTbx2.MaxLength = 2;
			hourTbx1.VerticalAlignment = hourTbx2.VerticalAlignment = minuteTbx1.VerticalAlignment = minuteTbx2.VerticalAlignment = VerticalAlignment.Center;
			hourTbx1.HorizontalAlignment = hourTbx2.HorizontalAlignment = minuteTbx1.HorizontalAlignment = minuteTbx2.HorizontalAlignment = HorizontalAlignment.Center;

			labelColon1.Content = labelColon2.Content = ":";
			labelColon1.VerticalAlignment = labelColon2.VerticalAlignment = VerticalAlignment.Center;
			labelColon1.HorizontalAlignment = labelColon2.HorizontalAlignment = HorizontalAlignment.Center;
			labelDash.Margin = new Thickness(10, 0, 10, 0);
			labelDash.FontSize = 20;
			labelDash.Content = "-";
			labelDash.VerticalAlignment = VerticalAlignment.Center;
			labelDash.HorizontalAlignment = HorizontalAlignment.Center;

			descriptionTbx.Margin = speakerTbx.Margin = new Thickness(19, 0, 0, 0);
			descriptionTbx.FontSize = speakerTbx.FontSize = 14;
			descriptionTbx.Height = speakerTbx.Height = 60;
			descriptionTbx.Width = 690;
			speakerTbx.Width = 280;
			descriptionTbx.TextWrapping = speakerTbx.TextWrapping = TextWrapping.Wrap;
			descriptionTbx.MaxLength = 300;
			speakerTbx.MaxLength = 150;

			deleteLectureBtn.Width = 50;
			deleteLectureBtn.Height = 60;
			deleteLectureBtn.Content = "Zaznacz";
			deleteLectureBtn.Margin = new Thickness(10, 0, 0, 0);
			deleteLectureBtn.Click += DeleteLectureBtn_Click;

			#endregion


			#region Nesting elements

			// Zagniezdzanie elementow
			dockPanelForTime.Children.Add(hourTbx1);
			dockPanelForTime.Children.Add(labelColon1);
			dockPanelForTime.Children.Add(minuteTbx1);
			dockPanelForTime.Children.Add(labelDash);
			dockPanelForTime.Children.Add(hourTbx2);
			dockPanelForTime.Children.Add(labelColon2);
			dockPanelForTime.Children.Add(minuteTbx2);
			mainStackPanel.Children.Add(dockPanelForTime);
			mainStackPanel.Children.Add(descriptionTbx);
			mainStackPanel.Children.Add(speakerTbx);
			mainStackPanel.Children.Add(deleteLectureBtn);

			#endregion

		}

		#region Helpers

		// Sprawdzenie poprawnosci godzin
		private void timeHour_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			int time;

			if (int.TryParse(tbx.Text, out time))
			{
				if (time > 23)
					tbx.Text = "00";

				else if (tbx.Text.Length == 1)
					tbx.Text = "0" + tbx.Text;
			}

			else
				tbx.Text = "00";
		}

		// Sprawdzenie poprawnosci minut
		private void timeMinute_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox tbx = (TextBox)sender;
			int time;

			if (int.TryParse(tbx.Text, out time))
			{
				if (time > 59)
					tbx.Text = "00";

				else if (tbx.Text.Length == 1)
					tbx.Text = "0" + tbx.Text;
			}

			else
				tbx.Text = "00";
		}

		// Zaznaczenie wykladu do usuniecia, zmiana tla by bylo wiadomo ktory jest zaznaczony
		private void DeleteLectureBtn_Click(object sender, RoutedEventArgs e)
		{
			toDeleteFlag ^= true;

			if (toDeleteFlag)
			{
				mainStackPanel.Background = new SolidColorBrush(Colors.DarkGray);
				deleteLectureBtn.Content = "Odznacz";
			}

			else
			{
				mainStackPanel.Background = new SolidColorBrush();
				deleteLectureBtn.Content = "Zaznacz";
			}

		}

		// Walidacja sprawdzajaca czy wpisywane sa liczby
		private void hourValidation(object sender, System.Windows.Input.KeyEventArgs e)
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

		#endregion

	}
}