﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace CommandServer
{
    class Program
    {
        static IHostBuilder CreateSocketServerBuilder()
        {
            return SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
                .UseCommand((commandOptions) =>
                {
                    // register commands one by one
                    commandOptions.AddCommand<ADD>();
                    commandOptions.AddCommand<MULT>();
                    commandOptions.AddCommand<SUB>();

                    // register all commands in one aassembly
                    //commandOptions.AddCommandAssembly(typeof(SUB).GetTypeInfo().Assembly);
                })
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "TestServer" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", "4040" }
                    });
                })
                .ConfigureLogging((hostCtx, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                })
                .ConfigureServices((hostCtx, services) =>
                {
                    services.AddOptions();
                    services.Configure<ServerOptions>(hostCtx.Configuration.GetSection("serverOptions"));
                });
        }

        static async Task Main(string[] args)
        {
            var host = CreateSocketServerBuilder().Build();        
            await host.RunAsync();
        }
    }
}
