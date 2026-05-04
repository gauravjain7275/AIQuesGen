using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace AIQuesGen.Services
{
    public class PdfService
    {
        public string ExtractTextFromPdf(IFormFile file)
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            using var reader = new PdfReader(stream);
            using var pdfDoc = new PdfDocument(reader);

            string text = "";
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
            }

            return text.Trim();
        }
    }
}
