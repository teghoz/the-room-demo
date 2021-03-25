using System;

namespace ServiceListAPI.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class ServiceListDomainException : Exception
    {
        public ServiceListDomainException()
        { }

        public ServiceListDomainException(string message)
            : base(message)
        { }

        public ServiceListDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
