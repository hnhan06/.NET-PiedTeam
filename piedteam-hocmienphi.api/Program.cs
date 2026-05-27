var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

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
      