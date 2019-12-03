using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Plano
    {
        public decimal Valor { get; set; }
        public string PlanoText { get; set; }
        public int PlanoID { get; set; }
        public int ProdutoID { get; set; }
        public string PaymentMethodCode { get; set; }
    }
}
