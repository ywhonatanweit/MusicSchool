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
    /// Interaction logic for songpage.xaml
    /// </summary>
    public partial class songpage : Page
    {
        public songpage()
        {
            InitializeComponent();
        }
        public songpage(song s)
        {
            InitializeComponent();
            SongUserControl sUC = new SongUserControl(s);
            sp.Children.Add(sUC);
        }
    }
}
