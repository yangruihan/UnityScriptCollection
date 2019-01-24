using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Common.Unity
{
    public static class FileUtils
    {
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

        private static string GetAbsolutePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}