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
    /// Interaction logic for AllMySongsPage.xaml
    /// </summary>
    public partial class AllMySongsPage : Page
    {
        public AllMySongsPage()
        {
            InitializeComponent();
            GetAllSongs()
;        }

        public async void GetAllSongs()
        {

        
            ApiService apiService = new ApiService();
            SongList songs = new SongList();
            songs = await apiService.SelectAllSongs();
            foreach (song s in songs)
            {
                SongUC suc = new SongUC(s);
                
                songswrapList.Children.Add(suc);
            }
        }

        private SongUC ShowSongData(song s)
        {
            throw new NotImplementedException();
        }
    }
}
