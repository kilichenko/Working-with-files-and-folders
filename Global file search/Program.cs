using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/*Написать приложение для поиска по всему диску файлов и каталогов, 
 * удовлетворяющих заданной маске. Необходимо вывести найденную информацию 
 * на экран в компактном виде (с нумерацией объектов) и запросить
 * у пользователя о дальнейших действиях. Варианты действий: удалить все найденное, 
 * удалить указанный файл (каталог), удалить диапазон файлов (каталогов).*/

namespace Global_file_search
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMenu mainMenu = new ConsoleMenu();
            mainMenu.AddItem("Search for Folders", SearchFolers)
                .AddItem("Search for Files", SearchFiles)
                .AddItem("Exit", () => { return; } );
            mainMenu.Execute();
        }

        static void GetDirectoryAndMask(out DirectoryInfo di, out Regex mask)
        {
            do
            {
                Console.WriteLine("Input folder path you would like to search in: ");
                string baseFolderPath = Console.ReadLine();
                if (baseFolderPath[baseFolderPath.Length - 1] != '\\')
                {
                    baseFolderPath += '\\';
                }
                di = new DirectoryInfo(baseFolderPath);
                if (!di.Exists)
                {
                    Console.WriteLine("Invalid path!");
                }
            } while (!di.Exists);      

            Console.WriteLine("Input what to search for." +
                "\n('*' for any sequence of characters, '?' for single any character)");
            mask = DosMaskToRegEx(Console.ReadLine());
        }

        static void SearchFolers()
        {
            try
            {
                GetDirectoryAndMask(out DirectoryInfo di, out Regex mask);
                List<DirectoryInfo> foundDirectories = FindFoldersInFolderRecursivelyByMask(di, mask);
                if (foundDirectories.Count == 0)
                {
                    Console.WriteLine("Nothing found!");
                    return;
                }
                for (int i = 0; i < foundDirectories.Count; i++)
                {
                    Console.WriteLine(i+1 + ". " + foundDirectories[i].FullName);
                }
                Console.Write("\nChoose what items to delete: ");
                DeleteFolderWithContents(foundDirectories[GetInt() - 1]);
                Console.WriteLine("Delete successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void SearchFiles()
        {
            try
            {
                GetDirectoryAndMask(out DirectoryInfo di, out Regex mask);
                List<FileInfo> foundFiles = FindFilesInFolderRecursivelyByMask(di, mask);
                if (foundFiles.Count == 0)
                {
                    Console.WriteLine("Nothing found!");
                    return;
                }
                for (int i = 0; i < foundFiles.Count; i++)
                {
                    Console.WriteLine(i+1 + ". " + foundFiles[i].FullName);
                }
                Console.Write("\nChoose what items to delete: ");
                foundFiles[GetInt()-1].Delete();
                Console.WriteLine("Delete successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DeleteFolderWithContents(DirectoryInfo folder)
        {
            foreach (FileInfo fi in folder.GetFiles())
            {
                fi.Delete();
            }

            foreach(DirectoryInfo di in folder.GetDirectories())
            {
                DeleteFolderWithContents(di);
            }

            folder.Delete();
        }

        static Regex DosMaskToRegEx(string Mask)
        {
            Mask = Mask.Replace(".", @"\.");
            Mask = Mask.Replace("?", ".");
            Mask = Mask.Replace("*", ".*");
            Mask = "^" + Mask + "$";
            return new Regex(Mask, RegexOptions.IgnoreCase);
        }

        static List<DirectoryInfo> FindFoldersInFolderRecursivelyByMask(DirectoryInfo folder, Regex regMask)
        {
            List<DirectoryInfo> allFoundFolders = new List<DirectoryInfo>();
            DirectoryInfo[] SubDi;
            try
            {
                SubDi = folder.GetDirectories();
            }
            catch
            {
                return allFoundFolders;
            }

            foreach (DirectoryInfo di in SubDi)
            {
                if (regMask.IsMatch(di.Name))
                {
                    allFoundFolders.Add(di);
                }
            }
            
            foreach (DirectoryInfo di in SubDi)
            {
                allFoundFolders.AddRange(
                    FindFoldersInFolderRecursivelyByMask(di, regMask));
            }

            return allFoundFolders;
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

        private static int GetInt()
        {
            string str = Console.ReadLine();
            int res;
            try
            {
                res = int.Parse(str);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid value!");
                return GetInt();
            }
        }
    }
}
