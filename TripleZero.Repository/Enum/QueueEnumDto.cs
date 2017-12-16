using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.EnumDto
{
    internal enum QueueCommand
    {
        UpdatePlayer = 1,
        UpdateGuild = 2,
        UpdateGuildWithNoChars = 3,
        UpdatePlayers = 4,
        GetNewCharacters = 5,
        Help = 6,
        UnKnown = 7,
        Test = 8,
        GetNewCharactersAndAbilities = 9
    }
    internal enum QueueStatus
    {
        PendingProcess = 0,
        Processing = 1,
        Failed = 101
    }
    internal enum QueueType
    {
        Player = 0,
        Guild = 1
    }
    internal enum PriorityEnum
    {
        DailyUpdate = 1,
        ManualLoad = 2
    }
}
