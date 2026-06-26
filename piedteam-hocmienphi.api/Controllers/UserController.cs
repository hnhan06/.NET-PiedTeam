using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using piedteam_hocmienphi.repository;
using piedteam_hocmienphi.repository.entity;
using piedteam_hocmienphi.service.UserService;
using piedteam_hocmienphi.service.Utils.JwtService;

namespace piedteam_hocmienphi.api.Controllers;

[ApiController]
[Route("[controller]")]
// Note | Annotation
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly JwtOptions _jwtOptions = new();
    
    public UserController(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        configuration.GetSection(nameof(JwtOptions)).Bind(_jwtOptions);
    }
    
    // Endpoint có tên khác là các APIs
    // VD: POST /api/auth/login     //Login
    // VD: POST /api/auth/register  //Đăng ký
    // VD: GET /api/users/{id}      //Lấy thông tin của 1 user
    // VD: PUT /api/users/{id}      //Cập nhật thông tin của 1 user
    // VD: DELETE /api/users/{id}   //Xóa 1 user
    
    // Những endpoint này làm thế nào nó xuất hiện -> Khai báo trong controller
    
    // Có mấy method của 1 request
    // GET, POST, PUT, PATCH, DELETE
    // GET: Lấy dữ liệu
    // POST: Tạo dữ liệu mới
    // PUT | PATCH: Cập nhật dữ liệu
    // DELETE: Xóa dữ liệu
    
    //1. Tại sao lại sinh ra các method này
    //2. 1 method mà mình dùng hết cho tất cả trường hợp được không
        // VD: POST mình dùng để lấy, tạo, cập nhật, xóa dữ liệu được không
        
    // GET khác với POST, PUT, DELETE ở đâu?
    // - GET thường sẽ không có request body, dữ liệu sẽ được truyền qua query hoặc route
    
    // Có mấy cách để FE truyền dữ liệu sang BE
        //biết là sẽ call API r, nhma những cái data mà FE gửi thì nó sẽ nằm ở đâu trong request
        
    // Thông thường FE sẽ truyền data qua 3 cách:
        // Query param: /api/users?name=tan&age=18
            //Là những param được nằm trên url sau dấu ?
            //như url trên thì chúng ta có 2 query param là name = tan và age = 18
        // Route param: /api/user/{id} | /api/users/1234
            //Là những param được nằm trên url sau dấu /
            //như url trên thì chúng ta có 1 route param là id = 1234
        // Body: thường sẽ dùng cho PUT, POST, PATCH, DELETE
        
    // Vì GET thường sẽ không có body, nên chúng ta hạn chế sd cho các API cần bảo mật
    // GET để login: GET api/auth/login?email=tan&password=123
    // thông thường khi login chúng ta thường xài method POST /api/auth/login, data sẽ được giấu ở trong body
    
    // Request là gì, là 1 yêu cầu xuống server, mong muốn server làm gì đó và trả ra kết quả
    // VD: 1 HTTP Request Login, mong muốn server xác thực thông tin đăng nhập, và trả ra kết quả là Token hoặc lỗi
    // Trong 1 HTTP Request thì sẽ có những thành phần sau:
        // URL: địa chỉ của API mà chúng ta muốn gọi
        // Method: GET, POST, PUT, ...
        // Header: chứa các thông tin về request, như là Content-type, Authorization, v.v..
        // Body: chứa dữ liệu mà chúng ta muốn gửi lên server (thường dùng cho POST, PUT, PATCH, DELETE)
        
    //Resful API: là 1 tiêu chuẩn để thiết kế API
    // dựa trên các phương thức HTTP và các quy tắc về URL
    // để tạo ra các API dễ hiểu, dễ sử dụng, dễ bảo trì
    
    // GetAll: GET /api/user/ -> theo chuẩn phải đặt các tham số vào để truy vấn
        // GetAllUsers: GET /api/user/getall
        // GetAllStaffs: GET /api/user/getall-staff
        // GetAllStudents: GET /api/user/getall-student
    // GetById: GET /api/user/{id}
    // Create: POST /api/user
        // CreateUser: POST /api/user/create-user
    // Update: PUT /api/user/{id}
    // Delete: DELETE /api/user/{id}

    [HttpGet("")]    //Query param: page index, page size sẽ thay đổi khi FE truyền vào
    public IActionResult GetAllUsers(string? searchTerm, int? pageIndex = 1, int? pageSize = 10)
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
            .Skip((pageIndex!.Value - 1) * pageSize!.Value)
            .Take(pageSize!.Value);
            
        var result = selectedQuery.ToList();
            
        return Ok(result);
    }
    
    [HttpPost("")]  //Body: dữ liệu sẽ đc truyền qua body, nên k cần phải đặt tham số trong url
    public IActionResult CreateUser(Request.CreateUserRequest requestBody)
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
        _dbContext.SaveChanges();
        
        return Ok();
    }
    
    [HttpGet("{id}")]   //Route param: Id sẽ thay đổi khi FE truyền vào
    public IActionResult GetUserById(string id)
    {
        var query = _dbContext.Users.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id.Equals(id));

        var selectedQuery = query.Select(x => new Response.GetAllUserResponse()
        {
            FirstName = x.FirstName,
            LastName = x.LastName,
            Age = x.Age,
            Email = x.Email
        });
        
        var result = selectedQuery.FirstOrDefault();
        
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteUserById(Guid id)
    {
        var query = _dbContext.Users.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id.Equals(id));
        
        var user = query.FirstOrDefault();

        if (user != null)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
        return Ok();
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateUserById(Guid id, Request.UpdateUserRequest requestBody)
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
            
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
        return Ok();
    }
    
    [HttpPost("Login")]
    public IActionResult Login(string email, string password)
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
            return BadRequest("Email không tồn tại");
        }

        if (user.Password != password)
        {
            return BadRequest("Password không đúng");
        }
        
        // claims đai diện cho các thông tin nằm trong payload của jwt
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Role, user.Role)
            // Quan trọng: Claim này (new Claim(ClaimTypes.Role, user.Role),) sẽ giúp mình phân quyền
            
        };
        
        var token = JwtService.GenerateToken(claims, _jwtOptions);
        
        return Ok(token);
    }
    
    [HttpPost("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        //Lấy tất cả user trong DB
        return Ok("Đây là user có trong DB");
    }
}

//BTVN:
// tạo mới user
// GetAll user theo phân trang và search, Order by các thứ
// GetUserById