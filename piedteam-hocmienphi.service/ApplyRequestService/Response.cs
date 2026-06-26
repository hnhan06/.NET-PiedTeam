using piedteam_hocmienphi.repository.enums;

namespace piedteam_hocmienphi.service.ApplyRequestService;

public class Response
{
    public class GetApplyRequestResponse
    {
        public Guid Id { get; set; }
        public string CvLink { get; set; }
        public string Description { get; set; }
        public ApplyRequestStatus Status { get; set; }
        public string? RejectReason { get; set; }
        
        public UserService.Response.GetAllUserResponse User  { get; set; }
        public List<CategoryService.Response.GetAllParentCategoryResponse> Categories { get; set; }
    }
}