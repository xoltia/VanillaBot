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
        private readonly YukoContext _db;
        private readonly IConfiguration _config;
        private readonly IQueryable<GuildConfig> _guildConfigs;

        public GuildConfigService(YukoContext db, IConfiguration config, DiscordSocketClient client)
        {
            _db = db;
            _config = config;
            _guildConfigs =  _db.GuildConfigs;

            client.UserJoined += UserJoined;
        }

        private async Task UserJoined(SocketGuildUser member)
        {
            ulong? roleId = await GetAutoRoleId(member.Guild);
            if (!roleId.HasValue) return;

            // TODO: maybe should somehow warn?
            IRole role = member.Guild.Roles.SingleOrDefault(r => r.Id == roleId.Value);
            if (role == null) return;

            await member.AddRoleAsync(role);
        }

        public Task<GuildConfig> GetGuildConfig(string guildId) =>
            _guildConfigs.SingleOrDefaultAsync(c => c.GuildId == guildId);

        public Task<int> SaveChanges() => 
            _db.SaveChangesAsync();

        public Task<string> GetPrefix(IGuild guild) =>
            GetPrefix(guild.Id.ToString());

        public Task<string> GetPrefix(ulong guildId) =>
            GetPrefix(guildId.ToString());

        public Task SetPrefix(IGuild guild, string prefix) => 
            SetPrefix(guild.Id.ToString(), prefix);

        public Task SetPrefix(ulong guildId, string prefix) =>
            SetPrefix(guildId.ToString(), prefix);

        public Task SetAutoRole(IGuild guild, IRole role) => 
            SetAutoRole(guild.Id.ToString(), role.Id.ToString());

        public Task<ulong?> GetAutoRoleId(IGuild guild) =>
            GetAutoRoleId(guild.Id.ToString());

        public async Task<string> GetPrefix(string guildId)
        {
            GuildConfig config = await _guildConfigs.SingleOrDefaultAsync(c => c.GuildId == guildId);
            if (config == null)
            {
                return _config["prefix"];
            }
            return config.Prefix ?? _config["prefix"];
        }

        public async Task SetPrefix(string guildId, string prefix)
        {
            GuildConfig config = await _guildConfigs.SingleOrDefaultAsync(c => c.GuildId == guildId);
            if (config == null)
            {
                config = new GuildConfig()
                {
                    GuildId = guildId,
                    Prefix = prefix,
                };
                await _db.GuildConfigs.AddAsync(config);
            }
            else
            {
                config.Prefix = prefix;
                _db.GuildConfigs.Update(config);
            }
            await _db.SaveChangesAsync();
        }

        public async Task SetAutoRole(string guildId, string roleId)
        {
            GuildConfig config = await _guildConfigs.SingleOrDefaultAsync(c => c.GuildId == guildId);
            if (config == null)
            {
                config = new GuildConfig()
                {
                    GuildId = guildId,
                    AutoRoleId = roleId,
                };
                await _db.GuildConfigs.AddAsync(config);
            }
            else
            {
                config.AutoRoleId = roleId;
                _db.GuildConfigs.Update(config);
            }
            await _db.SaveChangesAsync();
        }

        public async Task<ulong?> GetAutoRoleId(string guildId)
        {
            GuildConfig config = await _guildConfigs.SingleOrDefaultAsync(c => c.GuildId == guildId);
            if (config == null || config.AutoRoleId == null) return null;
            return ulong.Parse(config.AutoRoleId);
        }

        public async Task<IRole> GetAutoRole(IGuild guild)
        {
            ulong? roleId = await GetAutoRoleId(guild);
            if (!roleId.HasValue) return null;
            return guild.Roles.SingleOrDefault(r => r.Id == roleId.Value);
        }
    }
}
