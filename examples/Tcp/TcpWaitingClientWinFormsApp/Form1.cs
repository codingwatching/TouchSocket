//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://touchsocket.net/
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------

using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace TcpWaitingClientWinFormsApp;

public partial class Form1 : Form
{
    public Form1()
    {
        this.InitializeComponent();
        this.Load += this.Form1_Load;
    }

    private TcpService m_tcpService;
    private async void Form1_Load(object? sender, EventArgs e)
    {
        this.m_tcpService = await CreateService();

        this.UpdateServiceButtonUI();
    }
    private static async Task<TcpService> CreateService()
    {
        var service = new TcpService();

        await service.SetupAsync(new TouchSocketConfig()
             .SetListenIPHosts(7789)
             .ConfigureContainer(a =>
             {
                 a.AddConsoleLogger();
             })
             .ConfigurePlugins(a =>
             {
                 a.Add<MyPlugin1>();
             }));
        await service.StartAsync();

        service.Logger.Info("Server started");

        return service;
    }

    private TcpClient m_tcpClient;

    private async Task IsConnected()
    {
        try
        {
            if (this.m_tcpClient?.Online == true)
            {
                return;
            }
            this.m_tcpClient.SafeDispose();
            this.m_tcpClient = new TcpClient();

            this.m_tcpClient.Received = async (client, e) =>
            {
                //此处不能await，否则也会导致死锁
                _ = Task.Run(async () =>
                {
                    var waitingClient = client.CreateWaitingClient(new WaitingOptions());

                    var bytes = await waitingClient.SendThenReturnAsync("hello");
                });

                await Task.CompletedTask;
            };

            await this.m_tcpClient.SetupAsync(new TouchSocketConfig()
                .ConfigurePlugins(a =>
                {
                    a.Add(typeof(ITcpReceivedPlugin), (ReceivedDataEventArgs e) =>
                    {
                        Console.WriteLine($"PluginReceivedData:{e.ByteBlock.Span.ToString(Encoding.UTF8)}");
                    });
                })
                 .SetRemoteIPHost(this.textBox1.Text));

            await this.m_tcpClient.ConnectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void button2_Click(object sender, EventArgs e)
    {
        try
        {
            await this.IsConnected();
            var waitingClient = this.m_tcpClient.CreateWaitingClient(new WaitingOptions());

            this.cts = new CancellationTokenSource(5000);
            var bytes = await waitingClient.SendThenReturnAsync(this.textBox2.Text.ToUtf8Bytes(), this.cts.Token);
            if (!bytes .IsEmpty)
            {
                MessageBox.Show($"message:{bytes.Span.ToString(Encoding.UTF8)}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void button3_Click(object sender, EventArgs e)
    {
        try
        {
            await this.IsConnected();
            var waitingClient = this.m_tcpClient.CreateWaitingClient(new WaitingOptions()
            {
                FilterFuncAsync = async (response) =>
                {
                    var byteBlock = response.ByteBlock;
                    var requestInfo = response.RequestInfo;

                    if (byteBlock != null)
                    {
                        var str = byteBlock.Span.ToString(Encoding.UTF8);
                        if (str.Contains(this.textBox4.Text))
                        {
                            return true;
                        }
                        else
                        {
                            //数据不符合要求，waitingClient继续等待

                            //如果需要在插件中继续处理，在此处触发插件

                            await this.m_tcpClient.PluginManager.RaiseAsync(typeof(ITcpReceivedPlugin), this.m_tcpClient, new ReceivedDataEventArgs(byteBlock, requestInfo)).ConfigureAwait(false);
                        }
                    }
                    return false;
                }
            });

            this.cts = new CancellationTokenSource(500000);
            var bytes = await waitingClient.SendThenReturnAsync(this.textBox3.Text.ToUtf8Bytes(), this.cts.Token);

            if (!bytes.IsEmpty)
            {
                MessageBox.Show($"message:{bytes.Span.ToString(Encoding.UTF8)}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        await this.m_tcpClient?.CloseAsync();
    }

    private CancellationTokenSource cts;
    private void button5_Click(object sender, EventArgs e)
    {
        this.cts?.Cancel();
    }

    private void button4_Click(object sender, EventArgs e)
    {
        this.m_tcpService?.Dispose();
        this.m_tcpService = default;
        this.UpdateServiceButtonUI();
    }

    private void UpdateServiceButtonUI()
    {
        if (this.m_tcpService == null)
        {
            this.button4.Text = "启动服务";
        }
        else
        {
            this.button4.Text = "停止服务";
        }
    }
}

internal class MyPlugin1 : PluginBase, ITcpReceivedPlugin
{
    private readonly ILog m_logger;

    public MyPlugin1(ILog logger)
    {
        this.m_logger = logger;
    }

    public async Task OnTcpReceived(ITcpSession client, ReceivedDataEventArgs e)
    {
        this.m_logger.Info($"Plugin:{e.ByteBlock.ToString()}");

        if (client is ITcpSessionClient sessionClient)
        {
            await sessionClient.SendAsync(e.ByteBlock.Memory);
        }
    }
}