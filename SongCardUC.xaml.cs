using Model;
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
        public SongCardUC()
        {
            InitializeComponent();
        }
        public SongCardUC(song s)
        {
            InitializeComponent();

            name.Text = s.Name;
            artist.Text = s.Artistid.Name;


        }
    }
}
