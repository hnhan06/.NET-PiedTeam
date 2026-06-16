namespace piedteam_hocmienphi.service.ApplyRequestService;

public class Request
{
    public class CreateApplyRequestRequest
    {
        public string Description { get; set; }
        public string CvLink { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> CategoryIds { get; set; }
        // Khi người dùng tạo đơn đky trở thành Mentor
            // thì họ cũng phải đề cập tôi
            // những lĩnh vực mà họ có thể Mentor
    }
}