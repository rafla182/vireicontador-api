using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VireiContador.Cadastro.Model;

namespace VireiContador.API.Models.Requests
{
    public class SalvarRequest
    {
        public Cliente Cliente { get; set; }

        public Assinatura Assinatura { get; set; }

        public CartaoCredito Cartao { get; set; }
    }
}
