using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicSchoolWpf
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void songsclick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AllMySongsPage());
        }

        private void tunerclick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Tuner());
        }

        private void metroclick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new metronome());
        }
    }
}
