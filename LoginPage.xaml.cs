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
using System.Xml.Linq;

namespace MusicSchoolWpf
{

    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        ApiService perserv = new ApiService();
        ApiService adserv = new ApiService();

        PersonList plist = new PersonList();
        AdminList adlist = new AdminList();

        public LoginPage()
        {
            
            InitializeComponent();
            GetAllPersons();
            GetAllAdmins();
        }
        private async void GetAllPersons()
        {
            try
            {
                plist = await perserv.SelectAllPersons();
            }
            catch
            {
                MessageBox.Show("בעיה בטעינת המשתמשים מהשרת. ודא שה-API פועל.");
            }
        }
        private async void GetAllAdmins()
        {
            try
            {
                adlist = await adserv.SelectAllAdmins();
            }
            catch
            {
                MessageBox.Show("בעיה בטעינת מנהלי המערכת מהשרת. ודא שה-API פועל.");
            }
        }
        private async void Login(object sender, RoutedEventArgs e)
        {
            string name = loginusername.Text;
            string pass = loginpassword.Password;

            try
            {
                plist = await perserv.SelectAllPersons();


                person user = plist.Find(x => x.Name == name && x.Code == pass);

                if (user != null)
                {
                    NavigationService.Navigate(new HomePage2(user.Name));
                }
                else
                {
                    MessageBox.Show("שם משתמש או סיסמה שגויים");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בכניסה רגילה:\n" + ex.Message);
            }
        }



        private void Signuppage(object sender, RoutedEventArgs e)
        {












            //this.NavigationService.Navigate(new HomePage2(name));

        }

        private void SwitchTologin(object sender, RoutedEventArgs e)
        {
            signuppannel.Visibility = Visibility.Collapsed;
            adminpannel.Visibility = Visibility.Collapsed;
            loginpannel.Visibility = Visibility.Visible;
            signuppassword.Clear();
            signupusername.Clear();
            adminpassword.Clear();
            adminusername.Clear();
        }

        private void SwitchToSignup(object sender, RoutedEventArgs e)
        {
            loginpannel.Visibility = Visibility.Collapsed;
            adminpannel.Visibility = Visibility.Collapsed;
            signuppannel.Visibility = Visibility.Visible;

            loginpassword.Clear();
            loginusername.Clear();
            adminpassword.Clear();
            adminusername.Clear();

        }
        private void SwitchToadmin(object sender, RoutedEventArgs e)
        {
            loginpannel.Visibility = Visibility.Collapsed;
            signuppannel.Visibility = Visibility.Visible;
            loginpassword.Clear();
            loginusername.Clear();
            adminpassword.Clear();
            adminusername.Clear();
            loginpannel.Visibility = Visibility.Visible;

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

            this.NavigationService.Navigate(new HomePage2(name));

        }
        private async void InsertPerson(person p)
        {
            ApiService apiService = new ApiService();
            int x = await apiService.InsertAPerson(p);
            if (x > 0)
            {
                MessageBox.Show("ההרשמה לא נשמרה במסד הנתונים");

            }


        }

        private void Admin(object sender, RoutedEventArgs e)
        {
            loginpannel.Visibility = Visibility.Collapsed;
            signuppannel.Visibility = Visibility.Collapsed;
            adminpannel.Visibility = Visibility.Visible;

            loginpassword.Clear();
            loginusername.Clear();
            signuppassword.Clear();
            signupusername.Clear();
        }

        private async void adminsignupclick(object sender, RoutedEventArgs e)
        {
            string name = adminusername.Text;
            string pass = adminpassword.Password;

            try
            {
                plist = await perserv.SelectAllPersons();

                person personUser = plist.Find(x => x.Name == name && x.Code == pass);

                if (personUser == null)
                {
                    MessageBox.Show("שם משתמש או סיסמה שגויים");
                    return;
                }

                adlist = await adserv.SelectAllAdmins();

               
                Admin adminUser = adlist.Find(x => x.Id == personUser.Id);

                if (adminUser != null)
                {
                    MessageBox.Show("ברוך הבא, מנהל מערכת!");
                    NavigationService.Navigate(new AdminDashboardPage(personUser.Name));
                }
                else
                {
                    MessageBox.Show("המשתמש קיים, אבל הוא לא מוגדר כמנהל מערכת");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בכניסת אדמין:\n" + ex.Message);
            }
        }
    }
    }

