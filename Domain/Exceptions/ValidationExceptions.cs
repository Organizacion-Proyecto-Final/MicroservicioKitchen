using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// Se lanza cuando fallan validaciones de negocio.
    /// Se traduce a HTTP 400 Bad Request.
  
    public class ValidationExceptions : DomainException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationExceptions(string message)
            : base(message, "VALIDATION_ERROR")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationExceptions(Dictionary<string, string[]> errors)
            : base("Se encontraron errores de validación.", "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }
}
