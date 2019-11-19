using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VireiContador.API.Models.Requests;
using VireiContador.Cadastro.Servicos;
using VireiContadorP.API.Controllers;

namespace VireiContador.API.Controllers
{
    public class EmpresaController : BaseController
    {
        private readonly EmpresaServico empresaServico;

        public EmpresaController(EmpresaServico empresaServico)
        {
            this.empresaServico = empresaServico;
        }

        [HttpGet("api/cnpj/{cnpj}")]
        public async Task<IActionResult> PegarCnpj(string cnpj)
        {
            var empresas = empresaServico.PegarEmpresa(cnpj);
            return await Response(empresas, empresaServico.Notifications);
        }

    }
}
