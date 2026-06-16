namespace piedteam_hocmienphi.service.CategoryService;

public class Request
{
    public class CreateCategoryRequest
    {
        public Guid ParentID { get; set; }
        public string Name { get; set; }
    }

    public class UpdateCategoryRequest : CreateCategoryRequest;
}