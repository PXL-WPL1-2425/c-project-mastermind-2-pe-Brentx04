using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace Mastermind
{
    public partial class MainWindow : Window
    {
        private int attempts = 0;
        private const int maxAttempts = 10;
        private int totalPenaltyPoints = 0;
        private DispatcherTimer timer;
        private int counter = 0;
        private string[] colors = { "rood", "geel", "oranje", "wit", "groen", "blauw" };
        private string[] colorCode = new string[4];
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            GenerateSecretCode();
            InitializeTimer();
        }


        private void GenerateSecretCode()
        {
            for (int i = 0; i < 4; i++)
            {
                colorCode[i] = colors[random.Next(colors.Length)];
            }
        }



        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= 10)
            {
                attempts++;
                timer.Stop();
            }
            textBoxTime.Text = $"Timer: {counter} seconden";
        }


        private string GetComboBoxValue(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString().Trim().ToLower() ?? string.Empty;
        }


        private void CheckCode_Click(object sender, RoutedEventArgs e)
        {
            attempts++;
            counter = 0;
            timer.Start();
            attempsTextBox.Text = $"Attempts: {attempts}";




            string[] userColors = new string[]
            {
                GetComboBoxValue(comboBox1),
                GetComboBoxValue(comboBox2),
                GetComboBoxValue(comboBox3),
                GetComboBoxValue(comboBox4)
            };


            StackPanel attemptPanel = CreateAttemptPanel(userColors);
            historyList.Items.Add(attemptPanel);


            if (colorCode.SequenceEqual(userColors))
            {
                timer.Stop();
                MessageBox.Show("Proficiat! Je hebt gewonnen.");
                return;
            }

            if (attempts >= maxAttempts)
            {
                timer.Stop();

                MessageBoxResult result = MessageBox.Show(
                "Je hebt het spel verloren. De code was: " + string.Join(",", colorCode) +
                "Je hebt het maximale aantal pogingen bereikt! Probeer opnieuw.\t" +
                "Wilt u nogmaals proberen?",
                "Bevestigen",
                MessageBoxButton.YesNo);



                if (result == MessageBoxResult.Yes)
                    {
           
                    penaltyPointsLabel.Content = null;

                    attempts = 0;

                    counter = 0;

                    historyList.Items.Clear();
                    return;
                }


                if (result == MessageBoxResult.No)
                {
                    this.Close();
                }

            }

        }


        private StackPanel CreateAttemptPanel(string[] userColors)
        {
            StackPanel attemptPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            Label[] attemptLabels = new Label[4];

            for (int i = 0; i < 4; i++)
            {
                attemptLabels[i] = CreateAttemptLabel(GetColorBrush(userColors[i]));
                attemptPanel.Children.Add(attemptLabels[i]);
            }

            AddFeedbackAndPenalty(userColors, attemptPanel);
            return attemptPanel;


        }


        private Label CreateAttemptLabel(Brush background)
        {
            return new Label
            {
                Background = background,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2),
                Width = 25,
                Height = 25,
                Margin = new Thickness(2)
            };
        }


        private Brush GetColorBrush(string color)
        {
            return color.ToLower() switch
            {
                "wit" => new SolidColorBrush(Colors.White),
                "groen" => new SolidColorBrush(Colors.Green),
                "blauw" => new SolidColorBrush(Colors.Blue),
                "rood" => new SolidColorBrush(Colors.Red),
                "geel" => new SolidColorBrush(Colors.Yellow),
                "oranje" => new SolidColorBrush(Colors.Orange),
                _ => new SolidColorBrush(Colors.Transparent)
            };
        }


        private void AddFeedbackAndPenalty(string[] userColors, StackPanel attemptPanel)
        {
            totalPenaltyPoints = 0;

            for (int i = 0; i < colorCode.Length; i++)
            {
                string userColor = userColors[i];
                Label currentLabel = attemptPanel.Children[i] as Label;


                if (colorCode[i] == userColor)
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Green);
                    currentLabel.BorderThickness = new Thickness(3);
                }

                else if (colorCode.Contains(userColor))
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Wheat);
                    currentLabel.BorderThickness = new Thickness(3);
                }
                else
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Red);
                    currentLabel.BorderThickness = new Thickness(3);
                }


                if (colorCode[i] == userColor)
                {
                    totalPenaltyPoints += 0;
                }
                else if (colorCode.Contains(userColor))
                {
                    totalPenaltyPoints += 1;
                }
                else
                {
                    totalPenaltyPoints += 2;
                }
            }
            penaltyPointsLabel.Content = $"Strafpunten: {totalPenaltyPoints}";
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F12)
            {
                colorsTextBox.Visibility = Visibility.Visible;
                colorsTextBox.Text = string.Join(", ", colorCode);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bent u zeker dat u de applicatie wilt afsluiten?",
                                                       "Bevestigen",
                                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voer uw naam in.");
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
