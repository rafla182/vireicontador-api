using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using VireiContador.Infra.Configuracao;
using VireiContador.Infra.Repositories;

namespace VireiContador.Cadastro.Repositorio
{
    public class ClienteRepositorio : BaseRepository
    {
        public ClienteRepositorio(IOptions<Configuracao> configuracoes) : base(configuracoes.Value.ConnectionStringSql)
        {
        }

        public bool SalvarCliente()
        {
            return true;

        }
    }
}
