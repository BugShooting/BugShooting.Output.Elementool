using BS.Plugin.V3.Output;
using System;

namespace BugShooting.Output.Elementool
{

  public class Output: IOutput 
  {
    
    string name;
    string accountName;
    string userName;
    string password;
    string fileName;
    Guid fileFormatID;
    bool openItemInBrowser;
    int lastIssueNumber;

    public Output(string name, 
                  string accountName, 
                  string userName,
                  string password, 
                  string fileName,
                  Guid fileFormatID,
                  bool openItemInBrowser,
                  int lastIssueNumber)
    {
      this.name = name;
      this.accountName = accountName;
      this.userName = userName;
      this.password = password;
      this.fileName = fileName;
      this.fileFormatID = fileFormatID;
      this.openItemInBrowser = openItemInBrowser;
      this.lastIssueNumber = lastIssueNumber;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Information
    {
      get { return accountName; }
    }

    public string AccountName
    {
      get { return accountName; }
    }
       
    public string UserName
    {
      get { return userName; }
    }

    public string Password
    {
      get { return password; }
    }
          
    public string FileName
    {
      get { return fileName; }
    }

    public Guid FileFormatID
    {
      get { return fileFormatID; }
    }

    public bool OpenItemInBrowser
    {
      get { return openItemInBrowser; }
    }
    
    public int LastIssueNumber
    {
      get { return lastIssueNumber; }
    }

  }
}
