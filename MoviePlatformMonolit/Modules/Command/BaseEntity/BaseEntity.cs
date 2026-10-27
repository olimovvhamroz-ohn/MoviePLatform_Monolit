using MoviePlatformMonolit.Modules.Command.Interfaces;

namespace MoviePlatformMonolit.Modules.Command.BaseEntity;

public class BaseEntity : IHasCreation,IHasUpdate, IHasDeletion
{
    public long Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}