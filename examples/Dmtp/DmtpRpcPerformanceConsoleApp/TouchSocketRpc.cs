﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace RpcPerformanceConsoleApp
{
    public static class TouchSocketRpc
    {
        public static void StartServer()
        {
            var host = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddServiceHostedService<ITcpDmtpService, TcpDmtpService>(config =>
            {
                config.SetListenIPHosts(7789)
                       .ConfigureContainer(a =>
                       {
                           a.AddConsoleLogger();

                           a.AddRpcStore(store =>
                           {
                               store.RegisterServer<TestController>();
                           });
                       })
                       .ConfigurePlugins(a =>
                       {
                           a.UseDmtpRpc();
                       })
                       .SetDmtpOption(new DmtpOption()
                       {
                           VerifyToken = "Rpc"//设定连接口令，作用类似账号密码
                       });
            });
        })
        .Build();

            host.RunAsync();
        }

        public static void StartSumClient(int count)
        {
            var client = new TcpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpRpc();
                })
                .SetRemoteIPHost("127.0.0.1:7789")
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Rpc"
                }));
            client.Connect();

            var timeSpan = TimeMeasurer.Run(() =>
            {
                var actor = client.GetDmtpRpcActor();
                for (var i = 0; i < count; i++)
                {
                    var rs = actor.InvokeT<Int32>("Sum", InvokeOption.WaitInvoke, i, i);
                    if (rs != i + i)
                    {
                        Console.WriteLine("调用结果不一致");
                    }
                    if (i % 1000 == 0)
                    {
                        Console.WriteLine(i);
                    }
                }
            });
            Console.WriteLine(timeSpan);
        }

        public static void StartGetBytesClient(int count)
        {
            var client = new TcpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpRpc();
                })
                .SetRemoteIPHost("127.0.0.1:7789")
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Rpc"
                }));
            client.Connect();

            var timeSpan = TimeMeasurer.Run(() =>
            {
                var actor = client.GetDmtpRpcActor();
                for (var i = 1; i < count; i++)
                {
                    var rs = actor.InvokeT<byte[]>("GetBytes", InvokeOption.WaitInvoke, i);//测试10k数据
                    if (rs.Length != i)
                    {
                        Console.WriteLine("调用结果不一致");
                    }
                    if (i % 1000 == 0)
                    {
                        Console.WriteLine(i);
                    }
                }
            });
            Console.WriteLine(timeSpan);
        }

        public static void StartBigStringClient(int count)
        {
            var client = new TcpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpRpc();
                })
                .SetRemoteIPHost("127.0.0.1:7789")
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Rpc"
                }));
            client.Connect();


            var timeSpan = TimeMeasurer.Run(() =>
            {
                var actor = client.GetDmtpRpcActor();
                for (var i = 0; i < count; i++)
                {
                    var rs = actor.InvokeT<string>("GetBigString", InvokeOption.WaitInvoke);
                    if (i % 1000 == 0)
                    {
                        Console.WriteLine(i);
                    }
                }
            });
            Console.WriteLine(timeSpan);
        }
    }
}
