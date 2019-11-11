using System;
using System.Collections.Generic;
using System.Text;

namespace VireiContador.Infra.Configuracao
{
    public class Configuracao
    {
        public string ConnectionStringSql { get; set; }
        public string ConnectionStringMongo { get; set; }
        public bool Desenvolvimento { get; set; }
        public string DominioSite { get; set; }
        public string DominioAdmin { get; set; }
        public string DominioArquivos { get; set; }
        public string CaminhoFisicoArquivos { get; set; }
        public string NomeDiretorioArquivosTimes { get; set; }
        public string NomeDiretorioArquivosUsuario { get; set; }
        public string NomeDiretorioArquivosCampeonato { get; set; }
        public string NomeDiretorioArquivosBolao { get; set; }
        public string DimensaoImagemUsuario { get; set; }
        public string DimensaoImagemLogo { get; set; }
        public int MinutosParaRodarJob { get; set; }
        public int FonteId { get; set; }
        public string SrGooolEndPoint { get; set; }
        public string SrGooolClientId { get; set; }
        public string SrGooolClientSecret { get; set; }
        public string SrGooolRefreshToken { get; set; }
        public string TokenSecretKey { get; set; }
    }
}
