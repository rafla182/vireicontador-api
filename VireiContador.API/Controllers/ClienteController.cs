using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VireiContador.API.Models.Requests;
using VireiContador.Cadastro.Model;
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

        [HttpPost("api/salvar")]
        public async Task<IActionResult> SalvarCliente([FromBody] ClienteRequest cliente)
        {
            var empresas = clienteServico.SalvarCliente(cliente.Cliente);
            return await Response(empresas, clienteServico.Notifications);
        }


        [HttpPost("api/hash")]
        public async Task<IActionResult> SalvarPlano([FromBody] HashRequest hash)
        {
            var empresas = clienteServico.SalvarPlano(hash.Email, hash.Valor, hash.Plano, hash.Nome);
            return await Response(empresas, clienteServico.Notifications);
        }

        [HttpGet("api/hash/{email}")]
        public async Task<IActionResult> PegarPlano(string email)
        {
            var empresas = clienteServico.PegarPlano(email);
            return await Response(empresas, clienteServico.Notifications);
        }
    }
}
