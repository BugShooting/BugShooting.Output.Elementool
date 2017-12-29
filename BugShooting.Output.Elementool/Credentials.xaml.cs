using System.Windows;
using System.Windows.Controls;

namespace BugShooting.Output.Elementool
{
  partial class Credentials : Window
  {

    public Credentials(string accountName, string userName, string password, bool remember)
    {
      InitializeComponent();

      AccountName.Text = accountName;
      UserNameTextBox.Text = userName;
      PasswordBox.Password = password;
      RememberCheckBox.IsChecked = remember;

    }
    
    public string UserName
    {
      get { return UserNameTextBox.Text; }
    }
   
    public string Password
    {
      get { return PasswordBox.Password; }
    }

    public bool Remember
    {
      get { return RememberCheckBox.IsChecked.Value; }
    }
  
    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }
}
