namespace Auth.Application.DTO
{
    public class RequestConfirmEmailDTO
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
