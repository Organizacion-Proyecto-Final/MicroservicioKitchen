using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// Se lanza cuando hay un conflicto de concurrencia (optimistic concurrency).
    /// Se traduce a HTTP 409 Conflict.

    public class ConcurrencyException : DomainException
    {
        public ConcurrencyException(string message = "El recurso fue modificado por otro usuario. Por favor, recargue e intente nuevamente.")
            : base(message, "CONCURRENCY_ERROR")
        {
        }
    }
}
