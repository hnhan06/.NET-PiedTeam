using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class Mentor : BaseEntity<Guid>
{
    public string OrganizationName { get; set; }
    public string CurrentPosition { get; set; }
    
    public User User { get; set; }
    public Guid UserId { get; set; }
    
    public ICollection<MentorCategory> MentorCategories { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<MentorFreeTime> MentorFreeTimes { get; set; }
}