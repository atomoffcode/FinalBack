namespace RazerFinal.Models
{
    public class UserToken : BaseEntity
    {
        public AppUser? User { get; set; }
        public string? Token { get; set; }
    }
}
