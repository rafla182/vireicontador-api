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
            if (cliente.AtividadeSecundaria != null)
            {
                foreach (var atrividade in cliente.AtividadeSecundaria)
                {
                    atividadeSecundaria = atividadeSecundaria + atrividade.Codigo + " - " + atrividade.Descricao + ";";
                }
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

        public EmpresaSQL SalvarEmpresa(EmpresaSQL empresa, Assinatura assinatura, CartaoCredito cartao, List<Socio> socios, Competencia competencia)
        {
            var atividadeSecundaria = "";
            if (empresa.AtividadeSecundaria != null)
            {
                foreach (var atrividade in empresa.AtividadeSecundaria)
                {
                    atividadeSecundaria = atividadeSecundaria + atrividade.Codigo + " - " + atrividade.Descricao + ";";
                }
            }

            var sociosArray = "";
            if (socios != null)
            {
                foreach (var socio in socios)
                {
                    sociosArray = sociosArray + socio.Nome + "/" + socio.CPF + "/" + socio.Email + "/" + socio.Percentual + "/" + socio.Sexo + "/" + socio.Administrador + ";";
                }
            }

            const string sql = @"
                INSERT INTO empresa (cnpj, nome, 
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
                                    atividadePrimaria, 
                                    atividadeSecundaria,
                                    cartaoCredito,
                                    cvv,
                                    titularCartao,
                                    vencimento,     
                                    socios, 
                                    telefone2, 
                                    razaoSocial,
                                    nomeFantasia,
                                    funcionarios,
                                    regimeTributario,
                                    inscricaoEstadual, competenciaAno, competenciaMes)
                VALUES (@Cnpj,
                        @Nome, 
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
                        @TipoPagamento, 
                        @AtividadePrimaria, 
                        @AtividadeSecundaria, 
                        @CartaoCredito, 
                        @Cvv, 
                        @TitularCartao, 
                        @Vencimento,
                        @Socios,
                        @Telefone2,
                        @RazaoSocial,
                        @NomeFantasia,
                        @Funcionarios,
                        @RegimeTributario,
                        @InscricaoEstadual,
                        @CompetenciaAno,
                        @CompetenciaMes)
                ";

            var id = ExecuteQuery<int>(sql, new
            {
                Cnpj = empresa.CNPJ,
                Nome = empresa.Nome,
                Email = empresa.Email,
                Telefone = empresa.Telefone,
                Cpf = empresa.CPF,
                Logradouro = empresa.Logradouro,
                Numero = empresa.Numero,
                Complemento = empresa.Complemento,
                Cep = empresa.CEP,
                Bairro = empresa.Bairro,
                Cidade = empresa.Cidade,
                Estado = empresa.Estado,
                TipoPagamento = assinatura.TipoPagamento,
                AtividadePrimaria = empresa.AtividadePrimaria.Codigo + " - " + empresa.AtividadePrimaria.Descricao,
                AtividadeSecundaria = atividadeSecundaria,
                CartaoCredito = cartao.Numero,
                Cvv = cartao.CVV,
                TitularCartao = cartao.TitularCartao,
                Vencimento = cartao.Vencimento,
                Socios = sociosArray,
                Telefone2 = empresa.Telefone2,
                RazaoSocial = empresa.RazaoSocial,
                NomeFantasia = empresa.NomeFantasia,
                Funcionarios = empresa.Funcionarios,
                RegimeTributario = empresa.RegimeTributario,
                InscricaoEstadual = empresa.InscricaoEstadual,
                CompetenciaAno = competencia.Ano,
                CompetenciaMes = competencia.Mes
            });

            return empresa;

        }

        public Assinatura PegarPlano(string email)
        {
            var sql = $@"
                SELECT plano as descricao, valor, nome, funcionarios
                FROM vireicontador.simulaPlano 
                WHERE email = @Email
                ORDER BY dataCadastro desc
                LIMIT 1" ;

            return ExecuteQuery<Assinatura>(sql, new { Email = email });
        }

        public bool SalvarPlano(string email, string nome, decimal valor, string plano, int funcionarios)
        {
            const string sql = @"
                INSERT INTO vireicontador.simulaPlano (email, 
                                     plano, nome, valor, dataCadastro, funcionarios)
                VALUES (@Email, @Plano, @Nome, @Valor, NOW(), @Funcionarios)
                ";

            ExecuteQuery<int>(sql, new
            {
                Email = email,
                Nome = nome,
                Plano = plano,
                Valor = valor,
                Funcionarios = funcionarios
            });

            return true;

        }
    }
}
