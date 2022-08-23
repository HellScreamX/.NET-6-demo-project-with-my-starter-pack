
using System.Net.Mail;
using System.Text;
using Project.BusinessLogicLayer.CustomModels;
using SendGrid;
using SendGrid.Helpers.Mail;

public interface IEmailService
{
    Task SendEmailSendGridAPIAsync(string receiver, string templateId, object templateData);
}
public class EmailService : IEmailService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _apiKey;
    private readonly string _canSend;
    private readonly string _from;
    public EmailService(IWebHostEnvironment env, string apiKey, string canSend, string from)
    {
        _env = env;
        _apiKey = apiKey;
        _canSend = canSend;
        _from = from;
    }
    
    public async Task SendEmailSendGridAPIAsync(string receiver,string templateId ,object templateData)
    {
        var client = new SendGridClient(_apiKey);
        var msg = new SendGridMessage();
        msg.SetFrom(new EmailAddress(_from, "ME"));
        msg.AddTo(new EmailAddress(receiver));
        msg.SetTemplateId(templateId);
        msg.SetTemplateData(templateData);

        if(_canSend == "TRUE")
        {
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted) throw new CustomException(ErrorCode.EmailNotSent);
        }
    }
}