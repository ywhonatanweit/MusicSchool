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
    /// Interaction logic for SongUC.xaml
    /// </summary>
    public partial class SongUC : UserControl
    {
       
        public EventHandler<SongUC> OnSongUC;

        ApiService apiService = new ApiService();
        public SongUC(song s)
        {
            InitializeComponent();
            namebox.Text = s.Name;
 


        }
        private async void GetSong(song s)
        {





           // LyricsList ly = await apiService.SelectAllLyrics();
           //WrapPanel wrap=new WrapPanel();
           // foreach (var x in ly)
           // {
           //     wrap.Children.Add(new TextBlock() { Text = x.Lyricsname });
           // }


           // }
        }

        private void ShowSongData(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
