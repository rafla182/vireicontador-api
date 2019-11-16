using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Cadastro.Model
{
    public class Empresa
    {
        public string Nome { get; set; }
        public string Complemento { get; set; }

        public string Tipo { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Situacao { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string CEP { get; set; }
        public string Municipio { get; set; }
        public string Porte { get; set; }
        public string Abertura { get; set; }
        public string NaturezaJuridica { get; set; }
        public string UF{ get; set; }
        public string CapitalSocial { get; set; }
        public string Fantasia { get; set; }
        public string MotivoSituacao { get; set; }
        public string SituacaoEspecial { get; set; }
        public List<Atividade> AtividadePrincipal { get; set; }
    }

    public class Atividade
    {
        public string Text{ get; set; }
        public string Code { get; set; }
    }
}
