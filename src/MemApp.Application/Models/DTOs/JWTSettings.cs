namespace MemApp.Application.Models.DTOs
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }
    }
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderEmail { get; set; }
        public string DeliveryMethod { get; set;}
        public string PickupDirectoryLocation { get; set; }
        public string EnableSsl { get; set; }
        public string Enable { get; set; }
    }
    public class SmsSettings
    {
        public string SingleSmsUrl { get; set; }
        public string SingleSmsSid { get; set; }
        public string SingleSmsCsmsId { get; set; }
        public string SingleSmsApiToken { get; set; }
    }

    public class WhatsAppSettings
    {
        public string ApiUrl { get; set; }
        public string Token { get; set; }
    }

   

}
