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

var emailOptions = config.GetSection("Email").Get<EmailOptions>();

var client = new SendGridClient(apiKey);
var message = new SendGridMessage
{
    From = emailOptions.From,
    Subject = emailOptions.Subject,
    PlainTextContent = emailOptions.Body
};
message.AddTo(emailOptions.To);
var response = await client.SendEmailAsync(message);

// A success status code means SendGrid received the email request and will process it.
// Errors can still occur when SendGrid tries to send the email. 
// If email is not received, use this URL to debug: https://app.sendgrid.com/email_activity 
Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

public class EmailOptions
{
    public EmailAddress From { get; set; }
    public EmailAddress To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}