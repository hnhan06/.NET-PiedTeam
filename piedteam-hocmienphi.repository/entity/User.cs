using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class User : BaseEntity<Guid>
    //Khi mà kế thừa thì thằng cha luôn luôn nằm đầu tiên sau dấu :
    //Những thằng nằm sau thằng cha là interface
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Age { get; set; }
    public string? Avatar { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone  { get; set; }
    public string Role { get; set; }
    public DateOnly SendDate { get; set; }
    
    public Mentor? Mentor { get; set; }
    
    public ICollection<ApplyRequest> ApplyRequests { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}