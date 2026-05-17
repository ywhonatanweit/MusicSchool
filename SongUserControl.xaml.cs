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
            List<lyrics> currentLy = (List<lyrics>)ly.FindAll(x => x.Songid.Id == s.Id);
            currentLy.OrderBy(x => x.Placment).ToList();

            currentLy = ly.Where(x => x.Songid.Id == s.Id).OrderBy(x => x.Placment).ToList();

            // המיון לא בטוח טוב אולי כדאי להשאיר רק את השורה האחרונה וליצור בה את קארנטלי בתור ואר

            //נתונים:



            diffRating.Value = s.Difficultyid.Id;
            genrebox.Text = ("genre " + s.Gaenreid.Genrename);
            artistbox.Text = ("artist " + s.Artistid.Name);
            namebox.Text = s.Name;
            lanbox.Text = s.Languageid.Languagename;
            if (s.SongPic != null) // אם ה-Image הוא byte[]
            {
                pic.Source = ImageExtensions.ByteToImage(Convert.FromBase64String(s.SongPic));
            }
        
    

        // כאן תוכל להוסיף טעינת תמונה אם יש לך נתיב ב-s.Pic
        // if (!string.IsNullOrEmpty(s.Pic)) imgSong.Source = new BitmapImage(new Uri(s.Pic));


        songData.Children.Clear();

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


            }
        }




    }
}
