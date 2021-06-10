//------------------------------------------------------------------------------
//  此代码版权归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using RRQMCore.ByteManager;
using RRQMCore.Serialization;
using System;
using System.IO;

namespace RRQMSocket.FileTransfer
{
    /*
    若汝棋茗
    */

    internal static class FileBaseTool
    {
        #region Methods

        internal static void SaveProgressBlockCollection(RRQMStream stream, ProgressBlockCollection blocks)
        {
            if (stream.RRQMFileStream != null && stream.RRQMFileStream.CanWrite)
            {
                stream.RRQMFileStream.Position = 0;
                byte[] dataBuffer = SerializeConvert.RRQMBinarySerialize(PBCollectionTemp.GetFromProgressBlockCollection(blocks), true);
                stream.RRQMFileStream.Write(dataBuffer, 0, dataBuffer.Length);
                stream.RRQMFileStream.Flush();
            }
        }

        internal static bool WriteFile(RRQMStream stream, out string mes, long streamPosition, byte[] buffer, int offset, int length)
        {
            try
            {
                if (stream != null && stream.TempFileStream.CanWrite)
                {
                    stream.TempFileStream.Position = streamPosition;
                    stream.TempFileStream.Write(buffer, offset, length);
                    stream.TempFileStream.Flush();
                    mes = null;
                    return true;
                }
                mes = "流不可写";
                return false;
            }
            catch (Exception ex)
            {
                mes = ex.Message;
                return false;
            }
        }

        internal static void FileFinished(RRQMStream stream)
        {
            stream.Dispose();
            if (File.Exists(stream.fileInfo.FilePath + ".rrqm"))
            {
                File.Delete(stream.fileInfo.FilePath + ".rrqm");
            }
            if (File.Exists(stream.fileInfo.FilePath))
            {
                File.Delete(stream.fileInfo.FilePath);
            }
            if (File.Exists(stream.fileInfo.FilePath + ".temp"))
            {
                File.Move(stream.fileInfo.FilePath + ".temp", stream.fileInfo.FilePath);
            }
        }

        internal static ProgressBlockCollection GetProgressBlockCollection(FileInfo fileInfo, bool breakpointResume)
        {
            ProgressBlockCollection blocks = new ProgressBlockCollection();
            blocks.FileInfo = new FileInfo();
            blocks.FileInfo.Copy(fileInfo);
            long position = 0;
            if (breakpointResume && fileInfo.FileLength >= 100)
            {
                long blockLength = (long)(fileInfo.FileLength / 100.0);

                for (int i = 0; i < 100; i++)
                {
                    FileProgressBlock block = new FileProgressBlock();
                    block.Index = i;
                    block.FileHash = fileInfo.FileHash;
                    block.Finished = false;
                    block.StreamPosition = position;
                    block.UnitLength = i != 99 ? blockLength : fileInfo.FileLength - i * blockLength;
                    blocks.Add(block);
                    position += blockLength;
                }
            }
            else
            {
                FileProgressBlock block = new FileProgressBlock();
                block.Index = 0;
                block.FileHash = fileInfo.FileHash;
                block.Finished = false;
                block.StreamPosition = position;
                block.UnitLength = fileInfo.FileLength;
                blocks.Add(block);
            }
            return blocks;
        }

        internal static bool ReadFileBytes(string path, long beginPosition, ByteBlock byteBlock, int offset, int length)
        {
            FileStream fileStream = TransferFileStreamDic.GetFileStream(path);

            fileStream.Position = beginPosition;

            if (byteBlock.Buffer.Length < length + offset)
            {
                byteBlock.SetBuffer(new byte[length + offset]);
            }

            int r = fileStream.Read(byteBlock.Buffer, offset, length);
            if (r == length)
            {
                byteBlock.Position = offset + length;
                byteBlock.SetLength(offset + length);
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}