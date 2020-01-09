using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Socio
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public int Percentual { get; set; }
        public string Sexo { get; set; }
        public bool Administrador { get; set; }
    }
}
