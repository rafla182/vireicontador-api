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

        public Cliente SalvarCliente(Cliente cliente, Assinatura assinatura, CartaoCredito cartao)
        {
            var atividadeSecundaria = "";
            foreach (var atrividade in cliente.AtividadeSecundaria)
            {
                atividadeSecundaria = atividadeSecundaria + atrividade.Codigo + " - " + atrividade.Descricao + ";";
            }

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
                                    empresaEstado,
                                    empresaCidade,
                                    tipoPagamento,
                                    atividadePrimaria, 
                                    atividadeSecundaria,
                                    tipoSociedade,
                                    cartaoCredito,
                                    cvv,
                                    titularCartao,
                                    vencimento)
                VALUES (@Nome, 
                        @Email, 
                        @Telefone, 
                        @Cpf, 
                        @Logradouro, 
                        @Numero, 
                        @Complemento, 
                        @Cep, 
                        @Bairro, 
                        @Cidade, 
                        @Estado, 
                        NOW(), 
                        @EmpresaEstado,
                        @EmpresaCidade,
                        @TipoPagamento, 
                        @AtividadePrimaria, 
                        @AtividadeSecundaria, 
                        @TipoSociedade,                         
                        @CartaoCredito, 
                        @Cvv, 
                        @TitularCartao, 
                        @Vencimento)
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
                EmpresaEstado = cliente.EmpresaEstado,
                EmpresaCidade = cliente.EmpresaCidade,
                TipoPagamento = assinatura.TipoPagamento,
                AtividadePrimaria = cliente.AtividadePrimaria.Codigo + " - " + cliente.AtividadePrimaria.Descricao ,
                AtividadeSecundaria = atividadeSecundaria,
                TipoSociedade = cliente.TipoSociedade,
                CartaoCredito = cartao.Numero,
                Cvv = cartao.CVV,
                TitularCartao = cartao.TitularCartao,
                Vencimento = cartao.Vencimento
            });

            return cliente;

        }

        public Assinatura PegarPlano(string email)
        {
            var sql = $@"
                SELECT plano as descricao, valor, nome
                FROM vireicontador.simulaPlano 
                WHERE email = @Email
                ORDER BY dataCadastro desc
                LIMIT 1" ;

            return ExecuteQuery<Assinatura>(sql, new { Email = email });
        }

        public bool SalvarPlano(string email, string nome, decimal valor, string plano)
        {
            const string sql = @"
                INSERT INTO vireicontador.simulaPlano (email, 
                                     plano, nome, valor, dataCadastro)
                VALUES (@Email, @Plano, @Nome, @Valor, NOW())
                ";

            ExecuteQuery<int>(sql, new
            {
                Email = email,
                Nome = nome,
                Plano = plano,
                Valor = valor
            });

            return true;

        }
    }
}
