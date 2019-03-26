using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Common.Unity
{
    public static class FileUtils
    {
        public const string AssetsFolderName = "Assets";

        public static string ReadAllText(string fileName)
        {
            return File.ReadAllText(GetAbsolutePath(fileName));
        }

        public static string[] ReadAllLines(string fileName)
        {
            return File.ReadAllLines(GetAbsolutePath(fileName));
        }

        public static void DeleteFile(string fileName)
        {
            File.Delete(GetAbsolutePath(fileName));
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(GetAbsolutePath(fileName));
        }

        public static void CreateTextFile(string fileName, string content)
        {
            var fileInfo = new FileInfo(GetAbsolutePath(fileName));

            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            var sw = fileInfo.CreateText();
            sw.Write(content);
            sw.Close();
            sw.Dispose();
        }

        public static void AppendTextFile(string fileName, string content)
        {
            var t = new FileInfo(GetAbsolutePath(fileName));

            if (t.Directory != null && !t.Directory.Exists)
            {
                t.Directory.Create();
            }

            var sw = !t.Exists ? t.CreateText() : t.AppendText();

            sw.Write(content);
            sw.Close();
            sw.Dispose();
        }

        public static void CreateSerializableAsset(string fileName, object serializableObj)
        {
            FileStream file = null;
            try
            {
                var filePath = GetAbsolutePath(fileName);
                var bf = new BinaryFormatter();

                file = File.Exists(filePath) ? File.Open(filePath, FileMode.OpenOrCreate) : File.Create(filePath);
                bf.Serialize(file, serializableObj);
            }
            catch (Exception e)
            {
                Debug.LogError("[FileUtils] CreateSerializableAsset Error: " + e);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        public static object ReadSerializableAsset(string fileName)
        {
            object ret = null;
            FileStream file = null;
            try
            {
                var filePath = GetAbsolutePath(fileName);
                var bf = new BinaryFormatter();

                if (File.Exists(filePath))
                    file = File.Open(filePath, FileMode.Open);
                else
                    return null;

                ret = bf.Deserialize(file);
            }
            catch (Exception e)
            {
                Debug.LogError("[FileUtils] ReadSerializableAsset Error: " + e);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            return ret;
        }

        public static void CreateBinaryFile(string fileName, byte[] content, int length)
        {
            var filePath = GetAbsolutePath(fileName);
            var fileInfo = new FileInfo(filePath);

            try
            {
                if (fileInfo.Exists)
                    DeleteFile(fileName);

                Stream s;
                using (s = fileInfo.Create())
                    s.Write(content, 0, length);
            }
            catch (IOException e)
            {
                Debug.LogError("[FileUtils] CreateBinaryFile Error: " + e);
            }
        }

        public static Stream ReadBinaryFile(string fileName)
        {
            var filePath = GetAbsolutePath(fileName);

            if (!File.Exists(filePath)) return null;

            var fs = new FileStream(filePath, FileMode.Open);

            return fs;
        }

        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        public static string FullPathToAssetPath(string fullPath)
        {
            fullPath = FormatToUnityPath(fullPath);
            if (!fullPath.StartsWith(Application.dataPath))
            {
                return null;
            }

            var retPath = fullPath.Replace(Application.dataPath, "");
            return AssetsFolderName + retPath;
        }

        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path)?.ToLower();
        }

        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }

        public static string[] GetAllFilesInFolder(string path)
        {
            return GetSpecifyFilesInFolder(path);
        }

        public static string[] GetAllDirsInFolder(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var fileInfo = new FileInfo(filePath);
            var dirInfo = fileInfo.Directory;
            if (dirInfo != null && !dirInfo.Exists)
            {
                Directory.CreateDirectory(dirInfo.FullName);
            }
        }

        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeWriteAllBytes failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeWriteAllLines failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllText(outFile, text);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeWriteAllText failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeReadAllBytes failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static string[] SafeReadAllLines(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeReadAllLines failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeReadAllText failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static void DeleteDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath);
            var dirs = Directory.GetDirectories(dirPath);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }

        public static bool SafeClearDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }

                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeClearDir failed! path = {folderPath} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeDeleteDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeDeleteDir failed! path = {folderPath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }

                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeCopyFile(string fromFile, string toFile)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFile))
                    return false;

                if (!File.Exists(fromFile))
                    return false;

                CheckFileAndCreateDirWhenNeeded(toFile);
                SafeDeleteFile(toFile);
                File.Copy(fromFile, toFile, true);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"SafeCopyFile failed! formFile = {fromFile}, toFile = {toFile}, with err = {ex.Message}");
                return false;
            }
        }

        private static string GetAbsolutePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}