using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Input;

namespace MusicSchoolWpf
{
    public partial class HomePage2 : Page
    {
        public string UserName => username.Text;

        public HomePage2(string name)
        {
            InitializeComponent();

            username.Text = name;
        }

        private void tunerclick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Tuner());
        }
        
       private void practiceclick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Tuner());
        }
        private void songsclick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new songpage());
        }

        private void metroclick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new metronome());
        }

        private void ChordClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new chordlab());
        }

        private void ChordTrainerClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ChordTrainerPage());
        }

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new HomePage2(username.Text));
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