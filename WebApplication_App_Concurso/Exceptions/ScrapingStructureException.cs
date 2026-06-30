namespace WebApplication_App_Concurso.Exceptions
{
    public class ScrapingStructureException : ScrapingException
    {
        public ScrapingStructureException(string message)
            : base(message, 500) { }

        public ScrapingStructureException(string message, Exception innerException)
            : base(message, 500, innerException) { }
    }
}
