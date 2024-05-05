using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Helpers;
using EmailProvider.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EmailProvider.Services;

public class EmailService(ILogger<EmailService> logger, EmailClient emailClient) : IEmailService
{
    private readonly ILogger<EmailService> _logger = logger;
    private readonly EmailClient _emailClient = emailClient;

    public EmailRequestModel UnpackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var emailRequest = JsonConvert.DeserializeObject<EmailRequestModel>(Encoding.UTF8.GetString(message.Body));
            if (emailRequest != null)
            {
                var validation = CustomValidation.ValidateEmailRequest(emailRequest);
                if (!validation.IsValid)
                {
                    foreach (var error in validation.ValidationResults)
                    {
                        _logger.LogError($"Validation Error :: {error.ErrorMessage}");
                    }
                    return null!;
                }
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailService.UnpackEmailRequest() :: {ex.Message}");
        }
        return null!;
    }

    public bool SendEmail(EmailRequestModel emailRequest)
    {
        try
        {
            var result = _emailClient.Send(WaitUntil.Completed,
                senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
                recipientAddress: emailRequest.To,
                subject: emailRequest.Subject,
                htmlContent: emailRequest.HtmlBody,
                plainTextContent: emailRequest.PlainText);

            if (result.HasCompleted)
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailService.SendEmail() :: {ex.Message}");
        }
        return false;
    }
}