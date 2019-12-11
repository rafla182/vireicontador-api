using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Assinatura
    {
        public decimal Valor { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string TipoPagamento { get; set; }
    }
}
