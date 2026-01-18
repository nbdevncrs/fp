using System.Drawing;
using TagsCloudCore.DTO;
using TagsCloudCore.Infrastructure;
using TagsCloudCore.Layout.Abstractions;
using TagsCloudCore.Text.Abstractions;

namespace TagsCloudCore;

public sealed class WordsLayoutGenerator(
    IWordsProvider provider,
    IWordsPreprocessor preprocessor,
    IWordsFrequencyAnalyzer analyzer,
    IWordsFontSizer sizer,
    ICloudLayouter layouter)
    : IWordsLayoutGenerator
{
    public Result<IReadOnlyList<LayoutItem>> GenerateLayout()
    {
        return provider.GetWords()
            .Then(preprocessor.ProcessWords)
            .Then(analyzer.FindFrequencies)
            .Then(BuildLayout)
            .RefineError("Failed to generate words layout");
    }

    private Result<IReadOnlyList<LayoutItem>> BuildLayout(
        IReadOnlyDictionary<string, int> frequencies)
    {
        return ResultFactory.Of(IReadOnlyList<LayoutItem> () =>
        {
            var result = new List<LayoutItem>();

            var ordered = frequencies
                .OrderByDescending(f => f.Value)
                .ToList();

            if (ordered.Count == 0)
                return result;

            var maxFrequency = ordered[0].Value;

            foreach (var (word, frequency) in ordered)
            {
                var fontSize = sizer.GetFontSize(word, frequency, maxFrequency);

                var size = EstimateSize(word, fontSize);

                var rect = layouter.PutNextRectangle(size);

                result.Add(new LayoutItem(word, rect, fontSize));
            }

            return result;

        }, "Layout building failed");
    }

    private static Size EstimateSize(string word, int fontSize)
    {
        using var bitmap = new Bitmap(1, 1);
        using var graphics = Graphics.FromImage(bitmap);
        using var font = new Font("Arial", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        var sizeF = graphics.MeasureString(word, font);

        return new Size(
            (int)Math.Ceiling(sizeF.Width),
            (int)Math.Ceiling(sizeF.Height));
    }
}
