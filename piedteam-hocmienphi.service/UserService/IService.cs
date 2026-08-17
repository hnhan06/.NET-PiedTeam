namespace piedteam_hocmienphi.service.UserService;

public interface IService
{
    public Task<List<Response.GetAllUserResponse>> GetAllUsers(string? searchTerm, int pageIndex, int pageSize);
    
    public Task<string> CreateUser(Request.CreateUserRequest requestBody);
    
    public Task<string> UpdateUser(Guid id, Request.UpdateUserRequest requestBody);
    
    public Task<string> Login(string email, string password);
}