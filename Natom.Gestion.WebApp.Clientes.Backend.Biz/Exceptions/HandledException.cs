using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions
{
    public class HandledException : Exception
    {
        public HandledException(string error) : base(error)
        { }
    }
}
