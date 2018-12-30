﻿using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class MessagePackFormatterPerFieldTest
    {
        private MessagePackSerializer serializer = new MessagePackSerializer();

        [MessagePackObject]
        public class MyClass
        {
            [Key(0)]
            [MessagePackFormatter(typeof(Int_x10Formatter))]
            public int MyProperty1 { get; set; }
            [Key(1)]
            public int MyProperty2 { get; set; }
            [Key(2)]
            [MessagePackFormatter(typeof(String_x2Formatter))]
            public string MyProperty3 { get; set; }
            [Key(3)]
            public string MyProperty4 { get; set; }
        }

        [MessagePackObject]
        public struct MyStruct
        {
            [Key(0)]
            [MessagePackFormatter(typeof(Int_x10Formatter))]
            public int MyProperty1 { get; set; }
            [Key(1)]
            public int MyProperty2 { get; set; }
            [Key(2)]
            [MessagePackFormatter(typeof(String_x2Formatter))]
            public string MyProperty3 { get; set; }
            [Key(3)]
            public string MyProperty4 { get; set; }
        }

        public class Int_x10Formatter : IMessagePackFormatter<int>
        {
            public int Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
            {
                return MessagePackBinary.ReadInt32(ref byteSequence) * 10;
            }

            public void Serialize(IBufferWriter<byte> writer, int value, IFormatterResolver formatterResolver)
            {
                MessagePackBinary.WriteInt32(writer, value * 10);
            }
        }

        public class String_x2Formatter : IMessagePackFormatter<string>
        {
            public string Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
            {
                var s = MessagePackBinary.ReadString(ref byteSequence);
                return s + s;
            }

            public void Serialize(IBufferWriter<byte> writer, string value, IFormatterResolver formatterResolver)
            {
                MessagePackBinary.WriteString(writer, value + value);
            }
        }


        [Fact]
        public void FooBar()
        {
            {
                var bin = serializer.Serialize(new MyClass { MyProperty1 = 100, MyProperty2 = 9, MyProperty3 = "foo", MyProperty4 = "bar" });
                var json = serializer.ToJson(bin);
                json.Is("[1000,9,\"foofoo\",\"bar\"]");

                var r2 = serializer.Deserialize<MyClass>(bin);
                r2.MyProperty1.Is(10000);
                r2.MyProperty2.Is(9);
                r2.MyProperty3.Is("foofoofoofoo");
                r2.MyProperty4.Is("bar");
            }
            {
                var bin = serializer.Serialize(new MyStruct { MyProperty1 = 100, MyProperty2 = 9, MyProperty3 = "foo", MyProperty4 = "bar" });
                var json = serializer.ToJson(bin);
                json.Is("[1000,9,\"foofoo\",\"bar\"]");

                var r2 = serializer.Deserialize<MyStruct>(bin);
                r2.MyProperty1.Is(10000);
                r2.MyProperty2.Is(9);
                r2.MyProperty3.Is("foofoofoofoo");
                r2.MyProperty4.Is("bar");
            }
        }
    }
}
