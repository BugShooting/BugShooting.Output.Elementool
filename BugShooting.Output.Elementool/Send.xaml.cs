using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BugShooting.Output.Elementool
{
  partial class Send : Window
  {
 
    public Send(string accountName, int lastIssueNumber, string fileName)
    {
      InitializeComponent();

      AccountName.Text = accountName;
      NewIssue.IsChecked = true;
      IssueNumberTextBox.Text = lastIssueNumber.ToString();
      FileNameTextBox.Text = fileName;

      CommentTextBox.TextChanged += ValidateData;
      IssueNumberTextBox.TextChanged += ValidateData;
      FileNameTextBox.TextChanged += ValidateData;
      ValidateData(null, null);

    }

    public bool CreateNewIssue
    {
      get { return NewIssue.IsChecked.Value; }
    }
 
    public string Comment
    {
      get { return CommentTextBox.Text; }
    }

    public int IssueNumber
    {
      get { return Convert.ToInt32(IssueNumberTextBox.Text); }
    }

    public string FileName
    {
      get { return FileNameTextBox.Text; }
    }

    private void NewIssue_CheckedChanged(object sender, EventArgs e)
    {

      if (NewIssue.IsChecked.Value)
      {
        CommentControls.Visibility = Visibility.Visible;
        IssueNumberControls.Visibility = Visibility.Collapsed;

        CommentTextBox.SelectAll();
        CommentTextBox.Focus();
      }
      else
      {
        CommentControls.Visibility = Visibility.Collapsed;
        IssueNumberControls.Visibility = Visibility.Visible;

        IssueNumberTextBox.SelectAll();
        IssueNumberTextBox.Focus();
      }

      ValidateData(null, null);

    }

    private void IssueNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
    }
    
    private void ValidateData(object sender, EventArgs e)
    {
      OK.IsEnabled = ((CreateNewIssue && Validation.IsValid(CommentTextBox)) ||
                      (!CreateNewIssue && Validation.IsValid(IssueNumberTextBox))) &&
                     Validation.IsValid(FileNameTextBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }

  internal class ProjectItem
  {
    
    private string projectID;
    private string fullName;

    public ProjectItem(string projectID, string fullName)
    {
      this.projectID = projectID;
      this.fullName = fullName;
    }

    public string ProjectID
    {
      get { return projectID; }
    }

    public override string ToString()
    {
      return fullName;
    }

  }

}
