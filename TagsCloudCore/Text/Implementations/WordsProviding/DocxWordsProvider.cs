using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TagsCloudCore.Infrastructure;
using TagsCloudCore.Text.Abstractions;

namespace TagsCloudCore.Text.Implementations.WordsProviding;

public sealed class DocxWordsProvider(string filePath) : IWordsProvider
{
    public Result<IEnumerable<string>> GetWords()
    {
        return ResultFactory.Of<IEnumerable<string>>(() =>
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Input file not found: {filePath}");
            
            var words = new List<string>();

            using var document = WordprocessingDocument.Open(filePath, false);

            var body = document.MainDocumentPart?.Document.Body;

            if (body == null)
                return words;

            foreach (var paragraph in body.Elements<Paragraph>())
            {
                var text = paragraph.InnerText;

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                words.AddRange(WordsProvidingHelpers.SplitWords(text));
            }

            return words;

        }, "Failed to read docx file");
    }
}