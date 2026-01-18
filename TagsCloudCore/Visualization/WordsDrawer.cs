using System.Drawing;
using TagsCloudCore.DTO;

namespace TagsCloudCore.Visualization;

public static class WordsDrawer
{
    public static void DrawWords(
        Graphics graphics,
        IReadOnlyList<LayoutItem> elements,
        int minX,
        int minY,
        int padding,
        int offsetX,
        int offsetY,
        Color? textColor,
        string fontFamily)
    {
        var random = new Random();

        foreach (var (word, rect, fontSize) in elements)
        {
            DrawSingleWord(
                graphics,
                word,
                rect,
                fontSize,
                minX,
                minY,
                padding,
                offsetX,
                offsetY,
                random,
                textColor,
                fontFamily);
        }
    }

    private static void DrawSingleWord(
        Graphics graphics,
        string word,
        Rectangle rect,
        int fontSize,
        int minX,
        int minY,
        int padding,
        int offsetX,
        int offsetY,
        Random random,
        Color? textColor,
        string fontFamily)
    {
        if (!FontFamily.Families.Any(f => f.Name == fontFamily))
            throw new InvalidOperationException($"Font '{fontFamily}' not found");

        var shifted = rect with
        {
            X = rect.Left - minX + padding + offsetX,
            Y = rect.Top - minY + padding + offsetY
        };

        using var font = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        using var brush = textColor.HasValue
            ? new SolidBrush(textColor.Value)
            : new SolidBrush(Color.FromArgb(
                255,
                random.Next(40, 255),
                random.Next(40, 255),
                random.Next(40, 255)));

        var textSize = graphics.MeasureString(word, font);

        var textX = shifted.Left + (shifted.Width - textSize.Width) / 2;
        var textY = shifted.Top + (shifted.Height - textSize.Height) / 2;

        graphics.DrawString(word, font, brush, new PointF(textX, textY));
    }
}