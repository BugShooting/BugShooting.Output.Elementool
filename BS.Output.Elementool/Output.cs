namespace BS.Output.Elementool
{

  public class Output: IOutput 
  {
    
    string name;
    string accountName;
    string userName;
    string password;
    string fileName;
    string fileFormat;
    bool openItemInBrowser;
    int lastIssueNumber;

    public Output(string name, 
                  string accountName, 
                  string userName,
                  string password, 
                  string fileName, 
                  string fileFormat,
                  bool openItemInBrowser,
                  int lastIssueNumber)
    {
      this.name = name;
      this.accountName = accountName;
      this.userName = userName;
      this.password = password;
      this.fileName = fileName;
      this.fileFormat = fileFormat;
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

    public string FileFormat
    {
      get { return fileFormat; }
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
