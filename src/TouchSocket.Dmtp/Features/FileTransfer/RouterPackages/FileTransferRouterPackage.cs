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

/// <summary>
/// 文件传输请求包
/// </summary>
public class FileTransferRouterPackage : WaitRouterPackage
{
    /// <summary>
    /// 续传索引
    /// </summary>
    public int ContinuationIndex { get; set; }

    /// <summary>
    /// 文件信息
    /// </summary>
    public RemoteFileInfo FileInfo { get; set; }

    /// <summary>
    /// 分块大小
    /// </summary>
    public int FileSectionSize { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Metadata Metadata { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 资源句柄
    /// </summary>
    public int ResourceHandle { get; set; }

    /// <inheritdoc/>
    protected override bool IncludedRouter => true;

    /// <inheritdoc/>
    public override void PackageBody<TWriter>(ref TWriter writer)
    {
        base.PackageBody(ref writer);
        writer.WriteInt32(this.ContinuationIndex);
        writer.WriteString(this.Path);
        writer.WriteInt32(this.ResourceHandle);
        writer.WriteInt32(this.FileSectionSize);
        if (this.FileInfo is null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteNotNull();
            this.FileInfo.Package(ref writer);
        }
        
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

    /// <inheritdoc/>
    public override void UnpackageBody<TReader>(ref TReader reader)
    {
        base.UnpackageBody(ref reader);
        this.ContinuationIndex = reader.ReadInt32();
        this.Path = reader.ReadString();
        this.ResourceHandle = reader.ReadInt32();
        this.FileSectionSize = reader.ReadInt32();

        if (reader.ReadIsNull())
        {
            this.FileInfo = null;
        }
        else
        {
            this.FileInfo = new RemoteFileInfo();
            this.FileInfo.Unpackage(ref reader);
        }
        
        if (reader.ReadIsNull())
        {
            this.Metadata = null;
        }
        else
        {
            this.Metadata = new Metadata();
            this.Metadata.Unpackage(ref reader);
        }
    }
}