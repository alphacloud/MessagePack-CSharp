﻿#if !UNITY

using System;
using System.Buffers;

namespace MessagePack.Formatters
{
    public sealed class BinaryGuidFormatter : IMessagePackFormatter<Guid>
    {
        /// <summary>
        /// Unsafe binary Guid formatter. this is only allows on LittleEndian environment.
        /// </summary>
        public static readonly IMessagePackFormatter<Guid> Instance = new BinaryGuidFormatter();

        BinaryGuidFormatter()
        {
        }

        // Guid's underlying _a,...,_k field is sequential and same layout as .NET Framework and Mono(Unity).
        // But target machines must be same endian so restrict only for little endian.

        public unsafe void Serialize(IBufferWriter<byte> writer, Guid value, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) throw new Exception("BinaryGuidFormatter only allows on little endian env.");

            fixed (byte* dst = &writer.GetSpan(18)[0])
            {
                var src = &value;

                dst[0] = MessagePackCode.Bin8;
                dst[1] = 16;

                *(Guid*)(dst + 2) = *src;
            }

            writer.Advance(18);
        }

        public unsafe Guid Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) throw new Exception("BinaryGuidFormatter only allows on little endian env.");

            if (byteSequence.Length < 18)
            {
                throw new ArgumentOutOfRangeException();
            }

            return MessagePackBinary.Parse(ref byteSequence, 18, span =>
                {
                    fixed (byte* src = &span[0])
                    {
                        if (src[0] != MessagePackCode.Bin8)
                        {
                            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", span[0], MessagePackCode.ToFormatName(span[0])));
                        }
                        if (src[1] != 16)
                        {
                            throw new InvalidOperationException("Invalid Guid Size.");
                        }

                        var target = *(Guid*)(src + 2);
                        return target;
                    }
                });
        }
    }

    public sealed class BinaryDecimalFormatter : IMessagePackFormatter<Decimal>
    {
        /// <summary>
        /// Unsafe binary Decimal formatter. this is only allows on LittleEndian environment.
        /// </summary>
        public static readonly IMessagePackFormatter<Decimal> Instance = new BinaryDecimalFormatter();

        BinaryDecimalFormatter()
        {
        }

        // decimal underlying "flags, hi, lo, mid" fields are sequential and same layuout with .NET Framework and Mono(Unity)
        // But target machines must be same endian so restrict only for little endian.

        public unsafe void Serialize(IBufferWriter<byte> writer, Decimal value, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) throw new Exception("BinaryGuidFormatter only allows on little endian env.");

            fixed (byte* dst = &writer.GetSpan(18)[0])
            {
                var src = &value;

                dst[0] = MessagePackCode.Bin8;
                dst[1] = 16;

                *(Decimal*)(dst + 2) = *src;
            }

            writer.Advance(18);
        }

        public unsafe Decimal Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) throw new Exception("BinaryDecimalFormatter only allows on little endian env.");

            if (byteSequence.Length < 18)
            {
                throw new ArgumentOutOfRangeException();
            }

            return MessagePackBinary.Parse(ref byteSequence, 18, span =>
            {
                fixed (byte* src = &span[0])
                {
                    if (src[0] != MessagePackCode.Bin8)
                    {
                        throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", span[0], MessagePackCode.ToFormatName(span[0])));
                    }
                    if (src[1] != 16)
                    {
                        throw new InvalidOperationException("Invalid Guid Size.");
                    }

                    var target = *(Decimal*)(src + 2);
                    return target;
                }
            });
        }
    }
}

#endif