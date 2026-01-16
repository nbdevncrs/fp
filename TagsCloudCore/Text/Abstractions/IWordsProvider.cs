using TagsCloudCore.Infrastructure;

namespace TagsCloudCore.Text.Abstractions;

public interface IWordsProvider
{
    Result<IEnumerable<string>> GetWords();
}
