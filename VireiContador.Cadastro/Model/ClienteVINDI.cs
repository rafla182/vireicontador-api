using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class ClineteVINDIResult { 
        public IReadOnlyList<ClienteVINDI> Customers { get; set; }
    }
    public class ClienteVINDI
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string registry_code { get; set; }
        public string code { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Metada metadata { get; set; }
        public Address address { get; set; }
        public List<Phone> phones { get; set; }

    }

    public class Phone
    {
        public string phone_type { get; set; }
        public string number { get; set; }
        public string extension { get; set; }
    }

    public class Metada
    {

    }
    public class Address
    {
        public string street { get; set; }
        public string number { get; set; }
        public string additional_details { get; set; }
        public string zipcode { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }

    public class ClienteVINDIResponse
    {
        public ClienteVINDI Customer { get; set; }
    }

    public class ClienteListVINDIResponse
    {
        public List<ClienteVINDI> Customers { get; set; }
    }
}
