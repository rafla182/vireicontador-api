﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            this.servicoApi = servicoApi;
        }
        public AssinaturaVINDIRequest Salvar(Cliente cliente, Assinatura assinatura, CartaoCredito cartao)
        {
            var clienteVINDI = ObterCliente(cliente.Email);
            var customer = new ClienteVINDI();
            if (clienteVINDI == null)
                customer = SalvarCliente(cliente, assinatura, cartao);
            else
                customer = clienteVINDI;

            if (customer == null)
                return null;

            if (assinatura.TipoPagamento == "credit_card")
            {
                var profile = SalvarProfile(cartao, customer.id);
                var assinaturaRetorno = SalvarAssinatura(assinatura, customer.id, profile.id);

                EnviarDadosCliente(cliente, assinatura);
                return assinaturaRetorno;
            }
            else
            {
                var assinaturaRetorno = SalvarAssinaturaBoleto(assinatura, customer.id);
                EnviarDadosCliente(cliente, assinatura);
                return assinaturaRetorno;
            }
        }

        public AssinaturaVINDIRequest SalvarMigrar(EmpresaSQL empresa, Assinatura assinatura, CartaoCredito cartao, List<Socio> socios, Competencia competencia)
        {
            var clienteVINDI = ObterCliente(empresa.Email);
            
            ClienteVINDI customer;
            
            customer = clienteVINDI ?? SalvarEmpresa(empresa, assinatura, cartao, socios, competencia);

            if (customer != null)
            {
                if (assinatura.TipoPagamento == "credit_card")
                {
                    var profile = SalvarProfile(cartao, customer.id);
                    var assinaturaRetorno = SalvarAssinatura(assinatura, customer.id, profile.id);
                    EnviarDadosEmpresa(empresa, assinatura, socios, competencia);

                    return assinaturaRetorno;
                }
                else
                {
                    var assinaturaRetorno = SalvarAssinaturaBoleto(assinatura, customer.id);
                    EnviarDadosEmpresa(empresa, assinatura, socios, competencia);
                    return assinaturaRetorno;
                }
            }


            return null;
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
        //    var produtos=new List<ItemFaturaVINDI>();
        //    produtos.Add(new ItemFaturaVINDI()
        //    {
        //        product_id=fatura.ProdutoID
        //    });

        //    var faturaVINDI=new FaturaVINDI()
        //    {
        //        customer_id=clienteID,
        //        bill_items=produtos,
        //        payment_method_code=fatura.TipoPagamento
        //    };

        //    var url=$"https://sandbox-app.vindi.com.br:443/api/v1/subscriptions";

        //    var json=JsonConvert.SerializeObject(faturaVINDI);
        //    var result=servicoApi.PostDataAuth<FaturaVINDIRequest>(url, json);

        //    return result.Fatura;
        //}

        private AssinaturaVINDIRequest SalvarAssinatura(Assinatura plano, int clienteID, int profileID)
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
            var phones = new List<Phone>();
            phones.Add(new Phone
            {
                phone_type = "mobile",
                number = "55" + cliente.Telefone,
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


            var url = $"https://app.vindi.com.br:443/api/v1/customers";

            var json = JsonConvert.SerializeObject(customer);
            var result = servicoApi.PostDataAuth<ClienteVINDIResponse>(url, json);

            if (result.Errors.Any())
            {
                var errors = "";
                foreach (var item in result.Errors)
                {
                    errors = errors + item.id + " - " + item.message + " - " + item.parameter;
                    AdicionarNotificacao(errors);
                }

                return null;
            }

            clienteRepositorio.SalvarCliente(cliente, assinatura, cartao);
            return result.Customer;

        }

        public ClienteVINDI SalvarEmpresa(EmpresaSQL empresa, Assinatura assinatura, CartaoCredito cartao, List<Socio> socios, Competencia competencia)
        {
            var phones = new List<Phone>();
            phones.Add(new Phone
            {
                phone_type = "landline",
                number = "55" + empresa.Telefone
            });

            phones.Add(new Phone
            {
                phone_type = "mobile",
                number = "55" + empresa.Telefone2,
            });

            var customer = new ClienteVINDI()
            {
                name = empresa.RazaoSocial,
                email = empresa.Email,
                registry_code = empresa.CNPJ,
                phones = phones
            };


            var url = $"https://app.vindi.com.br:443/api/v1/customers";

            var json = JsonConvert.SerializeObject(customer);
            var result = servicoApi.PostDataAuth<ClienteVINDIResponse>(url, json);

            if (result.Errors.Any())
            {
                var errors = "";
                foreach (var item in result.Errors)
                {
                    errors = errors + item.id + " - " + item.message + " - " + item.parameter;
                    AdicionarNotificacao(errors);
                }

                return null;
            }

            clienteRepositorio.SalvarEmpresa(empresa, assinatura, cartao, socios, competencia);
            return result.Customer;

        }

        private ClienteVINDI ObterCliente(string email)
        {

            var url = $"https://app.vindi.com.br:443/api/v1/customers?query=email:" + email;
            var result = servicoApi.GetDataVINDI<ClienteListVINDIResponse>(url);
            return result?.Customers?.FirstOrDefault();
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


        public bool Contrato(string contrato, Cliente cliente)
        {

            var bytes = Convert.FromBase64String(contrato);
            Stream stream = new MemoryStream(bytes);

            SmtpClient client = new SmtpClient("smtp.vireicontador.com.br");
            client.Credentials = new NetworkCredential("contato@vireicontador.com.br", "Contador*1");

            client.Port = 587;

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("contato@vireicontador.com.br");

            mailMessage.To.Add(new MailAddress(cliente.Email));
            mailMessage.To.Add(new MailAddress("contato@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("comercial.vireicontador@gmail.com"));
            mailMessage.To.Add(new MailAddress("comercial@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("relacionamento@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("processos@vireicontador.com.br"));


            mailMessage.Body = $@"<html>

<body>
    <table align='center' border='0' width='600' cellspacing='0' cellpadding='0'>
        <tbody>
            <tr>
                <td align='center' height='166px' style='color:#98e3ed;font-weight:400;font-size:130%;padding:20px'>
                    <span style='padding:20px'><a title='Virei Contador' href='https://www.vireicontador.com.br' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHGZg2UhCX31-nFQfXdJ3SzNwqOxQ'><img alt='Virei Contador' border='0' src='https://ci5.googleusercontent.com/proxy/G1a29tDQBHW9FN4DqaUiUrijwwM_iw7kSyGWHHxGBLUtq2cE304QnqWWzzlLXxqa66Q5wESsKDs_wg915jjH7LSZXzFnNKnilzSkDx0l2vFDN1JETOqJhbg0sFJxoTL2GV5JhSCuqIw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/logo_virei_contador_email.png' class='CToWUd'></a></span></td>
            </tr>
            <tr>
                <td align='center' style='padding:20px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:900;font-size:24px;color:#00a7ba'>Olá {cliente.Nome},</span>
                    <br>
                    <br>
                    <span style='font-weight:400;font-size:14px;color:#8d8d8d'>Parabéns pelo grande passo de tornar<br>
sua <strong>empresa</strong> mais digital, simples e nas nuvens!</span></span>
                </td>
            </tr>
            <tr bgcolor='#00a7ba'>
                <td align='center' style='padding:30px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:900;font-size:24px;color:#ffffff'>Nosso contrato</span>
                    <br>
                    <br>
                    <span style='font-weight:400;font-size:14px;color:#ffffff'>Em anexo encaminhamos o nosso
<strong>contrato</strong> e pedimos que leia para ficar a par de todos seus <strong>
direitos e deveres</strong> ao contratar nosso serviço de contabilidade.</span>
                    <br>
                    <br>
                    </span>
                </td>
            </tr>
            <tr>
                <td align='center' style='padding:20px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:400;font-size:14px;color:#8d8d8d'>Qualquer dúvida entre em contato conosco:</span>
                    <br>
                    <br>
                    </span>
                    <table border='0' width='250' cellspacing='0' cellpadding='0'>
                        <tbody>
                            <tr>
                                <td align='center' style='padding:0'>
                                    <a href='mailto:contato@vireicontador.com.br' target='_blank'><img src='https://ci6.googleusercontent.com/proxy/LzyM9WbmKTzgjDX8W0M2pvK1nz94PE9tTsK1dMIUjOK5lgbHEJwWR3RMCRGNRA7HueTsFgvyxPPPGwWJEiiOc3QVqw0SDv_XFdleXz7g64v1S6Y67C6cxag=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/email_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://www.facebook.com/vireicontador/' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.facebook.com/vireicontador/&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHT23MA_-OoYHLCv2FrQVST0cB5eA'><img src='https://ci5.googleusercontent.com/proxy/9nx54UFYN4zmCwVlQqGdx5LmtBfhE_hOIJzVgLVMCP8m3ArkNPFzk0Jpi4YDSmyBn7V9eVGecLeG6w7LZ6JVjgBsDRhnXmYpVt-B_aoFfgDKKv7BAEY-Fw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/face_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://nwdsk.co/chat-form/NDx8W' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://nwdsk.co/chat-form/NDx8W&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNEtgJheAV28vl2di7a3oiufyBxFLg'><img src='https://ci6.googleusercontent.com/proxy/faCivr2fj4YwuvmWDYTDJrApQpLzJibju9c9C-L62loddphKet_Es_mUNyuY8Zb0HY-c7I-HaRUYZypUo4CnHM1B739T_4Hepg1n0k_ozhuchr4FDsdXkg=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/chat_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://api.whatsapp.com/send?phone=5527995244950' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://api.whatsapp.com/send?phone%3D5527995244950&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNE083d2DOhWvegq0P-X7DgKqwzyaw'><img src='https://ci3.googleusercontent.com/proxy/A4K6sWG47oZjC9UY2LLfcX9e0PKTJeKXjKQRO1LEN5vDvevahCpMV5HvH2QYhDztr6RxfVrJdFy--dOdW9j2DdHQNSxTtmvn2sFhMxCOsgOyIfhVbRUY4n0RhHM=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/whatsapp_icon.png' class='CToWUd'></a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><a href='https://vireicontador.com.br' rel='noopener noreferrer' style='color:#00a7ba;text-decoration:none' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNGEmvCgOrNqYQIdv-lfmlxYYrOwog'><span style='font-weight:600;font-size:12px;color:#00a7ba;line-height:22px'>vireicontador.com.br</span></a>
                    </span>
                </td>
            </tr>
        </tbody>
    </table>
</body>

</html>";
            mailMessage.Subject = "Seu contrato de serviços VIREI CONTADOR";

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();

            mailMessage.Attachments.Add(new Attachment(stream, "contrato.pdf"));

            client.Send(mailMessage);

            return true;



        }

        public bool MigrarContrato(string contrato, Empresa empresa)
        {

            var bytes = Convert.FromBase64String(contrato);
            Stream stream = new MemoryStream(bytes);

            SmtpClient client = new SmtpClient("smtp.vireicontador.com.br");
            client.Credentials = new NetworkCredential("contato@vireicontador.com.br", "Contador*1");

            client.Port = 587;

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("contato@vireicontador.com.br");
            mailMessage.To.Add(new MailAddress(empresa.Email));
            mailMessage.To.Add(new MailAddress("contato@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("comercial.vireicontador@gmail.com"));
            mailMessage.To.Add(new MailAddress("comercial@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("relacionamento@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("processos@vireicontador.com.br"));
            mailMessage.Body = $@"<html>

<body>
    <table align='center' border='0' width='600' cellspacing='0' cellpadding='0'>
        <tbody>
            <tr>
                <td align='center' height='166px' style='color:#98e3ed;font-weight:400;font-size:130%;padding:20px'>
                    <span style='padding:20px'><a title='Virei Contador' href='https://www.vireicontador.com.br' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHGZg2UhCX31-nFQfXdJ3SzNwqOxQ'><img alt='Virei Contador' border='0' src='https://ci5.googleusercontent.com/proxy/G1a29tDQBHW9FN4DqaUiUrijwwM_iw7kSyGWHHxGBLUtq2cE304QnqWWzzlLXxqa66Q5wESsKDs_wg915jjH7LSZXzFnNKnilzSkDx0l2vFDN1JETOqJhbg0sFJxoTL2GV5JhSCuqIw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/logo_virei_contador_email.png' class='CToWUd'></a></span></td>
            </tr>
            <tr>
                <td align='center' style='padding:20px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:900;font-size:24px;color:#00a7ba'>Olá {empresa.Nome},</span>
                    <br>
                    <br>
                    <span style='font-weight:400;font-size:14px;color:#8d8d8d'>Parabéns pelo grande passo de tornar<br>
sua <strong>empresa</strong> mais digital, simples e nas nuvens!</span></span>
                </td>
            </tr>
            <tr bgcolor='#00a7ba'>
                <td align='center' style='padding:30px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:900;font-size:24px;color:#ffffff'>Nosso contrato</span>
                    <br>
                    <br>
                    <span style='font-weight:400;font-size:14px;color:#ffffff'>Em anexo encaminhamos o nosso
<strong>contrato</strong> e pedimos que leia para ficar a par de todos seus <strong>
direitos e deveres</strong> ao contratar nosso serviço de contabilidade.</span>
                    <br>
                    <br>
                    </span>
                </td>
            </tr>
            <tr>
                <td align='center' style='padding:20px'><span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><span style='font-weight:400;font-size:14px;color:#8d8d8d'>Qualquer dúvida entre em contato conosco:</span>
                    <br>
                    <br>
                    </span>
                    <table border='0' width='250' cellspacing='0' cellpadding='0'>
                        <tbody>
                            <tr>
                                <td align='center' style='padding:0'>
                                    <a href='mailto:contato@vireicontador.com.br' target='_blank'><img src='https://ci6.googleusercontent.com/proxy/LzyM9WbmKTzgjDX8W0M2pvK1nz94PE9tTsK1dMIUjOK5lgbHEJwWR3RMCRGNRA7HueTsFgvyxPPPGwWJEiiOc3QVqw0SDv_XFdleXz7g64v1S6Y67C6cxag=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/email_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://www.facebook.com/vireicontador/' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.facebook.com/vireicontador/&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHT23MA_-OoYHLCv2FrQVST0cB5eA'><img src='https://ci5.googleusercontent.com/proxy/9nx54UFYN4zmCwVlQqGdx5LmtBfhE_hOIJzVgLVMCP8m3ArkNPFzk0Jpi4YDSmyBn7V9eVGecLeG6w7LZ6JVjgBsDRhnXmYpVt-B_aoFfgDKKv7BAEY-Fw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/face_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://nwdsk.co/chat-form/NDx8W' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://nwdsk.co/chat-form/NDx8W&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNEtgJheAV28vl2di7a3oiufyBxFLg'><img src='https://ci6.googleusercontent.com/proxy/faCivr2fj4YwuvmWDYTDJrApQpLzJibju9c9C-L62loddphKet_Es_mUNyuY8Zb0HY-c7I-HaRUYZypUo4CnHM1B739T_4Hepg1n0k_ozhuchr4FDsdXkg=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/chat_icon.png' class='CToWUd'></a>
                                </td>
                                <td style='padding:5px'>&nbsp;</td>
                                <td align='center' style='padding:0'>
                                    <a href='https://api.whatsapp.com/send?phone=5527995244950' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://api.whatsapp.com/send?phone%3D5527995244950&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNE083d2DOhWvegq0P-X7DgKqwzyaw'><img src='https://ci3.googleusercontent.com/proxy/A4K6sWG47oZjC9UY2LLfcX9e0PKTJeKXjKQRO1LEN5vDvevahCpMV5HvH2QYhDztr6RxfVrJdFy--dOdW9j2DdHQNSxTtmvn2sFhMxCOsgOyIfhVbRUY4n0RhHM=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/whatsapp_icon.png' class='CToWUd'></a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'><a href='https://vireicontador.com.br' rel='noopener noreferrer' style='color:#00a7ba;text-decoration:none' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNGEmvCgOrNqYQIdv-lfmlxYYrOwog'><span style='font-weight:600;font-size:12px;color:#00a7ba;line-height:22px'>vireicontador.com.br</span></a>
                    </span>
                </td>
            </tr>
        </tbody>
    </table>
</body>

</html>";
            mailMessage.Subject = "Seu contrato de serviços VIREI CONTADOR";

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();

            mailMessage.Attachments.Add(new Attachment(stream, "contrato.pdf"));

            client.Send(mailMessage);

            return true;



        }



        public bool EnviarDadosCliente(Cliente cliente, Assinatura assinatura)
        {
            var atividadeSecundaria = "";
            if (cliente.AtividadeSecundaria != null)
            {
                foreach (var atrividade in cliente.AtividadeSecundaria)
                {
                    atividadeSecundaria = atividadeSecundaria + atrividade.Codigo + " - " + atrividade.Descricao + ";";
                }
            }

            SmtpClient client = new SmtpClient("smtp.vireicontador.com.br");
            client.Credentials = new NetworkCredential("contato@vireicontador.com.br", "Contador*1");
            client.Port = 587;

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("contato@vireicontador.com.br");
            mailMessage.To.Add(new MailAddress("contato@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("comercial.vireicontador@gmail.com"));
            mailMessage.To.Add(new MailAddress("comercial@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("relacionamento@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("processos@vireicontador.com.br"));


            mailMessage.Body = $@"<html>
	<body>
		<table align='center' border='0' width='600' cellspacing='0' cellpadding='0'>
			<tbody>
				<tr>
					<td align='center' height='166px' style='color:#98e3ed;font-weight:400;font-size:130%;padding:20px'>
						<span style='padding:20px'>
							<a title='Virei Contador' href='https://www.vireicontador.com.br' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHGZg2UhCX31-nFQfXdJ3SzNwqOxQ'>
								<img alt='Virei Contador' border='0' src='https://ci5.googleusercontent.com/proxy/G1a29tDQBHW9FN4DqaUiUrijwwM_iw7kSyGWHHxGBLUtq2cE304QnqWWzzlLXxqa66Q5wESsKDs_wg915jjH7LSZXzFnNKnilzSkDx0l2vFDN1JETOqJhbg0sFJxoTL2GV5JhSCuqIw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/logo_virei_contador_email.png' class='CToWUd'>
								</a>
							</span>
						</td>
					</tr>
					<tr>
						<td align='center' style='padding:20px'>
							<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
								<span style='font-weight:900;font-size:24px;color:#00a7ba'>A abertura de nova empresa foi solicitada.</span>
								<br>
									<br>
										<span style='font-weight:400;font-size:14px;color:#8d8d8d'></span>
									</span>
								</td>
							</tr>
							<tr bgcolor='#00a7ba'>
								<td align='left' style='padding:30px'>
									<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
										<span style='font-weight:900;font-size:24px;color:#ffffff'>Dados do cliente</span>
										<br>
											<br>
												<span style='font-weight:400;font-size:14px;color:#ffffff'>Nome: {cliente.Nome} 

													<br/>
CPF: {cliente.CPF}

													<br/>
Telefone: {cliente.Telefone}

													<br/>
CEP: {cliente.CEP}

													<br/>
Estado: {cliente.Estado}

													<br/>
Cidade: {cliente.Cidade}

													<br/>
Bairro: {cliente.Bairro}

													<br/>
Logradouro: {cliente.Logradouro}

													<br/>
Numero: {cliente.Numero}

													<br/>
Complemento: {cliente.Complemento}

													<br/>
Empresa Cidade: {cliente.EmpresaCidade}

													<br/>
Empresa Estado: {cliente.EmpresaEstado}

													<br/>
Atividade Primária: {cliente.AtividadePrimaria.Descricao}

													<br/>
Atividades Secundárias: {atividadeSecundaria}

													<br/>
Tipo Sociedade: {cliente.TipoSociedade}

												    </span>
													    </span>
												    </td>
    </tr>
<tr bgcolor='#00a7ba'>
								<td align='left' style='padding:30px'>
									<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
										<span style='font-weight:900;font-size:24px;color:#ffffff'>Assinatura</span>
										<br>
											<br>
												<span style='font-weight:400;font-size:14px;color:#ffffff'>Nome: {assinatura.Nome} 

													<br/>
Descrição: {assinatura.Descricao}

													<br/>
Valor: {assinatura.Valor}

													<br/>
Tipo Pagamento: {(assinatura.TipoPagamento == "bank_slip" ? "Boleto" : "Cartão de Crédito")}

                                                    < br/>
Funcionarios: {assinatura.Funcionarios}

													<br/>
												</span>
												<br>
													<br>
													</span>
												</td>
												</span>
													</span>
												</td>
</tr>
											<tr>
												<td align='center' style='padding:20px'>
													<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
														<span style='font-weight:400;font-size:14px;color:#8d8d8d'>Qualquer dúvida entre em contato conosco:</span>
														<br>
															<br>
															</span>
															<table border='0' width='250' cellspacing='0' cellpadding='0'>
																<tbody>
																	<tr>
																		<td align='center' style='padding:0'>
																			<a href='mailto:contato@vireicontador.com.br' target='_blank'>
																				<img src='https://ci6.googleusercontent.com/proxy/LzyM9WbmKTzgjDX8W0M2pvK1nz94PE9tTsK1dMIUjOK5lgbHEJwWR3RMCRGNRA7HueTsFgvyxPPPGwWJEiiOc3QVqw0SDv_XFdleXz7g64v1S6Y67C6cxag=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/email_icon.png' class='CToWUd'>
																				</a>
																			</td>
																			<td style='padding:5px'>&nbsp;</td>
																			<td align='center' style='padding:0'>
																				<a href='https://www.facebook.com/vireicontador/' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.facebook.com/vireicontador/&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHT23MA_-OoYHLCv2FrQVST0cB5eA'>
																					<img src='https://ci5.googleusercontent.com/proxy/9nx54UFYN4zmCwVlQqGdx5LmtBfhE_hOIJzVgLVMCP8m3ArkNPFzk0Jpi4YDSmyBn7V9eVGecLeG6w7LZ6JVjgBsDRhnXmYpVt-B_aoFfgDKKv7BAEY-Fw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/face_icon.png' class='CToWUd'>
																					</a>
																				</td>
																				<td style='padding:5px'>&nbsp;</td>
																				<td align='center' style='padding:0'>
																					<a href='https://nwdsk.co/chat-form/NDx8W' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://nwdsk.co/chat-form/NDx8W&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNEtgJheAV28vl2di7a3oiufyBxFLg'>
																						<img src='https://ci6.googleusercontent.com/proxy/faCivr2fj4YwuvmWDYTDJrApQpLzJibju9c9C-L62loddphKet_Es_mUNyuY8Zb0HY-c7I-HaRUYZypUo4CnHM1B739T_4Hepg1n0k_ozhuchr4FDsdXkg=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/chat_icon.png' class='CToWUd'>
																						</a>
																					</td>
																					<td style='padding:5px'>&nbsp;</td>
																					<td align='center' style='padding:0'>
																						<a href='https://api.whatsapp.com/send?phone=5527995244950' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://api.whatsapp.com/send?phone%3D5527995244950&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNE083d2DOhWvegq0P-X7DgKqwzyaw'>
																							<img src='https://ci3.googleusercontent.com/proxy/A4K6sWG47oZjC9UY2LLfcX9e0PKTJeKXjKQRO1LEN5vDvevahCpMV5HvH2QYhDztr6RxfVrJdFy--dOdW9j2DdHQNSxTtmvn2sFhMxCOsgOyIfhVbRUY4n0RhHM=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/whatsapp_icon.png' class='CToWUd'>
																							</a>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																			<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
																				<a href='https://vireicontador.com.br' rel='noopener noreferrer' style='color:#00a7ba;text-decoration:none' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNGEmvCgOrNqYQIdv-lfmlxYYrOwog'>
																					<span style='font-weight:600;font-size:12px;color:#00a7ba;line-height:22px'>vireicontador.com.br</span>
																				</a>
																			</span>
																		</td>
																	</tr>
																</tbody>
															</table>
														</body>
													</html>";
            mailMessage.Subject = "Abertura de Nova empresa";

            client.Send(mailMessage);

            return true;



        }


        public bool EnviarDadosEmpresa(EmpresaSQL empresa, Assinatura assinatura, List<Socio> socios, Competencia competencia)
        {
            var atividadeSecundaria = "";
            if (empresa.AtividadeSecundaria != null)
            {
                foreach (var atrividade in empresa.AtividadeSecundaria)
                {
                    atividadeSecundaria = atividadeSecundaria + atrividade.Codigo + " - " + atrividade.Descricao + ";";
                }
            }

            SmtpClient client = new SmtpClient("smtp.vireicontador.com.br");
            client.Credentials = new NetworkCredential("contato@vireicontador.com.br", "Contador*1");
            client.Port = 587;

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("contato@vireicontador.com.br");
            mailMessage.To.Add(new MailAddress("contato@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("comercial.vireicontador@gmail.com"));
            mailMessage.To.Add(new MailAddress("comercial@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("relacionamento@vireicontador.com.br"));
            mailMessage.To.Add(new MailAddress("processos@vireicontador.com.br"));


            mailMessage.Body = $@"<html>
	<body>
		<table align='center' border='0' width='600' cellspacing='0' cellpadding='0'>
			<tbody>
				<tr>
					<td align='center' height='166px' style='color:#98e3ed;font-weight:400;font-size:130%;padding:20px'>
						<span style='padding:20px'>
							<a title='Virei Contador' href='https://www.vireicontador.com.br' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHGZg2UhCX31-nFQfXdJ3SzNwqOxQ'>
								<img alt='Virei Contador' border='0' src='https://ci5.googleusercontent.com/proxy/G1a29tDQBHW9FN4DqaUiUrijwwM_iw7kSyGWHHxGBLUtq2cE304QnqWWzzlLXxqa66Q5wESsKDs_wg915jjH7LSZXzFnNKnilzSkDx0l2vFDN1JETOqJhbg0sFJxoTL2GV5JhSCuqIw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/logo_virei_contador_email.png' class='CToWUd'>
								</a>
							</span>
						</td>
					</tr>
					<tr>
						<td align='center' style='padding:20px'>
							<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
								<span style='font-weight:900;font-size:24px;color:#00a7ba'>A migração de uma nova empresa foi solicitada.</span>
								<br>
									<br>
										<span style='font-weight:400;font-size:14px;color:#8d8d8d'></span>
									</span>
								</td>
							</tr>
							<tr bgcolor='#00a7ba'>
								<td align='left' style='padding:30px'>
									<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
										<span style='font-weight:900;font-size:24px;color:#ffffff'>Dados da empresa</span>
										<br>
											<br>
												<span style='font-weight:400;font-size:14px;color:#ffffff'>Nome: {empresa.Nome} 

													<br/>
CNPJ: {empresa.CNPJ}

													<br/>
Telefone: {empresa.Telefone}

													<br/>
CEP: {empresa.CEP}

													<br/>
Estado: {empresa.Estado}

													<br/>
Cidade: {empresa.Cidade}

													<br/>
Bairro: {empresa.Bairro}

													<br/>
Logradouro: {empresa.Logradouro}

													<br/>
Numero: {empresa.Numero}

													<br/>
Complemento: {empresa.Complemento}

													<br/>
Inscrição Estadual: {empresa.InscricaoEstadual}
													<br/>
Razão Social: {empresa.RazaoSocial}

													<br/>
Funcionarios: {empresa.Funcionarios}

													<br/>
Atividade Primária: {empresa.AtividadePrimaria.Descricao}

													<br/>
Atividades Secundárias: {atividadeSecundaria}

													<br/>
Regime Tributário : {empresa.RegimeTributario}

												    </span>
													    </span>
												    </td>
    </tr>
<tr bgcolor='#00a7ba'>
								<td align='left' style='padding:30px'>
									<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
										<span style='font-weight:900;font-size:24px;color:#ffffff'>Assinatura</span>
										<br>
											<br>
												<span style='font-weight:400;font-size:14px;color:#ffffff'>Nome: {assinatura.Nome} 

													<br/>
Descrição: {assinatura.Descricao}

													<br/>
Valor: {assinatura.Valor}

													<br/>
Tipo Pagamento: {(assinatura.TipoPagamento == "bank_slip" ? "Boleto" : "Cartão de Crédito")}

													<br/>
Funcionarios: {assinatura.Funcionarios}

													<br/>
												</span>
													</span>
												</td>
												</span>
													</span>
												</td>
</tr>
<tr bgcolor='#00a7ba'>
								<td align='left' style='padding:30px'>
									<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
										<span style='font-weight:900;font-size:24px;color:#ffffff'>Competência</span>
										<br>
											<br>
												<span style='font-weight:400;font-size:14px;color:#ffffff'>Mês: {competencia.Mes} 

													<br/>
Ano: {competencia.Ano}

													<br/>
												</span>
												<br>
													<br>
													</span>
												</td>
												</span>
													</span>
												</td>
</tr>
											<tr>
												<td align='center' style='padding:20px'>
													<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
														<span style='font-weight:400;font-size:14px;color:#8d8d8d'>Qualquer dúvida entre em contato conosco:</span>
														<br>
															<br>
															</span>
															<table border='0' width='250' cellspacing='0' cellpadding='0'>
																<tbody>
																	<tr>
																		<td align='center' style='padding:0'>
																			<a href='mailto:contato@vireicontador.com.br' target='_blank'>
																				<img src='https://ci6.googleusercontent.com/proxy/LzyM9WbmKTzgjDX8W0M2pvK1nz94PE9tTsK1dMIUjOK5lgbHEJwWR3RMCRGNRA7HueTsFgvyxPPPGwWJEiiOc3QVqw0SDv_XFdleXz7g64v1S6Y67C6cxag=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/email_icon.png' class='CToWUd'>
																				</a>
																			</td>
																			<td style='padding:5px'>&nbsp;</td>
																			<td align='center' style='padding:0'>
																				<a href='https://www.facebook.com/vireicontador/' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.facebook.com/vireicontador/&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNHT23MA_-OoYHLCv2FrQVST0cB5eA'>
																					<img src='https://ci5.googleusercontent.com/proxy/9nx54UFYN4zmCwVlQqGdx5LmtBfhE_hOIJzVgLVMCP8m3ArkNPFzk0Jpi4YDSmyBn7V9eVGecLeG6w7LZ6JVjgBsDRhnXmYpVt-B_aoFfgDKKv7BAEY-Fw=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/face_icon.png' class='CToWUd'>
																					</a>
																				</td>
																				<td style='padding:5px'>&nbsp;</td>
																				<td align='center' style='padding:0'>
																					<a href='https://nwdsk.co/chat-form/NDx8W' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://nwdsk.co/chat-form/NDx8W&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNEtgJheAV28vl2di7a3oiufyBxFLg'>
																						<img src='https://ci6.googleusercontent.com/proxy/faCivr2fj4YwuvmWDYTDJrApQpLzJibju9c9C-L62loddphKet_Es_mUNyuY8Zb0HY-c7I-HaRUYZypUo4CnHM1B739T_4Hepg1n0k_ozhuchr4FDsdXkg=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/chat_icon.png' class='CToWUd'>
																						</a>
																					</td>
																					<td style='padding:5px'>&nbsp;</td>
																					<td align='center' style='padding:0'>
																						<a href='https://api.whatsapp.com/send?phone=5527995244950' rel='noopener noreferrer' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://api.whatsapp.com/send?phone%3D5527995244950&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNE083d2DOhWvegq0P-X7DgKqwzyaw'>
																							<img src='https://ci3.googleusercontent.com/proxy/A4K6sWG47oZjC9UY2LLfcX9e0PKTJeKXjKQRO1LEN5vDvevahCpMV5HvH2QYhDztr6RxfVrJdFy--dOdW9j2DdHQNSxTtmvn2sFhMxCOsgOyIfhVbRUY4n0RhHM=s0-d-e1-ft#https://vireicontador.com.br/wp-content/uploads/2018/05/whatsapp_icon.png' class='CToWUd'>
																							</a>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																			<span style='font-family:' Open Sans ',Verdana,Geneva,sans-serif'>
																				<a href='https://vireicontador.com.br' rel='noopener noreferrer' style='color:#00a7ba;text-decoration:none' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://vireicontador.com.br&amp;source=gmail&amp;ust=1582135341064000&amp;usg=AFQjCNGEmvCgOrNqYQIdv-lfmlxYYrOwog'>
																					<span style='font-weight:600;font-size:12px;color:#00a7ba;line-height:22px'>vireicontador.com.br</span>
																				</a>
																			</span>
																		</td>
																	</tr>
																</tbody>
															</table>
														</body>
													</html>";
            mailMessage.Subject = "Migrou uma nova empresa";

            client.Send(mailMessage);

            return true;



        }

    }
}
