using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class PlanoVINDI
    {
        public int plan_id { get; set; }
        public int customer_id { get; set; }
        public string payment_method_code { get; set; }
        public List<ItemsVINDI> product_items { get; set; }
    }

    public class ItemsVINDI
    {
        public int product_id { get; set; }

    }

    public class PlanoVINDIRequest
    {
        public PlanoVINDI Plano { get; set; }
    }
}
