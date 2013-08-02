using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading;

namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        static readonly object sync = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFullPath(string filePath)
        {
            ArgumentAssertion.IsNotNull(filePath, "filePath");

            if (filePath.IndexOf(@":\\") != -1)
            {
                return filePath;
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteInfo(string filePath, string content)
        {
            WriteInfo(filePath, content, 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="tryTimes"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void WriteInfo(string filePath, string content, int tryTimes)
        {
            var times = 0;
            while (times < tryTimes)
            {
                lock (sync)
                {
                    try
                    {
                        WriteContent(filePath, content);
                        break;
                    }
                    catch (Exception ex)
                    {
                        times++;
                        Thread.Sleep(1500);
                        if (times == tryTimes)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void Delete(string filePath)
        {
            var fullPath = GetFullPath(filePath);
            File.Delete(fullPath);
        }

        static void WriteContent(string filePath, string content)
        {
            var fullPath = GetFullPath(filePath);
            CreateDirectoryForFile(fullPath);

            using (var sw = new StreamWriter(fullPath, true, Encoding.UTF8))
            {
                sw.Write(content);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool TryDeleteDirectory(string dir)
        {
            var target = GetFullPath(dir);
            if (Directory.Exists(target))
            {
                try
                {
                    Directory.Delete(target, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        public static void CreateDirectory(string dir)
        {
            var dicPath = GetFullPath(dir);
            if (Directory.Exists(dicPath) == false)
            {
                Directory.CreateDirectory(dicPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void CreateDirectoryForFile(string file)
        {
            var fullPath = GetFullPath(file);
            var dicPath = Path.GetDirectoryName(fullPath);
            if (Directory.Exists(dicPath) == false)
            {
                Directory.CreateDirectory(dicPath);
            }
        }        
    }
}
