namespace WebApplication_App_Concurso.Exceptions
{
    public class ScrapingConnectionException : AppException
    {
        public ScrapingConnectionException(string message, Exception innerException)
            : base(message, 503, innerException) { }
        public ScrapingConnectionException(string message)
            : base(message, 503) { }
    }
}
