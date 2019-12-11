using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class FaturaVINDI
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public int installments { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public List<FaturaCobrancaVINDI> charges { get; set; }
    }

    public class FaturaCobrancaVINDI
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public string due_at { get; set; }
        public int installments { get; set; }
        public string print_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        //public TransacaoVINDI last_transaction { get; set; }
    }

    public class TransacaoVINDI
    {
        public int id { get; set; }
        public string transaction_type { get; set; }
        public string status { get; set; }
        public decimal amount { get; set; }
        public int installments { get; set; }
        public string gateway_message { get; set; }
        public string gateway_response_code { get; set; }
        public string gateway_authorization { get; set; }
        public string gateway_transaction_id { get; set; }
        public DateTime created_at { get; set; }
        public GatewayVINDI gateway { get; set; }
      
    }

    public class GatewayVINDI
    {
        public int id { get; set; }
        public string connector { get; set; }
    }


}
