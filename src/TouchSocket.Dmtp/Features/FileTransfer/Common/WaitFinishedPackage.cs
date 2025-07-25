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

namespace TouchSocket.Dmtp.FileTransfer;

internal class WaitFinishedPackage : WaitRouterPackage
{
    public ResultCode Code { get; set; }
    public Metadata Metadata { get; set; }
    public int ResourceHandle { get; set; }

    public override void PackageBody<TWriter>(ref TWriter writer)
    {
        base.PackageBody(ref writer);
        writer.WriteInt32(this.ResourceHandle);
        if (this.Metadata is null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteNotNull();
            this.Metadata.Package(ref writer);
        }
        writer.WriteByte((byte)this.Code);
    }

    public override void UnpackageBody<TReader>(ref TReader reader)
    {
        base.UnpackageBody(ref reader);
        this.ResourceHandle = reader.ReadInt32();
        if (reader.ReadIsNull())
        {
            this.Metadata = null;
        }
        else
        {
            this.Metadata = new Metadata();
            this.Metadata.Unpackage(ref reader);
        }
        this.Code = (ResultCode)reader.ReadByte();
    }
}