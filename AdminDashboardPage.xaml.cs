using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace MusicSchoolWpf
{
    public partial class AdminDashboardPage : Page
    {
        private string adminName;

        public string AdminName => adminName;

        public AdminDashboardPage() : this("Admin")
        {
        }

        public AdminDashboardPage(string name)
        {
            InitializeComponent();

            adminName = name;
            username.Text = name;
        }

        private void ManageSongs_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AdminSongsPage(adminName));
        }

        private void SongsClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new songpage());
        }

        private void ChordClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new chordlab());
        }

        private void TunerClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Tuner());
        }

        private void MetroClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new metronome());
        }

        private void ChordTrainerClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ChordTrainerPage());
        }

        private void LogoutClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.LogoutToLogin();
            }
        }
    }
}