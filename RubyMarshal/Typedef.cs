using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;

namespace RubyMarshal.Types
{
    public abstract class HashKey:Base
    {
        public abstract override int GetHashCode();
        public abstract override string ToString();
        public sealed override bool Equals(object? obj) => obj == null ? false : (ToString() == obj.ToString());
    }
    public abstract class Base
    {
        public String? AsString() => this as String;
        public Fixnum? AsFixnum() => this as Fixnum;
        public Float? AsFloat() => this as Float;
        public Symbol? AsSymbol() => this as Symbol;
        public Bignum? AsBigNumber() => this as Bignum;
        public Array? AsArray() => this as Array;
        public Object? AsObject() => this as Object;
        public Data? AsData() => this as Data;
        public SymbolLink? AsSymbolLink() => this as SymbolLink;
        public ObjectReferences? AsObjectReferences() => this as ObjectReferences;
        public Nil? AsNil() => this as Nil;
        public Bignum? AsBignum() => this as Bignum;
        public True? AsTrue() => this as True;
        public False? AsFalse() => this as False;
        public Hash? AsHash() => this as Hash;
        public UserDefined? AsUserDefined() => this as UserDefined;
        public Extended? AsExtended() => this as Extended;
        public InstanceVariable? AsInstanceVariable() => this as InstanceVariable;
        public Regex? AsRegex() => this as Regex;
        public UserClass? AsUserClass() => this as UserClass;
        public UserMarshal? AsUserMarshal() => this as UserMarshal;
        public DefaultHash? AsDefaultHash() => this as DefaultHash;
        public Module? AsModule() => this as Module;
        public Class? AsClass() => this as Class;
        public Struct? AsStruct() => this as Struct;
        public abstract JsonNode? ToJson();

    }
    public class Object : Base,IEnumerable<KeyValuePair<HashKey,Base>>
    {
        public Base Name { get; private set; }
        private Dictionary<HashKey, Base> _Elements { get; set; } = new();
        
        public Base this[HashKey s]
        {
            get => _Elements[s];
        }

        public Base this[string s]
        {
            get => _Elements[String.CreateTempObject(s)];
        }

        public int Count => _Elements.Count;
        internal Object(Decoder ctx, BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br)!;
            var count =new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br);
                if (key != null)
                    _Elements.Add(key, value);
                else
                    throw new NotSupportedException();
            }
        }
        public override JsonNode ToJson()
        {
            var obj = new JsonObject
            {
                ["class"] = Name.ToJson()
            };
            foreach(var pair in _Elements)
                obj.Add(pair.Key.ToString(), pair.Value?.ToJson());
            return obj;
        }

        public IEnumerator<KeyValuePair<HashKey, Base>> GetEnumerator()=> _Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()=> _Elements.GetEnumerator();
    }
    public class ObjectReferences : Base
    {
        public Base Target { get; private set; }
        internal ObjectReferences(Decoder ctx, BinaryReader br)
        {
            var index = new Fixnum(br).ToInt32();
            Target = ctx.ObjectList[index];
        }

        public override JsonNode? ToJson()
        {
            return Target.ToJson();
        }
    }
    public class Array : Base,IEnumerable<Base>
    {
        private List<Base> _Data = new(1);
        public int Count => _Data.Count;
        internal Array(Decoder ctx,BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            var count = new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
                _Data.Add(Decoder.ReaderMap[br.ReadByte()].Read(ctx,br));
        }

        public IEnumerator<Base> GetEnumerator() => _Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()=> _Data.GetEnumerator();

        public override JsonNode ToJson()
        {
            var arr = new JsonArray();
            foreach(var ele in _Data)
                arr.Add(ele.ToJson());
            return arr;
        }

        public Base this[int index]
        {
            get => _Data[index];
        }

    }
    public class SymbolLink : HashKey
    {
        public Symbol Target { get; private set; }
        internal SymbolLink(Decoder ctx, BinaryReader br)
        {
            var index = new Fixnum(br).ToInt32();
            Target = ctx.SymbolList[index];
        }
        public override string ToString()
        {
            return Target.ToString();
        }
        public override JsonNode? ToJson()
        {
            return Target.ToJson();
        }

        public override int GetHashCode()
        {
            return Target.GetHashCode(); 
        }
    }
    public class Symbol : HashKey
    {
        public String Name { get; private set; }
        internal Symbol(Decoder ctx,BinaryReader br)
        {
            Name = new String(ctx,br);
            ctx.SymbolList.Add(this);
        }
        public override int GetHashCode()=> Name.GetHashCode();

        public override string ToString()=>Name.ToString();
        public override JsonNode? ToJson()
        {
            return ToString();
        }


    }
    public class Nil : Base
    {
        internal Nil(BinaryReader _){}

        public override JsonNode? ToJson()
        {
            return null;
        }
    }
    public class Bignum : Base
    {
        public BigInteger Value { get; private set; }
        internal Bignum(Decoder ctx,BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public override JsonNode? ToJson()
        {
            return Value.ToString();
        }
    }
    public class True : Base
    {
        public const bool Value = true;
        internal True(BinaryReader _){}

        public override JsonNode? ToJson()
        {
            return Value;
        }
    }
    public class False : Base
    {
        public const bool Value = true;
        internal False(BinaryReader _) { }

        public override JsonNode? ToJson()
        {
            return Value;
        }
    }
    public class InstanceVariable : Base, IEnumerable<KeyValuePair<HashKey, Base>>
    {
        public Base Name { get; private set; }
        private Dictionary<HashKey, Base> _Elements { get; set; } = new();
        public Base this[HashKey s]
        {
            get => _Elements[s];
        }
        public Base this[string s]
        {
            get => _Elements[String.CreateTempObject(s)];
        }
        public int Count => _Elements.Count;
        internal InstanceVariable(Decoder ctx,BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br)!;
            var count = new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br);
                if (key != null)
                    _Elements.Add(key, value);
                else
                    throw new NotSupportedException(); 

            }
        }
        public override JsonNode? ToJson()
        {
            var obj = new JsonObject
            {
                { "class", Name.ToJson() }
            };
            foreach (var pair in _Elements)
                obj.Add(pair.Key.ToString()!, pair.Value?.ToJson());
            return obj;
        }

        public IEnumerator GetEnumerator() => _Elements.GetEnumerator();

        IEnumerator<KeyValuePair<HashKey, Base>> IEnumerable<KeyValuePair<HashKey, Base>>.GetEnumerator() => _Elements.GetEnumerator();
    }
    public class Extended : Base
    {
        public Symbol Name { get; private set; }
        internal Extended(Decoder ctx,BinaryReader br)
        {
            throw new NotImplementedException();
        }

        public override JsonNode? ToJson()
        {
            return Name.ToJson();
        }
    }
    public class Fixnum : HashKey
    {
        public long Value { get; private set; }

        internal Fixnum(BinaryReader br)
        {
            byte b = br.ReadByte();
            switch (b)
            {
                case 0:
                    Value= 0;
                    break; 
                case 1:
                    Value= br.ReadByte();
                    break;
                case 0xff:
                    Value= - br.ReadByte();
                    break;
                case 2:
                    Value= br.ReadUInt16();
                    break; 
                case 0xfe:
                    Value= -br.ReadUInt16();
                    break;
                case 3:
                    {
                        byte[] buffer = new byte[4];
                        br.Read(buffer, 0, 3);
                        Value= BitConverter.ToUInt32(buffer);
                        break;
                    }
                case 0xfd:
                    {
                        byte[] buffer = new byte[4];
                        br.Read(buffer, 0, 3);
                        Value= -BitConverter.ToUInt32(buffer);
                        break;
                    }
                case 4:
                    Value= br.ReadUInt32();
                    break;
                case 0xfc:
                    Value= - br.ReadUInt32();
                    break;
                default:
                    {
                        var temp = (sbyte)b;
                        if (temp > 0)
                            Value= temp - 5;
                        else
                            Value= temp + 5;
                        break;
                    }
            }
        }
        public int ToInt32()
        {
            if (Value > int.MaxValue || Value < int.MinValue)
                throw new InvalidCastException();
            return (int)Value;
        }
        public uint ToUInt32()
        {
            if (Value < 0 || Value > uint.MaxValue)
                throw new InvalidCastException();
            return (uint)Value;
        }
        public ushort ToUInt16()
        {
            if (Value < 0 || Value > ushort.MaxValue)
                throw new InvalidCastException();
            return (ushort)Value;
        }
        public short ToInt16()
        {
            if (Value > short.MaxValue || Value < short.MinValue)
                throw new InvalidCastException();
            return (short)Value;
        }
        public override string ToString() => Value.ToString();

        public override JsonNode? ToJson()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    public class String : HashKey
    {
        public string Value { get; private set; }
        private String(string str)
        {
            Value = str;
        }
        internal static String CreateTempObject(string s)
        {
            return new String(s);
        }
        internal String(Decoder ctx, BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            var strlen = new Fixnum(br).ToInt32();
            Value = Encoding.UTF8.GetString(br.ReadBytes(strlen));
        }
        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode();

        public override JsonNode? ToJson()
        {
            return Value;
        }
    }
    public class Float : Base
    {
        private string _Data;
        public double Value { get; private set; }
        internal Float(Decoder ctx, BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            var length = new Fixnum(br).ToInt32();
            var str = Encoding.ASCII.GetString(br.ReadBytes(length));
            int index = str.IndexOf('\0');
            _Data = index == -1 ? str : str.Substring(0, index);
            Value = double.Parse(_Data);
        }
        public override string ToString() => _Data;

        public override JsonNode ToJson()
        {
            return Value;
        }
    }
    public class Data : Base
    {
        internal Data(Decoder ctx,BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class Hash : Base,IEnumerable<KeyValuePair<HashKey, Base>>
    {
        private Dictionary<HashKey, Base> _Elements { get; set; } = new();

        public Base this[HashKey s]
        {
            get => _Elements[s];
        }

        public Base this[string s]
        {
            get => _Elements[String.CreateTempObject(s)];
        }
        internal Hash(Decoder ctx,BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            var count = new Fixnum(br).ToInt32();
            for(int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br);
                if (key != null)
                    _Elements.Add(key, value);
                else
                    throw new NotSupportedException();
            }
        }
        public override JsonNode? ToJson()
        {
            var obj = new JsonObject();
            foreach (var pair in _Elements)
                obj.Add(pair.Key.ToString(), pair.Value?.ToJson());
            return obj;
        }

        public IEnumerator<KeyValuePair<HashKey, Base>> GetEnumerator() => _Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Elements.GetEnumerator();
    }
    public class Regex : Base
    {
        internal Regex(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class Struct : Base
    {
        internal Struct(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class UserClass : Base
    {
        internal UserClass(Decoder ctx,BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class UserMarshal : Base
    {
        internal UserMarshal(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class DefaultHash : Base
    {
        internal DefaultHash(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class Module : Base
    {
        internal Module(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class Class : Base
    {
        internal Class(Decoder ctx, BinaryReader br)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    public class UserDefined : Base
    {
        public Base Name { get; private set; }
        public byte[] RawData { get; private set; }

        internal UserDefined(Decoder ctx,BinaryReader br)
        {
            ctx.ObjectList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br);
            RawData = br.ReadBytes(new Fixnum(br).ToInt32());
        }

        public override JsonNode? ToJson()
        {
            var obj = new JsonObject();
            obj["class"] = Name.ToString();
            var arr = new JsonArray();
            foreach (var b in RawData)
                arr.Add(b);
            obj["data"] = arr;
            return obj;
        }
    }
}
