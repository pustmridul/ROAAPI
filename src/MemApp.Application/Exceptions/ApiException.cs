using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;

namespace MemApp.Application.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
            : base("Login failed! Invalid username or password.")
        {
        }
    }
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }
   
    public class UnauthorizeOperationException : Exception
    {
        public UnauthorizeOperationException(int permissionNo)
            : base($"UnauthorizeOperationException  {permissionNo} was not found.")
        {
        }
    }
    public class DataValidationException : Exception
    {
        public DataValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

       

        public IDictionary<string, string[]> Failures { get; }
    }
}