using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;

namespace Watchdog
{
    /// <summary>
    /// Manages the hashes of the stored files and their storage in the DB.
    /// </summary>
    public class FileHashes
    {
        /// <summary>
        /// A dictionary of the stored files' hashes.
        /// Key is the filepath, value is the MD5 hash of the file's contents.
        /// </summary>
        public readonly Dictionary<string, byte[]> FileMd5Hashes;

        public FileHashes()
        {
            FileMd5Hashes = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Update a file's hash in the dictionary and store its contents in the DB.
        /// </summary>
        /// <param name="fileDoc">New or updated file to store</param>
        /// <param name="documentStore">RavenDB document store</param>
        public void Update(WatchedFileDoc fileDoc, IDocumentStore documentStore)
        {
            if (FileMd5Hashes.TryGetValue(fileDoc.FilePath, out byte[] md5))
            {
                // If file hasn't changed, there's nothing to update
                if (md5.SequenceEqual(fileDoc.Md5))
                {
                    return;
                }

                // If file has changed, update hash
                FileMd5Hashes[fileDoc.FilePath] = fileDoc.Md5;
            }
            else
            {
                // If doesn't already exist , add new metadata entry
                FileMd5Hashes.Add(fileDoc.FilePath, fileDoc.Md5);
            }

            // Store the new/updated file
            using (var session = documentStore.OpenSession())
            {
                session.Store(fileDoc);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Remove a file's hash from the dictionary and remove its contents from the DB.
        /// </summary>
        /// <param name="files">List of paths for all local files in the watched directory</param>
        /// <param name="documentStore">RavenDB document store</param>
        public void Remove(string[] files, IDocumentStore documentStore)
        {
            // For each hash entry that doesn't correlate to an existing file
            foreach (var filepath in FileMd5Hashes.Keys.Where(filepath => !files.Contains(filepath)))
            {
                // Remove it from the hash dict
                FileMd5Hashes.Remove(filepath);

                // Remove the document from the DB
                using (var session = documentStore.OpenSession())
                {
                    session.Delete(filepath);
                    session.SaveChanges();
                }
            }
        }
    }
}