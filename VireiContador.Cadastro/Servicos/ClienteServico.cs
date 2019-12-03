using System;
using System.Collections.Generic;
using System.Text;
using FluentValidator;
using Newtonsoft.Json;
using VireiContador.Cadastro.Model;
using VireiContador.Cadastro.Repositorio;
using VireiContador.Infra.Models;
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
            this.servicoApi = servicoApi;
        }
        public Customer SalvarCliente(Cliente cliente)
        {
            try
            {
                var phones = new List<Phone>();
                phones.Add(new Phone
                {
                    phone_type = "",
                    number = cliente.Telefone,
                    extension = ""
                });

                var customer = new Customer()
                {
                    name = cliente.Nome,
                    email = cliente.Email,
                    address = new Address
                    {
                        street = cliente.Logradouro,
                        number = cliente.Numero,
                        additional_details = cliente.Complemento,
                        zipcode = cliente.CEP,
                        state = cliente.Estado,
                        city = cliente.Cidade,
                        country = "Brasil",
                        neighborhood = cliente.Bairro
                    },
                    phones = phones
                };

                var clienteSalvo = clienteRepositorio.SalvarCliente(cliente);

                var url = $"https://sandbox-app.vindi.com.br:443/api/v1/customers";

                var json = JsonConvert.SerializeObject(customer);
                var result = servicoApi.PostDataAuth<CustomerResponse>(url, json);

                return result.Customer;


            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o cliente.");
                return null;
            }
        }


        public bool SalvarPlano(string email, decimal valor, string plano, string nome)
        {
            try
            {
                return clienteRepositorio.SalvarPlano(email, nome, valor, plano);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano.");
                return false;
            }
        }

        public Plano PegarPlano(string email)
        {
            try
            {
                //var url = $"https://sandbox-app.vindi.com.br:443/api/v1/customers?page=1&sort_by=created_at&sort_order=asc";
                //var result = servicoApi.GetDataVINDI<CustomerResult>(url);

                return clienteRepositorio.PegarPlano(email);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano." + ex.InnerException);
                return 0;
            }
        }
    }
}
