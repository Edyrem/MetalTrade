namespace MetalTrade.Web.AdminPanel.ViewModels.User
{
    public class IndexUserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Photo { get; set; }
        public List<string>? Roles { get; set; }
    }
}
