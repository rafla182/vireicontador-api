using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FluentValidator;
using GroupDocs.Signature;
using GroupDocs.Signature.Options;
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
        public AssinaturaVINDIRequest Salvar(Cliente cliente, Assinatura assinatura, CartaoCredito cartao)
        {
            try
            {
                var clienteVINDI = ObterCliente(cliente.Email);
                var customer = new ClienteVINDI();
                if (clienteVINDI == null)
                    customer = SalvarCliente(cliente, assinatura, cartao);
                else
                    customer = clienteVINDI;

                if (assinatura.TipoPagamento == "credit_card")
                {
                    var profile = SalvarProfile(cartao, customer.id);
                    var assinaturaRetorno = SalvarAssinatura(assinatura, customer.id, profile.id);
                    return assinaturaRetorno;
                }
                else
                {
                    var assinaturaRetorno = SalvarAssinaturaBoleto(assinatura, customer.id);
                    return assinaturaRetorno;
                }
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o cliente.");
                return null;

            }
        }

        public AssinaturaVINDIRequest SalvarMigrar(EmpresaSQL empresa, Assinatura assinatura, CartaoCredito cartao, List<Socio> socios, Competencia competencia)
        {
            try
            {
                var clienteVINDI = ObterCliente(empresa.Email);
                var customer = new ClienteVINDI();
                if (clienteVINDI == null)
                    customer = SalvarEmpresa(empresa, assinatura, cartao, socios, competencia);
                else
                    customer = clienteVINDI;

                var clienteSalvo = clienteRepositorio.SalvarEmpresa(empresa, assinatura, cartao, socios, competencia);

                if (customer != null)
                {
                    if (assinatura.TipoPagamento == "credit_card")
                    {
                        var profile = SalvarProfile(cartao, customer.id);
                        var assinaturaRetorno = SalvarAssinatura(assinatura, customer.id, profile.id);
                        return assinaturaRetorno;
                    }
                    else
                    {
                        var assinaturaRetorno = SalvarAssinaturaBoleto(assinatura, customer.id);
                        return assinaturaRetorno;
                    }
                }


                return null;
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o cliente.");
                return null;

            }
        }

        private PerfilPagamentoRetornoVINDI SalvarProfile(CartaoCredito cartao, int clienteID)
        {
            var perfilVINDI = new PerfilPagamentoVINDI()
            {
                holder_name = cartao.TitularCartao,
                card_expiration = cartao.Vencimento.Insert(2, "/"),
                card_number = cartao.Numero,
                customer_id = clienteID,
                card_cvv = cartao.CVV,

            };

            var url = $"https://app.vindi.com.br:443/api/v1/payment_profiles";

            var json = JsonConvert.SerializeObject(perfilVINDI);
            var result = servicoApi.PostDataAuth<PerfilPagamentoVINDIRequest>(url, json);

            return result.payment_profile;
        }

        //private FaturaVINDI SalvarFatura(int clienteID, Fatura fatura)
        //{
        //    var produtos = new List<ItemFaturaVINDI>();
        //    produtos.Add(new ItemFaturaVINDI()
        //    {
        //        product_id = fatura.ProdutoID
        //    });

        //    var faturaVINDI = new FaturaVINDI()
        //    {
        //        customer_id = clienteID,
        //        bill_items = produtos,
        //        payment_method_code = fatura.TipoPagamento
        //    };

        //    var url = $"https://sandbox-app.vindi.com.br:443/api/v1/subscriptions";

        //    var json = JsonConvert.SerializeObject(faturaVINDI);
        //    var result = servicoApi.PostDataAuth<FaturaVINDIRequest>(url, json);

        //    return result.Fatura;
        //}

        private AssinaturaVINDIRequest SalvarAssinatura(Assinatura plano, int clienteID, int profileID)
        {
            var produtos = new List<ItemsVINDI>();
            produtos.Add(new ItemsVINDI()
            {
                product_id = plano.ProdutoId,
                pricing_schema = {
                    price = plano.Valor,
                    schema_type = "flat"
                }
            });


            var planoVINDI = new AssinaturaVINDI()
            {
                customer_id = clienteID,
                plan_id = plano.Id,
                product_items = produtos,
                payment_method_code = plano.TipoPagamento,
                payment_profile = new PerfilPagamentoAssinaturaVINDI()
                {
                    id = profileID
                },

            };

            var url = $"https://app.vindi.com.br:443/api/v1/subscriptions";

            var json = JsonConvert.SerializeObject(planoVINDI);
            var result = servicoApi.PostDataAuth<AssinaturaVINDIRequest>(url, json);

            return result;
        }

        private AssinaturaVINDIRequest SalvarAssinaturaBoleto(Assinatura plano, int clienteID)
        {
            var produtos = new List<ItemsVINDI>();
            produtos.Add(new ItemsVINDI()
            {
                product_id = plano.ProdutoId,
                pricing_schema = new PricingVINDI()
                {
                    price = plano.Valor,
                    schema_type = "flat"
                }
            });

            var planoVINDI = new AssinaturaVINDI()
            {
                customer_id = clienteID,
                plan_id = plano.Id,
                product_items = produtos,
                payment_method_code = plano.TipoPagamento
            };

            var url = $"https://app.vindi.com.br:443/api/v1/subscriptions";

            var json = JsonConvert.SerializeObject(planoVINDI);
            var result = servicoApi.PostDataAuth<AssinaturaVINDIRequest>(url, json);

            return result;
        }

        public ClienteVINDI SalvarCliente(Cliente cliente, Assinatura assinatura, CartaoCredito cartao)
        {
            try
            {
                var phones = new List<Phone>();
                phones.Add(new Phone
                {
                    phone_type = "mobile",
                    number = cliente.Telefone,
                    extension = ""
                });

                var customer = new ClienteVINDI()
                {
                    name = cliente.Nome,
                    email = cliente.Email,
                    registry_code = cliente.CPF,
                    address = new Address
                    {
                        street = cliente.Logradouro,
                        number = cliente.Numero,
                        additional_details = cliente.Complemento,
                        zipcode = cliente.CEP,
                        state = cliente.Estado,
                        city = cliente.Cidade,
                        country = "BR",
                        neighborhood = cliente.Bairro
                    },
                    phones = phones
                };

                var clienteSalvo = clienteRepositorio.SalvarCliente(cliente, assinatura, cartao);

                var url = $"https://app.vindi.com.br:443/api/v1/customers";

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

        public ClienteVINDI SalvarEmpresa(EmpresaSQL empresa, Assinatura assinatura, CartaoCredito cartao, List<Socio> socios, Competencia competencia)
        {
            try
            {
                var phones = new List<Phone>();
                phones.Add(new Phone
                {
                    phone_type = "landline",
                    number = empresa.Telefone,
                });

                phones.Add(new Phone
                {
                    phone_type = "mobile",
                    number = empresa.Telefone2,
                });

                var customer = new ClienteVINDI()
                {
                    name = empresa.Nome,
                    email = empresa.Email,
                    registry_code = empresa.CPF,
                    phones = phones
                };


                var url = $"https://app.vindi.com.br:443/api/v1/customers";

                var json = JsonConvert.SerializeObject(customer);
                var result = servicoApi.PostDataAuth<ClienteVINDIResponse>(url, json);

                if (result.Errors.Count() > 0)
                {
                    var errors = "";
                    foreach (var item in result.Errors)
                    {
                        errors = errors + item.id + " - " + item.message + " - " + item.parameter;
                        AdicionarNotificacao(errors);
                        return null;
                    }
                }

                return result.Customer;

            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o cliente.");
                return null;
            }
        }

        private ClienteVINDI ObterCliente(string email)
        {

            var url = $"https://app.vindi.com.br:443/api/v1/customers?query=email:" + email;
            var result = servicoApi.GetDataVINDI<ClienteListVINDIResponse>(url);
            if (result?.Customers != null)
            {
                return result?.Customers.FirstOrDefault();
            }
            return null;


        }

        public bool SalvarPlano(string email, decimal valor, string plano, string nome, int funcionarios)
        {
            try
            {
                return clienteRepositorio.SalvarPlano(email, nome, valor, plano, funcionarios);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano.");
                return false;
            }
        }

        public Assinatura PegarPlano(string email)
        {
            try
            {
                return clienteRepositorio.PegarPlano(email);
            }
            catch (Exception ex)
            {
                AdicionarNotificacao("Erro ao salvar o simula plano." + ex.InnerException);
                return null;
            }
        }

        public object Contrato()
        {
            using (Signature signature = new Signature("D:\\sample.pdf"))
            {
                TextSignOptions options = new TextSignOptions("John Smith")
                {
                    // set Text color
                    ForeColor = Color.Red
                };
                // sign document to file
                signature.Sign("D:\\signed.pdf", options);

                return null;
            }


        }
    }
}
