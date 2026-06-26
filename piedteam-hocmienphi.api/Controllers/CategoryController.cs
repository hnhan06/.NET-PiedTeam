using Microsoft.AspNetCore.Mvc;
using piedteam_hocmienphi.repository;
using piedteam_hocmienphi.repository.entity;
using piedteam_hocmienphi.service.CategoryService;
using piedteam_hocmienphi.service.UserService;
using Request = piedteam_hocmienphi.service.CategoryService.Request;
using Response = piedteam_hocmienphi.service.CategoryService.Response;

namespace piedteam_hocmienphi.api.Controllers;

[ApiController]
[Route("[controller]")]

public class CategoryController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public CategoryController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    //Dependency Injection (đây là thuật ngữ Core, k biết = rớt pv)
    
    //Khai báo các API sau:
    // Get all category
    [HttpGet("")]
    public IActionResult GetCategories()
    {
        var query = _dbContext.Categories.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.ParentId == null);
        
        query = query.OrderBy(x => x.Name);

        var selectedQuery = query.Select(x => new Response.GetAllParentCategoryResponse()
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
        //Mục đích của tạo GetAllParentCategoryResponse
        // API này chỉ cần 2 field Id, Name th, nhưng trong Db lúc nào cũng quăng ra full (id, name, parentId) nhưng api chỉ cần 2 field
        // Quăng ra mà k sài thì phí lắm

        var result = selectedQuery.ToList();
        
        return Ok(result);
    }
    
    // create category
    //yêu cầu có body là name và parentId
    [HttpPost("")]
    public IActionResult CreateCategory(Request.CreateCategoryRequest requestBody)
    {
        //requestBody do FE truyền cho mình khi FE gọi API
        // có nghĩa là nó muốn tạo record - 1 dòng dưới DB của mình
        // 1 dòng dưới Db sẽ có các field: Id, Name, ParentId
        
        //Lúc này BE phải tạo ra 1 object từ class Category và ínsert vào DB
        // vì Category table dưới DB nó đc tạo ra từ class Category
        // nên để add dữ liệu vào table Category th mình phải new mới
        // 1 object Category thì mới add được

        var newCategory = new Category()
        {
            Id = Guid.NewGuid(),
            Name = requestBody.Name,
            ParentId = requestBody.ParentID
        };
        
        _dbContext.Categories.Add(newCategory);
        // chỉ mới add vào bộ nhớ chưa add vào database
        _dbContext.SaveChanges();
        
        //Lấy tất cả Category trong DB
        return Ok();
    }
    
    // get category by id
    [HttpGet("{categoryId}")]
    public IActionResult GetCategoriesById(Guid categoryId)
    {
        var query = _dbContext.Categories.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.ParentId == categoryId && x.ParentId != null);
        
        query = query.OrderBy(x => x.Name);

        var selectedQuery = query.Select(x => new Response.GetAllParentCategoryResponse()
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
        
        var result = selectedQuery.ToList();
        
        return Ok(result);
    }
    
    // update category by id
    [HttpPut("{categoryId}")]
    public IActionResult UpdateCategoriesById(Guid categoryId, Request.UpdateCategoryRequest requestBody)
    {
        var query = _dbContext.Categories.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id == categoryId);
        
        var category = query.FirstOrDefault();

        if (category != null)
        {
            category.Name = requestBody.Name;
            category.ParentId = requestBody.ParentID;
            
            _dbContext.Categories.Update(category);
            _dbContext.SaveChanges();
        }
        
        return Ok();
    }
    
    // delete category by id
    [HttpDelete("{categoryId}")]
    public IActionResult DeleteCategoriesById(Guid categoryId)
    {
        // có id rồi thì bây giờ phải lấy thằng category trong DB ra, sau đó xóa nó
        var query = _dbContext.Categories.Where(x => x.IsDeleted == false);
        
        query = query.Where(x => x.Id == categoryId);
        
        var category = query.FirstOrDefault();

        if (category != null)
        {
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
        }
            
        return Ok();
    }
}