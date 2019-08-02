using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public class GuildConfigRepository : Repository<GuildConfig>, IGuildConfigRepository
    {
        public GuildConfigRepository(YukoContext context) : base(context)
        {
        }

        public Task<GuildConfig> GetConfigAsync(string guildId) =>
            _dbSet.SingleOrDefaultAsync(c => c.GuildId == guildId);

        public Task<GuildConfig> GetConfigAsync(IGuild guild) => 
            GetConfigAsync(guild.Id.ToString());

        public async Task<string> GetPrefixAsync(string guildId, string defaultPrefix = null) =>
            (await GetConfigAsync(guildId))?.Prefix ?? defaultPrefix;

        public Task<string> GetPrefixAsync(IGuild guild, string defaultPrefix = null) =>
            GetPrefixAsync(guild.Id.ToString(), defaultPrefix);

        public async Task<ulong?> GetAutoRoleIdAsync(string guildId)
        {
            string id = (await GetConfigAsync(guildId))?.AutoRoleId;
            if (id == null) return null;
            return ulong.Parse(id);
        }

        public Task<ulong?> GetAutoRoleIdAsync(IGuild guild) =>
            GetAutoRoleIdAsync(guild.Id.ToString());

        public async Task<IRole> GetAutoRoleAsync(IGuild guild)
        {
            ulong? id = await GetAutoRoleIdAsync(guild.Id.ToString());
            if (id == null) return null;
            return guild.Roles.SingleOrDefault(r => r.Id == id);
        }

        public async Task SetPrefixAsync(string guildId, string prefix)
        {
            GuildConfig config = await GetConfigAsync(guildId);
            if (config == null)
            {
                config = new GuildConfig()
                {
                    GuildId = guildId,
                    Prefix = prefix
                };
                await AddAsync(config);
            }
            else
            {
                config.Prefix = prefix;
                Update(config);
            }
        }

        public Task SetPrefixAsync(IGuild guild, string prefix) =>
            SetPrefixAsync(guild.Id.ToString(), prefix);

        public async Task SetAutoRoleAsync(string guildId, string autoRoleId)
        {
            GuildConfig config = await GetConfigAsync(guildId);
            if (config == null)
            {
                config = new GuildConfig()
                {
                    GuildId = guildId,
                    AutoRoleId = autoRoleId
                };
                await AddAsync(config);
            }
            else
            {
                config.AutoRoleId = autoRoleId;
                Update(config);
            }
        }

        public Task SetAutoRoleAsync(IGuild guild, IRole role) =>
            SetAutoRoleAsync(guild.Id.ToString(), role.Id.ToString());
    }
}
