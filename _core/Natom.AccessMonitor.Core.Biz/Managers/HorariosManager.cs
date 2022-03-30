using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.Core.Biz.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class HorariosManager : BaseManager
    {
        public HorariosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCountAsync(int placeId)
                    => _db.ConfigTolerancias
                            .Where(t => t.PlaceId == placeId)
                            .CountAsync();

        public async Task<List<ConfigTolerancia>> ObtenerDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, int placeId)
        {
            var queryable = _db.ConfigTolerancias
                                .Where(u => u.PlaceId == placeId);

            //FILTROS
            //if (!string.IsNullOrEmpty(filter))
            //{
            //    queryable = queryable.Where(p => p.Name.ToLower().Contains(filter.ToLower())
            //                                        || p.Address.ToLower().Contains(filter.ToLower())
            //                                        || p.Place.Name.ToLower().Contains(filter.ToLower()));
            //}


            //ORDEN
            var maxDT = DateTime.MaxValue;
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? (c.AplicaHasta ?? maxDT) :
                                                                    sortColumnIndex == 1 ? c.ConfiguroFechaHora :
                                                            maxDT)
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? (c.AplicaHasta ?? maxDT) :
                                                                    sortColumnIndex == 1 ? c.ConfiguroFechaHora :
                                                            maxDT);

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = await queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();

            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public spPanoramaActualResult GetPanoramaActual(int clienteId, int? placeId)
                            => _db.spPanoramaActualResult
                                    .FromSqlRaw("sp_panorama_actual {0}, {1}", clienteId, placeId)
                                    .AsEnumerable()
                                    .First();

        public List<spPanoramaPorcentajesResult> GetPanoramaPorcentajes(int clienteId, int? placeId)
                            => _db.spPanoramaPorcentajesResult
                                    .FromSqlRaw("sp_panorama_porcentajes {0}, {1}", clienteId, placeId)
                                    .AsEnumerable()
                                    .ToList();

        public async Task<ConfigTolerancia> GuardarAsync(int clienteId, int usuarioId, ConfigTolerancia configDto)
        {
            ConfigTolerancia config = null;
            if (configDto.ConfigToleranciaId == 0) //NUEVO
            {
                var vigente = await _db.ConfigTolerancias.FirstOrDefaultAsync(m => m.PlaceId == configDto.PlaceId && !m.AplicaHasta.HasValue);

                if (configDto.AlmuerzoHorarioDesde > configDto.AlmuerzoHorarioHasta)
                    throw new HandledException($"Almuerzo: El 'Rango horario DESDE' debe ser menor al 'Rango horario HASTA'.");

                if (vigente != null && vigente.AplicaDesde.Date > configDto.AplicaDesde.Date)
                    throw new HandledException($"El 'Aplica Desde' debe ser igual o superior a {vigente.AplicaDesde.ToString("dd/MM/yyyy")} ya que existe una configuración de horarios que aplica desde dicha fecha.");

                if (vigente != null && configDto.AplicaDesde.Date <= DateTime.Now.Date)
                    throw new HandledException($"El 'Aplica Desde' debe ser igual o superior a mañana {DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy")}.");


                config = new ConfigTolerancia()
                {
                    PlaceId = configDto.PlaceId,
                    ClienteId = clienteId,
                    IngresoToleranciaMins = configDto.IngresoToleranciaMins,
                    EgresoToleranciaMins = configDto.EgresoToleranciaMins,
                    AlmuerzoHorarioDesde = configDto.AlmuerzoHorarioDesde,
                    AlmuerzoHorarioHasta = configDto.AlmuerzoHorarioHasta,
                    AlmuerzoTiempoLimiteMins = configDto.AlmuerzoTiempoLimiteMins,
                    ConfiguroUsuarioId = usuarioId,
                    ConfiguroFechaHora = DateTime.Now,
                    AplicaDesde = configDto.AplicaDesde,
                    AplicaHasta = null
                };
                _db.ConfigTolerancias.Add(config);

                if (vigente != null)
                {
                    _db.Entry(vigente).State = EntityState.Modified;
                    vigente.AplicaHasta = config.AplicaDesde.AddDays(-1);
                }

                await _db.SaveChangesAsync();
            }

            return config;
        }

        public Task<ConfigTolerancia> ObtenerAsync(int configId)
                        => _db.ConfigTolerancias
                                .FirstAsync(u => u.ConfigToleranciaId.Equals(configId));

        public async Task<ConfigTolerancia> ObtenerVigenteAsync(int clientId, int placeId)
        {
            var config = await _db.ConfigTolerancias
                                .FirstOrDefaultAsync(u => !u.AplicaHasta.HasValue && u.PlaceId == placeId);

            if (config == null)
                config = await _db.ConfigTolerancias
                                .FirstOrDefaultAsync(u => !u.AplicaHasta.HasValue && u.Place.ClientId == clientId);

            return config;
        }
    }
}
