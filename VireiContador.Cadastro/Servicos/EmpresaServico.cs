using System;
using FluentValidator;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;
using VireiContador.Infra.Servico;

namespace VireiContador.Cadastro.Servicos
{
    public class EmpresaServico : Notifiable
    {
        private readonly EmpresaRepositorio empresaRepositorio;
        private readonly ServicoApi servicoApi;
        public EmpresaServico(EmpresaRepositorio empresaRepositorio, ServicoApi servicoApi)
        {
            this.empresaRepositorio = empresaRepositorio;
            this.servicoApi = servicoApi;
        }
        public Empresa PegarEmpresa(string cnpj)
        {
            var cnpjURL = $"https://www.receitaws.com.br/v1/cnpj/"+cnpj;

            var response = servicoApi.GetData<Empresa>(cnpjURL);
            return response;
        }

    }
}
