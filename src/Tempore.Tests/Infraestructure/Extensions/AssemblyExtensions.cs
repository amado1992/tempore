namespace Tempore.Tests.Infraestructure.Extensions
{
    using System.IO;
    using System.Reflection;

    public static class AssemblyExtensions
    {
        public static byte[] GetFileContent(this Assembly assembly, string resourceFileName)
        {
            using var manifestResourceStream = assembly.GetManifestResourceStream(resourceFileName);
            using var memoryStream = new MemoryStream();
            manifestResourceStream!.CopyTo(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }
    }
}