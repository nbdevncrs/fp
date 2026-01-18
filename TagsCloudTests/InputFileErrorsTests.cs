using FluentAssertions;
using TagsCloudCore.Text.Implementations.WordsProviding;

namespace TagsCloudTests;

[TestFixture]
public class InputFileErrorsTests
{
    [Test]
    public void GetWords_ShouldReturnThreeLevelError_WhenTxtFileNotFound()
    {
        var provider = new TxtWordsProvider("not_existing_file.txt");

        var result = provider.GetWords();

        result.IsSuccess.Should().BeFalse();
        
        result.Error.Should().Contain("Failed to read txt file");
        result.Error.Should().Contain("Input file not found");
    }

    [Test]
    public void GetWords_ShouldReturnThreeLevelError_WhenDocxFileNotFound()
    {
        var provider = new DocxWordsProvider("not_existing_file.docx");

        var result = provider.GetWords();

        result.IsSuccess.Should().BeFalse();
        
        result.Error.Should().Contain("Failed to read docx file");
        result.Error.Should().Contain("Input file not found");
    }
}