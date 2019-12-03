using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class FaturaVINDI
    {
        public int customer_id { get; set; }
        public string payment_method_code { get; set; }
        public List<ItemFaturaVINDI> bill_items { get; set; }
    }

    public class ItemFaturaVINDI
    {
        public int product_id { get; set; }
        public decimal amount { get; set; }
    }

    public class FaturaVINDIRequest
    {
        public FaturaVINDI Fatura { get; set; }
    }
}
