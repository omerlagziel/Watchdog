using System;
using Raven.Client.Documents;

namespace Watchdog
{
    class Program
    {
        /// <summary>
        /// Initialize the RavenDB document store.
        /// </summary>
        /// <param name="serverUrl">URL address of the RavenDB server</param>
        /// <param name="databaseName">Name of the RavenDB database</param>
        /// <returns>The initialized document store</returns>
        private static DocumentStore InitializeStore(string serverUrl, string databaseName)
        {
            var documentStore = new DocumentStore
            {
                Urls = new[] { serverUrl },
                Database = databaseName
            };

            documentStore.Initialize();
            return documentStore;
        }

        /// <summary>
        /// Sync the stored files and their hashes with the existing local files
        /// in the folder and update the DB.
        /// </summary>
        /// <param name="documentStore">RavenDB document store</param>
        /// <param name="folderPath">Path to watched folder</param>
        private static void SyncFolder(IDocumentStore documentStore, string folderPath)
        {
            // Get the stored files' hashes from the DB
            FileHashesDoc fileHashesDoc;
            using (var session = documentStore.OpenSession())
            {
                fileHashesDoc = session
                    .Load<FileHashesDoc>(folderPath) ?? new FileHashesDoc(folderPath);
            }

            fileHashesDoc.Sync(documentStore);
        }

        static void Main(string[] args)
        {
            try
            {
                var programArgs = new ProgramArgs(args);
                var documentStore = InitializeStore(programArgs.ServerUrl, programArgs.DatabaseName);

                switch (programArgs.Action)
                {
                    case ProgramArgs.Actions.SyncFolderToDb:
                        SyncFolder(documentStore, programArgs.FolderPath);
                        break;

                    default:
                        break;
                }
            }
            catch (ProgramArgsException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
