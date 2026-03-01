using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DesempenhoAcademicoMock
{
    // ==========================
    // ENUMS
    // ==========================
    public enum TipoAvaliacao
    {
        Prova = 1,
        Trabalho = 2
    }

    public enum SituacaoAcademica
    {
        Aprovado,
        ReprovadoPorNota,
        ReprovadoPorFrequencia,
        ReprovadoPorNotaEFrequencia
    }

    // ==========================
    // VALUE OBJECTS
    // ==========================
    [Owned]
    public class Nota
    {
        public decimal Valor { get; }

        private Nota(decimal valor)
        {
            if (valor < 0 || valor > 10)
                throw new ArgumentException("Nota deve estar entre 0 e 10");

            Valor = valor;
        }

        public static Nota Criar(decimal valor) => new Nota(valor);
    }

    public class Percentual
    {
        public decimal Valor { get; }

        private Percentual(decimal valor)
        {
            if (valor < 0 || valor > 100)
                throw new ArgumentException("Percentual deve estar entre 0 e 100");

            Valor = valor;
        }

        public static Percentual Criar(decimal valor) => new Percentual(valor);
    }

    // ==========================
    // ENTIDADES
    // ==========================
    public class Avaliacao
    {
        public int Id { get; }
        public int MatriculaId { get; }
        public string Nome { get; }
        public Nota Nota { get; }
        public TipoAvaliacao Tipo { get; }
        public bool Fechada { get; }

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
    }

    public class Frequencia
    {
        public int MatriculaId { get; }
        public int AulasAssistidas { get; }
        public int TotalAulas { get; }
        public bool Consolidada { get; }

        public Frequencia(int matriculaId, int aulasAssistidas, int totalAulas, bool consolidada)
        {
            MatriculaId = matriculaId;
            AulasAssistidas = aulasAssistidas;
            TotalAulas = totalAulas;
            Consolidada = consolidada;
        }

        public Percentual CalcularPercentual()
        {
            var percentual = (decimal)AulasAssistidas / TotalAulas * 100;
            return Percentual.Criar(percentual);
        }
    }

    public class Disciplina
    {
        public int Id { get; }
        public string Nome { get; }
        public Nota NotaMinima { get; }
        public Percentual FrequenciaMinima { get; }

        public Disciplina(int id, string nome, Nota notaMinima, Percentual frequenciaMinima)
        {
            Id = id;
            Nome = nome;
            NotaMinima = notaMinima;
            FrequenciaMinima = frequenciaMinima;
        }
    }

    // ==========================
    // RESULTADO
    // ==========================
    public record ResultadoDesempenho(
        Nota MediaFinal,
        Percentual Frequencia,
        SituacaoAcademica Situacao
    );

    // ==========================
    // SERVICE
    // ==========================
    public class DesempenhoAcademicoService
    {
        private const int PESO_PROVAS = 7;
        private const int PESO_TRABALHO = 3;

        public ResultadoDesempenho Calcular(
            Disciplina disciplina,
            IEnumerable<Avaliacao> avaliacoes,
            Frequencia frequencia)
        {
            Validar(avaliacoes, frequencia);

            var media = CalcularMedia(avaliacoes);
            var freq = frequencia.CalcularPercentual();
            var situacao = DeterminarSituacao(media, freq, disciplina);

            return new ResultadoDesempenho(media, freq, situacao);
        }

        private void Validar(IEnumerable<Avaliacao> avaliacoes, Frequencia frequencia)
        {
            if (!avaliacoes.Any())
                throw new Exception("Nenhuma avaliação");

            if (!avaliacoes.All(a => a.Fechada))
                throw new Exception("Avaliações abertas");

            if (!frequencia.Consolidada)
                throw new Exception("Frequência não consolidada");

            if (avaliacoes.Count(a => a.Tipo == TipoAvaliacao.Prova) != 2)
                throw new Exception("Devem existir duas provas");

            if (avaliacoes.Count(a => a.Tipo == TipoAvaliacao.Trabalho) != 1)
                throw new Exception("Deve existir um trabalho");
        }

        private Nota CalcularMedia(IEnumerable<Avaliacao> avaliacoes)
        {
            var provas = avaliacoes
                .Where(a => a.Tipo == TipoAvaliacao.Prova)
                .Select(a => a.Nota.Valor)
                .Average();

            var trabalho = avaliacoes
                .Single(a => a.Tipo == TipoAvaliacao.Trabalho)
                .Nota.Valor;

            var mediaFinal =
                (provas * PESO_PROVAS + trabalho * PESO_TRABALHO)
                / (PESO_PROVAS + PESO_TRABALHO);

            return Nota.Criar(mediaFinal);
        }

        private SituacaoAcademica DeterminarSituacao(
            Nota media,
            Percentual frequencia,
            Disciplina disciplina)
        {
            var reprovadoNota = media.Valor < disciplina.NotaMinima.Valor;
            var reprovadoFreq = frequencia.Valor < disciplina.FrequenciaMinima.Valor;

            if (reprovadoNota && reprovadoFreq)
                return SituacaoAcademica.ReprovadoPorNotaEFrequencia;

            if (reprovadoNota)
                return SituacaoAcademica.ReprovadoPorNota;

            if (reprovadoFreq)
                return SituacaoAcademica.ReprovadoPorFrequencia;

            return SituacaoAcademica.Aprovado;
        }
    }
}