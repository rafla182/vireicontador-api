using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VireiContador.API.Models.Requests
{
    public class HashRequest
    {
        public string Email { get; set; }
        public decimal Valor { get; set; }
        public string Plano { get; set; }
        public string Nome { get; set; }
        public int Funcionarios { get; set; }
    }
}
