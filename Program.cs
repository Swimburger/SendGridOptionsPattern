using System.Reflection;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

IConfiguration config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: false)
	.AddEnvironmentVariables()
	.AddCommandLine(args)
	.Build();

var apiKey = config["SendGrid:ApiKey"];
var fromEmailAddress = config["Email:FromEmailAddress"];
var fromName = config["Email:FromName"];
var toEmailAddress = config["Email:ToEmailAddress"];
var toName = config["Email:ToName"];
var emailSubject = config["Email:Subject"];
var emailBody = config["Email:Body"];

var client = new SendGridClient(apiKey);
var message = new SendGridMessage
{
	From = new EmailAddress(fromEmailAddress, fromName),
	Subject = emailSubject,
	PlainTextContent = emailBody
};
message.AddTo(new EmailAddress(toEmailAddress, toName));
var response = await client.SendEmailAsync(message);

// A success status code means SendGrid received the email request and will process it.
// Errors can still occur when SendGrid tries to send the email. 
// If email is not received, use this URL to debug: https://app.sendgrid.com/email_activity 
Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");