using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database;
using YukoBot.Services.Database.Models;

namespace YukoBot.Services
{
    public class GuildConfigService
    {
        private readonly IUnitOfWork _db;
        private readonly IConfiguration _config;

        public GuildConfigService(DbService dbService, IConfiguration config, DiscordSocketClient client)
        {
            _db = dbService.GetDbContext();
            _config = config;

            client.UserJoined += UserJoined;
        }

        private async Task UserJoined(SocketGuildUser member)
        {
            // TODO: maybe should somehow warn?
            IRole role = await GetAutoRole(member.Guild);
            if (role == null) return;

            await member.AddRoleAsync(role);
        }

        public Task<GuildConfig> GetGuildConfig(string guildId) =>
            _db.GuildConfigs.GetConfigAsync(guildId);

        public Task<int> SaveChanges() => 
            _db.SaveChangesAsync();

        public Task<string> GetPrefix(IGuild guild) =>
            GetPrefix(guild.Id.ToString());

        public Task SetPrefix(IGuild guild, string prefix) => 
            SetPrefix(guild.Id.ToString(), prefix);

        public Task SetAutoRole(IGuild guild, IRole role) => 
            SetAutoRole(guild.Id.ToString(), role.Id.ToString());

        public Task<ulong?> GetAutoRoleId(IGuild guild) =>
            GetAutoRoleId(guild.Id.ToString());

        public Task<string> GetPrefix(string guildId) =>
            _db.GuildConfigs.GetPrefixAsync(guildId, _config["prefix"]);

        public Task SetPrefix(string guildId, string prefix) =>
            _db.GuildConfigs.SetPrefixAsync(guildId, prefix);

        public Task SetAutoRole(string guildId, string roleId) =>
            _db.GuildConfigs.SetAutoRoleAsync(guildId, roleId);

        public Task<ulong?> GetAutoRoleId(string guildId) =>
            _db.GuildConfigs.GetAutoRoleIdAsync(guildId);

        public Task<IRole> GetAutoRole(IGuild guild) =>
            _db.GuildConfigs.GetAutoRoleAsync(guild);
    }
}
