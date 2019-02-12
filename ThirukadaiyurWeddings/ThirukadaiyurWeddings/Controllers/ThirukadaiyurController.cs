using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ThirukadaiyurWeddings.Models;
using System.Net;

namespace ThirukadaiyurWeddings.Controllers
{
    public class ThirukadaiyurController : Controller 
    {
        public ActionResult Thirukadaiyur_Temple()
        {
            ViewBag.title = "Thirukadaiyur Temple Marriages,Homams Booking | Call: +916379158216 | Abirami Arrangements";
            return View();
        }

        public ActionResult Thirukadaiyur_Temple_Details_Cost()
        {
            ViewBag.Title = "Thirukadaiyur Temple Marriages,Poojas Cost | Booking Call: +916379158216"; 

            return View();
        }

        public ActionResult Hotels_Rooms_Food_in_Thirukadaiyur()
        {
            ViewBag.Title = "Hotels,Rooms,Cottages in Thirukadaiyur | Call: +916379158216 | Abirami Arrangements";

            return View();
        }

        public ActionResult Thirukadaiyur_Temple_Contact_Details()
        {
            ViewBag.Title = "Thirukadaiyur Temple Booking Contact Details | Booking Call: +916379158216 | Abirami Arrangements"; 

            return View();
        }

        public ActionResult Thank_You()
        {
            return View();
        }

        #region Post Methods for Bookings
        [HttpGet]
        public ActionResult Thirukadaiyur_Temple_Booking()
        {
            ViewBag.Title = "Test heading test test-booking";

            return View();
        }

        [HttpPost]
        public ActionResult Thirukadaiyur_Temple_Booking(string Name, string mob, string altmob, DateTime date, string abishegam, string abishegamdays, string location, string brahmin)
        {
            try
            {
                string body = (" Name:" + Name + "\n" +
                            " MobileNumber: " + mob + "\n" +
                            " AlterneteMobile: " + altmob + "\n" +
                            " FunctionDate :" + date + "\n" +
                            " Location :" + location + "\n" +
                            " TypeofAbishegam : " + abishegam + "\n" +
                            " AbishegamDays :" + abishegamdays + "\n" +
                            " Bhramin : " + brahmin + "\n");

                SendEmail(body, "New_Booking");

                ViewBag.Title = "Test heading test test-booking";

                if (!string.IsNullOrEmpty(Name))
                {
                    TempData["customer"] = Name;
                }
                
                if (brahmin == "Yes")
                {
                    TempData["fee"] = "5000";
                }
                else
                {
                    TempData["fee"] = "2000";
                }
                return RedirectToAction("Payment");
            }
            catch (Exception e)
            {
                throw (e);
            }

        }

        [HttpPost]
        public ActionResult Thirukadaiyur_Hotels_Booking(string Name, DateTime chkindate, DateTime chkoutdate, string mobile, int Personcount, string address, string preference)
        {
            try
            {
                string body = (" Name:" + Name + "\n" +
                            " MobileNumber: " + mobile + "\n" +
                            " Checkindate: " + chkindate + "\n" +
                            " Checkoutdate :" + chkoutdate + "\n" +
                            " Personcount :" + Personcount + "\n" +
                            " Address : " + address + "\n" +
                            " preference :" + preference + "\n");

                SendEmail(body, "Hotel_Booking");
                if (!string.IsNullOrEmpty(Name))
                {
                    TempData["customer"] = Name;
                }
                TempData["fee"] = "1000";
              
                return RedirectToAction("Payment");
            }
            catch (Exception e)
            {
                throw (e);
            }


        }

        [HttpPost]
        public ActionResult Thirukadaiyur_Food_Booking(string Name,string mobilemno, int BFCount, DateTime Breakefastdate,int LCount, DateTime Lunchdate,int  DCount, DateTime Dinnerdate)
        {
            try
            {
                string body = (" Name:" + Name + "\n" +
                            " MobileNumber: " + mobilemno + "\n" +
                            " BreakFast: " + Breakefastdate + "Count:" + BFCount + "\n" +
                            " Lunch: " + Lunchdate + "Count:" + LCount + "\n" +
                            " Dinner: " + Dinnerdate + "Count:" + DCount);                           

                SendEmail(body, "Food_Booking");
                if (!string.IsNullOrEmpty(Name))
                {
                    TempData["customer"] = Name;
                }
                TempData["fee"] = "1000";

                return RedirectToAction("Payment");
            }
            catch (Exception e)
            {
                throw (e);
            }


        }

        [HttpPost]
        public ActionResult Thirukadaiyur_Temple_Contact_Details_Enquiry(string Name, string mobile, string address)
        {
            ViewBag.Title = "Test heading test test-contact";
            try
            {
                string body = (" Name:" + Name + "\n" +
                            " MobileNumber: " + mobile + "\n" +
                            " Address: " + address);

                SendEmail(body, "New_Enquiry");

                return RedirectToAction("Thank_You");
            }
            catch (Exception e)
            {
                throw (e);
            }

        }

        #endregion     

        #region Payment Methods
        public ActionResult Payment()
        {
            string Customer = "NewCustomer";
            string Advance = "2000";

            if (TempData.ContainsKey("customer"))
                Customer = TempData["customer"].ToString();

            if (TempData.ContainsKey("fee"))
                Advance = TempData["fee"].ToString();

            FormtoRequest model = new FormtoRequest();
            model.appId = ConfigurationManager.AppSettings["AppId"];
            model.orderId = DateTime.Now.Ticks.ToString();
            model.orderNote = "Abirami Arrangements Payment";
            model.orderCurrency = "INR";
            model.customerName = Customer;
            model.customerEmail = "abirami.arrangements@gmail.com";
            model.customerPhone = "9585831457";
            model.orderAmount = Advance;
            model.notifyUrl = ConfigurationManager.AppSettings["notifyUrl"].ToString();
            model.returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();

            string secretKey = ConfigurationManager.AppSettings["SecretKey"];
            string signatureData = "";
            PropertyInfo[] keys = model.GetType().GetProperties();
            keys = keys.OrderBy(key => key.Name).ToArray();

            foreach (PropertyInfo key in keys)
            {
                signatureData += key.Name + key.GetValue(model);
            }
            var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
            byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
            string signature = Convert.ToBase64String(gensignature);
            ViewData["signature"] = signature;
            string mode = ConfigurationManager.AppSettings["PaymentMode"];
            if (mode == "PROD")
            {
                ViewData["url"] = "https://www.cashfree.com/checkout/post/submit";
            }
            else
            {
                ViewData["url"] = "https://test.cashfree.com/billpay/checkout/post/submit";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult HandleRequest(FormtoRequest model)
        {
            string secretKey = ConfigurationManager.AppSettings["SecretKey"];
            string signatureData = "";
            PropertyInfo[] keys = model.GetType().GetProperties();
            keys = keys.OrderBy(key => key.Name).ToArray();

            foreach (PropertyInfo key in keys)
            {
                signatureData += key.Name + key.GetValue(model);
            }
            var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
            byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
            string signature = Convert.ToBase64String(gensignature);
            ViewData["signature"] = signature;

            string mode = ConfigurationManager.AppSettings["PaymentMode"];

            if (mode == "PROD")
            {
                ViewData["url"] = "https://www.cashfree.com/checkout/post/submit";
            }
            else
            {
                ViewData["url"] = "https://test.cashfree.com/billpay/checkout/post/submit";
            }
            return View(model);
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }
        #endregion 

        public void SendEmail(string body, string subject)
        {

            var fromAddress = new MailAddress("bala1991.phoenix@gmail.com", "New_Booking" + DateTime.Now);
            var toAddress = new MailAddress("abirami.arangements@gmail.com", "Abirami_Arrangements");
            const string fromPassword = "X3haquc6";

            if (ConfigurationManager.AppSettings["IsLive"] == "true")
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}