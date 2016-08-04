using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;



namespace ticonet
{
    public class PaymentController : Controller
    {
        [Authorize]
        public void regPay(string studentId, string familyId)
        {
            var result = new PaymentController().getPayment(studentId, familyId);

            //  queryMPITransaction(transaction_id);
        }
        public string getPayment(string studentId, string FamilyId)
        {
            return queryMPITransaction("183828b3-e573-478f-a311-e4a88d5b89c2");
        }

        private String queryMPITransaction(string transaction_id)
        {
            //אם אתם פונים ב webservice , יש לפנות לכתובת:  https://cguat2.creditguard.co.il/xpo/services/Relay

            //שם משתמש:           israeli
            //סיסמא:                    I!fr43s!34
            //מיד (mid):              938
            //מספר מסוף:           0962832


            string terminal_id = "0962922";
            string merchant_id = "1";
            string user = "israel";
            string password = "I!fr43s!34";
            string cg_gateway_url = "https://cguat2.creditguard.co.il/xpo/Relay";

            Random rand = new Random();
            string uniqueID = DateTime.Now.ToString("yyyyddMM") + rand.Next(0, 1000);
            String result = "";
            String poststring = "user=" + user +
                                  "&password=" + password +
                                  "&int_in=<ashrait>" +
                                  "<request>" +
                                   "<language>HEB</language>" +
                                   "<command>inquireTransactions</command>" +
                                   "<inquireTransactions>" +
                                    "<terminalNumber>" + terminal_id + "</terminalNumber>" +
                                    "<mainTerminalNumber/>" +
                                    "<queryName>mpiTransaction</queryName>" +
                                    "<mid>" + merchant_id + "</mid>" +
                                    "<mpiTransactionId>" + transaction_id + "</mpiTransactionId>" +
                                    "<userData1/>" +
                                    "<userData2/>" +
                                    "<userData3/>" +
                                    "<userData4/>" +
                                    "<userData5/>" +
                                   "</inquireTransactions>" +
                                  "</request>" +
                                 "</ashrait>";


            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(cg_gateway_url);
            objRequest.Method = "POST";
            objRequest.ContentLength = poststring.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(poststring);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();

                // Close and clean up the StreamReader
                sr.Close();
            }


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlNodeList Nodes = doc.GetElementsByTagName("statusText");
            string response = Nodes[0].InnerText;

            return response;
        }



        [Authorize]
        public bool SendSmsTest(string phoneNumber, string message)
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
                Task<string> x = PostDataToURLTest("http://api.smsim.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static async Task<string> PostDataToURLTest(string szUrl, string szData)
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










    }
}

