using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


/*Написать приложение, которое ищет в указанном каталоге файлы,
 * удовлетворяющие заданной маске, у которых дата последней модификации 
 * находится в указанном диапазоне. Поиск производится как в указанном каталоге, 
 * так и в его подкаталогах. Результаты поиска сбрасываются в файл отчета.*/

namespace Working_with_files_and_directories
{
    class Program
    {
        const string RESULTFILEPATH = "reults.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Input folder path you would like to search files in: ");
            string baseFolderPath = Console.ReadLine();
            if (baseFolderPath[baseFolderPath.Length - 1] != '\\')
            {
                baseFolderPath += '\\';
            }
            DirectoryInfo di = new DirectoryInfo(baseFolderPath);
            if (!di.Exists)
            {
                Console.WriteLine("Invalid path!");
                return;
            }

            Console.WriteLine("Input what to search for." +
                "\n('*' for any sequence of characters, '?' for single any character)");
            string mask = Console.ReadLine();
            Regex regExMask = DosMaskToRegEx(mask);

            DateTime from, to;
            Console.Write("Now, specify date range withing which files were modified usind dd-mm-yyyy format.");
            Console.Write("\nDate from: ");
            while (!DateTime.TryParse(Console.ReadLine(), out from))
            {
                Console.WriteLine("Wrong date, try again!");
            }
            Console.Write("Date to: ");
            while (!DateTime.TryParse(Console.ReadLine(), out to))
            {
                Console.WriteLine("Wrong date, try again!");
            }

            try
            {
                List<string> result = new List<string>();
                List<FileInfo> foundFiles = FindFilesInFolderRecursivelyByMask(di, regExMask);
                foreach (FileInfo fi in foundFiles)
                {
                    if (FileModifiedInDateRange(fi, from, to))
                    {
                        result.Add(fi.FullName);
                    }
                }
                foreach (string s in result)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("\n {0} total files found", result.Count);
                WriteStringsToFile(result, RESULTFILEPATH);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static List<FileInfo> FindFilesInFolderRecursivelyByMask(DirectoryInfo folder, Regex regMask)
        {
            List<FileInfo> allFoundFiles = new List<FileInfo>();
            FileInfo[] filesInCurFolder = null;
            try
            {
                filesInCurFolder = folder.GetFiles();
            }
            catch
            {
                return allFoundFiles;
            }

            foreach (FileInfo fi in filesInCurFolder)
            {
                if (regMask.IsMatch(fi.Name))
                {
                    allFoundFiles.Add(fi);
                }
            }

            DirectoryInfo[] SubDi = folder.GetDirectories();
            foreach (DirectoryInfo di in SubDi)
            {
                allFoundFiles.AddRange(
                    FindFilesInFolderRecursivelyByMask(di, regMask));
            }
            
            return allFoundFiles;
        }

        static Boolean FileModifiedInDateRange(FileInfo file, DateTime from, DateTime to)
        {
            if (file.LastWriteTime > from && file.LastWriteTime < to)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static Regex DosMaskToRegEx(string Mask)
        {
            Mask = Mask.Replace(".", @"\.");
            Mask = Mask.Replace("?", ".");
            Mask = Mask.Replace("*", ".*");
            Mask = "^" + Mask + "$";
            return new Regex(Mask, RegexOptions.IgnoreCase);
        }

        static void WriteStringsToFile(List<string> content, string filePath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                foreach (string s in content)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
    }
}
