using System.Net;
using System.Net.Mail;

namespace dotnet_store.Services;

public interface IEmailService
{
    // Burada kendimize bir Email servisi oluşturmuş olduk tabi ki asenkron. Task o işe yarar
    Task SendEmailAsync(string email, string subject, string message);
}

public class SmtpEmailService : IEmailService
{
    // Bu alan Interface'in içe eklemesi
    // -----------------------------------------

    private IConfiguration _configuration; // Bunu yazma sebebimiz appsettings.json dosyasından verileri alabilmek için

    public SmtpEmailService(IConfiguration configuration)
    { 

        _configuration = configuration;
    }

    // Burada email gönderme işlemini yapıyoruz.

    public async Task SendEmailAsync(string email, string subject, string message)
    {

        using (var client = new SmtpClient(_configuration["Email:Host"]))
        {
            client.UseDefaultCredentials = false;// Bunu diyerek kendi ayarlarımızı kullanacağımızı belirtiyoruz.
            client.Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]);

            client.Port = 587; // Bu port numarsı gmail için geçerlidir.
            client.EnableSsl = true; // SSL kullanmak zorundayız çünkü güvenli bir bağlantı kurmamız lazım.
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Username"]!), // Buraya email'in kimden gideceğini yazıyoruz.
                Subject = subject, // Buraya email'in konusunu yazıyoruz.
                Body = message, // Buraya email'in içeriğini yazıyoruz.
                IsBodyHtml = true // Buraya email'in içeriğinin html formatında olduğunu belirtiyoruz.
            };
            mailMessage.To.Add(email); // Buraya email'in kime gideceğini yazıyoruz.
            await client.SendMailAsync(mailMessage); // Burada email'i gönderiyoruz.
        }
    }
}
