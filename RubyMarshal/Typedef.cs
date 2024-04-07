using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace RubyMarshal.Types
{
    public abstract class HashKey:Base
    {
        public abstract override int GetHashCode();
        public abstract object GetUniqueValue();
        public sealed override bool Equals(object? obj)
        {
            var other = obj as HashKey;
            if (other==null)
                return false;
            object uv1 = GetUniqueValue(), uv2 = other.GetUniqueValue();
            if (uv1.GetType() != uv2.GetType())
                return false;
            return uv1.Equals(uv2); 
        }
    }
    public abstract class Base:IJsonSerialzable
    {
        public String? AsString() => this as String;
        public Fixnum? AsFixnum() => this as Fixnum;
        public Float? AsFloat() => this as Float;
        public Symbol? AsSymbol() => this as Symbol;
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
        public Regexp? AsRegex() => this as Regexp;
        public UserClass? AsUserClass() => this as UserClass;
        public UserMarshal? AsUserMarshal() => this as UserMarshal;
        public DefaultHash? AsDefaultHash() => this as DefaultHash;
        public Module? AsModule() => this as Module;
        public Class? AsClass() => this as Class;
        public Struct? AsStruct() => this as Struct;

        public abstract JsonNode? ToJson();
    }
    [AddRef]
    public class Object :Base,IEnumerable<KeyValuePair<HashKey,Base>>
    {
        public bool ContainsKey(HashKey s) => _Elements.ContainsKey(s);
        public bool ContainsKey(string s) => _Elements.ContainsKey(new String(s));
        public Base Name { get; private set; }
        private Dictionary<HashKey, Base> _Elements { get; set; } = new();
        public Base this[HashKey s]
        {
            get => _Elements[s];
        }

        public Base this[string s]
        {
            get => _Elements[new String(s)];
        }

        public int Count => _Elements.Count;
        internal Object(Decoder ctx, BinaryReader br,bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true)!;
            var count =new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true);
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
                obj.Add(pair.Key.GetUniqueValue().ToString()!, pair.Value!.ToJson());
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
            Target = ctx.ObjectReferenceList[index];
        }

        public override JsonNode? ToJson()=> Target.ToJson();
    }
    [AddRef]
    public class Array : Base, IEnumerable<Base>
    {
        private List<Base> _Elements = new(1);
        public int Count => _Elements.Count;
        internal Array(Decoder ctx,BinaryReader br,bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var count = new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
                _Elements.Add(Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true));
        }

        public IEnumerator<Base> GetEnumerator() => _Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()=> _Elements.GetEnumerator();

        public override JsonNode ToJson()=> new JsonArray(_Elements.Select(ele => ele.ToJson()).ToArray());

        public Base this[int index]
        {
            get => _Elements[index];
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
        public override JsonNode? ToJson()=> Target.ToJson();

        public override int GetHashCode()=> Target.GetHashCode();

        public override object GetUniqueValue() => Target.GetUniqueValue();

    }
    public class Symbol : HashKey
    {
        public string Name { get; private set; }
        internal Symbol(Decoder ctx,BinaryReader br)
        {
            ctx?.SymbolList.Add(this);
            var strlen = new Fixnum(br).ToInt32();
            Name = Encoding.UTF8.GetString(br.ReadBytes(strlen));
        }
        public override int GetHashCode()=> Name.GetHashCode();
        public override JsonNode? ToJson() => Name;
        public override object GetUniqueValue() => Name;
    }
    public class Nil : Base
    {
        internal Nil(){}
        public override JsonNode? ToJson() => null;
    }
    [AddRef]
    public class Bignum : Base
    {
        public BigInteger Value { get; private set; }
        internal Bignum(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var sign = br.ReadByte();
            var length = new Fixnum(br).ToInt32()*2;
            var temp= new BigInteger(br.ReadBytes(length));
            if (sign == (byte)'+')
                Value = temp;
            else if (sign == (byte)'-')
                Value = -temp;
            else
                throw new NotSupportedException();
        }
        public override JsonNode? ToJson() => Value.ToString();
    }
    public class True : Base
    {
        public const bool Value = true;
        internal True(){}
        public override JsonNode? ToJson() => Value;
    }
    public class False : Base
    {
        public const bool Value = false;
        internal False() {}
        public override JsonNode? ToJson() => Value;
    }
    [AddRef]
    public class InstanceVariable: HashKey, IEnumerable<KeyValuePair<HashKey, Base>>
    {
        public bool ContainsKey(HashKey s) => _Properties.ContainsKey(s);
        public bool ContainsKey(string s) => _Properties.ContainsKey(new String(s));
        public Base Base { get; private set; }
        private Dictionary<HashKey, Base> _Properties { get; set; } = new();
        public Base this[HashKey s]
        {
            get => _Properties[s];
        }
        public Base this[string s]
        {
            get => _Properties[new String(s)];
        }
        public int Count => _Properties.Count;
        internal InstanceVariable(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            Base = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,false)!;
            var count = new Fixnum(br).ToInt32();
            for (int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true);
                if (key != null)
                    _Properties.Add(key, value);
                else
                    throw new NotSupportedException(); 

            }
        }
        public override JsonNode? ToJson()
        {
            var obj = new JsonObject()
            {
                ["base"] = Base.ToJson()
            };
            foreach (var item in _Properties)
                obj[item.Key.GetUniqueValue().ToString()!] = item.Value.ToJson();
            return obj;
        }

        public IEnumerator GetEnumerator() => _Properties.GetEnumerator();

        IEnumerator<KeyValuePair<HashKey, Base>> IEnumerable<KeyValuePair<HashKey, Base>>.GetEnumerator() => _Properties.GetEnumerator();

        public override int GetHashCode()=>((HashKey) Base).GetHashCode();

        public override object GetUniqueValue() => ((HashKey)Base).GetUniqueValue();
    }
    [AddRef]
    public class Extended : Base
    {
        public Base Base { get; private set; }
        public Base Value { get; private set; }
        internal Extended(Decoder ctx,BinaryReader br,bool addToObjectReferenceList)
        {
            if (addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this); 
            Base = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br, true);
            Value = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br, true);
        }

        public override JsonNode? ToJson()
        {
            return new JsonObject
            {
                ["base"]=Base.ToJson(),
                ["value"]=Value.ToJson()
            };
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

        public Fixnum(long val)
        {
            Value = val;
        }
        public override JsonNode? ToJson() => Value;
        public override int GetHashCode()=> Value.GetHashCode();
        public override object GetUniqueValue() => Value;
    }
    [AddRef]
    public class String : HashKey
    {
        public string Value { get; private set; }
        internal String(string str)
        {
            Value = str;
        }
        internal String(Decoder ctx, BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var strlen = new Fixnum(br).ToInt32();
            Value = Encoding.UTF8.GetString(br.ReadBytes(strlen));
        }
        
        public override int GetHashCode() => Value.GetHashCode();

        public override JsonNode? ToJson() => Value;

        public override object GetUniqueValue() => Value;
    }
    [AddRef]
    public class Float : Base
    {
        private string _Data;
        public double Value { get; private set; }
        internal Float(Decoder ctx, BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var length = new Fixnum(br).ToInt32();
            var str = Encoding.ASCII.GetString(br.ReadBytes(length));
            int index = str.IndexOf('\0');
            _Data = index == -1 ? str : str[0..index];
            Value = double.Parse(_Data);
        }

        public override JsonNode ToJson() => Value;
    }
    [AddRef]
    public class Data : Base
    {
        internal Data(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            throw new NotImplementedException();
        }
        public override JsonNode ToJson()
        {
            throw new NotImplementedException();
        }
    }
    [AddRef]
    public class Hash : Base,IEnumerable<KeyValuePair<HashKey, Base>>
    {
        private Dictionary<HashKey, Base> _Elements { get; set; } = new();
        public bool ContainsKey(HashKey s) => _Elements.ContainsKey(s);
        public bool ContainsKey(string s) => _Elements.ContainsKey(new String(s));
        public bool ContainsKey(int s) => _Elements.ContainsKey(new Fixnum(s));
        public virtual Base this[HashKey s]
        {
            get => _Elements[s];
        }

        public Base this[int index]
        {
            get => this[new Fixnum(index)];
        }

        public Base this[string s]
        {
            get => this[new String(s)];
        }
        internal Hash(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var count = new Fixnum(br).ToInt32();
            for(int i = 0; i < count; i++)
            {
                var key = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br,true) as HashKey;
                var value = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br,true);
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
                obj.Add(pair.Key.GetUniqueValue().ToString()!, pair.Value?.ToJson());
            return obj;
        }

        public IEnumerator<KeyValuePair<HashKey, Base>> GetEnumerator() => _Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Elements.GetEnumerator();
    }
    [AddRef]
    public class Regexp : Base
    {
        public Regex Value { get; private set; }
        internal Regexp(Decoder ctx, BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var strlen = new Fixnum(br).ToInt32();
            var pattern = Encoding.UTF8.GetString(br.ReadBytes(strlen));
            var options = RegexOptions.None;
            int b = br.ReadByte();
            if ((b & 1) != 0)
                options |= RegexOptions.IgnoreCase;
            if ((b & 2) != 0)
                options |= RegexOptions.IgnorePatternWhitespace;
            if ((b & 4) != 0)
                options |= RegexOptions.Multiline;
            Value = new Regex(pattern, options);     
        }
        public override JsonNode? ToJson()
        {
            StringBuilder sb = new StringBuilder($"/{Value}/");
            var options = Value.Options;
            if (options.HasFlag(RegexOptions.Multiline))
                sb.Append('m');
            if (options.HasFlag(RegexOptions.IgnoreCase))
                sb.Append('i');
            if (options.HasFlag(RegexOptions.IgnorePatternWhitespace))
                sb.Append('x');
            return sb.ToString();
        }
    }
    [AddRef]
    public class Struct : Object
    {
        internal Struct(Decoder ctx, BinaryReader br, bool addToObjectReferenceList) : base(ctx, br,addToObjectReferenceList) { }
    }
    [AddRef]
    public class UserClass : Base
    {
        public Base Name { get; private set; }
        public Base Base { get; private set; }
        internal UserClass(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br,true);
            Base= Decoder.ReaderMap[br.ReadByte()].Read(ctx, br,true);
        }

        public override JsonNode? ToJson()
        {
            return new JsonObject()
            {
                ["class"] = Name.ToJson(),
                ["base"] = Base.ToJson()
            };
        }
    }
    [AddRef]
    public class UserMarshal : Base
    {
        internal UserMarshal(Decoder ctx, BinaryReader br, bool addToObjectReferenceList)
        {
            throw new NotImplementedException();
        }
        public override JsonNode? ToJson()
        {
            throw new NotImplementedException();
        }
    }
    [AddRef]
    public class DefaultHash : Hash
    {
        public Base DefaultValue { get; private set; }
        internal DefaultHash(Decoder ctx, BinaryReader br, bool addToObjectReferenceList) :base(ctx,br,addToObjectReferenceList)
        {
            DefaultValue = Decoder.ReaderMap[br.ReadByte()].Read(ctx,br,true);
        }
        public sealed override Base this[HashKey s]
        {
            get
            {
                if (!ContainsKey(s))
                    return DefaultValue;
                else
                    return base[s];
            }
        }
    }
    [AddRef]
    public class Module : Class
    {
        internal Module(Decoder ctx, BinaryReader br, bool addToObjectReferenceList) : base(ctx, br,addToObjectReferenceList){}
    }
    [AddRef]
    public class Class : Base
    {
        public string Name { get; private set; }
        internal Class(Decoder ctx, BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            var strlen = new Fixnum(br).ToInt32();
            Name = Encoding.UTF8.GetString(br.ReadBytes(strlen));
        }
        public override JsonNode? ToJson()
        {
            return new JsonObject()
            {
                ["class"] = Name
            };
        }
    }
    [AddRef]
    public class UserDefined : Base
    {
        public Base Name { get; private set; }
        public byte[] RawData { get; private set; }

        internal UserDefined(Decoder ctx,BinaryReader br, bool addToObjectReferenceList)
        {
            if(addToObjectReferenceList)
                ctx.ObjectReferenceList.Add(this);
            Name = Decoder.ReaderMap[br.ReadByte()].Read(ctx, br,true);
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
