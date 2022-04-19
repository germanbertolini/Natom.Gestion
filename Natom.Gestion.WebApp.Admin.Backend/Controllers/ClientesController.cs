using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Biz.Managers;
using Natom.Extensions.Auth.Attributes;
using Natom.Extensions.Auth.Services;
using Natom.Extensions.Cache.Services;
using Natom.Gestion.WebApp.Admin.Backend.DTO;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Autocomplete;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Clientes;
using Natom.Gestion.WebApp.Admin.Backend.DTO.DataTable;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Zonas;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Natom.Extensions.Logger.Services;

namespace Natom.Gestion.WebApp.Admin.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ClientesController : BaseController
    {
        private readonly AuthService _authService;
        private readonly DiscordService _discordService;
        private readonly CacheService _cacheService;

        public ClientesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
            _discordService = (DiscordService)serviceProvider.GetService(typeof(DiscordService));
        }

        // POST: clientes/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_clientes")]
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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: clientes/basics/data
        // GET: clientes/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new ClientesManager(_serviceProvider);
                ClienteDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedId));
                    var cliente = await manager.ObtenerClienteAsync(clienteId);
                    entity = new ClienteDTO().From(cliente);
                }

                var zonasManager = new ZonasManager(_serviceProvider);
                var zonas = await zonasManager.ObtenerZonasActivasAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        zonas = zonas.Select(zona => new ZonaDTO().From(zona))
                    }
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

        // POST: clientes/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> PostSaveAsync([FromBody] ClienteDTO clienteDto)
        {
            try
            {
                var dacpacManager = new DacpacManager(_serviceProvider);
                await dacpacManager.CheckDeploymentParamsAsync();

                var esAlta = string.IsNullOrEmpty(clienteDto.EncryptedId);
                var manager = new ClientesManager(_serviceProvider);
                var cliente = await manager.GuardarClienteAsync(clienteDto.ToModel());

                await RegistrarAccionAsync(clienteId: 0, cliente.ClienteId, nameof(Cliente), esAlta ? "Alta" : "Edición");

                if (esAlta)
                {
                    var successInfo = await dacpacManager.DeployNewDacpacAsync(cliente);
                    _loggerService.LogInfo(_transaction.TraceTransactionId, "DACPAC desplegado correctamente", new { result = successInfo });

                    var clienteConfigManager = new NegocioConfigManager(_serviceProvider);
                    var config = new NegocioConfig()
                    {
                        RazonSocial = cliente.EsEmpresa ? cliente.RazonSocial : $"{cliente.Nombre} {cliente.Apellido}",
                        NombreFantasia = cliente.NombreFantasia,
                        TipoDocumento = cliente.EsEmpresa ? "CUIT" : "DNI",
                        NumeroDocumento = cliente.NumeroDocumento,
                        Domicilio = cliente.Domicilio,
                        Localidad = cliente.Localidad,
                        Telefono = cliente.ContactoTelefono1,
                        Email = cliente.ContactoEmail1,
                        FechaMembresia = DateTime.Now.Date,
                        LogoBase64 = @"data:image/png;base64,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"
                    };
                    await clienteConfigManager.SaveConfigAsync(config, cliente.ClienteId);

                    _loggerService.LogInfo(_transaction.TraceTransactionId, "Datos negocio grabados correctamente", new { result = successInfo });
                }

                if (esAlta)
                    _ = _discordService.LogInfoAsync(":tada: NUEVO CLIENTE :partying_face:", _transaction.TraceTransactionId, new { Cliente = (cliente.EsEmpresa ? cliente.RazonSocial : $"{cliente.Nombre} {cliente.Apellido}"), Tipo = (cliente.EsEmpresa ? "Empresa" : "Particular") });

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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // DELETE: clientes/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                var cliente = await manager.DesactivarClienteAsync(clienteId);

                await _authService.DestroyTokensByScopeAndClientIdAsync(scope: "WebApp.Clientes", clienteId);

                _ = _discordService.LogInfoAsync(":rotating_light: BAJA DE CLIENTE :disappointed:", _transaction.TraceTransactionId, new { Cliente = (cliente.EsEmpresa ? cliente.RazonSocial : $"{cliente.Nombre} {cliente.Apellido}"), Tipo = (cliente.EsEmpresa ? "Empresa" : "Particular") });

                await RegistrarAccionAsync(clienteId: 0, clienteId, nameof(Cliente), "Baja");

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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: clientes/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_clientes")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedId));

                var manager = new ClientesManager(_serviceProvider);
                var cliente = await manager.ActivarClienteAsync(clienteId);

                _ = _discordService.LogInfoAsync(":tada: REACTIVACIÓN CLIENTE :partying_face:", _transaction.TraceTransactionId, new { Cliente = (cliente.EsEmpresa ? cliente.RazonSocial : $"{cliente.Nombre} {cliente.Apellido}"), Tipo = (cliente.EsEmpresa ? "Empresa" : "Particular") });

                await RegistrarAccionAsync(clienteId: 0, clienteId, nameof(Cliente), "Alta");

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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
