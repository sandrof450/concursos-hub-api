namespace WebApplication_App_Concurso.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected AppException(string message, int statusCode, Exception innerException)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
