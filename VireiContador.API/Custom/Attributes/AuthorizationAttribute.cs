using System;
using System.Security.Principal;
using VireiContadorP.API.Custom.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using VireiContador.Infra.Configuracao;

namespace VireiContadorP.API.Custom.Attributes
{
    public class AuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Configuracao configuracoes;

        public AuthorizationAttribute(IOptions<Configuracao> configuracoes)
        {
            this.configuracoes = configuracoes.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.HasCustomAttribute(typeof(AllowAnonymousAttribute))) return;

            var roles = new string[] { };

        //    var token = new Token(context.HttpContext.Request.Headers["Authorization"]);

        //    try
        //    {
        //        var user = token.HasValue
        //            ? JsonWebToken.DecodeToObject<UsuarioSessao>(token.Value, configuracoes.TokenSecretKey)
        //            : null;

        //        if (user != null)
        //        {
        //            if (context.ActionDescriptor.HasCustomAttribute(typeof(SomenteAdministradorAttribute)))
        //            {
        //                context.Result = new UnauthorizedResult();
        //            }

        //            var identity = new GenericIdentityCustom(user);
        //            var principal = new GenericPrincipal(identity, roles);
        //            context.HttpContext.User = principal;
        //        }
        //        else
        //        {
        //            context.Result = new UnauthorizedResult();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        context.Result = new UnauthorizedResult();
        //    }
        }
    }
}