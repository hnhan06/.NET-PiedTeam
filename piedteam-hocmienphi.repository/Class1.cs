namespace piedteam_hocmienphi.repository
{
    public class Class1
    {
    }
    
    
    // CODE FIRST | DATABASE FIRST
    // ORM
    
    // Thông thường để mà trên code có thể làm việc được với database
        // thì mình cần phải ánh xạ (mapping) từ các table lên code để dễ dàng thao tác với dữ liệu
    
    // Database First
    // Mình làm việc với 1 database đã có sẵn (Create Database bằng SQL)
    // vô tạo database, setup các field, các mối quan hệ, ràng buộc trong DB
    // sau đó ở trên code sd các Driver hoặc thư viện ORM để kết nối xuống DB
    // ở trên code sẽ tạo ra class tương ứng với các table trong DB
    // Nên sd khi nào? - Khi DB đã có sẵn và đang được sd trong nhiều năm. Được join vào dự án Maintain

    // Code First
    // Mình sẽ không setup DB thủ công bằng các câu lệnh SQL Query
        // Create Database, Create Contraint
    // Mình sẽ Design DB bằng các class trên code, trên code setup ntn thì sẽ được tạo ra như thế
        // mình setup trên code các field, các relationship
        // sau đó mình ánh xạ đến đoạn code để tạo ra các table trong DB
    // Vậy thì làm thế nào để ánh xạ từ code xuống các table trong DB
        // ORM - Object Relational Mapping - Entity Framework
        // nó sẽ là thằng trung gian đứng giữa làm nhiều thứ
            // nó sẽ đọc các class trên code, các attribute, các cấu hình sau đó tạo ra các câu lệnh SQL để tạo ra các bảng
            // nó cũng là thằng kết hợp  với LINQ khi sd các hàm Where..., translate sang SQL
                // .WHERE() => Select * From table Where ...
    // Thằng này đc sd khi:
        // Mình mới bắt đầu dự án, chưa có DB nào cả
        // Thiết kế bằng code thì nó sẽ dễ Maintain (dễ nhìn, dễ sửa đổi) hơn so với các câu lệnh SQL
        
    // Nếu mà không biết về LINQ + EF thì coi như mất 95% sức mạnh

}