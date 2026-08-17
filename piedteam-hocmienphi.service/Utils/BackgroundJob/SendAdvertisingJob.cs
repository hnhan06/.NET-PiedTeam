using Microsoft.EntityFrameworkCore;
using piedteam_hocmienphi.repository;
using piedteam_hocmienphi.repository.entity;
using piedteam_hocmienphi.service.Utils.Mail;
using Quartz;

namespace piedteam_hocmienphi.service.Utils.BackgroundJob;

[DisallowConcurrentExecution]
// Tôi bắt buộc Job trước đó hoàn thành xong thì mới chạy tiếp

public class SendAdvertisingJob : IJob
{
    private readonly AppDbContext _dbContext;
    private readonly IService _mailService;
    
    public SendAdvertisingJob(AppDbContext dbContext, IService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(
                DateTimeOffset.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
            ).DateTime
        );
        //
        int batchSize = 50;
        var notSendTodayUsers = await GetBatchUsersNotSendMailToday(today, batchSize);
        await SendBatchMail(notSendTodayUsers, today);
        
        _dbContext.Users.UpdateRange(notSendTodayUsers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetBatchUsersNotSendMailToday(DateOnly today, int batchSize)
    {
        var query = _dbContext.Users.Where(u => u.IsDeleted == false);

        query = query.Where(u => u.SendDate < today);

        query = query.OrderBy(u => u.Id).Take(batchSize);

        var result = await query.ToListAsync();

        return result;
    }

    public async Task SendBatchMail(List<User> users, DateOnly today)
    {
        // mang user -> 1 mang cong viec can gui
        // hanh dong nay goi la anh xa
        // -> select
        
        /*
         var taskList = new List<Task>();
        
        foreach (var x in users)
        {
            var mailContent = new MailContent()
            {
                To = x.Email,
                Body = "HI",
                Subject = "HI",
            };
            
            var userTask = Task.Run(async () =>
            { 
                await _mailService.SendMail(mailContent);
            });
            
            taskList.Add(userTask);
            x.SendDate = today;
        }
         */

        var taskList = users.Select(async user =>
        {
            try
            {
                var mailContent = new MailContent() { To = user.Email, Body = "HI", Subject = "HI" };
                await _mailService.SendMail(mailContent);
                user.SendDate = today;
            }
            catch (Exception ex)
            {
                // LOG lỗi
            }
        });
        
        await Task.WhenAll(taskList);
    }
}

public static class TimeZoneUtils
{
    public static TimeZoneInfo GetVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Windows
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Linux/macOS
        }
    }
}

/*
     Background Job:
        Là 1 tác vụ được thực thi ngầm, tách biệt khỏi luồng xử lí chính
        Tác vụ do hệ thống thực thi
        
    Khi nào server mình sẽ chạy công việc này:
        Setup định kỳ (CronJob):
            + cứ mỗi 7h sáng là gửi thư quảng cáo cho khách hàng
            + xóa mềm (IsDeleted = true), theo tgian những record xóa mềm sẽ tăng lên,
                nhma những dòng record này thì cta ko động đến
            => Dữ liệu dư thừa, database nặng lên, phải định kỳ xóa nó đi
        Mình sẽ nhờ nó làm công việc này giúp mình (bận quá)
            *** 1 hàm Register bao gồm:
                xuống DB kiểm tra email tồn tại (10ms)
                tính toán để tạo Password Hashing (2ms)
                lưu User này vào Database (5ms)
                gửi email thông báo verify tài khoản (10s)
            Logic gửi email là 1 logic rất là nặng và tốn nhiều tgian để hoàn thành
                ta có thể chọn khi user bấm register khỏi cho đợi email, trả ra response
                cái việc mà gửi email thông báo sẽ do luồng background job gửi
    
    Bây giờ muốn gửi mail cho tất cả các user trong hệ thống vào lúc 7h sáng
        Đối với user ít, tầm 20 người thì ngon
            Cứ 7h sáng gọi 20 người đó ra gửi mail thôi
        Đối với 1 triệu user thì sao, 1.000.000 * 20s
            Xài Task.WhenAll để tối ưu (trong vòng 20s đó sẽ có 100 người đươc gửi mail)
                => trong vòng 5 giờ sẽ gửi xong cho 1tr người
            Tại sao trong vòng 20s không gửi 1tr người luôn?
                - 1 task được ra, giống như mở thêm 1 luồng bất đồng bộ chạy
                server xử lí 1tr luồng cùng 1 lúc ko nổi
                nên là mới đánh đổi tgian lại, dài hơn 1 xíu, 1 lần gửi 1000 người thôi
                nhưng server vẫn chịu đc và sống tốt
        Thêm 1 vấn đề nữa, bây giờ mình chỉ gửi mail 1 lúc 1000 người thôi,
            vậy thì bây giờ cta phải có 1 cái gì đó để đánh dấu
            là user nào đã gửi, user nào chưa gửi
            => Nghĩ ngay đến isSend
            cái isSend này chỉ ngon khi dòng dữ liệu này đụng đến 1 lần rồi thôi
            VD trạng thái của đơn hàng - isPending
            nếu như mà trong vòng 1 giờ không thanh toán, thì mình chuyển thành fail
                
            Vậy isSend thì sao? Nếu để isSend ở user, mình sẽ k biết đc user nào đã gửi hay chưa
            Ngày 6 để isSend = true, qua ngày 7 isSend cũng = true luôn, vậy làm sao hệ thống phân biệt
     */