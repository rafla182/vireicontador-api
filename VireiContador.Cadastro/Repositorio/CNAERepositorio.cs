using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using VireiContador.Cadastro.Model;
using VireiContador.Infra.Configuracao;
using VireiContador.Infra.Repositories;

namespace VireiContador.Cadastro.Repositorio
{
    public class CNAERepositorio : BaseRepository
    {
        public CNAERepositorio(IOptions<Configuracao> configuracoes) : base(configuracoes.Value.ConnectionStringSql)
        {
        }

        public IReadOnlyCollection<CNAE> ListarCNAE()
        {
            var sql = $@"
                SELECT id, codigo, anexo, baixo_risco, fator_r, aliquota, atendido, descricao
                FROM vireicontador.cnae c; ";

            return ExecuteQueryList<CNAE>(sql);
        }
    }
}
