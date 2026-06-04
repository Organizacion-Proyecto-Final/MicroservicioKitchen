using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// Se lanza cuando hay un conflicto de estado (ej: intentar cancelar una orden ya entregada).
    /// Se traduce a HTTP 409 Conflict.
    public class ConflictException : DomainException
    {
        public ConflictException(string message)
            : base(message, "CONFLICT")
        {
        }
    }
}
