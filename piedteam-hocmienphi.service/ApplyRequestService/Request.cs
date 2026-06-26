namespace piedteam_hocmienphi.service.ApplyRequestService;

public class Request
{
    public class CreateApplyRequestRequest
    {
        public string Description { get; set; }
        public string CvLink { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> CategoryIds { get; set; } // Mảng CategoryIds
        // Khi người dùng tạo đơn đky trở thành Mentor
            // thì họ cũng phải đề cập tôi
            // những lĩnh vực mà họ có thể Mentor
    }
    
    public class ReviewApplyRequestRequest
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
        // Nếu IsApproved = true thì Reason có thể null
        // Nếu IsApproved = false thì Reason là bắt buộc
    }
}