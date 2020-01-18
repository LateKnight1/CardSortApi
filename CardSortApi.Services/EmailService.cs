using System;
using CardSortApi.Repositories;
using CardSortApi.Services.Interfaces;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using CardSortApi.Domain.Models;

namespace CardSortApi.Services
{
	public class EmailService : IEmailService
	{
		public void SendWelcomeEmail(User user)
		{
			var emailServer = new SmtpClient("mail.cardsortapp.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("no-reply@cardsortapp.com", "54mMyl0v3!")
			};
			var message = new MailMessage {From = new MailAddress("no-reply@cardsortapp.com")};

			message.To.Add(user.Email);
			message.Subject = "Welcome to CardSort";
			var body = "";
			using (var reader = new StreamReader(Path.Combine(HttpContext.Current.Server.MapPath("~/EmailTemplates"), "welcomeTemplate.html")))
			{
				body = reader.ReadToEnd();
			}

			body = body.Replace("{username}", user.Username);
			body = body.Replace("{verifToken}", user.Verification_Token);

			message.IsBodyHtml = true;
			message.Body = body;

			emailServer.Send(message);
		}

		public RequestResponse<bool> ResendWelcomeEmail(User user)
		{
			try
			{
				SendWelcomeEmail(user);
				return new RequestResponse<bool>
				{
					Succeeded = true,
					ResponseBody = true,
				};
			}
			catch (Exception e)
			{
				return new RequestResponse<bool>
				{
					Succeeded = false,
					ErrorMessage = e.Message
				};
			}
		}
	}
}