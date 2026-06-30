namespace WebApplication_App_Concurso.Exceptions
{
    public class RepositoryException : AppException
    {
        public RepositoryException(string message)
        : base(message, 500) { }

        public RepositoryException(string message, Exception innerException)
        : base(message, 500, innerException) { }
    }
}
