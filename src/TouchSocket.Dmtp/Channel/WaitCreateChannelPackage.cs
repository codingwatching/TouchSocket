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

using TouchSocket.Core;

namespace TouchSocket.Dmtp;

/// <summary>
/// WaitCreateChannelPackage
/// </summary>
internal class WaitCreateChannelPackage : WaitRouterPackage
{
    /// <summary>
    /// 通道Id
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Metadata Metadata { get; set; }

    /// <summary>
    /// 随机Id
    /// </summary>
    public bool Random { get; set; }

    public override void PackageBody<TWriter>(ref TWriter writer)
    {
        base.PackageBody(ref writer);
        writer.WriteBoolean(this.Random);
        writer.WriteInt32(this.ChannelId);

        if (this.Metadata is null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteNotNull();
            this.Metadata.Package(ref writer);
        }
    }

    public override void UnpackageBody<TReader>(ref TReader reader)
    {
        base.UnpackageBody(ref reader);
        this.Random = reader.ReadBoolean();
        this.ChannelId = reader.ReadInt32();
        if (reader.ReadIsNull())
        {
            this.Metadata = null;
        }
        else
        {
            var metadata = new Metadata();
            metadata.Unpackage(ref reader);
            this.Metadata = metadata;
        }
    }
}