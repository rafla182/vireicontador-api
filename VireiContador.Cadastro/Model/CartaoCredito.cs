using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class CartaoCredito
    {
        public string Numero { get; set; }
        public string CVV { get; set; }
        public string TitularCartao { get; set; }
        public string Vencimento { get; set; }
    }
}
