using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class MentorCategory : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public Mentor Mentor { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}