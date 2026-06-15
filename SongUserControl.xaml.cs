using Model;
using NoaMedia;
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

            List<lyrics> currentLy = ly.Where(x => x.Songid.Id == s.Id).OrderBy(x => x.Placment).ToList();

            diffRating.Value = s.Difficultyid.Id;
            genrebox.Text = ("genre " + s.Gaenreid.Genrename);
            artistbox.Text = ("artist " + s.Artistid.Name);
            namebox.Text = s.Name;
            lanbox.Text = s.Languageid.Languagename;

            if (s.SongPic != null) 
            {
                pic.Source = ImageExtensions.ByteToImage(Convert.FromBase64String(s.SongPic));
            }

            songData.Children.Clear();

            foreach (lyrics lyr in currentLy)
            {
                StackPanel lyPanel = new StackPanel() { Margin = new Thickness(10, 0, 10, 10) };

                if (lyr.Chordid != null && !string.IsNullOrEmpty(lyr.Chordid.Name))
                {
                    TextBlock chordText = new TextBlock();
                    chordText.Text = lyr.Chordid.Name;
                    chordText.Foreground = Brushes.DarkRed; 
                    chordText.FontWeight = FontWeights.Bold;
                    chordText.FontSize = 14;
                    lyPanel.Children.Add(chordText);
                }

                TextBlock lyText = new TextBlock();
                lyText.Text = lyr.Lyricsname;
                lyText.Foreground = Brushes.White;
                lyText.FontSize = 16;
                lyPanel.Children.Add(lyText);

                songData.Children.Add(lyPanel);
            }
        }



    }
}
