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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MusicSchoolWpf
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

        // Changed RoutedEventArgs to MouseButtonEventArgs to match MouseLeftButtonDown
        private void tunerclick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Tuner());
        }

        private void songsclick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new songpage());
        }

        private void metroclick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new metronome());
        }

        private void ChordClick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new chordlab());
        }

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            // Note: Ensure 'username' exists on MainWindow or grab it from your LoginPage/session!
            // string name = username.Text; 

            string name = "Musician"; // Temporary fallback if username is inside the LoginPage
            MainFrame.Navigate(new HomePage2(name));
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is LoginPage)
            {
                SideBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                SideBar.Visibility = Visibility.Visible;
            }
        }
    }
}