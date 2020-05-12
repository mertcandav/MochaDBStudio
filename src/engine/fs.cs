using System.IO;
using System.Text;

namespace MochaDBStudio.engine {
    /// <summary>
    /// FileSystem class for file system tasks.
    /// </summary>
    public sealed class fs {
        public static bool ExistsFolder(string path) =>
            Directory.Exists(path);
        public static bool ExistsFile(string path) =>
            File.Exists(path);
        public static string[] GetAllFolders(string path) =>
            Directory.GetDirectories(path);
        public static string[] GetAllFolders(string path,string searchPattern) =>
            Directory.GetDirectories(path,searchPattern);
        public static string[] GetAllFiles(string path) =>
            Directory.GetFiles(path);
        public static string[] GetAllFiles(string path,string searchPattern) =>
            Directory.GetFiles(path,searchPattern);
        public static FileInfo GetFileInfo(string path) =>
            new FileInfo(path);
        public static DirectoryInfo GetFolderInfo(string path) =>
            new DirectoryInfo(path);
        public static void DeleteFile(string path) =>
            File.Delete(path);
        public static void DeleteFolder(string path) =>
            Directory.Delete(path);
        public static void DeleteFolder(string path,bool all) =>
            Directory.Delete(path,all);
        public static void RenameFile(string path,string newName) =>
            File.Move(path,Combine(GetFileInfo(path).DirectoryName,newName));
        public static void RenameFolder(string path,string newName) =>
            Directory.Move(path,path.Substring(0,path.Length - GetFolderInfo(path).Name.Length) + newName);
        public static void CreateTextFile(string path) =>
            File.CreateText(path);
        public static void CreateFile(string path,string fileName) =>
            File.Create(Combine(path,fileName));
        public static void WriteTextFile(string path,string fileName,string content) =>
            File.WriteAllText(Combine(path,fileName),content,Encoding.UTF8);
        public static void WriteTextFile(string path,string content) =>
            File.WriteAllText(path,content,Encoding.UTF8);
        public static string ReadFileText(string path,string fileName) =>
            File.ReadAllText(Combine(path,fileName),Encoding.UTF8);
        public static string ReadFileText(string path) =>
            File.ReadAllText(path,Encoding.UTF8);
        public static string[] ReadFileLines(string path,string fileName) =>
            File.ReadAllLines(Combine(path,fileName),Encoding.UTF8);
        public static string[] ReadFileLines(string path) =>
            File.ReadAllLines(path,Encoding.UTF8);
        public static void CreateFolder(string path,string folderName) =>
            Directory.CreateDirectory(Combine(path,folderName));
        public static void CopyFile(string path,string copyPath) =>
            File.Copy(path,copyPath);
        public static string Combine(string path1,string path2) =>
            Path.Combine(path1,path2);
        public static string GetFileNameFromPath(string path) {
            var finfo = GetFileInfo(path);
            var name = finfo.Name;
            name = name.Substring(0, name.Length-finfo.Extension.Length);
            return name;
        }
    }
}