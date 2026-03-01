using System.Threading.Tasks;
using Modules.Academico.Domain.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing.Layout;

namespace Modules.Academico.Application.UseCases
{
    public class DocumentoUseCase
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly AppDbContext _dbContext;

        public DocumentoUseCase(IDocumentoRepository documentoRepository, AppDbContext dbContext)
        {
            _documentoRepository = documentoRepository;
            _dbContext = dbContext;
        }

        public async Task<byte[]> GerarAtestadoMatricula(int alunoId)
        {
            // Buscar dados do usuário/aluno do banco de dados
            var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == alunoId);
            
            if (usuario == null)
            {
                throw new InvalidOperationException($"Usuário com ID {alunoId} não encontrado.");
            }

            // Dados obtidos do banco
            var nomeAluno = usuario.Nome ?? "Nome não disponível";
            var rg = usuario.Rg ?? "RG não cadastrado";
            var codigoUsp = usuario.Registro ?? "Código USP não disponível";
            var endereco = MontarEndereco(usuario);
            var semestreIdeal = "8";
            var semestreMaximo = "12";
            var modalidade = "EP";
            var codigoAuth = GerarCodigoAutenticidade();
            var validadeDoc = DateTime.Now.AddMonths(6).ToString("dd/MM/yyyy");
            var dataEmissao = DateTime.Now.ToString("dd/MM/yyyy");
            var horaEmissao = DateTime.Now.ToString("HH:mm:ss");
            var curso = "Bacharelado em Matemática Aplicada e Computacional";

            // Criar documento PDF usando PdfSharp
            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    int yPosition = 20;
                    const int margemEsquerda = 30;
                    const int margemDireita = 30;
                    const int larguraConteudo = 595 - margemEsquerda - margemDireita; // A4 width - margens

                    var fonteTitulo = new XFont("Arial", 12,  XFontStyleEx.Bold);
                    var fonteSubtitulo = new XFont("Arial", 11, XFontStyleEx.Bold);
                    var fonteNormal = new XFont("Arial", 10, XFontStyleEx.Regular);
                    var fontePequena = new XFont("Arial", 9, XFontStyleEx.Regular);
                    var fontePequenaBold = new XFont("Arial", 9, XFontStyleEx.Bold);

                    // CABEÇALHO
                    gfx.DrawString("Júpiter - Sistema de Gestão Acadêmica da Pró-Reitoria de Graduação", 
                        fonteSubtitulo, new XSolidBrush(XColors.Black), new XRect(margemEsquerda, yPosition, larguraConteudo, 20));
                    yPosition += 20;

                    gfx.DrawString("ATESTADO DE MATRÍCULA", 
                        fonteSubtitulo, new XSolidBrush(XColors.Black), new XRect(margemEsquerda, yPosition, larguraConteudo, 20));
                    yPosition += 25;

                    // Linha horizontal
                    gfx.DrawLine(new XPen(XColors.Black, 0.5f), margemEsquerda, yPosition, 595 - margemDireita, yPosition);
                    yPosition += 10;

                    // Meta-informações
                    DrawMetaInfo(gfx, margemEsquerda, ref yPosition, "Unidade:", "45 - Instituto de Matemática, Estatística e Ciência da Computação", fontePequenaBold, fontePequena);
                    DrawMetaInfo(gfx, margemEsquerda, ref yPosition, "Aluno:", $"{codigoUsp}/1 - {nomeAluno}", fontePequenaBold, fontePequena);
                    DrawMetaInfo(gfx, margemEsquerda, ref yPosition, "Ingresso:", "Vestibular 3 Lista - 07/03/2025", fontePequenaBold, fontePequena);
                    DrawMetaInfo(gfx, margemEsquerda, ref yPosition, "Curso:", $"45070 - {curso} (noturno)", fontePequenaBold, fontePequena);

                    yPosition += 10;
                    gfx.DrawLine(new XPen(XColors.Black, 0.5f), margemEsquerda, yPosition, 595 - margemDireita, yPosition);
                    yPosition += 15;

                    // CONTEÚDO PRINCIPAL
                    string textoDeclaracao = $"ATESTO, atendendo a requerimento da interessada, que {nomeAluno}, RG {rg}, " +
                        $"código USP {codigoUsp}, é aluna regularmente matriculada neste semestre letivo, no curso de {curso} " +
                        $"com duração ideal de {semestreIdeal} e máxima de {semestreMaximo} semestres, desta Unidade.";

                    DrawWrappedText(gfx, textoDeclaracao, margemEsquerda, ref yPosition, larguraConteudo, fonteNormal);

                    yPosition += 10;
                    string textoEndereco = $"Informamos que o endereço cadastrado pela aluna em nosso sistema corporativo é {endereco}.";
                    DrawWrappedText(gfx, textoEndereco, margemEsquerda, ref yPosition, larguraConteudo, fonteNormal);

                    yPosition += 10;
                    gfx.DrawString($"Modalidade de ingresso: {modalidade}", fonteNormal, new XSolidBrush(XColors.Black), 
                        new XRect(margemEsquerda, yPosition, larguraConteudo, 20));
                    yPosition += 20;

                    string textoAtencao = "ATENÇÃO: Este é um documento oficial da Pró-Reitoria de Graduação da USP, " +
                        "com autenticação eletrônica e dispensa carimbo e assinatura.";
                    DrawWrappedText(gfx, textoAtencao, margemEsquerda, ref yPosition, larguraConteudo, fontePequena);

                    // DADOS DE EMISSÃO
                    yPosition += 20;
                    gfx.DrawString($"Documento emitido às {horaEmissao} horas do dia {dataEmissao} (hora e data de Brasília).",
                        fontePequena, new XSolidBrush(XColors.Black), new XRect(margemEsquerda, yPosition, larguraConteudo, 20), 
                        XStringFormats.Center);
                    yPosition += 15;

                    gfx.DrawString($"Código de controle de autenticidade: {codigoAuth}",
                        fontePequena, new XSolidBrush(XColors.Black), new XRect(margemEsquerda, yPosition, larguraConteudo, 20),
                        XStringFormats.Center);
                    yPosition += 15;

                    gfx.DrawString($"Documento válido até {validadeDoc}.",
                        fontePequena, new XSolidBrush(XColors.Black), new XRect(margemEsquerda, yPosition, larguraConteudo, 20),
                        XStringFormats.Center);

                    // RODAPÉ
                    int rodapeY = 750;
                    gfx.DrawLine(new XPen(XColors.Black, 0.5f), margemEsquerda, rodapeY, 595 - margemDireita, rodapeY);
                    
                    gfx.DrawString("A autenticidade deste documento pode ser verificada na página https://uspdigital.usp.br/iddigital",
                        new XFont("Arial", 7), new XSolidBrush(XColors.Black), 
                        new XRect(margemEsquerda, rodapeY + 5, larguraConteudo, 20), XStringFormats.Center);

                    gfx.DrawString("Endereço da unidade:", 
                        new XFont("Arial", 7), new XSolidBrush(XColors.Black),
                        new XRect(margemEsquerda, rodapeY + 25, larguraConteudo, 10));
                    gfx.DrawString("do Matão 1010 - Butantã",
                        new XFont("Arial", 7), new XSolidBrush(XColors.Black),
                        new XRect(margemEsquerda, rodapeY + 35, larguraConteudo, 10));
                    gfx.DrawString("CEP: 05508-090    São Paulo - SP    CNPJ: 63.025.530/0008-80",
                        new XFont("Arial", 7), new XSolidBrush(XColors.Black),
                        new XRect(margemEsquerda, rodapeY + 45, larguraConteudo, 10));
                }

                // Converter para byte array
                using (var stream = new System.IO.MemoryStream())
                {
                    document.Save(stream, false);
                    return stream.ToArray();
                }
            }
        }

        private string MontarEndereco(Modules.Autenticacao.Domain.Entities.Usuario usuario)
        {
            var partes = new List<string>();
            
            if (!string.IsNullOrEmpty(usuario.Endereco))
                partes.Add(usuario.Endereco);
            if (!string.IsNullOrEmpty(usuario.Numero))
                partes.Add(usuario.Numero);
            if (!string.IsNullOrEmpty(usuario.Complemento))
                partes.Add(usuario.Complemento);
            if (!string.IsNullOrEmpty(usuario.Bairro))
                partes.Add(usuario.Bairro);
            if (!string.IsNullOrEmpty(usuario.Cidade))
                partes.Add(usuario.Cidade);
            if (!string.IsNullOrEmpty(usuario.Estado))
                partes.Add(usuario.Estado);
            if (!string.IsNullOrEmpty(usuario.Cep))
                partes.Add(usuario.Cep);

            return string.Join(" - ", partes);
        }

        private string GerarCodigoAutenticidade()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var grupos = new string[4];
            
            for (int i = 0; i < 4; i++)
            {
                grupos[i] = new string(Enumerable.Range(0, 4)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());
            }
            
            return string.Join("-", grupos);
        }

        private void DrawMetaInfo(XGraphics gfx, int esquerda, ref int yPosition, string label, string valor, XFont fonteBold, XFont fonteNormal)
        {
            gfx.DrawString(label, fonteBold, new XSolidBrush(XColors.Black), new XRect(esquerda, yPosition, 80, 15));
            gfx.DrawString(valor, fonteNormal, new XSolidBrush(XColors.Black), new XRect(esquerda + 85, yPosition, 400, 15));
            yPosition += 15;
        }

        private void DrawWrappedText(XGraphics gfx, string texto, int x, ref int y, int largura, XFont fonte)
        {
            var textoLayout = new XTextFormatter(gfx);
            textoLayout.DrawString(texto, fonte, new XSolidBrush(XColors.Black), 
                new XRect(x, y, largura, 1000), XStringFormats.TopLeft);
            
            // Aproximação de altura baseada no número de linhas
            int linhas = (int)Math.Ceiling(gfx.MeasureString(texto, fonte).Width / largura) + 1;
            y += linhas * 15;
        }
    }
}