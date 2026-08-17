namespace piedteam_hocmienphi.service.Utils.Mail;

public class MailContent
{
    public required string To { get; set; }         //Địa chỉ gửi đến
    public required string Subject { get; set; }    //Chủ để
    public required string Body { get; set; }       //Nội dung
}