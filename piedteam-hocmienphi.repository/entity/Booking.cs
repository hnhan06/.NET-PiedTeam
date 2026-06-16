using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class Booking : BaseEntity<Guid>
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    
    public User User { get; set; }
    public Guid UserId { get; set; }
    
    public Mentor Mentor { get; set; }
    public Guid MentorId { get; set; }
}