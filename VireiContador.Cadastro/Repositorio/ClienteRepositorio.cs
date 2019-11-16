using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using VireiContador.Cadastro.Model;
using VireiContador.Infra.Configuracao;
using VireiContador.Infra.Repositories;

namespace VireiContador.Cadastro.Repositorio
{
    public class ClienteRepositorio : BaseRepository
    {
        public ClienteRepositorio(IOptions<Configuracao> configuracoes) : base(configuracoes.Value.ConnectionStringSql)
        {
        }

        public Cliente SalvarCliente(Cliente cliente)
        {
            const string sql = @"
                INSERT INTO cliente (nome, 
                                     email, 
                                    telefone, 
                                    cpf, 
                                    logradouro, 
                                    numero, 
                                    complemento, 
                                    cep, 
                                    bairro,     
                                    cidade, 
                                    estado, 
                                    dataCadastro, 
                                    tipoPagamento, 
                                    empresaCidade, 
                                    atividadePrimariaId, 
                                    atividadeSecundariaId,
                                    tipoSociedade,
                                    atividadeDesc,
                                    queroSerCliente,
                                    cartaoCredito,
                                    cvv,
                                    titularCartao,
                                    vencimento)
                VALUES (@Nome, @Email, @Telefone, @Cpf, @Logradouro, @Numero, @Complemento, @Cep, @Bairro, @Cidade, @Estado, NOW(), @TipoPagamento, @EmpresaCidade, @AtividadePrimariaId, @AtividadeSecundariaId, @TipoSociedade, @AtividadeDesc, @QueroSerCliente, @CartaoCredito, @Cvv, @TitularCartao, @Vencimento)
                ";

            var id = ExecuteQuery<int>(sql, new
            {
                Nome = cliente.Nome,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                Cpf = cliente.CPF,
                Logradouro = cliente.Logradouro,
                Numero = cliente.Numero,
                Complemento = cliente.Complemento,
                Cep = cliente.CEP,
                Bairro = cliente.Bairro,
                Cidade = cliente.Cidade,
                Estado = cliente.Estado,
                TipoPagamento = cliente.TipoPagamento,
                EmpresaCidade = cliente.EmpresaCidade,
                AtividadePrimariaId = cliente.AtividadePrimaria.Id,
                AtividadeSecundariaId = cliente.AtividadeSecundaria.Id,
                TipoSociedade = cliente.TipoSociedade,
                AtividadeDesc = cliente.AtividadeDesc,
                QueroSerCliente = cliente.QueroSerCliente,
                CartaoCredito = cliente.CartaoCredito.Numero,
                Cvv = cliente.CartaoCredito.CVV,
                TitularCartao = cliente.CartaoCredito.TitularCartao,
                Vencimento = cliente.CartaoCredito.Vencimento
            });

            return cliente;

        }
    }
}
