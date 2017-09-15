using BS.Output.Elementool.Elementool.BugTracking;
using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BS.Output.Elementool
{
  public class OutputAddIn: V3.OutputAddIn<Output>
  {

    protected override string Name
    {
      get { return "elementool"; }
    }

    protected override Image Image64
    {
      get  { return Properties.Resources.logo_64; }
    }

    protected override Image Image16
    {
      get { return Properties.Resources.logo_16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Attach screenshots to elementool issues."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {
      
      Output output = new Output(Name, 
                                 String.Empty, 
                                 String.Empty, 
                                 String.Empty, 
                                 "Screenshot",
                                 String.Empty, 
                                 true,
                                 1);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;
      
      if (edit.ShowDialog() == true) {

        return new Output(edit.OutputName,
                          edit.AccountName,
                          edit.UserName,
                          edit.Password,
                          edit.FileName,
                          edit.FileFormat,
                          edit.OpenItemInBrowser,
                          Output.LastIssueNumber);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValueCollection SerializeOutput(Output Output)
    {

      OutputValueCollection outputValues = new OutputValueCollection();

      outputValues.Add(new OutputValue("Name", Output.Name));
      outputValues.Add(new OutputValue("AccountName", Output.AccountName));
      outputValues.Add(new OutputValue("UserName", Output.UserName));
      outputValues.Add(new OutputValue("Password",Output.Password, true));
      outputValues.Add(new OutputValue("OpenItemInBrowser", Convert.ToString(Output.OpenItemInBrowser)));
      outputValues.Add(new OutputValue("FileName", Output.FileName));
      outputValues.Add(new OutputValue("FileFormat", Output.FileFormat));
      outputValues.Add(new OutputValue("LastIssueNumber", Output.LastIssueNumber.ToString()));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {

      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["AccountName", ""].Value, 
                        OutputValues["UserName", ""].Value,
                        OutputValues["Password", ""].Value, 
                        OutputValues["FileName", "Screenshot"].Value, 
                        OutputValues["FileFormat", ""].Value,
                        Convert.ToBoolean(OutputValues["OpenItemInBrowser", Convert.ToString(true)].Value),
                        Convert.ToInt32(OutputValues["LastIssueNumber", "1"].Value));

    }

    protected override async Task<V3.SendResult> Send(IWin32Window Owner, Output Output, V3.ImageData ImageData)
    {

      try
      {

        string url = "http://www.elementool.com";
        
        BugTracking elementoolClient = new BugTracking();
        elementoolClient.Url = String.Format("{0}/WebServices/BugTracking.asmx", url);
    
        string userName = Output.UserName;
        string password = Output.Password;
        bool showLogin = string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password);
        bool rememberCredentials = false;

        string fileName = V3.FileHelper.GetFileName(Output.FileName, Output.FileFormat, ImageData);

        while (true)
        {

          if (showLogin)
          {

            // Show credentials window
            Credentials credentials = new Credentials(Output.AccountName, userName, password, rememberCredentials);

            var credentialsOwnerHelper = new System.Windows.Interop.WindowInteropHelper(credentials);
            credentialsOwnerHelper.Owner = Owner.Handle;

            if (credentials.ShowDialog() != true)
            {
              return new V3.SendResult(V3.Result.Canceled);
            }

            userName = credentials.UserName;
            password = credentials.Password;
            rememberCredentials = credentials.Remember;

          }

          try
          {
          
            Microsoft.Web.Services2.Security.Tokens.UsernameToken objUsernameToken = new Microsoft.Web.Services2.Security.Tokens.UsernameToken(String.Format("{0}\\{1}", Output.AccountName, userName), password, Microsoft.Web.Services2.Security.Tokens.PasswordOption.SendHashed);
            
            elementoolClient.RequestSoapContext.Security.Tokens.Clear();
            elementoolClient.RequestSoapContext.Security.Tokens.Add(objUsernameToken);

            elementoolClient.RequestSoapContext.Security.Elements.Clear();
            elementoolClient.RequestSoapContext.Security.Elements.Add(new Microsoft.Web.Services2.Security.MessageSignature(objUsernameToken));

            await Task.Factory.StartNew(() => elementoolClient.LoginCheck());

          }
          catch
          {
            showLogin = true;
            continue;
          }


          // Show send window
          Send send = new Send(Output.AccountName, Output.LastIssueNumber, fileName);

          var sendOwnerHelper = new System.Windows.Interop.WindowInteropHelper(send);
          sendOwnerHelper.Owner = Owner.Handle;

          if (!send.ShowDialog() == true)
          {
            return new V3.SendResult(V3.Result.Canceled);
          }

          int issueNumber;
          
          if (send.CreateNewIssue)
          {
            BugTrackingIssue issue = await Task.Factory.StartNew(() => elementoolClient.GetNewIssue());
            issue.FieldsArray[0].Value = send.Comment;
            issue = await Task.Factory.StartNew(() => elementoolClient.SaveIssue(issue));
            issueNumber = issue.IssueNumber;
          }
          else
          {
            issueNumber = send.IssueNumber;
          }

          string fullFileName = String.Format("{0}.{1}", send.FileName, V3.FileHelper.GetFileExtention(Output.FileFormat));
          byte[] fileBytes = V3.FileHelper.GetFileBytes(Output.FileFormat, ImageData);
          
          AddAttachmentResult addAttachmentResult;
          using (MemoryStream fileStream = new MemoryStream(fileBytes))
          {
            Microsoft.Web.Services2.Attachments.Attachment attachment = new Microsoft.Web.Services2.Attachments.Attachment("none", fileStream);
            elementoolClient.RequestSoapContext.Attachments.Add(attachment);
            addAttachmentResult = await Task.Factory.StartNew(() => elementoolClient.AddAttachment(issueNumber, fullFileName));
          }

          if (addAttachmentResult != AddAttachmentResult.OK)
          {
            return new V3.SendResult(V3.Result.Failed, addAttachmentResult.ToString());
          }


          // Open issue in browser
          if (Output.OpenItemInBrowser)
          {
            V3.WebHelper.OpenUrl(String.Format("{0}/Services/Common/quickview.aspx?usrname={1}&accntname={2}&issueno={3}", url, userName, Output.AccountName, issueNumber));
          }


          return new V3.SendResult(V3.Result.Success,
                                    new Output(Output.Name,
                                              Output.AccountName,
                                              (rememberCredentials) ? userName : Output.UserName,
                                              (rememberCredentials) ? password : Output.Password,
                                              Output.FileName,
                                              Output.FileFormat,
                                              Output.OpenItemInBrowser,
                                              issueNumber));

        }

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }

  }
}
