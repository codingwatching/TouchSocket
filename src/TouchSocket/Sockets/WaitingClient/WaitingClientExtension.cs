//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Sockets
{
    /// <summary>
    /// WaitingClientExtensions
    /// </summary>
    public static class WaitingClientExtension
    {
        /// <summary>
        /// WaitingClient
        /// </summary>
        public static readonly IDependencyProperty<object> WaitingClientProperty =
            DependencyProperty<object>.Register("WaitingClient", typeof(WaitingClientExtension), null);

        /// <summary>
        /// 获取可等待的客户端。
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="client"></param>
        /// <param name="waitingOptions"></param>
        /// <returns></returns>
        public static IWaitingClient<TClient> GetWaitingClient<TClient>(this TClient client, WaitingOptions waitingOptions = WaitingOptions.AllAdapter) where TClient : IClient, IDefaultSender, ISender
        {
            if (client.GetValue(WaitingClientProperty) is IWaitingClient<TClient> c1)
            {
                c1.WaitingOptions = waitingOptions;
                return c1;
            }

            WaitingClient<TClient> waitingClient = new WaitingClient<TClient>(client, waitingOptions);
            client.SetValue(WaitingClientProperty, waitingClient);
            return waitingClient;
        }
    }
}