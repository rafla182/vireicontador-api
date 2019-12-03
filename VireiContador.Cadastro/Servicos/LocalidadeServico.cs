using FluentValidator;
using System;
using System.Collections.Generic;
using System.Text;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;

namespace VireiContador.Cadastro.Servicos
{
    public class LocalidadeServico : Notifiable
    {
        private readonly LocalidadeRespositorio localidadeRepositorio;
        public LocalidadeServico(LocalidadeRespositorio localidadeRepositorio)
        {
            this.localidadeRepositorio = localidadeRepositorio;
        }
        public IReadOnlyCollection<Estado> PegarEstado()
        {
            return localidadeRepositorio.PegarEstado();
        }

        public IReadOnlyCollection<Cidade> PegarCidade(string sigla)
        {
            return localidadeRepositorio.PegarCidade(sigla);
        }
    }
}
