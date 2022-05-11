using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSendGrid(options =>
            options.ApiKey = context.Configuration["SendGrid:ApiKey"]
        );
        services.Configure<EmailOptions>(context.Configuration.GetSection("Email"));
        services.AddTransient<EmailSender>();
    })
    .Build();

var emailSender = host.Services.GetRequiredService<EmailSender>();
await emailSender.SendEmail();

public class EmailSender
{
    private readonly ISendGridClient sendGridClient;
    private readonly EmailOptions emailOptions;

    public EmailSender(IOptions<EmailOptions> emailOptions, ISendGridClient sendGridClient)
    {
        this.sendGridClient = sendGridClient;
        this.emailOptions = emailOptions.Value;
    }

    public async Task SendEmail()
    {
        var message = new SendGridMessage
        {
            From = emailOptions.From,
            Subject = emailOptions.Subject,
            PlainTextContent = emailOptions.Body
        };
        message.AddTo(emailOptions.To);
        var response = await sendGridClient.SendEmailAsync(message);

        // A success status code means SendGrid received the email request and will process it.
        // Errors can still occur when SendGrid tries to send the email. 
        // If email is not received, use this URL to debug: https://app.sendgrid.com/email_activity 
        Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");
    }
}

public class EmailOptions
{
    public EmailAddress From { get; set; }
    public EmailAddress To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}