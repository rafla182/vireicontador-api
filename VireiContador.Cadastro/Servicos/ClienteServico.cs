using System;
using System.Collections.Generic;
using System.Text;
using FluentValidator;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;

namespace VireiContador.Cadastro.Servicos
{
    public class ClienteServico : Notifiable
    {
        private readonly ClienteRepositorio clienteRepositorio;
        public ClienteServico(ClienteRepositorio clienteRepositorio)
        {
            this.clienteRepositorio = clienteRepositorio;
        }
        public bool SalvarCliente()
        {
            return clienteRepositorio.SalvarCliente();
        }
    }
}
