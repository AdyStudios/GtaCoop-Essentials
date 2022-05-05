using System;
using GTAServer.PluginAPI;
using GTAServer;
using GTAServer.PluginAPI.Attributes;
using GTAServer.PluginAPI.Entities;
using GTAServer.PluginAPI.Events;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Essentials
{
    public class Essentials : IPlugin
    {
        public string Name => "Essentials";

        public string Description => "An example plugin for demonstrating the plugin API";

        public string Author => "Ady";

        public static Settings settings;

        public bool OnEnable(GameServer gameServer, bool isAfterServerLoad) 
        {
            AddAllCommands(gameServer);
            loadSettings();
            return true; 
        }

        public static void AddAllCommands(GameServer gameServer)
        {
            gameServer.RegisterCommand("ping", ping);
            gameServer.RegisterCommand("ptt", teleportOtherPlayer);
            gameServer.RegisterCommand("dc", telldiscord);
        }

        static void loadSettings()
        {
            settings = JsonFileReader.Read<(>(@"C:\Users\Kara Ádám\Documents\GTASERVER\gtaserver.core\publish\windows-Release\Plugins\Essentials\settings.json");
        }

        [Command("ping")]
        public static void ping(CommandContext ctx, List<string> args)
        {
            ctx.SendMessage("Pong!");
        }
        [Command("ptt")]
        public static void teleportOtherPlayer(CommandContext ctx, List<string> args)
        {
            bool forceUse = false;
            if (args.Count < 1 || args.Count > 2)
            {
                ctx.SendMessage("Incorrect arguments!");
                return;
            }
            if (args[0] == "true")
            {
                forceUse = true;
                try
                {
                    args[0] = args[1];
                }
                catch(Exception ex)
                {
                    ctx.SendMessage(ex.ToString());
                    ctx.SendMessage("[DEBUG] Player not specified");
                    return;
                }
            }
            if(ctx.Sender is ConsoleCommandSender && !forceUse)
            {
                Console.WriteLine("Cannot be used from console!");
                return;
            }
            else
            {
                try
                {
                    GTAServer.ProtocolMessages.Client ptt_client = ctx.GameServer.Clients.Find(x => x.Name == args[1]);
                    string ptt_name = ptt_client.Name;

                    ctx.SendMessage($"Teleporting {ptt_name}...");

                    ptt_client.Position = new GTAServer.ProtocolMessages.Vector3(ctx.Client.Position.X, ctx.Client.Position.Y + 5, ctx.Client.Position.Z);
                }
                catch (Exception e)
                {
                    ctx.SendMessage(e.ToString());
                    ctx.SendMessage("Could not find player!");
                    return;
                }
            }
        }

        [Command("discord")]
        public static void telldiscord(CommandContext ctx, List<string> args)
        {
            ctx.SendMessage(settings.discord);
        }

    }

    public static class JsonFileReader
    {
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }

    public class Settings
    {
        public string discord;
    }
}
