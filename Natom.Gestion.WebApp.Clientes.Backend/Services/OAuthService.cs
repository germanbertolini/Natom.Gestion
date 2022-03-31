using Jose;
using Natom.Gestion.WebApp.Clientes.Backend.Entities;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Services
{
    public class OAuthService
    {
        private static byte[] secretKey = new byte[] { 120, 23, 20, 132, 89, 18, 11, 171, 61, 29, 13, 121,
                                                        0, 72, 129, 48, 40, 23, 15, 73, 44, 225, 221, 59,
                                                        117, 153, 47, 99, 89, 36, 143, 23, 12, 48, 40 };

        public static Token GenerateAccessToken(string scope, Usuario usuario, List<Permiso> permisos, long tokenDurationInSeconds)
        {
            var token = new Token
            {
                UserId = usuario.UsuarioId,
                ClientId = 1,
                Scope = scope,
                CreationTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()
        };

            if (usuario.FechaHoraConfirmacionEmail.HasValue)
            {
                token.Duration = tokenDurationInSeconds;
                token.ExpirationTime = new DateTimeOffset(DateTime.Now.AddSeconds(tokenDurationInSeconds)).ToUnixTimeSeconds();
                token.WaitingEmailConfirmation = false;
                token.Permissions = permisos?.Select(p => p.PermisoId).ToList() ?? new List<string>();
            }
            else
            {
                var durationTemp = 365 * 24 * 60 * 60;
                token.Duration = durationTemp;
                token.ExpirationTime = new DateTimeOffset(DateTime.Now.AddSeconds(durationTemp)).ToUnixTimeSeconds();
                token.WaitingEmailConfirmation = true;
                token.Permissions = permisos?.Select(p => p.PermisoId).ToList() ?? new List<string>();
            }

            return token;
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static string Encode(Token accessToken)
                                => JWT.Encode(accessToken, secretKey, JwsAlgorithm.HS256);

        public static Token Decode(string accessToken)
                                => JWT.Decode<Token>(accessToken, secretKey, JwsAlgorithm.HS256);
    }
}
