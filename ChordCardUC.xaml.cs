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
    /// Interaction logic for ChordCardUC.xaml
    /// </summary>
    public partial class ChordCardUC : UserControl
    {
        private chord crd;
        public ChordCardUC(chord c) // בהנחה שיש לך Model שנקרא Chord
        {
            InitializeComponent();
            crd = c;
            lblChordName.Text = c.Name;
            lblDifficulty.Text = $"קושי: {c.Difficulty}";

            if (c.Chordpic != null) // אם ה-Image הוא byte[]
            {
                imgChord.Source = ImageExtensions.ByteToImage(Convert.FromBase64String(c.Chordpic));
            }
        }

       
    }
}
