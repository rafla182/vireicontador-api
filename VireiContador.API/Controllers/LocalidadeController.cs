using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VireiContador.Cadastro.Servicos;
using VireiContadorP.API.Controllers;

namespace VireiContador.API.Controllers
{
    public class LocalidadeController : BaseController
    {
        private readonly LocalidadeServico localidadeServico;

        public LocalidadeController(LocalidadeServico localidadeServico)
        {
            this.localidadeServico = localidadeServico;
        }
        [HttpGet("api/estado")]
        public async Task<IActionResult> ListaEstados()
        {
            var empresas = localidadeServico.PegarEstado();
            return await Response(empresas, localidadeServico.Notifications);
        }
        [HttpGet("api/estado/{sigla}")]
        public async Task<IActionResult> ListaCidades(string sigla)
        {
            var empresas = localidadeServico.PegarCidade(sigla);
            return await Response(empresas, localidadeServico.Notifications);
        }
    }
}
