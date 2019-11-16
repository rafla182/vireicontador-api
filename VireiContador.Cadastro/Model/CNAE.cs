using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class CNAE
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Anexo { get; set; }
        public bool BaixoRisco { get; set; }
        public bool FatorR { get; set; }
        public string Aliquota { get; set; }
        public string Descricao { get; set; }
    }
}
