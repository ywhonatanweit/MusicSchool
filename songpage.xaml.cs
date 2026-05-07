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
    /// Interaction logic for songpage.xaml
    /// </summary>
    public partial class songpage : Page
    {

        ApiService api=new ApiService();
        SongList songlist = new SongList();
        public songpage()
        {
            InitializeComponent();

            GetAllSongs();

        }



        public async void GetAllSongs()
        {
            SongCardUC song;
            songlist =await api.SelectAllSongs();

            foreach (song s in songlist)
            {
                song = new SongCardUC(s);
                sp.Children.Add(song);
            }
        }
        public songpage(song s)
        {
            InitializeComponent();
            SongUserControl sUC = new SongUserControl(s);
            sp.Children.Add(sUC);
        }
    }
}
