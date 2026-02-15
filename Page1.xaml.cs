using Model;
using Service;
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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        ApiService apiService=new ApiService();
        public Page1()
        {
            InitializeComponent();
            SongData();
           


        }
        private async void SongData()
        {
            SongList songs =await apiService.SelectAllSongs();
            SongUserControl songUser =new SongUserControl( songs[0]);
            sp.Children.Add(songUser);
        }
    }
}
