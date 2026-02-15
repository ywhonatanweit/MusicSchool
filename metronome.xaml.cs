using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for metronome.xaml
    /// </summary>
    public partial class metronome : Page
    {
        private int bpm = 0;
        public metronome()
        {
            InitializeComponent();
        }
        private void IncreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm += 5;
            UpdateBpmText();
        }

        private void DecreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm -= 5;
            if (bpm < 0) bpm = 0; 
            UpdateBpmText();
        }

        private void UpdateBpmText()
        {
            BpmTextBox.Text = bpm.ToString();
        }

        private void playbpm(object sender, RoutedEventArgs e)
        {
            //לעשות צליל שמופיע כמות הפעמים בדקה של הערך bpm
        }
    }
}
