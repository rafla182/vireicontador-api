﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using GroupDocs.Signature.Options;
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
        public async Task<IActionResult> Salvar([FromBody] SalvarRequest salvar)
        {
            var empresas = clienteServico.Salvar(salvar.Cliente, salvar.Assinatura, salvar.Cartao);
            return await Response(empresas, clienteServico.Notifications);
        }

        [HttpPost("api/hash")]
        public async Task<IActionResult> SalvarPlano([FromBody] HashRequest hash)
        {
            var empresas = clienteServico.SalvarPlano(hash.Email, hash.Valor, hash.Plano, hash.Nome, hash.Funcionarios);
            return await Response(empresas, clienteServico.Notifications);
        }

        [HttpGet("api/hash/{email}")]
        public async Task<IActionResult> PegarPlano(string email)
        {
            var empresas = clienteServico.PegarPlano(email);
            return await Response(empresas, clienteServico.Notifications);
        }
        [HttpGet("api/contrato")]
        public async Task<IActionResult> PegarContrato()
        {       
            var empresas = clienteServico.Contrato();
            return await Response(empresas, clienteServico.Notifications);
        }
    }
}
