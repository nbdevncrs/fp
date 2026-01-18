using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudCore.DTO;
using TagsCloudCore.Visualization;

namespace TagsCloudTests;

[TestFixture]
public class VisualizerErrorsTests
{
    [Test]
    public void SaveLayoutToFile_ShouldFail_WhenCloudDoesNotFitImage()
    {
        var layout = new[]
        {
            new LayoutItem("word", new Rectangle(0, 0, 1000, 1000), 20)
        };

        var result = CloudVisualizer.SaveLayoutToFile("test.png", layout, "Arial", imageSize: new Size(100, 100));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Tag cloud does not fit into image size");
    }

    [Test]
    public void SaveLayoutToFile_ShouldFail_WhenFontDoesNotExist()
    {
        var layout = new[]
        {
            new LayoutItem("hello", new Rectangle(0, 0, 100, 50), 20)
        };

        var result = CloudVisualizer.SaveLayoutToFile("test.png", layout, "DefinitelyNotARealFont");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Font 'DefinitelyNotARealFont' not found");
    }
}