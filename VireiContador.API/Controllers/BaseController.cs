using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VireiContadorP.API.Custom.Attributes;
using VireiContadorP.API.Models.Responses;
using FluentValidator;
using Microsoft.AspNetCore.Mvc;

namespace VireiContadorP.API.Controllers
{
    [ServiceFilter(typeof(AuthorizationAttribute))]
    public class BaseController : Controller
    {
        protected async Task<IActionResult> Response(object resultado, IReadOnlyCollection<Notification> notificacoes)
        {
            if (!notificacoes.Any())
            {
                return Ok(new ResponseBase
                {
                    Sucesso = true,
                    Resultado = resultado
                });
            }
            return BadRequest(new ResponseBase
            {
                Sucesso = false,
                Resultado = resultado,
                Erros = notificacoes
            });
        }

        protected async Task<IActionResult> ResponseFileSpreadsheet(MemoryStream objeto, IReadOnlyCollection<Notification> notificacoes)
        {
            if (!notificacoes.Any())
            {
                return File(objeto, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "");
            }
            return BadRequest(new ResponseBase
            {
                Sucesso = false,
                Erros = notificacoes
            });
        }
    }
}