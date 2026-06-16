using piedteam_hocmienphi.repository.abstraction;

namespace piedteam_hocmienphi.repository.entity;

public class Category : BaseEntity<Guid>
{
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    
    public ICollection<Category> Children { get; set; } //mảng, array, list
    
    public ICollection<MentorCategory> MentorCategories { get; set; }
    public ICollection<ApplyRequestCategory> ApplyRequestCategories { get; set; }
    
    //Nhờ khai báo ParentId và Parent mà Entity Framework sẽ tự động hiểu rằng:
        // giữa 2 trường này có mối quan hệ với nhau
        // và sẽ tự động tạo ra 1 foreign key trong DB để liên kết 2 bảng Category với nhau
        
    // Nhờ việc khai báo ICollection<Category> Children 
        // mà Entity Framework sẽ tự động hiểu
        // và tạo ra 1 mối quan hệ 1 nhiều
}