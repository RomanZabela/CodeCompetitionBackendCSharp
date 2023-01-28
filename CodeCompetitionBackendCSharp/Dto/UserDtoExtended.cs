namespace CodeCompetitionBackendCSharp.Dto
{
	public class UserDtoExtended
	{
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public bool Blocked { get; set; }	
		public string RoleName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;

    }
}
