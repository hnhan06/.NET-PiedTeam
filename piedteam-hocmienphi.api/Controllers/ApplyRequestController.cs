using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using piedteam_hocmienphi.api.Extensions;
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
        // => Dùng select để ánh xạ, x lúc này tượng trưng cho 1 CategoryId
        
        // FE chỉ truyền cho mình 1 list requestBody.CategoryIds
        // Nhưng mà những dữ liệu mà mentor mong muốn được mentoring category thì nó nằm ở bảng ApplyRequestCategory
        // Vậy nên mình phải ánh xạ từ list requestBody.CategoryIds sang list ApplyRequestCategory
        var applyRequestCategories  = requestBody.CategoryIds.Select(
            x => new ApplyRequestCategory()
            {
                Id = Guid.NewGuid(),
                ApplyRequestId = request.Id,
                CategoryId = x
            }
        );
        // req1 | cate1 (kinh tế)
        // req2 | cate2 (phần mềm)
        
        _dbcontext.ApplyRequestCategories.AddRange(applyRequestCategories);
        _dbcontext.SaveChanges();
        
        return Ok();
    }
    
    // Tôi sẽ apply Authorization theo policy (quy tắc) là AdminPolicy
    [Authorize(Policy = JwtExtensions.AdminPolicy)]
    [HttpGet("")]
    public IActionResult GetAllApplyRequest(string? searchTerm = null, 
        ApplyRequestStatus? status = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        Guid[]? categoryIds = null, // filter theo category
        int pageIndex = 1, 
        int pageSize = 10)
    {
        var query = _dbcontext.ApplyRequests.Where(x => x.IsDeleted == false);

        if (searchTerm != null)
        {
            query = query.Where(x =>
                x.Description.Contains(searchTerm) ||
                x.User.FirstName.Contains(searchTerm) ||
                x.User.LastName.Contains(searchTerm));
        }
        // đó giờ chúng ta chỉ tìm kiếm ở table hiện tại thôi
        // x.Description.Contains(searchTerm) ||
        // còn 2 thằng dưới thì nó update lên tí
        // x.User.FirstName.Contains(searchTerm) ||
        // x.User.LastName.Contains(searchTerm));
        // lúc này thì nó sẽ tự động join sang table User để tìm kiếm

        // if (fromDate != null)
        // {
        //     
        // }
        
        //categoryIds là những CategoryId mà FE muốn tìm kiếm | muốn filter
        // Tôi muốn tìm những lá đơn thuộc loại Category này
        // VD: tôi muốn tìm những lá đơn thuộc "Kinh tế"
            // Mentor A apply có thể loại là Kinh tế -> lấy
            // Mentor B apply có thể loại là Nấu ăn & Kinh tế -> lấy
        if (categoryIds != null && categoryIds.Length > 0)
        {
            query = query.Where(x => x.ApplyRequestCategories.Any(apC => categoryIds.Contains(apC.CategoryId)));
        }

        if (status != null)
        {
            query = query.Where(x => x.Status == status);
        }

        var selectedQuery = query.Select(x => new Response.GetApplyRequestResponse()
        {
            Id = x.Id,
            Description = x.Description,
            CvLink = x.CvLink,
            Status = x.Status,
            RejectReason = x.RejectionReason,
            User = new service.UserService.Response.GetAllUserResponse()
            {
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                Age = x.User.Age
            },
            Categories = x.ApplyRequestCategories.Select(apC => new service.CategoryService.Response.GetAllParentCategoryResponse()
            {
                Id = apC.Category.Id,
                Name = apC.Category.Name
            }).ToList()
        });

        selectedQuery = selectedQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        
        var result = selectedQuery.ToList();
        
        return Ok(result);
    }
    
    // Lấy ra những đơn apply của tôi
    // Khi mà đã Authentication và Authorization thì có nghĩa là gì?
        // bạn chính là user trong hệ thống của chúng tôi
        // và bạn có quyền hạn truy cập các api mà chúng tôi cho phép
    // Vì hệ thống đã biết chúng ta là ai rồi thế nên chúng ta có thể lược bỏ
        // và k cần truyền những field k cần thiết ví dụ: Guid userId,
    // Vậy thì hệ thống biết người là ai, userId, email, firstName, lastName bằng cách nào?\
    // Hệ thống sẽ biết được, tại vì chúng ta đã ghi lại những thông tin đó vào
        // payload mà (xem lại ở phần Login)
    [Authorize(Policy = JwtExtensions.MentorPolicy)]
    [HttpGet("me")]
    public IActionResult GetMyApplyRequest(
        ApplyRequestStatus? status = null, 
        int pageIndex = 1, 
        int pageSize = 10)
    {
        var userIdString = HttpContext.User.Claims.FirstOrDefault(
            x => x.Type.Equals("userId")
        )!.Value;
        
        var userId = Guid.Parse(userIdString);
        
        var query = _dbcontext.ApplyRequests.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.UserId == userId);

        if (status != null)
        {
            query = query.Where(x => x.Status == status);
        }

        var selectedQuery = query.Select(x => new Response.GetApplyRequestResponse()
        {
            Id = x.Id,
            Description = x.Description,
            CvLink = x.CvLink,
            Status = x.Status,
            RejectReason = x.RejectionReason,
            User = new service.UserService.Response.GetAllUserResponse()
            {
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                Age = x.User.Age
            },
            Categories = x.ApplyRequestCategories.Select(apC => new service.CategoryService.Response.GetAllParentCategoryResponse()
            {
                Id = apC.Category.Id,
                Name = apC.Category.Name
            }).ToList()
        });

        selectedQuery = selectedQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        
        var result = selectedQuery.ToList();
        
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetApplyRequestDetail(Guid id)
    {
        var query = _dbcontext.ApplyRequests.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id == id );

        var selectedQuery = query.Select(x => new Response.GetApplyRequestResponse()
        {
            Id = x.Id,
            Description = x.Description,
            CvLink = x.CvLink,
            Status = x.Status,
            RejectReason = x.RejectionReason,
            User = new service.UserService.Response.GetAllUserResponse()
            {
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                Age = x.User.Age
            },
            Categories = x.ApplyRequestCategories.Select(apC => new service.CategoryService.Response.GetAllParentCategoryResponse()
            {
                Id = apC.Category.Id,
                Name = apC.Category.Name
            }).ToList()
        });
        
        var result = selectedQuery.FirstOrDefault();
        
        return Ok(result);
    }
    
    [HttpPost("{id}/review")]
    public IActionResult ReviewApplyRequest(Guid id, Request.ReviewApplyRequestRequest requestBody)
    {
        var query = _dbcontext.ApplyRequests.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id == id );

        query = query.Include(x => x.User)
            .Include(x => x.ApplyRequestCategories);
        
        var applyRequest = query.FirstOrDefault();

        if (applyRequest == null)
        {
            return NotFound();
        }

        if (requestBody.IsApproved)
        {
            applyRequest.Status = ApplyRequestStatus.Approved;
            applyRequest.User.Role = "Mentor";  // bị lỗi null
            // Tại sao api call lại bị null
            // Auto Join nó chỉ hđ khi mình sử dụng Select thôi
            // Còn ở đây nếu mà muốn chấm ra User xài thì mình phải sử dụng Include để join thủ công
            
            // Tạo mới entity Mentor và Category cho mentor đó
            var mentor = new Mentor()
            {
                Id = Guid.NewGuid(),
                UserId = applyRequest.UserId
            };
            _dbcontext.Mentors.Add(mentor);
            _dbcontext.SaveChanges();

            var mentorCategories = applyRequest.ApplyRequestCategories.Select(x => new MentorCategory()
            {
                Id = Guid.NewGuid(),
                MentorId = mentor.Id,
                CategoryId = x.CategoryId
            });
            _dbcontext.MentorCategories.AddRange(mentorCategories);
            _dbcontext.SaveChanges();

        }
        else
        {
            applyRequest.Status = ApplyRequestStatus.Rejected;
            applyRequest.RejectionReason = requestBody.Reason;
        }
        
        _dbcontext.ApplyRequests.Update(applyRequest);
        _dbcontext.SaveChanges();
        
        return Ok();
    }
}