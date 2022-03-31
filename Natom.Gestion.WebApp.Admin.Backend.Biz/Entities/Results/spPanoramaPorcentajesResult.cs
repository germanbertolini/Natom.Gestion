using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Results
{
    [Keyless]
    public class spPanoramaPorcentajesResult
    {
        public int Modalidad { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadPresentes { get; set; }
    }
}
