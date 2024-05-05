using EmailProvider.Models;
using System.ComponentModel.DataAnnotations;

namespace EmailProvider.Helpers;

public static class CustomValidation
{
    public static ValidationModel<EmailRequestModel> ValidateEmailRequest(EmailRequestModel emailRequest)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(emailRequest);
        var isValid = Validator.TryValidateObject(emailRequest, context, validationResults, true);

        return new ValidationModel<EmailRequestModel>
        {
            IsValid = isValid,
            Value = emailRequest,
            ValidationResults = validationResults
        };
    }
}
