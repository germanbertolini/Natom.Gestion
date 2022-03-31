using Natom.Gestion.WebApp.Clientes.Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Services
{
    public class TransactionService
    {
        public Token Token { get; set; }
        public DateTime DateTime { get; set; }

        public TransactionService()
        {
            this.DateTime = DateTime.Now;
        }
    }
}
