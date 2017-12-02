using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Enums.QueueEnum
{
    public enum Command
    {
        UpdatePlayer = 1,
        UpdateGuild = 2,
        UpdateGuildWithNoChars = 3,
        UpdatePlayers = 4,
        GetNewCharacters = 5,
        Help = 6,
        UnKnown = 7,
        Test = 8,
        GetNewCharactersAndAbilities = 9,
    }

    public enum QueueStatus
    {
        PendingProcess = 0,
        Processing = 1,
        Failed = 101
    }

    public enum QueueType
    {
        Player = 0,
        Guild = 1
    }
}
