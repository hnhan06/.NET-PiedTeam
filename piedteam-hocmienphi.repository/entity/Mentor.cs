using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class Mentor : BaseEntity<Guid>
{
    public string? OrganizationName { get; set; }
    public string? CurrentPosition { get; set; }
    
    // Tại sao ở đây nên là nullable mà không phải là required
    // - Vì khi user trở thành mentor sau khi được approved
        // thì lúc này ta mới tạo mới Mentor
        // nếu không đổi thành nullable, thì 2 field đó ở đâu ra
    // => Flow đúng phải là khi họ login lần đầu sau khi đc approved
    // cta yêu cầu họ bắt buộc điền thông tin bổ sung
    
    public User User { get; set; }
    public Guid UserId { get; set; }
    
    public ICollection<MentorCategory> MentorCategories { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<MentorFreeTime> MentorFreeTimes { get; set; }
}