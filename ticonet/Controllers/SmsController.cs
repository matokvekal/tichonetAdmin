using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace ticonet
{
    internal class SmsController : Controller
    {

        //ajax  confirmSms (sms Phonenumber)
        //sesion smsTimes=1
        //random nuber 4 digits
        //save the kod in session 1 hour
        //if(SendSms(Phonenumber, your code is random)//true is ok
        //show the user textbox for insert kod
        //give thh user button to ajax
        //build user ajax to get phone number and code
        //if phone $ kod from session ok - update db and seccess to uset(chnge the X to v)





         [Authorize]
        public bool  SendSms(string phoneNumber,string message)
        {
         

            try
            {
                string messageInterval = "5";
                string userName = "hasaot";
                string password = "tichonet1234";
                string messageText = System.Security.SecurityElement.Escape(message);
                string senderName = "tichonet";
                string senderNumber = "054-1111111";
                //set phone numbers
                //string phonesList = "0503333333;0503333334;0503333335;0503333336;0503333337";
                string phonesList = phoneNumber;
                //set additional parameters
                //string timeToSend = "04/11/2016 15:30";
                // create XML
                StringBuilder sbXml = new StringBuilder();
                sbXml.Append("<Inforu>");
                sbXml.Append("<User>");
                sbXml.Append("<Username>" + userName + "</Username>");
                sbXml.Append("<Password>" + password + "</Password>");
                sbXml.Append("</User>");
                sbXml.Append("<Content Type=\"sms\">");
                sbXml.Append("<Message>" + messageText + "</Message>");
                sbXml.Append("</Content>");
                sbXml.Append("<Recipients>");
                sbXml.Append("<PhoneNumber>" + phonesList + "</PhoneNumber>");
                sbXml.Append("</Recipients>");
                sbXml.Append("<Settings>");
                sbXml.Append("<SenderName>" + senderName + "</SenderName>");
                sbXml.Append("<SenderNumber>" + senderNumber + "</SenderNumber>");
                sbXml.Append("<MessageInterval>" + messageInterval + "</MessageInterval>");
                //sbXml.Append("<TimeToSend>" + timeToSend + "</TimeToSend>");
                //  <DelayInSeconds>60</DelayInSeconds>
                sbXml.Append("</Settings>");
                sbXml.Append("</Inforu >");
                string strXML = HttpUtility.UrlEncode(sbXml.ToString(), System.Text.Encoding.UTF8);
                Task<string> x= PostDataToURL("http://api.smsim.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);

                return true;
            }
            catch
            {
                return false;
            }
        }

       internal static async Task<string> PostDataToURL(string szUrl, string szData)
        {
            //Setup the web request
            string szResult = string.Empty;
            WebRequest Request = WebRequest.Create(szUrl);
            Request.Timeout = 30000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            //Set the POST data in a buffer
            byte[] PostBuffer;
            try
            {
                // replacing " " with "+" according to Http post RPC
                szData = szData.Replace(" ", "+");
                //Specify the length of the buffer
                PostBuffer = Encoding.UTF8.GetBytes(szData);
                Request.ContentLength = PostBuffer.Length;

                Task<string> t = new Task<string>(() =>
                    {
                        //Open up a request stream
                        Stream RequestStream = Request.GetRequestStream();
                        //Write the POST data
                        RequestStream.Write(PostBuffer, 0, PostBuffer.Length);
                        //Close the stream
                        RequestStream.Close();
                        //Create the Response object
                        WebResponse Response;
                        Response = Request.GetResponse();
                        //Create the reader for the response
                        StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                        //Read the response
                        szResult = sr.ReadToEnd();
                        //Close the reader, and response
                        sr.Close();
                        Response.Close();
                        return szResult;
                    });
                t.Start();
                string result = await t;
                return result;
              
            }
            catch (Exception e)
            {
                return szResult;
            }
        }





       //[Authorize]
       //public bool SendSms(string phoneNumber, string message)
       //{
       //    try
       //    {
       //        string messageInterval = "5";
       //        string userName = "hasaot";
       //        string password = "tichonet1234";
       //        string messageText = System.Security.SecurityElement.Escape(message);
       //        string senderName = "tichonet";
       //        string senderNumber = "054-1111111";
       //        //set phone numbers
       //        //string phonesList = "0503333333;0503333334;0503333335;0503333336;0503333337";
       //        string phonesList = phoneNumber;
       //        //set additional parameters
       //        //string timeToSend = "04/11/2016 15:30";
       //        // create XML
       //        StringBuilder sbXml = new StringBuilder();
       //        sbXml.Append("<Inforu>");
       //        sbXml.Append("<User>");
       //        sbXml.Append("<Username>" + userName + "</Username>");
       //        sbXml.Append("<Password>" + password + "</Password>");
       //        sbXml.Append("</User>");
       //        sbXml.Append("<Content Type=\"sms\">");
       //        sbXml.Append("<Message>" + messageText + "</Message>");
       //        sbXml.Append("</Content>");
       //        sbXml.Append("<Recipients>");
       //        sbXml.Append("<PhoneNumber>" + phonesList + "</PhoneNumber>");
       //        sbXml.Append("</Recipients>");
       //        sbXml.Append("<Settings>");
       //        sbXml.Append("<SenderName>" + senderName + "</SenderName>");
       //        sbXml.Append("<SenderNumber>" + senderNumber + "</SenderNumber>");
       //        sbXml.Append("<MessageInterval>" + messageInterval + "</MessageInterval>");
       //        //sbXml.Append("<TimeToSend>" + timeToSend + "</TimeToSend>");
       //        //  <DelayInSeconds>60</DelayInSeconds>
       //        sbXml.Append("</Settings>");
       //        sbXml.Append("</Inforu >");
       //        string strXML = HttpUtility.UrlEncode(sbXml.ToString(), System.Text.Encoding.UTF8);
       //        string result = PostDataToURL("http://api.smsim.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);
       //        return true;
       //    }
       //    catch
       //    {
       //        return false;
       //    }
       //}

       //internal static string PostDataToURL(string szUrl, string szData)
       //{
       //    //Setup the web request
       //    string szResult = string.Empty;
       //    WebRequest Request = WebRequest.Create(szUrl);
       //    Request.Timeout = 30000;
       //    Request.Method = "POST";
       //    Request.ContentType = "application/x-www-form-urlencoded";
       //    //Set the POST data in a buffer
       //    byte[] PostBuffer;
       //    try
       //    {
       //        // replacing " " with "+" according to Http post RPC
       //        szData = szData.Replace(" ", "+");
       //        //Specify the length of the buffer
       //        PostBuffer = Encoding.UTF8.GetBytes(szData);
       //        Request.ContentLength = PostBuffer.Length;
       //        //Open up a request stream
       //        Stream RequestStream = Request.GetRequestStream();
       //        //Write the POST data
       //        RequestStream.Write(PostBuffer, 0, PostBuffer.Length);
       //        //Close the stream
       //        RequestStream.Close();
       //        //Create the Response object
       //        WebResponse Response;
       //        Response = Request.GetResponse();
       //        //Create the reader for the response
       //        StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
       //        //Read the response
       //        szResult = sr.ReadToEnd();
       //        //Close the reader, and response
       //        sr.Close();
       //        Response.Close();
       //        return szResult;
       //    }
       //    catch (Exception e)
       //    {
       //        return szResult;
       //    }
       //}




    }
}

