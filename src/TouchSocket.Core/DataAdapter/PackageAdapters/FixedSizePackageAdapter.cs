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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Resources;

namespace TouchSocket.Core;

/// <summary>
/// 固定长度数据包处理适配器。
/// </summary>
public class FixedSizePackageAdapter : SingleStreamDataHandlingAdapter
{
    /// <summary>
    /// 包剩余长度
    /// </summary>
    private int m_surPlusLength = 0;

    /// <summary>
    /// 临时包
    /// </summary>
    private ByteBlock m_tempByteBlock;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fixedSize">数据包的长度</param>
    public FixedSizePackageAdapter(int fixedSize)
    {
        this.FixedSize = fixedSize;
    }

    /// <inheritdoc/>
    public override bool CanSendRequestInfo => false;

    /// <inheritdoc/>
    public override bool CanSplicingSend => true;

    /// <summary>
    /// 获取已设置的数据包的长度
    /// </summary>
    public int FixedSize { get; private set; }

    /// <summary>
    /// 预处理
    /// </summary>
    /// <param name="byteBlock"></param>
    protected override async Task PreviewReceivedAsync(IByteBlockReader byteBlock)
    {
        if (this.CacheTimeoutEnable && DateTimeOffset.UtcNow - this.LastCacheTime > this.CacheTimeout)
        {
            this.Reset();
        }
        var array = byteBlock.Memory.GetArray();
        var buffer = array.Array;
        var r = byteBlock.Length;
        if (this.m_tempByteBlock == null)
        {
            await this.SplitPackage(buffer, 0, r).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        else
        {
            if (this.m_surPlusLength == r)
            {
                this.m_tempByteBlock.Write(new ReadOnlySpan<byte>(buffer, 0, this.m_surPlusLength));
                await this.PreviewHandle(this.m_tempByteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                this.m_tempByteBlock = null;
                this.m_surPlusLength = 0;
            }
            else if (this.m_surPlusLength < r)
            {
                this.m_tempByteBlock.Write(new ReadOnlySpan<byte>(buffer, 0, this.m_surPlusLength));
                await this.PreviewHandle(this.m_tempByteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                this.m_tempByteBlock = null;
                await this.SplitPackage(buffer, this.m_surPlusLength, r).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }
            else
            {
                this.m_tempByteBlock.Write(new ReadOnlySpan<byte>(buffer, 0, r));
                this.m_surPlusLength -= r;
                if (this.UpdateCacheTimeWhenRev)
                {
                    this.LastCacheTime = DateTimeOffset.UtcNow;
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task PreviewSendAsync(ReadOnlyMemory<byte> memory, CancellationToken token = default)
    {
        var dataLen = memory.Length;
        if (dataLen != this.FixedSize)
        {
            throw new OverlengthException(TouchSocketCoreResource.ValueMoreThan.Format(nameof(memory.Length), this.FixedSize));
        }
        var byteBlock = new ByteBlock(this.FixedSize);

        byteBlock.Write(memory.Span);

        byteBlock.SetLength(this.FixedSize);
        try
        {
            await this.GoSendAsync(byteBlock.Memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        finally
        {
            byteBlock.Dispose();
        }
    }

    /// <inheritdoc/>
    [Obsolete("该接口已被弃用，请使用SendAsync直接代替")]
    protected override async Task PreviewSendAsync(IList<ArraySegment<byte>> transferBytes)
    {
        var length = 0;
        foreach (var item in transferBytes)
        {
            length += item.Count;
        }

        if (length != this.FixedSize)
        {
            throw new OverlengthException(TouchSocketCoreResource.ValueMoreThan.Format(nameof(length), this.FixedSize));
        }
        var byteBlock = new ByteBlock(this.FixedSize);

        foreach (var item in transferBytes)
        {
            byteBlock.Write(new ReadOnlySpan<byte>(item.Array, item.Offset, item.Count));
        }

        byteBlock.SetLength(this.FixedSize);
        try
        {
            await this.GoSendAsync(byteBlock.Memory,CancellationToken.None).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        finally
        {
            byteBlock.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override void Reset()
    {
        this.m_tempByteBlock.SafeDispose();
        this.m_tempByteBlock = null;
        this.m_surPlusLength = 0;
        base.Reset();
    }

    private async Task PreviewHandle(ByteBlock byteBlock)
    {
        try
        {
            byteBlock.Position = 0;
            await this.GoReceivedAsync(byteBlock, null).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        finally
        {
            byteBlock.Dispose();
        }
    }

    private async Task SplitPackage(byte[] dataBuffer, int index, int r)
    {
        while (index < r)
        {
            if (r - index >= this.FixedSize)
            {
                var byteBlock = new ByteBlock(this.FixedSize);
                byteBlock.Write(new ReadOnlySpan<byte>(dataBuffer, index, this.FixedSize));
                await this.PreviewHandle(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                this.m_surPlusLength = 0;
            }
            else//半包
            {
                this.m_tempByteBlock = new ByteBlock(this.FixedSize);
                this.m_surPlusLength = this.FixedSize - (r - index);
                this.m_tempByteBlock.Write(new ReadOnlySpan<byte>(dataBuffer, index, r - index));
                if (this.UpdateCacheTimeWhenRev)
                {
                    this.LastCacheTime = DateTimeOffset.UtcNow;
                }
            }
            index += this.FixedSize;
        }
    }
}