using Discord;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public interface IGuildConfigRepository : IRepository<GuildConfig>
    {
        Task<GuildConfig> GetConfigAsync(string guildId);
        Task<GuildConfig> GetConfigAsync(IGuild guild);


        Task<string> GetPrefixAsync(string guildId, string defaultPrefix = null);
        Task<string> GetPrefixAsync(IGuild guild, string defaultPrefix = null);

        Task<ulong?> GetAutoRoleIdAsync(string guildId);
        Task<ulong?> GetAutoRoleIdAsync(IGuild guild);
        Task<IRole> GetAutoRoleAsync(IGuild guild);

        Task SetPrefixAsync(string guildId, string prefix);
        Task SetPrefixAsync(IGuild guild, string prefix);

        Task SetAutoRoleAsync(string guildId, string autoRoleId);
        Task SetAutoRoleAsync(IGuild guild, IRole role);
    }
}
