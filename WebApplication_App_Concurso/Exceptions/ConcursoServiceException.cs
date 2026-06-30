namespace WebApplication_App_Concurso.Exceptions
{
    public class ConcursoServiceException : AppException
    {
        public ConcursoServiceException(string message)
        : base(message, 400) { }

        public ConcursoServiceException(string message, Exception innerException)
            : base(message, 400) { }
    }
}
