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
    /// Interaction logic for SongUserControl.xaml
    /// </summary>
    public partial class SongUserControl : UserControl
    {
        ApiService apiService = new ApiService();
        public SongUserControl(song s)
        {
            InitializeComponent();

            GetSong(s);


        }
        private async void GetSong(song s) 
        {
            LyricsList ly = await apiService.SelectAllLyrics();
            List < lyrics > currentLy = (List<lyrics>)ly.FindAll(x => x.Songid.Id == s.Id) ;
            currentLy.OrderBy(x => x.Placment).ToList();
            foreach (lyrics lyr in currentLy)
            {
                StackPanel lyPanel = new StackPanel();
                TextBlock chordText = new TextBlock();
                chordText.Text = lyr.Chordid.Name.ToString();
                TextBlock lyText = new TextBlock();
                lyText.Text= lyr.Lyricsname;
                lyPanel.Children.Add( chordText );
                lyPanel.Children.Add( lyText );
                songData.Children.Add ( lyPanel );
                diffbox.Text = ("difficulty "+ s.Difficultyid);
                genrebox.Text = ("genre "+ s.Difficultyid);
                artistbox.Text = ("artist " + s.Artistid);
                namebox.Text = s.Name;


            }
        }




    }
}
