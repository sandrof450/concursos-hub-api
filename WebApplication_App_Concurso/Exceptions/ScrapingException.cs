namespace WebApplication_App_Concurso.Exceptions
{
    public class ScrapingException: AppException
    {
        public ScrapingException(string message, int statusCode, Exception innerException)
        : base(message, 400) { }

        public ScrapingException(string message, Exception innerException)
            : base(message, 400, innerException) { }

        public ScrapingException(string message, int statusCode) 
            : base(message, statusCode) {}
        public ScrapingException(string message)
            : base(message, 400) { }
    }
}
