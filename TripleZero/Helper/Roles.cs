using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TripleZero.Helper
{
    public static class Roles
    {
        public static bool UserInRole(SocketCommandContext context,string roleTocheck)
        {
            var user = context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == roleTocheck);

            if (user.Roles.Contains(role))  return true;  else return false; 
        }
    }
}
