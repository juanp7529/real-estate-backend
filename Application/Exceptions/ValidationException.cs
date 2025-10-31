namespace real_estate_api.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
  : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
    : base($"Validation error in field '{field}'.")
        {
            Errors = new Dictionary<string, string[]>
    {
    { field, new[] { error } }
   };
        }
    }
}
