using Microsoft.AspNetCore.Mvc;
using piedteam_hocmienphi.repository;
using piedteam_hocmienphi.repository.entity;
using piedteam_hocmienphi.repository.enums;
using piedteam_hocmienphi.service.ApplyRequestService;

namespace piedteam_hocmienphi.api.Controllers;

[ApiController]
[Route("[controller]")]

public class ApplyRequestController : ControllerBase
{
    private readonly AppDbContext _dbcontext;

    public ApplyRequestController(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    // Flow thiết kế api ntn?
    /*
        Xác định có những api nào
            API này sẽ làm nhiệm vu gì
            Xác định các endpoint (method, url)
            Ai sẽ gọi API này
            Ngay lập tức vào Controller khai báo luôn -> chạy đc Swagger cho an tâm
        Để làm hành động này thì cần những yêu cầu gì (input)
            Ngay lập tức list ra luôn
            Đi vào Entity ngắm nghía xem có field gì
            Sau khi xác định đc input r, vào Service liền tạo folder Request
            Sau đó lắp đồ chơi này vào cái API mà nó cần ở Controller
            Sau khi mà xđ đc input là gì -> quay ra hỏi FE có cần trả thêm gì không
            Nếu cần -> đi ngay vào Service tạo class Response
            Nếu k -> thì thôi
        Sau khi Controller đủ đồ chơi (input, output) ==> implement logic xử lí bên trong
     
     */
    
    [HttpPost("")]
    public IActionResult CreateApplyRequest(Request.CreateApplyRequestRequest requestBody)
    {
        var request = new ApplyRequest()
        {
            Id = Guid.NewGuid(),
            UserId = requestBody.UserId,
            Description =  requestBody.Description,
            CvLink =  requestBody.CvLink,
            Status = ApplyRequestStatus.Pending
        };
        
        _dbcontext.ApplyRequests.Add(request);
        _dbcontext.SaveChanges();
        
        // Apply Request Category là bảng lưu trữ những lĩnh vực mà người Mentor apply
        
        // FE: public List<Guid> CategoryIds
        // Trong DB thì lưu trữ bằng Entity ApplyRequestCategory
        // Làm sao biến 1 List<Guid> -> 1 List<ApplyRequestCategory>
        // => Dùng select để ánh xạ
        var applyRequestCategories  = requestBody.CategoryIds.Select(
            x => new ApplyRequestCategory()
            {
                Id = Guid.NewGuid(),
                ApplyRequestId = request.Id,
                CategoryId = x
            }
        );
        
        _dbcontext.ApplyRequestCategories.AddRange(applyRequestCategories);
        _dbcontext.SaveChanges();
        
        return Ok();
    }
    
    [HttpGet("")]
    public IActionResult GetAllApplyRequest()
    {
        return Ok();
    }
    
    [HttpGet("me")]
    public IActionResult GetMyApplyRequest()
    {
        return Ok();
    }
    
    [HttpGet("{id}")]
    public IActionResult GetApplyRequestDetail()
    {
        return Ok();
    }
    
    [HttpPost("{id}/review")]
    public IActionResult ReviewApplyRequest()
    {
        return Ok();
    }
}