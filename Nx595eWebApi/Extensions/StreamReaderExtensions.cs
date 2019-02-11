using System.IO;

namespace Nx595eWebApi.Extensions
{
    public static class StreamReaderExtensions
    {
        public static void SkipLines(this StreamReader reader, int count)
        {
            for (int i = 0; i < count; i++) reader.ReadLine();
        }
    }
}
