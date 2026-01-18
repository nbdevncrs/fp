using TagsCloudCore.DTO;
using TagsCloudCore.Infrastructure;

namespace TagsCloudCore;

public interface IWordsLayoutGenerator
{
    Result<IReadOnlyList<LayoutItem>> GenerateLayout();
}