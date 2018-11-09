using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class TestCode
    {
        public MyiRentEntities db = new MyiRentEntities();

        public int runTest()
        {
            try
            {
                // Send the Property File (FTP)
                Wakefield wake = new Wakefield();
                var getLastCollection = db.collections.OrderByDescending(c => c.CollectionsID).Select(a => a.CollectionsID).First();
                string ftpPath = wake.ftp + wake.ftpFolder + "Collection" + getLastCollection.ToString() + ".csv";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                WebClient client = new WebClient();
                Byte[] fileBytes = client.DownloadData("http://myirent.com/beta/collection.csv");
                //byte[] fileBytes = Encoding.Default.GetBytes(sb.ToString());
                //Enter FTP Server credentials
                request.Credentials = new NetworkCredential(wake.UserName, wake.Password);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = true;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                // Send Email
                SendUsEmail message = new SendUsEmail();
                message.sendAlert(response.StatusCode + ": " + response.StatusDescription, "Test FTP Send to Collection");
                response.Close();
            } catch(Exception any)
            {
                Console.Write(any.ToString());
                SendUsEmail error = new SendUsEmail();
                error.sendError(any.ToString(), "Test FTP Send to Collections Error");
            }
            

            return 0;
        }
    }
}