using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBRPan
{
    class ConsoleCommands
    {
        public static void Handle(string command)
        {
            switch (command.ToLower())
            {
                case "create":
                    {
                        Dictionary<string, string> post = new Dictionary<string, string>();
                        string v = "";
                        foreach (string e in Emulator.versions)
                            v += e + "/";
                        Console.WriteLine("   > Emu Type - {0}", v);
                        string emuV = Console.ReadLine();
                        int ct = 0;
                        foreach (string e in Emulator.versions)
                            if (emuV == e) ct++;
                        if (ct == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Versão {0} não existe", emuV);
                            break;
                        }
                        post.Add("emu_type", emuV);
                        Console.WriteLine("   > Emu Name");
                        Emulator.name = Console.ReadLine();
                        Console.WriteLine("   > Db Host");
                        post.Add("db_host", Console.ReadLine());
                        Console.WriteLine("   > Db Port");
                        post.Add("db_port", Console.ReadLine());
                        Console.WriteLine("   > Db User");
                        post.Add("db_user", Console.ReadLine());
                        Console.WriteLine("   > Db Pass");
                        post.Add("db_pass", Console.ReadLine());
                        Console.WriteLine("   > Db Name");
                        post.Add("db_name", Console.ReadLine());
                        Console.WriteLine("   > Host");
                        post.Add("host", Console.ReadLine());
                        Console.WriteLine("   > Tcp");
                        post.Add("tcp", Console.ReadLine());
                        Console.WriteLine("   > Mus");
                        post.Add("mus", Console.ReadLine());
                        Console.WriteLine("   > Figuredata");
                        post.Add("figuredata", Console.ReadLine());
                        Console.WriteLine("   > Start Credits");
                        post.Add("start_credits", Console.ReadLine());
                        Console.WriteLine("   > Pixels Give");
                        post.Add("pixels_give", Console.ReadLine());
                        Console.WriteLine("   > Dia Give");
                        post.Add("dia_give", Console.ReadLine());
                        Console.WriteLine("   > Interval");
                        post.Add("interval", Console.ReadLine());
                        Console.WriteLine("   > Message");
                        post.Add("message", Console.ReadLine());
                        Console.WriteLine("   > Furnidata");
                        post.Add("furnidata", Console.ReadLine());
                        post.Add("user", "Server");
                        Handles.handleCreate(post);
                        break;
                    }
                case "changev":
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("   > Emu Name");
                        string emun = Console.ReadLine();
                        if (emun == "")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Digite um nome");
                            break;
                        }
                        DirectoryInfo di = new DirectoryInfo("Emuladores/" + emun);
                        if (!Directory.Exists(di.FullName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Emulador {0} não existe", emun);
                            break;
                        }
                        foreach (var process in Process.GetProcessesByName(emun))
                            process.Kill();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Emulador {0} desligado", Emulator.name);
                        Dictionary<string, string> post = new Dictionary<string, string>();
                        string v = "";
                        foreach (string e in Emulator.versions)
                            v += e + "/";
                        Console.WriteLine("   > Emu Type - {0}", v);
                        string emuV = Console.ReadLine();
                        int ct = 0;
                        foreach (string e in Emulator.versions)
                            if (emuV == e) ct++;
                        if (ct == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Versão {0} não existe", emuV);
                            break;
                        }
                        Emulator.name = emun;
                        post.Add("emu_type", emuV);
                        Console.WriteLine("   > Db Host");
                        post.Add("db_host", Console.ReadLine());
                        Console.WriteLine("   > Db Port");
                        post.Add("db_port", Console.ReadLine());
                        Console.WriteLine("   > Db User");
                        post.Add("db_user", Console.ReadLine());
                        Console.WriteLine("   > Db Pass");
                        post.Add("db_pass", Console.ReadLine());
                        Console.WriteLine("   > Db Name");
                        post.Add("db_name", Console.ReadLine());
                        Console.WriteLine("   > Host");
                        post.Add("host", Console.ReadLine());
                        Console.WriteLine("   > Tcp");
                        post.Add("tcp", Console.ReadLine());
                        Console.WriteLine("   > Mus");
                        post.Add("mus", Console.ReadLine());

                        Console.WriteLine("   > Figuredata");
                        post.Add("figuredata", Console.ReadLine());
                        Console.WriteLine("   > Start Credits");
                        post.Add("start_credits", Console.ReadLine());
                        Console.WriteLine("   > Pixels Give");
                        post.Add("pixels_give", Console.ReadLine());
                        Console.WriteLine("   > Dia Give");
                        post.Add("dia_give", Console.ReadLine());
                        Console.WriteLine("   > Interval");
                        post.Add("interval", Console.ReadLine());
                        Console.WriteLine("   > Message");
                        post.Add("message", Console.ReadLine());
                        Console.WriteLine("   > Furnidata");
                        post.Add("furnidata", Console.ReadLine());
                        post.Add("user", "Server");
                        try
                        {
                            Functions.DelAll(di);
                        }
                        catch (Exception e)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("Error: {0}", e.Message);
                            break;
                        }
                        finally
                        {
                            Handles.handleCreate(post);
                        }
                        break;
                    }
                case "delete":
                    {
                        Console.WriteLine("   > Emu Name");
                        string emun = Console.ReadLine();

                        if (emun == "")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Digite um nome");
                            break;
                        }
                        DirectoryInfo di = new DirectoryInfo("Emuladores/" + emun);
                        if (!Directory.Exists(di.FullName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("    Emulador {0} não existe", emun);
                            break;
                        }
                        foreach (var process in Process.GetProcessesByName(emun))
                            process.Kill();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Emulador {0} desligado", Emulator.name);
                        Functions.DelAll(di);
                        break;
                    }
                default:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("       CREATE                Crie um novo emulador");
                    Console.WriteLine("       CHANGEV               Troque a versão de um emulador");
                    Console.WriteLine("       DELETE                Apague um emulador");
                    break;
            }
        }
    }
}
