using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class ApplyRequestCategory : BaseEntity<Guid>
{
    public Guid ApplyRequestId { get; set; }
    public ApplyRequest ApplyRequest { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}