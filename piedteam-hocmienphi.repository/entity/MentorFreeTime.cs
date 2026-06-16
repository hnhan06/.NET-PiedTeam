using piedteam_hocmienphi.repository.abstraction;
using piedteam_hocmienphi.repository.enums;

namespace piedteam_hocmienphi.repository.entity;

public class MentorFreeTime : BaseEntity<Guid>
{
    public MentorFreeTimeDateOfWeek  DayOfWeek { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    
    public Mentor Mentor { get; set; }
    public Guid  MentorId { get; set; }
}