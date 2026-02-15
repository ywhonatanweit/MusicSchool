using Model;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        ApiService perserv = new ApiService();
        PersonList plist = new PersonList();
        public LoginPage()
        {
            InitializeComponent();
            GetAllPersons();
        }
        private async void GetAllPersons()
        {
            plist = await perserv.SelectAllPersons();
        }
        private void Login(object sender, RoutedEventArgs e)
        {
            string name = loginusername.Text;
            string pass = loginpassword.Password;
            MessageBox.Show(plist[0].Name + " " + plist[0].Code);
            person user = plist.Find(x => x.Name == name && x.Code == pass);
            if (user != null)
            {
                this.NavigationService.Navigate(new Page1());
            }

        }



        private void Signuppage(object sender, RoutedEventArgs e)
        {












            this.NavigationService.Navigate(new Page2());

        }

        private void SwitchTologin(object sender, RoutedEventArgs e)
        {
            signuppannel.Visibility = Visibility.Collapsed;
            loginpannel.Visibility = Visibility.Visible;
            signuppassword.Clear();
            signupusername.Clear();
        }

        private void SwitchToSignup(object sender, RoutedEventArgs e)
        {
            loginpannel.Visibility = Visibility.Collapsed;
            signuppannel.Visibility = Visibility.Visible;
            loginpassword.Clear();
            loginusername.Clear();
        }

        private void newsignup(object sender, RoutedEventArgs e)
        {
            string name = signupusername.Text;
            string pass = signuppassword.Password;
            person p = new person();
            p.Name = name;
            p.Code = pass;
            InsertPerson(p);
            MessageBox.Show("welcome");
            signuppannel.Visibility = Visibility.Collapsed;
            loginpannel.Visibility = Visibility.Visible;
            signuppassword.Clear();
            signupusername.Clear();
            loginpassword.Clear();
            loginusername.Clear();
        }
        private async void InsertPerson(person p)
        {
            ApiService apiService = new ApiService();
            int x = await apiService.InsertAPerson(p);
            if (x > 0)
            {
           
            }


        }
    }
}
