using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class PerfilPagamentoRetornoVINDI
    {
        public int id { get; set; }
        public string status { get; set; }
        public string holder_name { get; set; }
        public string registry_code { get; set; }
        public string bank_branch { get; set; }
        public string bank_account { get; set; }
        public DateTime card_expiration { get; set; }
        public string card_number_first_six { get; set; }
        public string card_number_last_four { get; set; }
        public string token { get; set; }
        public string gateway_token { get; set; }
        public string type { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public PaymentCompanyVINDI payment_company { get; set; }
        public PaymentMethodVINDI payment_method { get; set; }
        public CustomerVINDI customer { get; set; }
    }

    public class PaymentCompanyVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }
    public class PaymentMethodVINDI
    {
        public int id { get; set; }
        public string public_name { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }

    public class CustomerVINDI
    {
        public int id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }

}
