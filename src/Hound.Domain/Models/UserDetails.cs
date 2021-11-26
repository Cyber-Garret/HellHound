namespace Hound.Domain.Models;

public class UserDetails
{
	public UserDetails(string username, string nickname, string reason)
	{
		Username = username;
		Nickname = nickname;
		Reason = reason;
	}

	public string Username { get; set; }
	public string Nickname { get; set; }
	public string Reason { get; set; }

	public override string ToString() =>
		Username + ", " + Nickname + ", " + Reason;
}