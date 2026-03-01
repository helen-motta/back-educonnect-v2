using Microsoft.EntityFrameworkCore;

namespace Modules.Academico.Domain.ValueObjects
{
    [Owned]
    public class Nota : IEquatable<Nota>
    {
        public decimal Valor { get; private set; }
        private const decimal NOTA_MINIMA = 0m;
        private const decimal NOTA_MAXIMA = 10m;

        private Nota(decimal valor)
        {
            if (valor < NOTA_MINIMA || valor > NOTA_MAXIMA)
                throw new ArgumentException($"Nota deve estar entre {NOTA_MINIMA} e {NOTA_MAXIMA}");

            Valor = valor;
        }

        public static Nota Criar(decimal valor)
        {
            return new Nota(valor);
        }

        public bool Equals(Nota? other)
        {
            return other != null && Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Nota);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

        public static bool operator ==(Nota? left, Nota? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Nota? left, Nota? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Valor.ToString("F2");
        }
    }
}
