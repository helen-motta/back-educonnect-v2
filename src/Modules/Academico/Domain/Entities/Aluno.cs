namespace Modules.Academico.Domain.Entities
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        public Aluno(int id, string nome, string matricula)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do aluno é obrigatório");
            if (string.IsNullOrWhiteSpace(matricula))
                throw new ArgumentException("Matrícula é obrigatória");

            Id = id;
            Nome = nome;
            Matricula = matricula;
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }
    }
}
