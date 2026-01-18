using System.Drawing;
using FakeItEasy;
using FluentAssertions;
using TagsCloudCore;
using TagsCloudCore.Infrastructure;
using TagsCloudCore.Layout.Abstractions;
using TagsCloudCore.Text.Abstractions;
using TagsCloudCore.Text.Implementations.WordsFrequencyAnalyzing;

namespace TagsCloudTests;

[TestFixture]
public class WordsLayoutGeneratorTests
{
    [Test]
    public void GenerateLayout_ShouldReturnLayoutForEachUniqueWord()
    {
        var provider = A.Fake<IWordsProvider>();
        A.CallTo(() => provider.GetWords())
            .Returns(ResultFactory.Ok<IEnumerable<string>>(["apple", "banana", "apple"]));

        var generator = CreateGenerator(provider);

        var result = generator.GenerateLayout().GetValueOrThrow();

        result.Should().HaveCount(2);
        result.Select(r => r.Word).Should().BeEquivalentTo("apple", "banana");
    }

    [Test]
    public void GenerateLayout_ShouldOrderWordsByFrequencyDescending()
    {
        var provider = A.Fake<IWordsProvider>();
        A.CallTo(() => provider.GetWords())
            .Returns(ResultFactory.Ok<IEnumerable<string>>(["cat", "dog", "cat", "cat", "dog", "mouse"]));

        var generator = CreateGenerator(provider);

        var result = generator.GenerateLayout().GetValueOrThrow();

        result.Select(r => r.Word).Should().ContainInOrder("cat", "dog", "mouse");
    }

    [Test]
    public void GenerateLayout_ShouldPassCorrectFrequenciesToSizer()
    {
        var provider = A.Fake<IWordsProvider>();
        A.CallTo(() => provider.GetWords())
            .Returns(ResultFactory.Ok<IEnumerable<string>>(["a", "a", "b"]));

        var sizer = A.Fake<IWordsFontSizer>();
        A.CallTo(() => sizer.GetFontSize(A<string>._, A<int>._, A<int>._)).Returns(10);

        var generator = CreateGenerator(provider, sizer: sizer);

        generator.GenerateLayout().GetValueOrThrow();

        A.CallTo(() => sizer.GetFontSize("a", 2, 2)).MustHaveHappenedOnceExactly();

        A.CallTo(() => sizer.GetFontSize("b", 1, 2)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void GenerateLayout_ShouldCallLayouterForEachWord()
    {
        var provider = A.Fake<IWordsProvider>();
        A.CallTo(() => provider.GetWords())
            .Returns(ResultFactory.Ok<IEnumerable<string>>(["x", "y", "z"]));

        var layouter = A.Fake<ICloudLayouter>();
        A.CallTo(() => layouter.PutNextRectangle(A<Size>._)).Returns(new Rectangle(0, 0, 10, 10));

        var generator = CreateGenerator(provider, layouter: layouter);

        generator.GenerateLayout().GetValueOrThrow();

        A.CallTo(() => layouter.PutNextRectangle(A<Size>._)).MustHaveHappened(3, Times.Exactly);
    }

    [Test]
    public void GenerateLayout_ShouldReturnFontSizeProvidedBySizer()
    {
        var provider = A.Fake<IWordsProvider>();
        A.CallTo(() => provider.GetWords()).Returns(ResultFactory.Ok<IEnumerable<string>>(["hello"]));

        var sizer = A.Fake<IWordsFontSizer>();
        A.CallTo(() => sizer.GetFontSize(A<string>._, A<int>._, A<int>._)).Returns(42);

        var generator = CreateGenerator(provider, sizer: sizer);

        var result = generator.GenerateLayout().GetValueOrThrow().Single();

        result.FontSize.Should().Be(42);
    }

    private static WordsLayoutGenerator CreateGenerator(
        IWordsProvider provider,
        IWordsFontSizer? sizer = null,
        ICloudLayouter? layouter = null)
    {
        var preprocessor = A.Fake<IWordsPreprocessor>();
        A.CallTo(() => preprocessor.ProcessWords(A<IEnumerable<string>>._))
            .ReturnsLazily((IEnumerable<string> words) => words);

        return new WordsLayoutGenerator(
            provider,
            preprocessor,
            new WordsFrequencyAnalyzer(),
            sizer ?? CreateDefaultSizer(),
            layouter ?? CreateDefaultLayouter());
    }

    private static IWordsFontSizer CreateDefaultSizer()
    {
        var sizer = A.Fake<IWordsFontSizer>();
        A.CallTo(() => sizer.GetFontSize(A<string>._, A<int>._, A<int>._)).Returns(10);

        return sizer;
    }

    private static ICloudLayouter CreateDefaultLayouter()
    {
        var layouter = A.Fake<ICloudLayouter>();
        A.CallTo(() => layouter.PutNextRectangle(A<Size>._)).Returns(new Rectangle(0, 0, 10, 10));

        return layouter;
    }
}