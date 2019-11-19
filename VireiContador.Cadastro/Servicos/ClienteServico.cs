using System;
using System.Collections.Generic;
using System.Text;
using FluentValidator;
using Newtonsoft.Json;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;
using VireiContador.Infra.Notificacoes;
using VireiContador.Infra.Servico;

namespace VireiContador.Cadastro.Servicos
{
    public class ClienteServico : Notificavel
    {
        private readonly ClienteRepositorio clienteRepositorio;
        private readonly ServicoApi servicoApi;
        public ClienteServico(ClienteRepositorio clienteRepositorio, ServicoApi servicoApi)
        {
            this.clienteRepositorio = clienteRepositorio;
        }
        public Cliente SalvarCliente(Cliente cliente)
        {
            try
            {
                var clienteSalvo = clienteRepositorio.SalvarCliente(cliente);

                //var url = $"{configuration.FenixInnApi}${partnerID}/enable";

                //var json = JsonConvert.SerializeObject(partner);
                //var result = servicoApi.PostData<Cliente>(url, json);
                return clienteSalvo;

            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o cliente.");
                return null;
            }
        }


        public bool SalvarPlano(string email, decimal valor)
        {
            try
            {
                return clienteRepositorio.SalvarPlano(email, valor);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano.");
                return false;
            }
        }

        public decimal PegarPlano(string email)
        {
            try
            {
                return clienteRepositorio.PegarPlano(email);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano.");
                return 0;
            }
        }
    }
}
