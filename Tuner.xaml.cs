//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace MusicSchoolWpf
//{
//    /// <summary>
//    /// Interaction logic for Tuner.xaml
//    /// </summary>
//    public partial class Tuner : Page
//    {
//        public Tuner()
//        {
//            InitializeComponent();
//        }
//        //private bool isElectric = true;

//        //private void SwitchHead_Click(object sender, RoutedEventArgs e)
//        //{
//        //    if (isElectric)
//        //    {
//        //        ElectricHead.Visibility = Visibility.Collapsed;
//        //        AcousticHead.Visibility = Visibility.Visible;
//        //    }
//        //    else
//        //    {
//        //        ElectricHead.Visibility = Visibility.Visible;
//        //        AcousticHead.Visibility = Visibility.Collapsed;
//        //    }

//        //    isElectric = !isElectric;
//        //}
//        private bool isElectric = true;



//        // החלפת חשמלית / אקוסטית
//        private void SwitchHead_Click(object sender, RoutedEventArgs e)
//        {
//            if (isElectric)
//            {
//                ElectricHead.Visibility = Visibility.Collapsed;
//                AcousticHead.Visibility = Visibility.Visible;
//            }
//            else
//            {
//                ElectricHead.Visibility = Visibility.Visible;
//                AcousticHead.Visibility = Visibility.Collapsed;
//            }

//            isElectric = !isElectric;
//        }

//        // מעבר לימני
//        private void HandSwitch_Unchecked(object sender, RoutedEventArgs e)
//        {
//            HeadTransform.ScaleX = 1;
//            HandSwitch.Content = "Right Handed";
//        }

//        // מעבר לשמאלי
//        private void HandSwitch_Checked(object sender, RoutedEventArgs e)
//        {
//            HeadTransform.ScaleX = -1;
//            HandSwitch.Content = "Left Handed";
//        }
//    }
//}
using System.Windows;
using System.Windows.Controls;

namespace MusicSchoolWpf
{
    public partial class Tuner : Page
    {
        private bool isElectric = true;
        private bool isLeftHanded = false;

        public Tuner()
        {
            InitializeComponent();
            UpdateView();
        }

        private void SwitchHead_Click(object sender, RoutedEventArgs e)
        {
            isElectric = !isElectric;
            UpdateView();
        }

        private void HandSwitch_Checked(object sender, RoutedEventArgs e)
        {
            isLeftHanded = true;
            HandSwitch.Content = "Left Handed";
            UpdateView();
        }

        private void HandSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            isLeftHanded = false;
            HandSwitch.Content = "Right Handed";
            UpdateView();
        }

        private void UpdateView()
        {
            // להסתיר הכל
            ElectricRight.Visibility = Visibility.Collapsed;
            ElectricLeft.Visibility = Visibility.Collapsed;
            AcousticRight.Visibility = Visibility.Collapsed;
            AcousticLeft.Visibility = Visibility.Collapsed;

            // להציג רק את המתאים
            if (isElectric && !isLeftHanded)
                ElectricRight.Visibility = Visibility.Visible;

            else if (isElectric && isLeftHanded)
                ElectricLeft.Visibility = Visibility.Visible;

            else if (!isElectric && !isLeftHanded)
                AcousticRight.Visibility = Visibility.Visible;

            else
                AcousticLeft.Visibility = Visibility.Visible;
        }
    }
}
