using System;

namespace VireiContador.Infra.Models
{
    public class Sql
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        public Sql()
        {
            DataCriacao = new DateTime();
            Ativo = true;
        }
    }
}