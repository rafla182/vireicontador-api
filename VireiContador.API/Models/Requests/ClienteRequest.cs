using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VireiContador.Cadastro.Model;

namespace VireiContador.API.Models.Requests
{
    public class ClienteRequest
    {
        public Customer Cliente { get; set; }
    }
}
