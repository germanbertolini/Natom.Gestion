using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.HistoricoCambios;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HistoricoCambiosController : BaseController
    {
        public HistoricoCambiosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // GET: historicoCambios/get?entity={entity}&encrypted_id={encrypted_id}
        [HttpGet]
        [ActionName("get")]
        public async Task<IActionResult> GetAsync([FromQuery] string entity, [FromQuery] string encrypted_id)
        {
            try
            {
                var manager = new BaseManager(_serviceProvider);
                var historico = await manager.ConsultarHistoricoCambiosAsync(EncryptionService.Decrypt2<int>(encrypted_id, entity), entity);

                return Ok(new ApiResultDTO<List<HistoricoListDTO>>
                {
                    Success = true,
                    Data = historico.Select(h => new HistoricoListDTO().From(h)).ToList()
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
