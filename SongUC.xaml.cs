using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private async void GetSong(SongList sl)
        {





            //LyricsList ly = await apiService.SelectAllLyrics();
            WrapPanel wrap = new WrapPanel();
            foreach (song s in sl)
            {
                TextBlock name = new TextBlock();
                name.Text = s.Name;
                TextBlock artistname = new TextBlock();
                artistname.Text = s.Artistid.Name;
                TextBlock genre = new TextBlock();
                genre.Text = s.Gaenreid.Genrename;
                TextBlock diff = new TextBlock();
                diff.Text = s.Difficultyid.Diff.ToString();
                TextBlock lan = new TextBlock();
                lan.Text = s.Languageid.Languagename;
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(name); 
                stackPanel.Children.Add(artistname);
                stackPanel.Children.Add(genre);
                stackPanel.Children.Add(diff);
                stackPanel.Children.Add(lan);


                wrap.Children.Add(stackPanel );
            }


        }
        

        private void ShowSongData(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
