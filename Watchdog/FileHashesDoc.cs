using System.IO;
using Raven.Client.Documents;

namespace Watchdog
{
    /// <summary>
    /// Represents the document containing all of the hashes of the stored watched files
    /// </summary>
    public class FileHashesDoc
    {
        public readonly string Id;
        public string FolderPath => Id;
        public FileHashes FileHashes;

        /// <summary>
        /// Constructor for creating a new document
        /// </summary>
        /// <param name="folderPath">Path to watched folder</param>
        public FileHashesDoc(string folderPath)
        {
            Id = folderPath;
            FileHashes = new FileHashes();
        }

        /// <summary>
        /// Sync the stored files and their hashes with the existing local files and update the DB.
        /// </summary>
        /// <param name="documentStore">RavenDB document store</param>
        public void Sync(IDocumentStore documentStore)
        {
            var localFiles = Directory.GetFiles(FolderPath);

            // Update hash (and stored file) for each file in the watched directory
            foreach (var filepath in localFiles)
            {
                var fileDoc = new WatchedFileDoc(filepath);
                FileHashes.Update(fileDoc, documentStore);
            }

            // Remove hash (and stored file) for each hash entry that doesn't correlate to an existing file
            FileHashes.Remove(localFiles, documentStore);

            // Update metadata document
            using (var session = documentStore.OpenSession())
            {
                session.Store(this);
                session.SaveChanges();
            }
        }
    }
}