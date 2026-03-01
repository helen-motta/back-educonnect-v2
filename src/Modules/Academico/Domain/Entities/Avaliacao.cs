using Modules.Academico.Enums;
using Modules.Academico.Domain.ValueObjects;

namespace Modules.Academico.Domain.Entities
{
    public class Avaliacao
    {
        // EF Core
        public Avaliacao() { }

        public Avaliacao(
            int id,
            int matriculaId,
            string nome,
            decimal nota,
            TipoAvaliacao tipo,
            bool fechada)
        {
            Id = id;
            MatriculaId = matriculaId;
            Nome = nome;
            Nota = Nota.Criar(nota);
            Tipo = tipo;
            Fechada = fechada;
        }

        public int Id { get; private set; }
        public int MatriculaId { get; private set; }
        public string Nome { get; private set; }
        public Nota Nota { get; private set; }
        public TipoAvaliacao Tipo { get; private set; }
        public bool Fechada { get; private set; }

        // ========================
        // Regras de Domínio
        // ========================

        public void Fechar()
        {
            if (Fechada)
                throw new InvalidOperationException("A avaliação já está fechada");

            Fechada = true;
        }

        public void AlterarNota(decimal novaNota)
        {
            if (Fechada)
                throw new InvalidOperationException("Não é possível alterar nota de avaliação fechada");

            Nota = Nota.Criar(novaNota);
        }
    }
}
