using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using AarhusWebDevCoop4.ViewModels;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace AarhusWebDevCoop4.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            {
                // sender mail
                MailMessage message = new MailMessage();
                message.To.Add("vickibuhl86@gmail.com");
                message.Subject = model.Subject;
                message.From = new MailAddress(model.Email, model.Name);
                message.Body = model.Message + "\n my email is: " + model.Email;

                // mailopsætning
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network; smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com"; smtp.Port = 587;
                    //for authentication to work, add web/app in google account
                    smtp.Credentials = new System.Net.NetworkCredential("vickibuhl86@gmail.com", "vtuucuephckczoav");
                    smtp.EnableSsl = true;

                    smtp.Send(message);
                }

                TempData["success"] = true;

                IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "Comment");

                comment.SetValue("name", model.Name);
                comment.SetValue("email", model.Email);
                comment.SetValue("subject", model.Subject);
                comment.SetValue("message", model.Message);

                // Save
                Services.ContentService.Save(comment);

                //Save and publish
                //Services.ContentService.SaveAndPublishWithStatus(comment);

                return RedirectToCurrentUmbracoPage();
            }

            

            

        }
    }
}