using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using piedteam_hocmienphi.repository;
using piedteam_hocmienphi.repository.entity;
using piedteam_hocmienphi.service.Utils.JwtService;
using MailService = piedteam_hocmienphi.service.Utils.Mail;
using MediaService = piedteam_hocmienphi.service.Utils.MediaService;

namespace piedteam_hocmienphi.service.UserService;

public class Service : IService
{
    private readonly AppDbContext _dbContext;
    private readonly MailService.IService _mailService;
    private readonly MediaService.IService _mediaService;
    private readonly JwtOptions _jwtOptions = new();

    public Service(AppDbContext dbContext, MailService.IService mailService, MediaService.IService mediaService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mailService = mailService;
        _mediaService = mediaService;
        configuration.GetSection(nameof(JwtOptions)).Bind(_jwtOptions);
    }

    public async Task<List<Response.GetAllUserResponse>> GetAllUsers(string? searchTerm, int pageIndex, int pageSize)
    {
        var query = _dbContext.Users.Where(x => x.IsDeleted == false);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var sanitizerText = searchTerm.ToLower().Trim();
            
            query = query
                .Where(x => 
                    x.FirstName.ToLower().Contains(sanitizerText) || 
                    x.LastName.ToLower().Contains(sanitizerText) ||
                    x.Email.ToLower().Contains(sanitizerText)
                );
        }

        query = query.OrderBy(x => x.Email);

        var selectedQuery = query.Select(x => new Response.GetAllUserResponse()
        {
            FirstName = x.FirstName,
            LastName = x.LastName,
            Age = x.Age,
            Email = x.Email,
        });

        selectedQuery = selectedQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
            
        var result = await selectedQuery.ToListAsync();
        
        return result;
    }

    public async Task<string> CreateUser(Request.CreateUserRequest requestBody)
    {
        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = requestBody.FirstName,
            LastName = requestBody.LastName,
            Age = "",
            Email = requestBody.Email,
            Password = requestBody.Password,
            Phone =  "",
            Role = "Student"
        };
        
        _dbContext.Users.Add(newUser);
        
        await _dbContext.SaveChangesAsync();

        var mailContent = new MailService.MailContent()
        {
            Body = MailService.MailTemplates.GetHelloTemplate(newUser.FirstName + " " + newUser.LastName),
            To = newUser.Email,
            Subject = "Welcome to Hoc Mien Phi System"
        };

        await _mailService.SendMail(mailContent);

        return "Add user thành công";
    }

    public async Task<string> UpdateUser(Guid id, Request.UpdateUserRequest requestBody)
    {
        var query = _dbContext.Users.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id.Equals(id));
        
        var user = query.FirstOrDefault();

        if (user != null)
        {
            user.FirstName = requestBody.FirstName;
            user.LastName = requestBody.LastName;
            user.Age = requestBody.Age;
            user.Email = requestBody.Email;
            user.Password = requestBody.Password;
            user.Phone = requestBody.Phone;

            if (requestBody.Avatar != null)
            {
                user.Avatar = await _mediaService.UploadImageAsync(requestBody.Avatar);
            }
            
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        return "Update user thành công";
    }

    public async Task<string> Login(string email, string password)
    {
        /*
         // Tại sao phải login
        // - Vì phải giới hạn quyền hạn đc gọi đến resource
        // VD: bạn phải là User (đã đky hệ thống) thì bạn mới đc mua hàng
        
        // Authentication & Authorization
            Authen: bạn có được quyền vào hệ thống của tôi không
            Author: sau khi vào hệ thống thì bạn có quyền gì
                    VD: Admin có quyền tạo
                        Mentor có quyền tạo lịch rãnh
                        
            Vậy thì thông thường, cta dùng kỹ thuật gì để xác thực và phân quyền?
            - Thông thường mình hay sử dụng JWT để xác thực và phân quyền
            
            JWT: Json Web Token là 1 chuỗi token đc mã hóa, truyền giữa Client (Frontend) và Server (Backend) 
                để xác thực và phân quyền
            
            JWT gồm 3 phần:
                Header: chứa thông tin về thuật toán mã hóa và loại token
                Payload: chứa thông tin về người dùng và quyền hạn của nguười dùng
                Signature: chứa chữ ký số để xác thực token (sign(header+payload, secret))
         */
        
        /*
         // Đầu tiên phải tìm kiếm cái tài khoản với email đó có tồn tại k
            // Nếu mà có thì mới tính tiếp đc
                // Tiếp tục so sánh password người dùng vào với password có trong DB
                // Nếu mà trùng thì bạn chính là chủ nhận của tài khoản -> trả ra JWT token cho bạn để xác thực và phân quyền
                // Nếu mà k trùng, bạn k phải chủ nhân của tài khoản thì cút
            // Nếu mà k tồn tại email thì cút
         */
        
        var query = _dbContext.Users.Where(x => x.IsDeleted == false);

        query = query.Where(x => x.Email.Equals(email));

        var user = query.FirstOrDefault();
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (user.Password != password)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        
        // claims đai diện cho các thông tin nằm trong payload của jwt
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Role, user.Role),
            // Quan trọng: Claim này (new Claim(ClaimTypes.Role, user.Role),) sẽ giúp mình phân quyền
            new Claim("UserId", user.Id.ToString()),
            new Claim("Role", user.Role)
        };
        
        var token = JwtService.GenerateToken(claims, _jwtOptions);
        
        // Phần này chỉ là phần cách mà server tạo ra 1 cái Token
        // Nhưng mà chưa đủ
        // Chúng ta còn phải cấu hình cách mà server xử lí 1 cái Token
        // Liệu rằng Token này có thực sự là của mình hay không
        
        return token;
    }
}