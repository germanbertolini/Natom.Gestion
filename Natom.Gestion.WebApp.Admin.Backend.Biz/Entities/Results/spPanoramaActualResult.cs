using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Results
{
    [Keyless]
    public class spPanoramaActualResult
    {
        public int CantidadTotal { get; set; }
        public int CantidadPresentes { get; set; }
        public int CantidadAusentes { get; set; }
        public int PorcentajeAsistencia { get; set; }
    }
}
