using Azure.Messaging.ServiceBus;
using EmailProvider.Models;

namespace EmailProvider.Services
{
    public interface IEmailService
    {
        bool SendEmail(EmailRequestModel emailRequest);
        EmailRequestModel UnpackEmailRequest(ServiceBusReceivedMessage message);
    }
}