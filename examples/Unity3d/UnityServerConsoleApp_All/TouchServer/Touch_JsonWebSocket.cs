using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Http.WebSockets;
using TouchSocket.JsonRpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;
using UnityRpcProxy;

namespace UnityServerConsoleApp_All.TouchServer;

/// <summary>
/// Web Socket
/// </summary>
public class Touch_JsonWebSocket : BaseTouchServer
{
    private readonly JsonHttpDmtpService dmtpService = new JsonHttpDmtpService();
    public async Task StartService(int port)
    {
        var config = new TouchSocketConfig()//配置
            .SetListenIPHosts(port)

            .ConfigureContainer(a =>
            {
                a.AddConsoleLogger();//注册一个日志组

                //注册rpc服务
                a.AddRpcStore(store =>
                {
                    store.RegisterServer<UnityRpcStore>();
#if DEBUG
                    var code = store.GetProxyCodes("UnityRpcProxy_Json_HttpDmtp", typeof(JsonRpcAttribute));
                    File.WriteAllText("../../../RPCStore/UnityRpcProxy_Json_HttpDmtp.cs", code);
#endif
                });
            })
            .ConfigurePlugins(a =>
            {
                a.UseWebSocket()
                 .SetWSUrl("/ws");

                //启用json rpc插件
                a.UseWebSocketJsonRpc()
                .SetAllowJsonRpc((websocket, context) => true);//让所有请求WebSocket都加载JsonRpc插件


                a.Add<Touch_JsonWebSocket_Log_Plguin>();

            });

        await this.dmtpService.SetupAsync(config);
        await this.dmtpService.StartAsync();


        this.dmtpService.Logger.Info($"TCP_JsonWebSocket已启动，监听端口：{port}");
    }
}
/// <summary>
/// 状态日志打印插件
/// </summary>
internal class Touch_JsonWebSocket_Log_Plguin : PluginBase, IWebSocketHandshakedPlugin, IWebSocketClosedPlugin
//,IWebSocketReceivedPlugin
{

    public async Task OnWebSocketClosed(IWebSocket webSocket, ClosedEventArgs e)
    {
        webSocket.Client.Logger.Info($"TCP_WebSocket:客户端{webSocket.Client.IP}已断开");
        await e.InvokeNext();
    }

    public async Task OnWebSocketHandshaked(IWebSocket webSocket, HttpContextEventArgs e)
    {
        webSocket.Client.Logger.Info($"TCP_WebSocket:客户端{webSocket.Client.IP}已连接");
        await e.InvokeNext();
    }
}

/// <summary>
/// 自定义HttpDmtpService
/// </summary>
internal class JsonHttpDmtpService : TouchSocket.Dmtp.HttpDmtpService<JsonHttpDmtpSessionClient>
{
    protected override JsonHttpDmtpSessionClient NewClient()
    {
        return new JsonHttpDmtpSessionClient();
    }
}

/// <summary>
/// 自定义HttpDmtpSessionClient
/// </summary>
internal class JsonHttpDmtpSessionClient : TouchSocket.Dmtp.HttpDmtpSessionClient
{

    private readonly Timer timer;
    public JsonHttpDmtpSessionClient()
    {
        this.timer = new Timer(this.ClientReverseRPC, null, 1 * 1000, 10 * 1000);
    }

    private readonly Random Random = new Random();
    private async void ClientReverseRPC(object? client)
    {
        if (this.Online)
        {
            try
            {
                await this.GetJsonRpcActionClient().AddAsync(1, 2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.StopReverseRPC();
            }

        }
        else
        {
            this.StopReverseRPC();
        }
    }

    internal void StopReverseRPC()
    {
        this.timer?.Dispose();
    }
}
