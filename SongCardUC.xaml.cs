using Model;
using NoaMedia;
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
    /// Interaction logic for SongCardUC.xaml
    /// </summary>
    public partial class SongCardUC : UserControl
    {
        private song sng;
        public SongCardUC()
        {
            InitializeComponent();
        }
        public SongCardUC(song s)
        {
            InitializeComponent();

            sng = s;
            name.Text = s.Name;
            artist.Text = s.Artistid.Name;
            lan.Text = s.Languageid.Languagename;
            genre.Text = s.Gaenreid.Genrename;
            diff.Value = s.Difficultyid.Id;
            if (s.SongPic != null)
            {
                pic.Source = ImageExtensions.ByteToImage(Convert.FromBase64String(s.SongPic));
            }
        }

        private void Show(object sender, RoutedEventArgs e)
        {
            // ניווט לעמוד השיר (משתמשים ב-NavigationService של העמוד שמכיל את הכרטיסייה)
            //var parentPage = Window.GetWindow(this).FindName("MainFrame") as Frame;
            // הערה: אם אתה משתמש בפריימים לניווט, נשתמש בזה:
            NavigationService.GetNavigationService(this)?.Navigate(new songpage(sng));
        }
    }
}
