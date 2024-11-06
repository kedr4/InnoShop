namespace UserService.Api.Controllers.Users.Requests
{
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; }
    }
}
