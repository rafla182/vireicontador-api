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
        public ClienteVINDI Salvar(Cliente cliente, Plano plano, Fatura fatura)
        {
            var customer = SalvarCliente(cliente);
            var assinatura = SalvarAssinatura(plano, customer.id);
            var faturas = SalvarFatura(customer.id, fatura);

            return customer;
        }

        private FaturaVINDI SalvarFatura(int clienteID, Fatura fatura)
        {
            var produtos = new List<ItemFaturaVINDI>();
            produtos.Add(new ItemFaturaVINDI()
            {
                product_id = fatura.ProdutoID
            });

            var faturaVINDI = new FaturaVINDI()
            {
                customer_id = clienteID,
                bill_items = produtos,
                payment_method_code = fatura.PaymentMethodCode
            };

            var url = $"https://sandbox-app.vindi.com.br:443/api/v1/subscriptions";

            var json = JsonConvert.SerializeObject(faturaVINDI);
            var result = servicoApi.PostDataAuth<FaturaVINDIRequest>(url, json);

            return result.Fatura;
        }

        private PlanoVINDI SalvarAssinatura(Plano plano, int clienteID)
        {
            var produtos = new List<ItemsVINDI>();
            produtos.Add(new ItemsVINDI()
            {
                product_id = plano.ProdutoID
            });

            var planoVINDI = new PlanoVINDI()
            {
                customer_id = clienteID,
                plan_id = plano.PlanoID,
                product_items = produtos,
                payment_method_code = plano.PaymentMethodCode
            };

            var url = $"https://sandbox-app.vindi.com.br:443/api/v1/subscriptions";

            var json = JsonConvert.SerializeObject(planoVINDI);
            var result = servicoApi.PostDataAuth<PlanoVINDIRequest>(url, json);

            return result.Plano;
        }

        public ClienteVINDI SalvarCliente(Cliente cliente)
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

                var customer = new ClienteVINDI()
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
                var result = servicoApi.PostDataAuth<ClienteVINDIResponse>(url, json);

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
                return null;
            }
        }
    }
}
