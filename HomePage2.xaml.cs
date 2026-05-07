using Model;
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
    /// Interaction logic for HomePage2.xaml
    /// </summary>
    public partial class HomePage2 : Page
    {
        public HomePage2(string name)
        {
            InitializeComponent();
            username.Text = name;
            // לשים את האות הראשונה של השם בתוך העיגול משתמש
        }

        private void tunerclick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Tuner());

        }

        private void songsclick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new songpage());
        }

        private void metroclick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new metronome());

        }

      
    }
}
