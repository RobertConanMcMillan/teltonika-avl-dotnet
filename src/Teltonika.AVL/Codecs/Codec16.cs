﻿using System.Buffers;
using Teltonika.AVL.Attributes;
using Teltonika.AVL.Extensions;

namespace Teltonika.AVL.Codecs;

[AvlCodecParser(AvlCodec.Codec16)]
public class Codec16 : IAvlCodec
{
    public AvlRecord[] Parse(ref SequenceReader<byte> reader, int count)
    {
        var records = new AvlRecord[count];

        for (int i = 0; i < count; i++)
        {
            records[i] = new()
            {
                Timestamp = reader.ReadLongBigEndian(),
                Priority = reader.ReadByte(),
                Longitude = reader.ReadIntBigEndian(),
                Latitude = reader.ReadIntBigEndian(),
                Altitude = reader.ReadShortBigEndian(),
                Angle = reader.ReadShortBigEndian(),
                Satellites = reader.ReadByte(),
                Speed = reader.ReadShortBigEndian(),
                EventId = reader.ReadShortBigEndian(),
                GenerationType = reader.ReadByte(),
                Elements = ReadElements(ref reader)
            };
        }

        return records;
    }

    private static AvlIOElement[] ReadElements(ref SequenceReader<byte> reader)
    {
        var elements = new AvlIOElement[reader.ReadByte()];
        var i = 0;

        for (int size = 1; size <= 8; size *= 2)
        {
            var count = reader.ReadByte();

            for (; count > 0; --count)
            {
                elements[i++] = new()
                {
                    Id = reader.ReadShortBigEndian(),
                    Value = reader.ReadByteArray(size)
                };
            }
        }

        return elements;
    }
}