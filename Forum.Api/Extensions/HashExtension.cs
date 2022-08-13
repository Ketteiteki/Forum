using System.Security.Cryptography;
using System.Text;

namespace Forum.Api.Extensions;

public static class HashExtension
{
	public static string ToHMACSHA512CryptoHash(this string value, string salt)
	{
		var hmac = new HMACSHA512 {Key = Encoding.Default.GetBytes(salt)};
		byte[] bytes = Encoding.Default.GetBytes(value);
		byte[] result = hmac.ComputeHash(bytes);
		return Convert.ToBase64String(result);
	}
}