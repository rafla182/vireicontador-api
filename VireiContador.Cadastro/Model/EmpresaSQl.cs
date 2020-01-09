using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class EmpresaSQL
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string CNPJ { get; set; }
        public string Telefone { get; set; }
        public string Telefone2 { get; set; }
        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public CNAE AtividadePrimaria { get; set; }
        public List<CNAE> AtividadeSecundaria { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string RegimeTributario { get; set; }
        public int Funcionarios { get; set; }
        public bool InscricaoEstadual { get; set; }


    }
}
