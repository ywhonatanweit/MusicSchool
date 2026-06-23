using Model;
using NoaMedia;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MusicSchoolWpf
{
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

            name.Text = s.Name ?? "";
            artist.Text = s.Artistid != null ? s.Artistid.Name : "";
            lan.Text = s.Languageid != null ? s.Languageid.Languagename : "";
            genre.Text = s.Gaenreid != null ? s.Gaenreid.Genrename : "";
            diff.Value = s.Difficultyid != null ? s.Difficultyid.Id : 0;

            pic.Source = ImageExtensions.ToBitmapImage(s.SongPic, s.Songpath);
        }

        private void Show(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this)?.Navigate(new songpage(sng));
        }
    }
}