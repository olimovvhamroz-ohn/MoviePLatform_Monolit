

namespace MoviePLatform_Monolit.Modules.Command.BaseEntity;

public class BaseEntity
{
    public long Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
}