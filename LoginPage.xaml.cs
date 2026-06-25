using Model;
using Service;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MusicSchoolWpf
{
    public partial class LoginPage : Page
    {
        private readonly ApiService perserv = new ApiService();
        private readonly ApiService adserv = new ApiService();

        private PersonList plist = new PersonList();
        private AdminList adlist = new AdminList();

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
            string name = loginusername.Text.Trim();
            string pass = loginpassword.Password;

            try
            {
                plist = await perserv.SelectAllPersons();

                person? user = plist.Find(x => x.Name == name && x.Code == pass);

                if (user != null)
                {
                    MessageBox.Show(" ברוך הבא! " + loginusername.Text);
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

        private async void newsignup(object sender, RoutedEventArgs e)
        {
            string name = signupusername.Text.Trim();
            string pass = signuppassword.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("יש למלא שם משתמש וסיסמה");
                return;
            }

            try
            {
                person p = new person
                {
                    Name = name,
                    Code = pass
                };

                int result = await perserv.InsertAPerson(p);

                if (result <= 0)
                {
                    MessageBox.Show("ההרשמה לא נשמרה במסד הנתונים");
                    return;
                }

                MessageBox.Show(" המשתמש נוצר בהצלחה, ברוך הבא " + signupusername.Text);
                NavigationService.Navigate(new HomePage2(name));
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בהרשמה:\n" + ex.Message);
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
            string name = adminusername.Text.Trim();
            string pass = adminpassword.Password;

            try
            {
                plist = await perserv.SelectAllPersons();

                person? personUser = plist.Find(x => x.Name == name && x.Code == pass);

                if (personUser == null)
                {
                    MessageBox.Show("שם משתמש או סיסמה שגויים");
                    return;
                }

                adlist = await adserv.SelectAllAdmins();

                Admin? adminUser = adlist.Find(x => x.Id == personUser.Id);

                if (adminUser != null)
                {
                    MessageBox.Show( " ברוך הבא, מנהל מערכת! " + adminusername.Text);
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
        //private async void adminsignupclick(object sender, RoutedEventArgs e)
        //{
        //    string name = adminusername.Text.Trim();
        //    string pass = adminpassword.Password;

        //    try
        //    {
        //        plist = await perserv.SelectAllPersons();

        //        person? personUser = plist.Find(x => x.Name == name && x.Code == pass);

        //        if (personUser == null)
        //        {
        //            MessageBox.Show("שם משתמש או סיסמה שגויים");
        //            return;
        //        }

        //        // פתרון זמני ובטוח:
        //        // אם המשתמש קיים והוא מתחבר דרך מסך אדמין - נכנס לדף אדמין
        //        // בלי לקרוא כרגע ל-SelectAllAdmins שמפיל את Access
        //        MessageBox.Show("ברוך הבא, מנהל מערכת!");
        //        NavigationService.Navigate(new AdminDashboardPage(personUser.Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("בעיה בכניסת אדמין:\n" + ex.Message);
        //    }
        //}
    }
}