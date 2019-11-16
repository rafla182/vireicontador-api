using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using VireiContador.Infra.Configuracao;
using VireiContador.Infra.Repositories;

namespace VireiContador.Cadastro.Repositorio
{
    public class EmpresaRepositorio : BaseRepository
    {
        public EmpresaRepositorio(IOptions<Configuracao> configuracoes) : base(configuracoes.Value.ConnectionStringSql)
        {
        }

    }
}
