using Discord;
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

        public GuildConfigService(YukoContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _guildConfigs =  _db.GuildConfigs;
        }

        public Task<string> GetPrefix(IGuild guild) =>
            GetPrefix(guild.Id.ToString());

        public Task<string> GetPrefix(ulong guildId) =>
            GetPrefix(guildId.ToString());

        public Task SetPrefix(IGuild guild, string prefix) =>
            SetPrefix(guild.Id.ToString(), prefix);

        public Task SetPrefix(ulong guildId, string prefix) =>
            SetPrefix(guildId.ToString(), prefix);

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
    }
}
