namespace StartupsFront.Models
{
    public class UserModel
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public UserModel()
        {

        }
    }
}
