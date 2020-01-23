using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class AssinaturaVINDI
    {
        public int plan_id { get; set; }
        public int customer_id { get; set; }
        public string payment_method_code { get; set; }
        public List<ItemsVINDI> product_items { get; set; }
        public PerfilPagamentoAssinaturaVINDI payment_profile { get; set; }
    }

    public class ItemsVINDI
    {
        public PricingVINDI pricing_schema;

        public int product_id { get; set; }
    }

    public class PricingVINDI
    {
        public decimal price { get; set; }
        public string schema_type { get; set; }
    }

    public class AssinaturaVINDIRequest
    {
        public AssinaturaRetornoVINDI Subscription { get; set; }
        public FaturaVINDI Bill { get; set; }
    }

    public class CustomerAssinaturaVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
    public class PlanVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ProductItemVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public ProductVINDI Product { get; set; }
    }

    public class ProductVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class PaymentMethod
    {
        public int id { get; set; }
        public string name { get; set; }
        public string public_name { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }
    public class AssinaturaRetornoVINDI
    {
        public int id { get; set; }
        public string status { get; set; }
        public DateTime start_at { get; set; }
        public string interval { get; set; }
        public int interval_count { get; set; }
        public string billing_trigger_type { get; set; }
        public int billing_trigger_day { get; set; }
        public int installments { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public CustomerAssinaturaVINDI customer { get; set; }
        public PlanVINDI plan { get; set; }
        public List<ProductItemVINDI> product_items { get; set; }
        public PaymentMethod payment_method { get; set; }

    }
}
