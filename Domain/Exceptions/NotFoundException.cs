using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// Se lanza cuando una entidad no es encontrada.
    /// Se traduce a HTTP 404 Not Found.
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} con id '{id}' no fue encontrado.", "NOT_FOUND")
        {
        }

        public NotFoundException(string message)
            : base(message, "NOT_FOUND")
        {
        }
    }
}
