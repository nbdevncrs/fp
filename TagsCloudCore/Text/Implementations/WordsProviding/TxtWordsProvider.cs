using TagsCloudCore.Infrastructure;
using TagsCloudCore.Text.Abstractions;

namespace TagsCloudCore.Text.Implementations.WordsProviding;

public sealed class TxtWordsProvider(string filePath) : IWordsProvider
{
    public Result<IEnumerable<string>> GetWords()
    {
        return ResultFactory.Of<IEnumerable<string>>(() =>
        {
            var words = new List<string>();

            using var reader = new StreamReader(filePath);

            while (reader.ReadLine() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                words.AddRange(WordsProvidingHelpers.SplitWords(line));
            }

            return words;
        }, "Failed to read txt file");
    }
}