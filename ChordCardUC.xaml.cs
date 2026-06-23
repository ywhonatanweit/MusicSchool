using Model;
using NoaMedia;
using System.Windows.Controls;

namespace MusicSchoolWpf
{
    public partial class ChordCardUC : UserControl
    {
        private chord crd;

        public ChordCardUC(chord c)
        {
            InitializeComponent();

            crd = c;

            lblChordName.Text = c.Name ?? "";

            if (c.Difficulty != null)
                lblDifficulty.Text = $"קושי: {c.Difficulty.Diff}";
            else
                lblDifficulty.Text = "קושי: לא ידוע";

            imgChord.Source = ImageExtensions.ToBitmapImage(c.Chordpic, c.Chordpath);
        }
    }
}