using Microsoft.EntityFrameworkCore;

namespace Modules.Academico.Domain.ValueObjects
{
    [Owned]
    public class Percentual : IEquatable<Percentual>
    {
        public decimal Valor { get; private set; }
        private const decimal PERCENTUAL_MINIMO = 0m;
        private const decimal PERCENTUAL_MAXIMO = 100m;

        private Percentual(decimal valor)
        {
            if (valor < PERCENTUAL_MINIMO || valor > PERCENTUAL_MAXIMO)
                throw new ArgumentException($"Percentual deve estar entre {PERCENTUAL_MINIMO} e {PERCENTUAL_MAXIMO}");

            Valor = valor;
        }

        public static Percentual Criar(decimal valor)
        {
            return new Percentual(valor);
        }

        public bool Atende(Percentual minimo)
        {
            return Valor >= minimo.Valor;
        }

        public bool Equals(Percentual? other)
        {
            return other != null && Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Percentual);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

        public static bool operator ==(Percentual? left, Percentual? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Percentual? left, Percentual? right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(Percentual left, Percentual right)
        {
            return left.Valor < right.Valor;
        }

        public static bool operator >(Percentual left, Percentual right)
        {
            return left.Valor > right.Valor;
        }

        public override string ToString()
        {
            return $"{Valor:F2}%";
        }
    }
}
