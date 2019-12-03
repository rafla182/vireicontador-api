using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Fatura
    {
        public string PaymentMethodCode { get; set; }
        public decimal Valor { get; set; }
        public int PlanoID { get; set; }
        public int ProdutoID { get; set; }

    }
}
