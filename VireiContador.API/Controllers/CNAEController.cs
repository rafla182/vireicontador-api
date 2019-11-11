using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VireiContador.Cadastro.Servicos;
using VireiContadorP.API.Controllers;

namespace VireiContador.API.Controllers
{
    public class CNAEController : BaseController
    {
        private readonly CnaeServico cnaeServico;
        public CNAEController(CnaeServico cnaeServico)
        {
            this.cnaeServico = cnaeServico;
        }
        [HttpGet("api/cnaes")]
        public async Task<IActionResult> ListarCNAES()
        {
            var empresas = cnaeServico.ListarCnaes();
            return await Response(empresas, cnaeServico.Notifications);
        }
    }


}
    