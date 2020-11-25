using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MakeMyIndex
{
    class Program
    {
        private const string CONFKEY_COMPUTERNAME = "computername";
        private const string CONFKEY_DIR = "dir";
        private const string CONFKEY_SHARE_DIR = "shareDir";
        private const string CONFKEY_BACLUP_DIR = "backupDir";
        private const string CONFKEY_BACLUP_SHARE_DIR = "backupShareDir";

        static void Main(string[] args)
        {
            bool flagUsage = false;
            bool flagDir = false;
            string dir = null;
            string shareDir = null;
            string backupDir = null;
            string backupShareDir = null;
            string myIndexFilename = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (String.Compare(arg, "/f", true) == 0 || String.Compare(arg, "-f", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    dir = args[++i];
                    flagDir = false;
                }
                else if (String.Compare(arg, "/d", true) == 0 || String.Compare(arg, "-d", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    dir = args[++i];
                    flagDir = true;
                }
                else if (String.Compare(arg, "/s", true) == 0 || String.Compare(arg, "-s", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    shareDir = args[++i];
                }
                else if (String.Compare(arg, "/b", true) == 0 || String.Compare(arg, "-b", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    backupDir = args[++i];
                }
                else if (String.Compare(arg, "/bs", true) == 0 || String.Compare(arg, "-bs", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    backupShareDir = args[++i];
                }
                else if (String.Compare(arg, "/o", true) == 0 || String.Compare(arg, "-o", true) == 0)
                {
                    if (i + 1 >= args.Length)
                    {
                        flagUsage = true;
                        break;
                    }

                    myIndexFilename = args[++i];
                }
                else
                {
                    flagUsage = true;
                    break;
                }
            }

            if (dir == null || myIndexFilename == null)
            {
                flagUsage = true;
            }

            if (flagUsage)
            {
                Console.WriteLine("usage: MakeMyIndex [options] /o indexFilename");
                Console.WriteLine("       /f dir             対象ディレクトリ配下のファイル名をインデックスに格納する(デフォルト)");
                Console.WriteLine("       /d dir             対象ディレクトリ配下のディレクトリ名をインデックスに格納する");
                Console.WriteLine("       /s shareDir        共有フォルダを指定する");
                Console.WriteLine("       /b backupDir       バックアップディレクトリ指定");
                Console.WriteLine("       /bs backupShareDir バックアップ共有フォルダ指定");
                Console.WriteLine("       /o indexFilename   出力ファイル指定(必須)");

                return;
            }

            using (StreamWriter sw = new StreamWriter(myIndexFilename, false, Encoding.GetEncoding("shift_jis")))
            {
                sw.WriteLine(CONFKEY_COMPUTERNAME + "=" + Environment.MachineName);

                sw.WriteLine(CONFKEY_DIR + "=" + dir);

                if (shareDir != null)
                {
                    sw.WriteLine(CONFKEY_SHARE_DIR + "=" + shareDir);
                }

                if (backupDir != null)
                {
                    sw.WriteLine(CONFKEY_BACLUP_DIR + "=" + backupDir);
                }

                if (backupShareDir != null)
                {
                    sw.WriteLine(CONFKEY_BACLUP_SHARE_DIR + "=" + backupShareDir);
                }

                sw.WriteLine("");

                List<String> list = null;
                if (flagDir)
                {
                    list = GetDirectories(dir);
                }
                else
                {
                    list = GetFiles(dir);
                }

                foreach (string fullName in list)
                {
                    string partName = fullName.Substring(dir.Length + 1);
                    sw.WriteLine(partName);
                }
            }
        }

        private static List<string> GetFiles(string dir)
        {
            List<string> retList = new List<string>();

            string[] subDirs = Directory.GetDirectories(dir);
            foreach(string subDir in subDirs)
            {
                List<string> subDirFileList = GetFiles(subDir);
                retList.AddRange(subDirFileList);
            }

            string[] files = Directory.GetFiles(dir);
            retList.AddRange(files);

            return retList;
        }

        private static List<string> GetDirectories(string dir)
        {
            List<string> retList = new List<string>();

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                List<string> subDirDirList = GetDirectories(subDir);
                if (subDirDirList.Count == 0)
                {
                    retList.Add(subDir);
                }
                else
                {
                    retList.AddRange(subDirDirList);
                }
            }

            return retList;
        }
    }
}
