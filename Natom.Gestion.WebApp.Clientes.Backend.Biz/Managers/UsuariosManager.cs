using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Helpers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Auth;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class UsuariosManager : BaseManager
    {
        public UsuariosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public async Task<Usuario> LoginAsync(string email, string password)
        {
            if (email.Equals(_configuration["Admin:User"]) && password.Equals(_configuration["Admin:Password"]))
            {
                return new Usuario
                {
                    Nombre = "Admin",
                    Apellido = "",
                    Email = "admin@admin.com",
                    FechaHoraAlta = new DateTime(2021, 12, 01),
                    UsuarioId = 0
                };
            }

            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()) && !u.FechaHoraBaja.HasValue);
            if (usuario == null)
                throw new HandledException("Usuario y/o clave incorrecta");

            if (usuario.FechaHoraBaja.HasValue)
                throw new HandledException("Usuario dado de baja");

            if (!string.IsNullOrEmpty(usuario.SecretConfirmacion) && usuario.FechaHoraConfirmacionEmail == null)
                throw new HandledException("Revise su casilla de correo electrónico para establecer la contraseña");

            if (!usuario.Clave.Equals(EncryptionService.CreateMD5(password)))
                throw new HandledException("Usuario y/o clave incorrecta");

            return usuario;
        }

        public Task<List<Permiso>> ObtenerPermisosAsync(int usuarioId)
        {
            if (usuarioId == 0) //ADMIN
                return _db.Permisos.ToListAsync();

            return _db.UsuariosPermisos
                        .Include(p => p.Permiso)
                        .Where(p => p.UsuarioId == usuarioId)
                        .Select(p => p.Permiso)
                        .ToListAsync();
        }

        public string ObtenerEstado(Usuario usuario)
        {
            if (usuario.FechaHoraBaja.HasValue)
                return "ELIMINADO";
            else if (!usuario.FechaHoraConfirmacionEmail.HasValue)
                return "PENDIENTE DE CONFIRMACIÓN";
            else
                return "ACTIVO";
        }

        public Task<int> ObtenerUsuariosCountAsync()
                    => _db.Usuarios
                            .Where(p => !p.FechaHoraBaja.HasValue)
                            .CountAsync();

        public async Task<List<Usuario>> ObtenerUsuariosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection)
        {
            var queryable = _db.Usuarios.Where(p => !p.FechaHoraBaja.HasValue);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Nombre.ToLower().Contains(filter.ToLower())
                                                    || p.Apellido.ToLower().Contains(filter.ToLower())
                                                    || p.Email.ToLower().Contains(filter.ToLower()));
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.Nombre :
                                                                  sortColumnIndex == 1 ? c.Apellido :
                                                                  sortColumnIndex == 2 ? c.Email :
                                                                  sortColumnIndex == 3 ? c.FechaHoraAlta.Ticks.ToString() :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Nombre :
                                                                  sortColumnIndex == 1 ? c.Apellido :
                                                                  sortColumnIndex == 2 ? c.Email :
                                                                  sortColumnIndex == 3 ? c.FechaHoraAlta.Ticks.ToString() :
                                                            "");

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = await queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();

            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public async Task<Usuario> GuardarUsuarioAsync(IConfiguration configuration, UserDTO user)
        {
            Usuario usuario = null;
            if (string.IsNullOrEmpty(user.EncryptedId)) //NUEVO
            {
                if (await _db.Usuarios.AnyAsync(u => u.Email.ToLower().Equals(user.Email.ToLower()) && !u.FechaHoraBaja.HasValue))
                    throw new HandledException("Ya existe un usuario con este Email.");

                usuario = new Usuario()
                {
                    Nombre = user.FirstName,
                    Apellido = user.LastName,
                    Email = user.Email,
                    FechaHoraAlta = DateTime.Now,
                    SecretConfirmacion = Guid.NewGuid().ToString("N"),
                    Permisos = user.Permisos.Select(p_encryptedId => new UsuarioPermiso() { PermisoId = EncryptionService.Decrypt<string>(p_encryptedId) }).ToList()
                };

                await EnviarEmailParaConfirmarRegistroAsync(configuration, usuario);

                _db.Usuarios.Add(usuario);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int usuarioId = EncryptionService.Decrypt<int>(user.EncryptedId);

                if (await _db.Usuarios.AnyAsync(u => u.Email.ToLower().Equals(user.Email.ToLower()) && !u.FechaHoraBaja.HasValue && u.UsuarioId != usuarioId))
                    throw new HandledException("Ya existe un usuario con este Email.");

                usuario = await _db.Usuarios
                                    .Include(u => u.Permisos)
                                    .FirstAsync(u => u.UsuarioId.Equals(usuarioId));

                _db.Entry(usuario).State = EntityState.Modified;
                usuario.Nombre = user.FirstName;
                usuario.Apellido = user.LastName;

                foreach (var permiso in usuario.Permisos)
                    _db.Entry(permiso).State = EntityState.Deleted;

                usuario.Permisos = user.Permisos.Select(p_encryptedId => new UsuarioPermiso() { PermisoId = EncryptionService.Decrypt<string>(p_encryptedId) }).ToList();
                await _db.SaveChangesAsync();
            }

            return usuario;
        }

        private Task EnviarEmailParaRecuperarClaveAsync(IConfiguration configuration, Usuario usuario)
        {
            string subject = "Recuperar clave";
            var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { s = usuario.SecretConfirmacion, e = usuario.Email }));
            var data = Uri.EscapeDataString(Convert.ToBase64String(dataBytes));
            string link = new Uri($"{configuration["CORS:FrontendOrigin"]}/users/confirm/{data}").AbsoluteUri;
            string body = String.Format("<h2>Mundo Mascota .:. Sistema de Gestión</h2><br/><br/>Por favor, para <b>recuperar la clave de acceso al sistema</b> haga clic en el siguiente link: {0}", link);
            return EmailHelper.EnviarMailAsync(configuration, subject, body, usuario.Email, usuario.Nombre);
        }

        private Task EnviarEmailParaConfirmarRegistroAsync(IConfiguration configuration, Usuario usuario)
        {
            string subject = "Confirmar registración";
            var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { s = usuario.SecretConfirmacion, e = usuario.Email }));
            var data = Uri.EscapeDataString(Convert.ToBase64String(dataBytes));
            string link = new Uri($"{configuration["CORS:FrontendOrigin"]}/users/confirm/{data}").AbsoluteUri;
            string body = String.Format("<h2>Mundo Mascota .:. Sistema de Gestión</h2><br/><br/>Por favor, para <b>generar la clave de acceso al sistema</b> haga clic en el siguiente link: {0}", link);
            return EmailHelper.EnviarMailAsync(configuration, subject, body, usuario.Email, usuario.Nombre);
        }

        public async Task EliminarUsuarioAsync(int usuarioId)
        {
            var usuario = await _db.Usuarios
                                    .Include(u => u.Permisos)
                                    .FirstAsync(u => u.UsuarioId.Equals(usuarioId));

            _db.Entry(usuario).State = EntityState.Modified;
            usuario.FechaHoraBaja = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task RecuperarUsuarioAsync(IConfiguration configuration, int usuarioId)
        {
            var usuario = await _db.Usuarios
                                    .FirstAsync(u => u.UsuarioId.Equals(usuarioId));

            _db.Entry(usuario).State = EntityState.Modified;
            usuario.SecretConfirmacion = Guid.NewGuid().ToString("N");
            usuario.FechaHoraConfirmacionEmail = null;
            usuario.Clave = null;

            await EnviarEmailParaRecuperarClaveAsync(configuration, usuario);

            await _db.SaveChangesAsync();
        }

        public async Task ConfirmarUsuarioAsync(string secret, string clave)
        {
            var usuario = await _db.Usuarios
                                    .FirstAsync(u => u.SecretConfirmacion.Equals(secret));

            _db.Entry(usuario).State = EntityState.Modified;
            usuario.FechaHoraConfirmacionEmail = DateTime.Now;
            usuario.SecretConfirmacion = null;
            usuario.Clave = EncryptionService.CreateMD5(clave);

            await _db.SaveChangesAsync();
        }

        public Task<List<Permiso>> ObtenerListaPermisosAsync()
                        => _db.Permisos.OrderBy(p => p.PermisoId).ToListAsync();

        public Task<Usuario> ObtenerUsuarioAsync(int usuarioId)
                        => _db.Usuarios
                                .Include(u => u.Permisos)
                                .FirstAsync(u => u.UsuarioId.Equals(usuarioId));
    }
}
