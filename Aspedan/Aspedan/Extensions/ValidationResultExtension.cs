namespace Aspedan.Extensions;

public static class ValidationResultExtensions
{
	public static List<ValidationErrorDetail> ToErrorDetails(this ValidationResult validationResult)
	{
		return validationResult.Errors
			.Select(error => new ValidationErrorDetail (error.PropertyName, error.ErrorMessage))
			.ToList();
	}
}
