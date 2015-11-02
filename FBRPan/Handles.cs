using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBRPan
{
    class Handles
    {
        static void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            ShowOutput(e.Data, ConsoleColor.Green);
        }

        static void CaptureError(object sender, DataReceivedEventArgs e)
        {
            ShowOutput(e.Data, ConsoleColor.Red);
        }

        static void ShowOutput(string data, ConsoleColor color)
        {
            if (data != null)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine("Received: {0}", data);
                Console.ForegroundColor = oldColor;
            }
        }
        public static void handleStart(Dictionary<string, string> post)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Emulador sendo iniciado: {0}", Emulator.name);

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.CreateNoWindow = true;
                psi.UseShellExecute = true;
                psi.WorkingDirectory = Environment.CurrentDirectory + "/Emuladores/" + Emulator.name;
                psi.FileName = Environment.CurrentDirectory + "/Emuladores/" + Emulator.name + "/" + Emulator.name + ".exe";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = true;
                Process.Start(psi);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Emulador {0} iniciado", Emulator.name);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public static void handleCreate(Dictionary<string, string> post)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Criando {0}", Emulator.name);
            Console.ForegroundColor = ConsoleColor.Black;
            Functions.Copy("EmuDefault/" + post["emu_type"], "Emuladores/" + Emulator.name, Emulator.name);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            emuCreateConfig(post["emu_type"], Emulator.name, post);
            Console.WriteLine("Emulador {0} criado por {1}", Emulator.name, post["user"]);
        }
        public static string urlDecode(string url)
        {
            url = url.Replace("%3A", ":");
            url = url.Replace("%2F", "/");
            return url;
        }
        public static void emuCreateConfig(string type, string name, Dictionary<string, string> post)
        {
            using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/fbrpan.ini"))
            {
                foreach (KeyValuePair<string, string> p in post)
                {
                    string value = urlDecode(p.Value);
                    sw.WriteLine(p.Key + "=" + value);
                }
            }
            switch (type)
            {
                case "gte":
                    string path = "Emuladores/" + name + "/config.conf";
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.Write("## Gold Tree Emulator 3.0 System Configuration File\n\r");
                        sw.Write("\n\r");
                        sw.Write("## MySQL Configuration\n\r");
                        sw.Write("db.hostname={0}\n\r", post["db_host"]);
                        sw.Write("db.port={0}\n\r", post["db_port"]);
                        sw.Write("db.username={0}\n\r", post["db_user"]);
                        sw.Write("db.password={0}\n\r", post["db_pass"]);
                        sw.Write("db.name={0}\n\r", post["db_name"]);
                        sw.Write("\n\r");
                        sw.Write("## MySQL pooling setup (controls amount of connections)\n\r");
                        sw.Write("db.pool.minsize=5\n\r");
                        sw.Write("db.pool.maxsize=55\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Game TCP/IP Configuration\n\r");
                        sw.Write("game.tcp.bindip={0}\n\r", Emulator.host);
                        sw.Write("game.tcp.port={0}\n\r", post["tcp"]);
                        sw.Write("game.tcp.conlimit=5000\n\r");
                        sw.Write("game.tcp.proxyip={0}\n\r", Emulator.host);
                        sw.Write("\n\r");
                        sw.Write("## Client configuration\n\r");
                        sw.Write("client.ping.enabled=1\n\r");
                        sw.Write("client.ping.interval=30000\n\r");
                        sw.Write("\n\r");
                        sw.Write("## MUS TCP/IP Configuration\n\r");
                        sw.Write("mus.tcp.bindip={0}\n\r", Emulator.host);
                        sw.Write("mus.tcp.port={0}\n\r", post["mus"]);
                        sw.Write("mus.tcp.allowedaddr={0}\n\r", Emulator.host);
                        sw.Write("\n\r");
                        sw.Write("## Console Configuration\n\r");
                        sw.Write("emu.messages.connections=1\n\r");
                        sw.Write("emu.messages.roommgr=1\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Automatic shutdown (DONT CHANGE VALUES IF YOU DONT WANT ENABLE THIS)\n\r");
                        sw.Write("shutdown-server=\n\r");
                        sw.Write("shutdown-server-player-limit=0\n\r");
                        sw.Write("shutdown-warning-alert=!!!Hotel shutdown!!!\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Automatic update\n\r");
                        sw.Write("gte.update.noticy.disable=0\n\r");
                        sw.Write("gte.update.autoupdate=0\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Anti Advertising Configuration ###\n\r");
                        sw.Write("anti.ads.enable=1\n\r");
                        sw.Write("\n\r");
                        sw.Write("## The following configuration defines the last rank that will have anti advertisement enabled\n\r");
                        sw.Write("## ranks above this integer will be able to post links freely\n\r");
                        sw.Write("anti.ads.rank=1\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Message that is displayed after ad preventation\n\r");
                        sw.Write("anti.ads.msg=Advertising will lead you to be banned\n\r");
                        sw.Write("\n\r");
                        sw.Write("## Some ad (alert) shit to client\n\r");
                        sw.Write("ads.disable=0\n\r");
                        sw.Write("ads.allowrdonlyrandomads=1\n\r");
                        sw.Write("\n\r");
                        sw.Write("debug=0\n\r");
                    }
                    break;
                case "mercury":
                    using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/config.ini"))
                    {
                        sw.WriteLine("## Mercury Emulator System Configuration File");
                        sw.WriteLine("## Must be edited for the server to work");
                        sw.WriteLine("");
                        sw.WriteLine("## MySQL Configuration");
                        sw.WriteLine("db.hostname={0}", post["db_host"]);
                        sw.WriteLine("db.port={0}", post["db_port"]);
                        sw.WriteLine("db.username={0}", post["db_user"]);
                        sw.WriteLine("db.password={0}", post["db_pass"]);
                        sw.WriteLine("db.name={0}", post["db_name"]);
                        sw.WriteLine("");
                        sw.WriteLine("## MySQL pooling setup (controls amount of connections)");
                        sw.WriteLine("db.pool.minsize=1");
                        sw.WriteLine("db.pool.maxsize=500");
                        sw.WriteLine("");
                        sw.WriteLine("## Game TCP/IP Configuration");
                        sw.WriteLine("game.tcp.bindip={0}", Emulator.host);
                        sw.WriteLine("game.tcp.port={0}", post["tcp"]);
                        sw.WriteLine("game.tcp.conlimit=11000");
                        sw.WriteLine("game.tcp.conperip=100");
                        sw.WriteLine("game.tcp.enablenagles=true");
                        sw.WriteLine("");
                        sw.WriteLine("## MUS TCP/IP Configuration");
                        sw.WriteLine("mus.tcp.bindip={0}", Emulator.host);
                        sw.WriteLine("mus.tcp.port={0}", post["mus"]);
                        sw.WriteLine("mus.tcp.allowedaddr={0}", Emulator.host);
                        sw.WriteLine("");
                        sw.WriteLine("## Client configuration");
                        sw.WriteLine("client.ping.enabled=1");
                        sw.WriteLine("client.ping.interval=20000");
                        sw.WriteLine("client.maxrequests=300");
                    }
                    using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/extra_settings.txt"))
                    {
                        sw.WriteLine("# Mercury ExtraSettings #");
                        sw.WriteLine("");
                        sw.WriteLine("# Credits/Pixels Loop");
                        sw.WriteLine("currency.loop.enabled=true");
                        sw.WriteLine("currency.loop.time.in.minutes=15");
                        sw.WriteLine("credits.to.give=3000");
                        sw.WriteLine("pixels.to.give=100");
                        sw.WriteLine("");
                        sw.WriteLine("# Diamonds Loop");
                        sw.WriteLine("diamonds.loop.enabled=true");
                        sw.WriteLine("diamonds.to.give=0");
                        sw.WriteLine("diamonds.vip.only=true");
                        sw.WriteLine("");
                        sw.WriteLine("# Navigator New 2014 Style");
                        sw.WriteLine("navigator.newstyle.enabled=false");
                        sw.WriteLine("");
                        sw.WriteLine("# Change Name Settings #");
                        sw.WriteLine("change.name.staff=false");
                        sw.WriteLine("change.name.vip=false");
                        sw.WriteLine("change.name.everyone=false");
                        sw.WriteLine("");
                        sw.WriteLine("# Beta stuff and NUX gifts #");
                        sw.WriteLine("enable.beta.camera=true");
                        sw.WriteLine("newuser.gifts.enabled=false");
                        sw.WriteLine("newuser.gift.yttv2.id=true");
                        sw.WriteLine("");
                        sw.WriteLine("# Interactive stuff #");
                        sw.WriteLine("everyone.use.floor=true");
                        sw.WriteLine("figuredata.url={0}", urlDecode(post["figuredata"]));
                        sw.WriteLine("youtube.thumbnail.suburl=youtubethumbnail.php?Video");
                    }
                    break;
                case "azure":
                    using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/Settings/main.ini"))
                    {
                        sw.WriteLine("## Azure Emulator System Configuration File");
                        sw.WriteLine("## Must be edited for the server to work");
                        sw.WriteLine("");
                        sw.WriteLine("## MySQL Configuration");
                        sw.WriteLine("db.hostname={0}", post["db_host"]);
                        sw.WriteLine("db.port={0}", post["db_port"]);
                        sw.WriteLine("db.username={0}", post["db_user"]);
                        sw.WriteLine("db.password={0}", post["db_pass"]);
                        sw.WriteLine("db.name={0}", post["db_name"]);
                        sw.WriteLine("db.type=MySQL");
                        sw.WriteLine("");
                        sw.WriteLine("## MySQL pooling setup (controls amount of connections)");
                        sw.WriteLine("db.pool.minsize=1");
                        sw.WriteLine("db.pool.maxsize=500");
                        sw.WriteLine("");
                        sw.WriteLine("## Game TCP/IP Configuration");
                        sw.WriteLine("game.tcp.bindip={0}", Emulator.host);
                        sw.WriteLine("game.tcp.port={0}", post["tcp"]);
                        sw.WriteLine("game.tcp.conlimit=11000");
                        sw.WriteLine("game.tcp.enablenagles=true");
                        sw.WriteLine("");
                        sw.WriteLine("##Tcp Flood AntiDDoS  Comment: For proxy antiddos=false ");
                        sw.WriteLine("game.tcp.antiddos=true");
                        sw.WriteLine("game.tcp.conperip=5");
                        sw.WriteLine("");
                        sw.WriteLine("## MUS TCP/IP Configuration");
                        sw.WriteLine("mus.tcp.bindip={0}", Emulator.host);
                        sw.WriteLine("mus.tcp.port={0}", post["mus"]);
                        sw.WriteLine("mus.tcp.allowedaddr={0}", Emulator.host);
                        sw.WriteLine("");
                        sw.WriteLine("## Client configuration");
                        sw.WriteLine("client.ping.enabled=1");
                        sw.WriteLine("client.ping.interval=20000");
                        sw.WriteLine("client.maxrequests=300");
                        sw.WriteLine("");
                        sw.WriteLine("## Developer Settings");
                        sw.WriteLine("Debug=false");
                        sw.WriteLine("");
                        sw.WriteLine("## Language System");
                        sw.WriteLine("system.lang=english");
                        sw.WriteLine("");
                        sw.WriteLine("## Console Clear Timer");
                        sw.WriteLine("console.clear.enabled=true");
                        sw.WriteLine("console.clear.time=20000");
                        sw.WriteLine("");
                        sw.WriteLine("## Bots Settings");
                        sw.WriteLine("game.roomswithbotscolor=0");
                        sw.WriteLine("game.botdefaultcolor=31");
                        sw.WriteLine("game.botbadge=BOT");

                    }
                    using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/Settings/other.ini"))
                    {
                        sw.WriteLine("# Azure ExtraSettings #");
                        sw.WriteLine("");
                        sw.WriteLine("# CRYPTO. Only for powerful servers.");
                        sw.WriteLine("rc4.client.side.enabled=false");
                        sw.WriteLine("");
                        sw.WriteLine("# Credits/Pixels Loop");
                        sw.WriteLine("currency.loop.enabled=true");
                        sw.WriteLine("currency.loop.time.in.minutes={0}", post["interval"]);
                        sw.WriteLine("credits.to.give={0}", post["start_credits"]);
                        sw.WriteLine("pixels.to.give={0}", post["pixels_give"]);
                        sw.WriteLine("");
                        sw.WriteLine("# Diamonds Loop");
                        sw.WriteLine("diamonds.loop.enabled=true");
                        sw.WriteLine("diamonds.to.give={0}", post["dia_give"]);
                        sw.WriteLine("diamonds.vip.only=true");
                        sw.WriteLine("");
                        sw.WriteLine("# Navigator New 2014 Style");
                        sw.WriteLine("navigator.newstyle.enabled=true");
                        sw.WriteLine("");
                        sw.WriteLine("# Change Name Settings #");
                        sw.WriteLine("change.name.staff=false");
                        sw.WriteLine("change.name.vip=false");
                        sw.WriteLine("change.name.everyone=false");
                        sw.WriteLine("");
                        sw.WriteLine("# Beta stuff and NUX gifts #");
                        sw.WriteLine("enable.beta.camera=true");
                        sw.WriteLine("newuser.gifts.enabled=false");
                        sw.WriteLine("newuser.gift.yttv2.id=");
                        sw.WriteLine("");
                        sw.WriteLine("# API Camera & Thumbnails #");
                        sw.WriteLine("api.hash=0");
                        sw.WriteLine("api.hotel.domain=swf.hosthp.in");
                        sw.WriteLine("");
                        sw.WriteLine("# Interactive stuff #");
                        sw.WriteLine("everyone.use.floor=true");
                        sw.WriteLine("admin.can.useHTML=true");
                        sw.WriteLine("commands.new.page=false");
                        sw.WriteLine("figuredata.url={0}", urlDecode(post["figuredata"]));
                        sw.WriteLine("furnidata.url={0}", urlDecode(post["furnidata"]));
                        sw.WriteLine("youtube.thumbnail.suburl=youtubethumbnail.php?Video");

                    }
                    using (StreamWriter sw = File.CreateText("Emuladores/" + name + "/Settings/Welcome/message.txt"))
                    {
                        sw.WriteLine("{0}", post["message"]);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}