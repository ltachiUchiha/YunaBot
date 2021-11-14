﻿using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Yuna.DataStructs;
using Yuna.Services;

namespace Yuna.Handlers
{
    public class ConfigData
    {
        public static string ConfigPath { get; set; } = "config.json";
        public static Config Config { get; set; }

        //Initialize the Config and Global Properties.
        public async Task InitializeAsync()
        {
            string json;

            //Checking whether config.json exists.
            if (!File.Exists(ConfigPath))
            {
                json = JsonConvert.SerializeObject(CreateNewConfig(), Formatting.Indented);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                await LogService.LogCritAsync("Bot", "No config file was found. A new one was created, please fill config file and restart the bot.");
                await Task.Delay(-1);
            }

            json = File.ReadAllText(ConfigPath, new UTF8Encoding(false));
            Config = JsonConvert.DeserializeObject<Config>(json);
        }

        //If there is no config in the folder, an empty one is created.
        private static Config CreateNewConfig() => new Config
        {
            DiscordToken = "Null",
            DefaultPrefix = "!",
            Status = "Change me",
            Activity = ActivityType.Listening,
            BlacklistedChannels = new List<ulong>()
        };
    }
}
