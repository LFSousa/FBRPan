using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace FBRPan
{
    public class Emulator
    {
        public static string name;
        public static string[] versions = { "gte", "azure" };                               //Emulators
        public static string[] block_edit = { "tcp", "mus", "user", "pass", "emu_type" };
        public static string host = "127.0.0.1";  //VPS IP
        public static string website = "http://lalala.com"; //Website URL
    }
    
    public class MyHttpServer : HttpServer
    {
        public MyHttpServer(int port)
            : base(port)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            p.writeSuccess();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Tentativa de acesso via GET");
            p.outputStream.WriteLine("User not autenticated");
        }


        public static void HandleCommand(string command)
        {
            Console.BackgroundColor = ConsoleColor.White;
            ConsoleCommands.Handle(command);
        }


        public static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {

            //    Create
            //New_User
            //User
            //Pass
            //SSO
            //action
            //emu_type
            //emu_name
            //db_host
            //db_port
            //db_user
            //db_pass
            //db_name
            //host
            //tcp
            //mus
            //Figuredata
            //Furnidata

            //     Others
            //key

            Dictionary<String, String> post = new Dictionary<string, string>();
            

            if(HttpProcessor.debug)
                Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();
            if (data.Contains("&"))
            {
                foreach (string i in data.Split('&'))
                {
                    string[] sp = i.Split('=');
                    post[sp[0]] = DecodeUrlString(sp[1]);
                }
            }
            p.writeSuccess();
            if (!post.ContainsKey("user") || !post.ContainsKey("pass"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Tentativa de acesso direto ao POST sem autenticacao");
                p.outputStream.WriteLine("User not autenticated");
            }
            else
            {
                if (autenticate(post))
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Tentativa de acesso inválida: {0}", post["user"]);
                    p.outputStream.WriteLine("User not autenticated");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    try
                    {
                        using (var streamReader = new StreamReader(Environment.CurrentDirectory + "/Emuladores/" + Emulator.name + "/fbrpan.ini"))
                        {
                            string text;
                            while ((text = streamReader.ReadLine()) != null)
                            {
                                if (text.Length < 1 || text.StartsWith("#")) continue;
                                var num = text.IndexOf('=');
                                if (num == -1) continue;
                                var key = text.Substring(0, num);
                                var value = text.Substring((num + 1));

                                Data.Add(key, value);
                            }
                            streamReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(string.Format("Could not process configuration file: {0}", ex.Message));
                    }
                    switch (post["action"])
                    {
                        case "create":
                            if(!emuExists(Emulator.name) && post["user"] == "admin"){
                                p.outputStream.WriteLine("1");
                                post["user"] = post["new_user"];
                                Emulator.name = post["user"];
                                Handles.handleCreate(post);
                            }
                            else
                                p.outputStream.WriteLine("0");
                            break;
                        case "login":
                            p.outputStream.WriteLine("1");
                            break;
                        case "start":
                            if (Process.GetProcessesByName(Emulator.name).Length == 0)
                                Handles.handleStart(post);
                            p.outputStream.WriteLine(Process.GetProcessesByName(Emulator.name).Length);
                            break;
                        case "stop":
                            foreach (var process in Process.GetProcessesByName(Emulator.name))
                                process.Kill();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Emulador {0} desligado", Emulator.name);
                            p.outputStream.WriteLine(0);
                            break;
                        case "restart":
                            try
                            {
                                foreach (var process in Process.GetProcessesByName(Emulator.name))
                                    process.Kill();
                                Console.WriteLine("Emulador {0} desligado", Emulator.name);
                            }
                            finally
                            {
                                    Handles.handleStart(post);
                            }
                            p.outputStream.WriteLine(Process.GetProcessesByName(Emulator.name).Length);
                            break;
                        case "changev":
                            if (emuExists(Emulator.name))
                            {
                                try
                                {
                                    foreach (var process in Process.GetProcessesByName(Emulator.name))
                                        process.Kill();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Emulador {0} desligado", Emulator.name);
                                }
                                finally
                                {
                                    Functions.DelAll(new DirectoryInfo("Emuladores/" + Emulator.name));
                                    string t = post["emu_type"];
                                    foreach (string a in Emulator.block_edit)
                                        post[a] = Data[a];
                                    post["emu_type"] = t;
                                    Handles.handleCreate(post);
                                    p.outputStream.WriteLine("1");
                                }
                            }
                            else
                                p.outputStream.WriteLine("0");
                            break;
                        case "verify":
                            if (Process.GetProcessesByName(Emulator.name).Length == 0)
                                p.outputStream.WriteLine("0");
                            else
                                p.outputStream.WriteLine("1");
                            break;
                        case "title":
                            if (Process.GetProcessesByName(Emulator.name).Length != 0)
                            {
                                foreach (var process in Process.GetProcessesByName(Emulator.name))
                                {
                                    p.outputStream.WriteLine(process.MainWindowTitle);
                                }
                            }
                            break;
                        case "get_config":
                            try
                            {
                                p.outputStream.WriteLine(Data[post["key"]]);
                            }
                            catch { p.outputStream.WriteLine(""); }
                            break;
                        case "get_versions":
                            foreach (string v in Emulator.versions)
                            {
                                if (Data["emu_type"] == v)
                                    p.outputStream.WriteLine("<option value='{0}' selected='selected'>{0}</option>", v);
                                else
                                    p.outputStream.WriteLine("<option value='{0}'>{0}</option>", v);
                            }
                            break;
                        case "set_config":
                            try
                            {
                                using (StreamWriter sw = File.CreateText("Emuladores/" + Emulator.name + "/fbrpan.ini"))
                                {
                                    foreach (KeyValuePair<string, string> z in post)
                                    {
                                        string value = "";
                                        try
                                        {
                                            value = z.Value;
                                            value = value.Replace("%3A", ":");
                                            value = value.Replace("%2F", "/");
                                            foreach (string s in Emulator.block_edit)
                                            {
                                                if (z.Key == s)
                                                {
                                                    if (Data.ContainsKey(s))
                                                        value = Data[s];
                                                    else value = "";
                                                }
                                            }
                                        }
                                        catch { }

                                        sw.WriteLine(z.Key + "=" + value);
                                    }
                                    foreach (string s in Emulator.block_edit)
                                    {
                                        if (!post.ContainsKey(s))
                                        {
                                            if(Data.ContainsKey(s))
                                                sw.WriteLine(s + "=" + Data[s]);
                                        }                                            
                                    }
                                }
                                foreach (string a in Emulator.block_edit)
                                {
                                    try
                                    {
                                        post[a] = Data[a];
                                    }
                                    catch
                                    {
                                        post[a] = "";
                                    }
                                }
                                Handles.emuCreateConfig(Data["emu_type"], Emulator.name, post);
                                p.outputStream.WriteLine("1");
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                p.outputStream.WriteLine("0");
                            }
                            break;

                        case "emu_info":
                            p.outputStream.WriteLine("<b>"+Emulator.name.ToUpper() + "</b><br>TCP: <b>" + Data["tcp"] + "</b> MUS: <b>" + Data["mus"] + "</b>");
                            break;
                    }
                }
            }
        }

        private string getFromSite(string p, Dictionary<string, string> post)
        {
            try
            {
                WebRequest request = WebRequest.Create(@Emulator.website + "/login.php?act="+p+"&user=" + @post["user"] + "&pass=" + @post["pass"]);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return responseFromServer;
            }
            catch
            {
                return "Error";
            }
        }

        private bool emuExists(string nome)
        {
            if (new DirectoryInfo("Emuladores/" + nome).Exists)
                return true;
            else
                return false;
        }


        private bool autenticate(Dictionary<string, string> post)
        {
            try
            {
                WebRequest request = WebRequest.Create(@Emulator.website + "/login.php?user=" + @post["user"] + "&pass=" + @post["pass"]);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                if (responseFromServer == "0")
                    return true;
                else
                {
                    Emulator.name = responseFromServer;
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Erro ao conectar ao servidor de auth: {0}", e.Message);
            }
            return true;
        }
    }

    public class MyHost
    {
        public static int Main(String[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            int port = 4456;
            Console.Title = "FBRPan - Emulator Panel";
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("");
            Console.WriteLine(@"        .--------.  .--------.    .--------.                                   ");
            Console.WriteLine(@"        : .------´  : .-----. :   : .-----. :                                  ");
            Console.WriteLine(@"        : `------.  : :_____: ;   : :_____: ;   .-----. .-----. .-.  .-.       ");
            Console.WriteLine(@"        : .------´  : .-----.´    : .-.  .-´    : .-. : : .-. : :  \ : :       ");
            Console.WriteLine(@"        : :         : :_____:`:   : :  \  \     : `-´ ; : `-´ : : \ `´ :       ");
            Console.WriteLine(@"        :_:         :________ :   :_:    \ _\   :_:¨¨´  :_:¨:_: :_:  :_:        ");
            Console.BackgroundColor = ConsoleColor.Black;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"                             Developed by FilipeBR                              ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("> Starting...");
            HttpServer httpServer;
            if (args.GetLength(0) > 0)
            {
                httpServer = new MyHttpServer(Convert.ToInt16(args[0]));
            }
            else
            {
                httpServer = new MyHttpServer(port);
            }
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
            Console.WriteLine("> Started");
            Console.WriteLine("> Running on port {0}", port);
            Console.Title = Console.Title + " - Runnig on port " + port;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                MyHttpServer.HandleCommand(Console.ReadLine());
            }
        }   
    }
}