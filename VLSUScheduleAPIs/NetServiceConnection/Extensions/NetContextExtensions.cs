using Microsoft.Extensions.DependencyInjection;
using NetServiceConnection.NetContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetServiceConnection.Extensions
{
    public static class NetContextExtensions
    {
        public static NetContext.NetContext UserAuthorization(this NetContext.NetContext netContext, Func<string> getHeader)
        {
            netContext.preHeader.Add((x) => x.DefaultRequestHeaders.Add("Authorization", getHeader()));
            return netContext;
        }

        public static void UseLazyModeling(this NetContext.NetContext netContext) => netContext.isLazy = true;

        public static void AddNetContext<T>(this IServiceCollection services, Type networkAccess = null) where T : NetContext.NetContext
        {
            services.AddSingleton(typeof(INetworkModelAccess<>), networkAccess ?? typeof(HttpLoad<>));
            services.AddSingleton<T>();
        }

        public static void AddNetContext<T>(this IServiceCollection services, Func<IServiceProvider, T> construct, Type networkAccess = null) where T : NetContext.NetContext
        {
            services.AddSingleton(typeof(INetworkModelAccess<>), networkAccess ?? typeof(HttpLoad<>));
            services.AddSingleton(construct);
        }
    }
}
