namespace Modules.Autenticacao.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
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
        public string Registro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Rg { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public bool NotificarTarefas { get; set; } = true;
        public bool NotificarAvisos { get; set; } = true;
        public bool NotificarNotas { get; set; } = true;

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
            TentativasFalhas = (TentativasFalhas ?? 0) + 1;

            if (TentativasFalhas >= limiteTentativas)
                BloqueadoAte = DateTime.UtcNow.AddMinutes(minutosDesbloqueio);
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
