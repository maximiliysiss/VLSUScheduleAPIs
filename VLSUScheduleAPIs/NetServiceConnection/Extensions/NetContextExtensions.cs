using System;
using System.Collections.Generic;
using System.Text;

namespace NetServiceConnection.Extensions
{
    public static class NetContextExtensions
    {
        public static void UserAuthorization(this NetContext.NetContext netContext, Func<string> getHeader)
        {
            netContext.preHeader.Add((x) => x.DefaultRequestHeaders.Add("Authorization", getHeader()));
        }

        public static void UseLazyModeling(this NetContext.NetContext netContext) => netContext.isLazy = true;
    }
}
