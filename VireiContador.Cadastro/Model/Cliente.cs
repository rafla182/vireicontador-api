using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Cliente
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public bool EmpresaCidade { get; set; }
        public CNAE AtividadePrimaria { get; set; }
        public CNAE AtividadeSecundaria { get; set; }
        public string TipoSociedade { get; set; }
        public string AtividadeDesc { get; set; }
        public bool QueroSerCliente { get; set; }
        public string TipoPagamento { get; set; }
        public CartaoCredito CartaoCredito { get; set; }
        public decimal ValorPlano { get; set; }
    }
    public class CartaoCredito
    {
        public string Numero { get; set; }
        public string CVV { get; set; }
        public string TitularCartao { get; set; }
        public string Vencimento { get; set; }
    }

}

