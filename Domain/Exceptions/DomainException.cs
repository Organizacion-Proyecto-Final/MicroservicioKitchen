using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// Excepción base para errores de dominio (reglas de negocio violadas).
    /// Se traduce a HTTP 400 Bad Request.
    public class DomainException : Exception
    {
        public string ErrorCode { get; }

        public DomainException(string message, string errorCode = "DOMAIN_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
