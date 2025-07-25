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

namespace TouchSocket.Dmtp.Redis;

internal class RedisRequestWaitPackage : RedisResponseWaitPackage
{
    public string key;
    public TimeSpan? timeSpan;
    public RedisPackageType packageType;

    public override void Package<TWriter>(ref TWriter writer)
    {
        base.Package(ref writer);
        writer.WriteString(this.key);
        writer.WriteByte((byte)this.packageType);
        if (this.timeSpan.HasValue)
        {
            writer.WriteByte(1);
            writer.WriteTimeSpan(this.timeSpan.Value);
        }
        else
        {
            writer.WriteByte(0);
        }
    }

    public override void Unpackage<TReader>(ref TReader reader)
    {
        base.Unpackage(ref reader);
        this.key = reader.ReadString();
        this.packageType = (RedisPackageType)reader.ReadByte();
        if (reader.ReadByte() == 1)
        {
            this.timeSpan = reader.ReadTimeSpan();
        }
    }
}