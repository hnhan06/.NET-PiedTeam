using Microsoft.EntityFrameworkCore;
using piedteam_hocmienphi.repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// Kiến trúc 3 Layer (tầng)

// Tầng API
    // Chịu trách nhiệm khai báo các endpoint
        // nhận req, trả ra res
    // config hệ thống
    // tầng API gọi tới Sevice
// Tầng Service
    // Chịu trách nhiệm  xử lý nghiệp vụ
    // tương tác với tầng Repository để lấy dữ liệu
    // tầng Service gọi tới Repository
// Tầng Repository
    // Chịu trách nhiệm tương tác với database
    // cấu hình những thứ liên quan tới database
    
// A có 1 cái Req đăng nhập vào hệ thống
    // Tầng API: muốn đăng nhập hệ thống
        // chui vào đây nè: POST /api/auth/login  
            // nhận req body {email: "tan", password: "123"}
    // Tầng API lúc này gọi xuống tầng Service có cái hàm là
        // Xử lí login: LoginHandler (email, password)
        // Lúc này hàm login trong Service hãy chạy như sau
            // kiểm tra email | ng này có tồn tại trong database hay không
            // ng dùng này có bị banned hay không
            // nếu có lỗi thì trả về lỗi
            // nếu không có lỗi thì trả về Token đăng nhập
    // Tầng Service lúc này gọi xuống tầng Repository có cái hàm là
        // GetUserByEmail (email)
        // hàm này sẽ chạy câu lệnh SQL để lấy thông tin ng dùng ra khỏi database
        
        
// Một quá trình phát triển phần mềm thường sẽ trải qua:
    // Đi tìm hiểu nhu cầu của khách hàng
        // hiểu được nhu cầu rồi thì phân tách ra các Requirement
    // Sau đó dựa vào các Requirement, chúng ta thiết kế ra hệ thống
        // Thiết kê kiến trúc của hệ thống
        // Thiết ế ra database
    // Implements các Requirement lên code
    // Test các Requirement đã được Implement
    // Đóng gói - deploy - giao hàng
    // Nhận tiền
    // Bảo trì

// Nơi cho các học sinh đi vào nên tảng tiềm kiếm các Mentor để học tập, trao đổi kiền thức, kinh nghiệm với nhau
// Khi mà mentor muốn apply vào nền tảng, thì phải điền thông tin, sau đó Admin sẽ duyệt
// Mentor thì đi vào nền tảng, tạo những lịch rảnh và các hs sẽ book
// Để 1 User có thể trở thành 1 Mentor thì User sẽ điền thông tin để apply làm Mentor
        // 1 User sẽ có yêu cầu (ApplyRequest) trở thành Mentor
        // Mqh  giữa User và ApplyRequest
        // => Mqh 1-N: có thể có đơn bị từ chối hoặc duyệt
            // Khi mà có User nộp đơn apply thì sẽ thông báo cho Admin
            // Khi mà Admin duyệt (accept, reject) thì sẽ thng báo cho User
    // Admin sẽ duyệt những thông tin đó, nếu thông tin hợp lệ sẽ duyệt
    // Nếu được duyệt, User sẽ trở thành Mentor
    // Chỉ có User nào có quyền Admin thì mới được sd các API như lấy đơn hệ thống/phê duyệt đơn

    // API:
        // Tạo đơn
            // POST /api/applyRequest
                //để gọi đc API này, cần cv và mô tả bản thân
        // Lấy các đơn apply của tôi
            // GET /api/applyRequest/me
            // Mentor cần api này để xem tiến độ | tình trạng của đơn
        // Lấy tất cả các đơn apply (dành cho Admin)
            // GET /api/applyRequest
            // Admin cần api này để duyệt đơn User
        // Lấy thông tin chi tiết của đơn này (dành cho Admin)
            // GET /api/applyRequest/{id}
        // Duyệt đơn apply (dành cho Admin)
            // POST /api/applyRequest/{id}/review
            // Khi duyệt đơn Admin sẽ chọn đồng ý hoặc từ chối
            // Nếu từ chối phải có lý do
            // Khi duyệt đơn xong thì role của User phải được đổi thành Mentor
            // Khi duyệt đơn, hệ thống sẽ thông báo cho User về kq của đơn apply đó

// App này bao gồm nhiều người xài:
    // Admin:
        // Quản lí User (hs và mentor) của nền tảng
        // Quản lí và phê duyệt đơn để trở thành Mentor
    // Mentor:
        // tạo lịch trình để hs book
        // quản lí lịch book
        // quản l profile
    // Student
        
        
// Kỹ thuật Snapshoot
    // Trong năm 2026 anh Tân bán áo giá 1000
    // Bảo thấy đẹp nên mua tại thời điểm đó
    // Năm 2027 anh Tân đổi giá áo thành 2000
    // Nếu không có lưu trữ ngày mua và giá thành thời điểm đó thì sẽ bị hiểu sai giá
    //** Giống vụ mấy năm trước mượn vàng với giá 32tr nhưng bây h trả tiền thì bị đòi vàng với giá là 70tr
