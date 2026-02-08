using System.Security.Cryptography;
using System.Text;

namespace VideoGameApi.Api.Services
{
    public interface IProductKeyService
    {
        string Generate();
    }

    public class ProductKeyService : IProductKeyService
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string Generate()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[20];
            rng.GetBytes(bytes);

            var sb = new StringBuilder("VG-");

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);

                if ((i + 1) % 5 == 0 && i != bytes.Length - 1)
                    sb.Append("-");
            }

            return sb.ToString();
        }
    }
}
