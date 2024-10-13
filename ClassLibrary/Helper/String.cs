using System.Text;

namespace Albstones.Helper
{
    public static class String
    {

        public static byte[] ToBytes(this string input, int count = 0)
        {

            if (count > 0)
            {
                int padding = input.Length < count ? count - input.Length : 0;
                string inputPadding = input + new string(' ', padding);

                return Encoding.UTF8.GetBytes(inputPadding[..count]);
            }
            else
            {
                return Encoding.UTF8.GetBytes(input);
            }
        }
    }
}
