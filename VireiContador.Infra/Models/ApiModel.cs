using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Infra.Models
{
    public class ApiModel<T>
    {
        public bool sucesso { get; set; }
        public List<T> erros { get; set; }
        public T resultado { get; set; }
    }
}
