﻿using SWGoH;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Repository
{
    public interface IMongoDBRepository
    {
        Task<List<CharacterConfig>> GetCharactersConfig();
        Task<List<GuildConfig>> GetGuildsConfig();
        Task<Player> GetPlayer(string userName);
        Task<Guild> GetGuildPlayers(string guildName);
        Task<List<Player>> GetAllPlayersWithoutCharacters();
        Task<string> SendPlayerToQueue(string playerName);
        Task<string> SendGuildToQueue(string guildName);
        Task<List<Queue>> GetQueue();
        Task<Queue> RemoveFromQueue(string name);
        Task<CharacterConfig> SetCharacterAlias(string characterFullName, string alias);
        Task<CharacterConfig> RemoveCharacterAlias(string characterFullName, string alias);
    }
}
