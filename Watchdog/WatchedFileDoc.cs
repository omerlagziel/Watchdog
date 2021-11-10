using System.IO;
using System.Security.Cryptography;

namespace Watchdog
{
    /// <summary>
    /// Represents a document for a stored watched file
    /// </summary>
    public class WatchedFileDoc
    {
        public readonly string Id;

        /// <summary>
        /// Readable accessor for the file path.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string FilePath => Id;

        public byte[] Content;
        public byte[] Md5;

        public WatchedFileDoc(string filepath)
        {
            // Id is set as the file path because it's unique,
            // and makes for simpler document handling
            Id = filepath;
            Content = File.ReadAllBytes(filepath);
            Md5 = HashContent(Content);
        }

        /// <summary>
        /// Hash a buffer using the MD5 algorithm.
        /// </summary>
        /// <param name="buffer">Buffer to hash</param>
        /// <returns>MD5 hash</returns>
        private static byte[] HashContent(byte[] buffer)
        {
            using var md5 = MD5.Create();
            md5.TransformFinalBlock(buffer, 0, buffer.Length);

            return md5.Hash;
        }
    }
}
