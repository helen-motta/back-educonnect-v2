namespace Modules.Autenticacao.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string? ResetSenhaToken { get; set; }
        public DateTime? DataExpiraTokenResetSenha { get; set; }
        public int IdPerfil { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public DateTime? DataAceiteTermos { get; set; }
        public int? VersaoTermos { get; set; }
        public int? TentativasFalhas { get; set; } = 0;
        public DateTime? BloqueadoAte { get; set; }
        public string Registro { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }

        public Usuario()
        {
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
        }

        public bool EstaAtivo()
        {
            return Ativo;
        }

        public bool EstaBloqueado()
        {
            if (BloqueadoAte == null)
                return false;

            return BloqueadoAte > DateTime.UtcNow;
        }

        public void ResetarTentativas()
        {
            BloqueadoAte = null;
            TentativasFalhas = 0;
        }

        public void IncrementarTentativasFalhas(int limiteTentativas = 5, int minutosDesbloqueio = 30)
        {
            if (TentativasFalhas >= limiteTentativas)
            {
                BloqueadoAte = DateTime.UtcNow.AddMinutes(minutosDesbloqueio);
                return;
            }
            TentativasFalhas += 1;
        }

        public void AtualizarUltimoLogin()
        {
            UltimoLogin = DateTime.UtcNow;
        }

        public bool NecessitaAceitarTermos(int versaoTermosAtual)
        {
            return DataAceiteTermos == null || VersaoTermos < versaoTermosAtual;
        }
    }
}
