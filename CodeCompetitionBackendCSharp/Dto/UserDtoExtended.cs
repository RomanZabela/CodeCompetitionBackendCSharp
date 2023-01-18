namespace CodeCompetitionBackendCSharp.Dto
{
	public class UserDtoExtended
	{
        public string Id { get; set; }
        public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool Blocked { get; set; }
		public string RoleName { get; set; }
        public string RoleId { get; set; }

    }
}
