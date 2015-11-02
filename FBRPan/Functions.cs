using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBRPan
{
    class Functions
    {
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copiando {0}", fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public static bool emuExists(string nome)
        {
            if (new DirectoryInfo("Emuladores/" + nome).Exists)
                return true;
            else
                return false;
        }
        public static void DelAll(DirectoryInfo source)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Deletando {0}", fi.Name);
                fi.Delete();
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DelAll(diSourceSubDir);
            }
            source.Delete();
        }
        public static void Copy(string sourceDirectory, string targetDirectory, string name)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
            File.Move(@targetDirectory + "/server.exe", @targetDirectory + "/" + name + ".exe");
        }
    }
}
