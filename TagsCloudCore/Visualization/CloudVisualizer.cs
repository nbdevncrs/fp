using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TagsCloudCore.DTO;
using TagsCloudCore.Infrastructure;

namespace TagsCloudCore.Visualization;

public static class CloudVisualizer
{
    public static Result<None> SaveLayoutToFile(
        string filePath,
        IReadOnlyList<LayoutItem> layout,
        string fontFamily,
        Color? backgroundColor = null,
        Color? textColor = null,
        Size? imageSize = null,
        int padding = 50)
    {
        return ResultFactory.OfAction(() =>
        {
            if (layout.Count == 0)
            {
                SaveEmptyImage(filePath, backgroundColor);
                return;
            }

            var bounds = CalculateBounds(layout, padding);

            var bitmapSize = imageSize ?? new Size(bounds.width, bounds.height);
            
            if (imageSize.HasValue &&
                (bounds.width > imageSize.Value.Width ||
                 bounds.height > imageSize.Value.Height))
            {
                throw new InvalidOperationException("Tag cloud does not fit into image size");
            }

            using var bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            using var graphics = CreateGraphics(bitmap, backgroundColor);

            var offsetX = imageSize.HasValue ? (bitmap.Width - bounds.width) / 2 : 0;
            var offsetY = imageSize.HasValue ? (bitmap.Height - bounds.height) / 2 : 0;

            WordsDrawer.DrawWords(
                graphics,
                layout,
                bounds.minX,
                bounds.minY,
                padding,
                offsetX,
                offsetY,
                textColor,
                fontFamily);

            bitmap.Save(filePath, ImageFormat.Png);

        }, "Failed to render and save tag cloud");
    }

    private static void SaveEmptyImage(string filePath, Color? backgroundColor)
    {
        using var bitmap = new Bitmap(300, 300);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(backgroundColor ?? Color.Black);
        bitmap.Save(filePath, ImageFormat.Png);
    }

    private static (int minX, int minY, int width, int height) CalculateBounds(
        IReadOnlyList<LayoutItem> elements,
        int padding)
    {
        var minX = elements.Min(e => e.Rectangle.Left);
        var maxX = elements.Max(e => e.Rectangle.Right);
        var minY = elements.Min(e => e.Rectangle.Top);
        var maxY = elements.Max(e => e.Rectangle.Bottom);

        var width = Math.Min((maxX - minX) + padding * 2, 5000);
        var height = Math.Min((maxY - minY) + padding * 2, 5000);

        return (minX, minY, width, height);
    }

    private static Graphics CreateGraphics(Bitmap bitmap, Color? backgroundColor)
    {
        var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.Clear(backgroundColor ?? Color.Black);
        return graphics;
    }
}
