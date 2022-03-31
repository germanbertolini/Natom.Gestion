using Microsoft.AspNetCore.Mvc;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Autocomplete;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Clientes;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Clientes.CtaCte;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Zonas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ClientesController : BaseController
    {
        public ClientesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: clientes/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerClientesCountAsync();
                var usuarios = await manager.ObtenerClientesDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<ClienteDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<ClienteDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new ClienteDTO().From(usuario)).ToList()
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: clientes/basics/data
        // GET: clientes/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                ClienteDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var cliente = await manager.ObtenerClienteAsync(clienteId);
                    entity = new ClienteDTO().From(cliente);
                }

                var zonasManager = new ZonasManager(_serviceProvider);
                var zonas = await zonasManager.ObtenerZonasActivasAsync();

                var preciosManager = new PreciosManager(_serviceProvider);
                var listasDePrecios = await preciosManager.ObtenerListasDePreciosAsync();
                
                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        zonas = zonas.Select(zona => new ZonaDTO().From(zona)),
                        listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista))
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: clientes/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] ClienteDTO clienteDto)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var cliente = await manager.GuardarClienteAsync(clienteDto);

                await RegistrarAccionAsync(cliente.ClienteId, nameof(Cliente), string.IsNullOrEmpty(clienteDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<ClienteDTO>
                {
                    Success = true,
                    Data = new ClienteDTO().From(cliente)
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: clientes/search?filter={filter}
        [HttpGet]
        [ActionName("search")]
        public async Task<IActionResult> GetSearchAsync([FromQuery] string filter = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                var clientes = await manager.BuscarClientesAsync(size: 20, filter);

                return Ok(new ApiResultDTO<AutocompleteResponseDTO<ClienteDTO>>
                {
                    Success = true,
                    Data = new AutocompleteResponseDTO<ClienteDTO>
                    {
                        Total = clientes.Count,
                        Results = clientes.Select(cliente => new ClienteDTO().From(cliente)).ToList()
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // DELETE: clientes/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                await manager.DesactivarClienteAsync(clienteId);

                await RegistrarAccionAsync(clienteId, nameof(Cliente), "Baja");

                return Ok(new ApiResultDTO
                {
                    Success = true
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: clientes/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                await manager.ActivarClienteAsync(clienteId);

                await RegistrarAccionAsync(clienteId, nameof(Cliente), "Alta");

                return Ok(new ApiResultDTO
                {
                    Success = true
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: clientes/cta_cte/list?encryptedClienteId={encryptedClienteId}&filterDate={filterDate}
        [HttpPost]
        [ActionName("cta_cte/list")]
        public async Task<IActionResult> PostMovimientosCtaCteListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedClienteId, [FromQuery] string filterDate = null)
        {
            try
            {
                int clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedClienteId));

                DateTime dt;
                DateTime? dtFilter = null;
                if (DateTime.TryParse(filterDate, out dt))
                    dtFilter = dt;

                var manager = new CuentasCorrientesManager(_serviceProvider);
                var movimientosCount = await manager.ObtenerMovimientosCtaCteClienteCountAsync(clienteId);
                var movimientos = await manager.ObtenerMovimientosCtaCteClienteDataTableAsync(clienteId, request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, dtFilter);
                var disponible = await manager.ObtenerDisponibleActualCtaCteClienteAsync(clienteId);
                var monto = await manager.ObtenerMontoActualCtaCteClienteAsync(clienteId);

                return Ok(new ApiResultDTO<DataTableResponseDTO<ClienteCtaCteMovimientoDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<ClienteCtaCteMovimientoDTO>
                    {
                        RecordsTotal = movimientosCount,
                        RecordsFiltered = movimientos.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = movimientos.Select(movimiento => new ClienteCtaCteMovimientoDTO().From(movimiento)).ToList(),
                        ExtraData = new
                        {
                            monto = monto,
                            disponible = disponible
                        }
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: clientes/cta_cte/resume?encryptedClienteId={encryptedClienteId}
        [HttpGet]
        [ActionName("cta_cte/resume")]
        public async Task<IActionResult> GetCtaCteResumeAsync([FromQuery] string encryptedClienteId)
        {
            try
            {
                ClienteCtaCteResumeDTO resume = null;

                int clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedClienteId));

                var clienteMgr = new ClientesManager(_serviceProvider);
                var cliente = await clienteMgr.ObtenerClienteAsync(clienteId);

                //SI TIENE CUENTA CORRIENTE
                if (cliente.MontoCtaCte > 0)
                {
                    var manager = new CuentasCorrientesManager(_serviceProvider);
                    var disponible = await manager.ObtenerDisponibleActualCtaCteClienteAsync(clienteId);
                    var monto = await manager.ObtenerMontoActualCtaCteClienteAsync(clienteId);
                    resume = new ClienteCtaCteResumeDTO
                    {
                        Monto = monto,
                        Disponible = disponible,
                        Deudor = monto - disponible
                    };
                }

                return Ok(new ApiResultDTO<ClienteCtaCteResumeDTO>
                {
                    Success = true,
                    Data = resume
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: clientes/cta_cte/save
        [HttpPost]
        [ActionName("cta_cte/save")]
        public async Task<IActionResult> PostMovimientoCtaCteSaveAsync([FromBody] ClienteCtaCteMovimientoDTO movimientoDto)
        {
            try
            {
                var manager = new CuentasCorrientesManager(_serviceProvider);
                var movimiento = await manager.GuardarMovimientoCtaCteClienteAsync(movimientoDto, (int)(_token?.UserId ?? 0));

                return Ok(new ApiResultDTO<ClienteCtaCteMovimientoDTO>
                {
                    Success = true,
                    Data = new ClienteCtaCteMovimientoDTO().From(movimiento)
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: clientes/saldos/deudor?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("saldos/deudor")]
        public async Task<IActionResult> GetSaldoDeudorAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new CuentasCorrientesManager(_serviceProvider);
                var monto = await manager.ObtenerMontoActualCtaCteClienteAsync(clienteId);
                decimal saldo = 0;

                if (monto > 0)
                {
                    var disponible = await manager.ObtenerDisponibleActualCtaCteClienteAsync(clienteId);
                    saldo = monto - disponible;
                }
                else
                {
                    saldo = 0;
                }

                return Ok(new ApiResultDTO<decimal>
                {
                    Success = true,
                    Data = saldo
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
