using System;

namespace Watchdog
{
    /// <summary>
    /// Represents error that occurs when invalid program command line arguments are provided.
    /// </summary>
    public class ProgramArgsException : Exception
    {
        public ProgramArgsException(string message) : base(message)
        {
        }
        public ProgramArgsException(int argsLength) : base(
            $"Invalid amount of arguments, expected {(ushort)ProgramArgs.Args.Count} and received {argsLength}.\n" +
            "Usage: <path_to_exe> <action> <folder_path> <server_url> <database_name>")
        {
        }
    }

    /// <summary>
    /// Represents the program command line arguments.
    /// </summary>
    public class ProgramArgs
    {
        public enum Args : ushort
        {
            Action,
            FolderPath,
            ServerUrl,
            DatabaseName,

            // Always last
            Count
        }

        public enum Actions : ushort
        {
            SyncFolderToDb
        }

        public readonly Actions Action;
        public readonly string FolderPath;
        public readonly string ServerUrl;
        public readonly string DatabaseName;

        public ProgramArgs(string[] args)
        {
            if (args.Length != (ushort)Args.Count)
            {
                throw new ProgramArgsException(args.Length);
            }

            Action = GetAction(args[(ushort)Args.Action]);
            FolderPath = args[(ushort)Args.FolderPath];
            ServerUrl = args[(ushort)Args.ServerUrl];
            DatabaseName = args[(ushort)Args.DatabaseName];
        }

        private static Actions GetAction(string action)
        {
            return action switch
            {
                "sync_folder_to_db" => Actions.SyncFolderToDb,
                _ => throw new ProgramArgsException($"Invalid action parameter '{action}'."),
            };
        }
    }
}