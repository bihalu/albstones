using System.Text;

namespace Albstones.Helper
{
    public static class Byte
    {
        public static string BytesToString(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
