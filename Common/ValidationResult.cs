namespace Shahbazi.Store.Common;

public class ValidationResult
{
    public class ValidationError
    {
        public string PropertyName { get; }
        public string ErrorMessage { get; }

        public ValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }

    public bool IsValid => Errors.Length == 0;
    public ValidationError[] Errors { get; }

    public ValidationResult(ValidationError[] error)
    {
        Errors = error;
    }
}