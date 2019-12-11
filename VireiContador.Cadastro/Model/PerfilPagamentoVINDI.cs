using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class PerfilPagamentoVINDI
    {
        public int id { get; set; }
        public string holder_name { get; set; }
        public string card_expiration { get; set; }
        public string card_number { get; set; }
        public string card_cvv { get; set; }
        public string payment_method_code { get; set; }
        public string payment_company_code { get; set; }
        public int customer_id { get; set; }
    }
    public class PerfilPagamentoVINDIRequest
    {
        public PerfilPagamentoRetornoVINDI payment_profile { get; set; }
    }

    public class PerfilPagamentoAssinaturaVINDI
    {
        public int id { get; set; }
    }

}