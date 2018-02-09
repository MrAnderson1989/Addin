using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace AddInWebApp.Common
{
    public static class Log
    {
        //使用
        //Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        //log.log(basePath);

        private static string logFile = AppDomain.CurrentDomain.BaseDirectory + "log\\log.txt";
        private static StreamWriter writer;
        private static FileStream fileStream = null;
        private static object obj = "文件";

        public static void log(string info)
        {
            lock (obj)
            {
                try
                {

                    CreateDirectory(logFile);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);

                    if (!fileInfo.Exists)
                    {
                        fileStream = fileInfo.Create();
                        writer = new StreamWriter(fileStream);
                    }
                    else
                    {
                        fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        writer = new StreamWriter(fileStream);
                    }

                    writer.WriteLine("-----------------" + DateTime.Now + "-----------------");
                    writer.WriteLine(info);
                    writer.WriteLine("------------------------------------------------------");

                }
                finally
                {
                    if (writer != null)
                    {

                        writer.Close();
                        writer.Dispose();
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                }
            }
        }

        public static void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}