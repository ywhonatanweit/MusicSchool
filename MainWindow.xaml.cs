using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MusicSchoolWpf
{
    public partial class MainWindow : Window
    {
        private bool isAdminMode = false;

        private string currentUserName = "Musician";
        private string currentAdminName = "Admin";

        private readonly Brush activeBrush = Brushes.Red;
        private readonly Brush inactiveBrush = (Brush)new BrushConverter().ConvertFromString("#2E2E2E")!;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void LogoutToLogin()
        {
            isAdminMode = false;
            currentUserName = "Musician";
            currentAdminName = "Admin";

            SideBar.Visibility = Visibility.Collapsed;

            MainFrame.Navigate(new LoginPage());
        }

        private void tunerclick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Tuner());
        }

        private void songsclick(object sender, MouseButtonEventArgs e)
        {
            if (isAdminMode)
            {
                MainFrame.Navigate(new AdminSongsPage(currentAdminName));
            }
            else
            {
                MainFrame.Navigate(new songpage());
            }
        }

        private void metroclick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new metronome());
        }

        private void ChordClick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new chordlab());
        }

        private void ChordTrainerClick(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new ChordTrainerPage());
        }

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            if (isAdminMode)
            {
                MainFrame.Navigate(new AdminDashboardPage(currentAdminName));
            }
            else
            {
                MainFrame.Navigate(new HomePage2(currentUserName));
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is LoginPage)
            {
                isAdminMode = false;
                SideBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                SideBar.Visibility = Visibility.Visible;
            }

            if (e.Content is HomePage2 homePage)
            {
                isAdminMode = false;
                currentUserName = homePage.UserName;
            }
            else if (e.Content is AdminDashboardPage adminDashboard)
            {
                isAdminMode = true;
                currentAdminName = adminDashboard.AdminName;
            }
            else if (e.Content is AdminSongsPage adminSongsPage)
            {
                isAdminMode = true;
                currentAdminName = adminSongsPage.AdminName;
            }

            HomeIcon.Foreground =
                e.Content is HomePage2 || e.Content is AdminDashboardPage
                    ? activeBrush
                    : inactiveBrush;

            songicon.Foreground =
                e.Content is songpage || e.Content is AdminSongsPage
                    ? activeBrush
                    : inactiveBrush;

            metroicon.Foreground =
                e.Content is metronome
                    ? activeBrush
                    : inactiveBrush;

            chordicon.Foreground =
                e.Content is chordlab
                    ? activeBrush
                    : inactiveBrush;

            tunericon.Foreground =
                e.Content is Tuner
                    ? activeBrush
                    : inactiveBrush;

            if (chordTrainerIcon != null)
            {
                chordTrainerIcon.Foreground =
                    e.Content is ChordTrainerPage
                        ? activeBrush
                        : inactiveBrush;
            }
        }
    }
}