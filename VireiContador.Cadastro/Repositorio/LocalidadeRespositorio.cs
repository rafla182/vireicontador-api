using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using VireiContador.Cadastro.Model;
using VireiContador.Infra.Configuracao;
using VireiContador.Infra.Repositories;

namespace VireiContador.Cadastro.Repositorio
{
    public class LocalidadeRespositorio : BaseRepository
    {
        public LocalidadeRespositorio(IOptions<Configuracao> configuracoes) : base(configuracoes.Value.ConnectionStringSql)
        {
        }
        public IReadOnlyCollection<Estado> PegarEstado()
        {
            var sql = $@"
                SELECT id, nome, sigla
                FROM vireicontador.estado";

            return ExecuteQueryList<Estado>(sql);
        }

        public IReadOnlyCollection<Cidade> PegarCidade(string sigla)
        {
            var sql = $@"
                SELECT id, nome, ativo, estado
                FROM vireicontador.cidade
                WHERE sigla = @Sigla";

            return ExecuteQueryList<Cidade>(sql, new { Sigla = sigla });
        }
    }
}
