namespace SmartCity.AppCore.Features.AppUsers.Queries.Results
{
    public class GetUserRolesResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
