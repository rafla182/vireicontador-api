using System;
using System.Collections.Generic;
using System.Text;
using FluentValidator;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;

namespace VireiContador.Cadastro.Servicos
{
    public class CnaeServico : Notifiable
    {
        private readonly CNAERepositorio cnaeRepositorio;
        public CnaeServico(CNAERepositorio cnaeRepositorio)
        {
            this.cnaeRepositorio = cnaeRepositorio;
        }
        public IReadOnlyCollection<CNAE> ListarCnaes()
        {
            return cnaeRepositorio.ListarCNAE();
        }
    }
}
