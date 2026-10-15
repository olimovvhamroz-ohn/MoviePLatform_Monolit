


namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.REQUEST;

public class RecordViewRequest
{
    public long MovieId { get; set; }
    public int PositionSeconds { get; set; }
    public bool IsCompleted { get; set; }
}