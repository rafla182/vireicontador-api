using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VireiContador.Cadastro.Servicos;
using VireiContadorP.API.Controllers;

namespace VireiContador.API.Controllers
{
    public class ClienteController : BaseController
    {
        private readonly ClienteServico clienteServico;

        public ClienteController(ClienteServico clienteServico)
        {
            this.clienteServico = clienteServico;
        }
        [HttpGet("api/salvar")]
        public async Task<IActionResult> SalvarCliente()
        {
            var empresas = clienteServico.SalvarCliente();
            return await Response(empresas, clienteServico.Notifications);
        }
    }
}
