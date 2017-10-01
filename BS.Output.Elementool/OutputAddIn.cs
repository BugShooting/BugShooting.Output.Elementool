using BS.Output.Elementool.Elementool.BugTracking;
using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
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

        WSHttpBinding binding = new WSHttpBinding();
        binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
        binding.MaxBufferPoolSize = 52428800;
        binding.MaxReceivedMessageSize = 2147483647;

        X509Certificate2 certificate = new X509Certificate2();
        certificate.Import(Convert.FromBase64String("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"));

        EndpointAddress endpoint = new EndpointAddress(new Uri(String.Format("{0}/ElementoolWcfService/BugTracking/", url)), EndpointIdentity.CreateX509CertificateIdentity(certificate));

        BugTrackingServiceClient elementoolClient = new BugTrackingServiceClient(binding, endpoint);

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

            elementoolClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            elementoolClient.ClientCredentials.UserName.UserName = string.Format("{0}\\{1}", Output.AccountName, userName);
            elementoolClient.ClientCredentials.UserName.Password = password;
            
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
              BugTrackingIssue issue = await elementoolClient.GetNewIssueAsync();
              issue.FieldsArray[0].Value = send.Comment;
              issue = await elementoolClient.SaveIssueAsync(issue);
              issueNumber = issue.IssueNumber;
            }
            else
            {
              issueNumber = send.IssueNumber;
            }

            string fullFileName = String.Format("{0}.{1}", send.FileName, V3.FileHelper.GetFileExtention(Output.FileFormat));
            byte[] fileBytes = V3.FileHelper.GetFileBytes(Output.FileFormat, ImageData);
          
            AddAttachmentResult addAttachmentResult = await elementoolClient.AddAttachmentAsync(issueNumber, fullFileName, fileBytes);
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
          catch (Exception ex)
          {
            showLogin = true;
            continue;
          }

        }

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }

  }
}
