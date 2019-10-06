using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace ld45
{
    public class filesystem
    {
        private static readonly List<string> Directories = new List<string>();
        private static readonly List<string> ArchivePaths = new List<string>();

        public static void AddDirectory(string path)
        {
            Directories.Add(path);
        }
        
        internal static bool PrintPaks(string[] param)
        {
            string search = param.Length > 0 ? param[0] : "";

            log.WriteLine("indexed directories:");
            foreach (var dir in Directories)
                if (dir.StartsWith(search)) log.WriteLine("- " + dir);

            return false;
        }

        public static string GetBaseDirectory()
        {
            return Directories[0];
        }

        public static string[] GetAllFiles(string searchPattern)
        {
            var s = new List<string>();
            foreach (var dir in Directories)
            {
                if (!Directory.Exists(dir)) continue;

                var fileEntries = Directory.GetFiles(dir, searchPattern);
                foreach (var fileName in fileEntries)
                    s.Add(fileName);
            }

            return s.ToArray();
        }
        public static string GetPath(string filename, bool create = false, bool overwrite = false)
        {
            if (!overwrite)
            {
                foreach (var dir in Directories)
                {
                    var path = dir + "/" + filename;
                    if (File.Exists(path)) return Path.GetFullPath(path);
                }
            }
            else
            {
                if (File.Exists(filename)) File.Delete(filename);
            }

            if (create)
            {
                log.WriteLine("trying to create..");

                var p = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(p) && !Directory.Exists(p))
                    Directory.CreateDirectory(p);

                var s = File.Create(filename);
                s.Close();
                log.WriteLine("created file " + Path.GetFullPath(filename));
                return Path.GetFullPath(filename);
            }

            return null;
        }

        public static bool Exists(string filename, bool checkZips = true)
        {
            foreach (var dir in Directories)
            {
                var path = dir + "/" + filename;
                if (File.Exists(path)) return true;
            }

            return false;
        }

        public static bool TryPath(string filename, out string path)
        {
            foreach (var dir in Directories)
            {
                var tpath = dir + "/" + filename;
                if (File.Exists(tpath))
                {
                    path = Path.GetFullPath(tpath);
                    return true;
                }
            }

            path = null;
            return false;
        }

        public static Stream Open(string filename, bool create = false, bool overwrite = false)
        {
            foreach (var dir in Directories)
            {
                var path = dir + "/" + filename;
                if (File.Exists(path))
                    return File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            if (create)
            {
                return File.Open(GetPath(filename, true, overwrite), FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
            }

            return null;
        }
    }
}