using RubyMarshal.Types;

namespace RubyMarshal.Reader
{
    public abstract class BaseReader
    {
        public abstract Base Read(Decoder ctx,BinaryReader br);
    }
    public class ArrayReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Types.Array(ctx,br);
    }
    public class HashReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Hash(ctx,br);
    }
    public class ObjectReferencesReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) =>new ObjectReferences(ctx,br);
    }
    public class NilReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Nil(br);
    }
    public class ObjectReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Types.Object(ctx,br);
    }
    public class SymbolReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br)=> new Symbol(ctx,br);
    }
    public class SymbolLinkReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new SymbolLink(ctx,br);
    }
    public class FixnumReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Fixnum(br);
    }
    public class StringReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Types.String(ctx,br);
    }
    public class BignumReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Bignum(ctx, br);
    }
    public class InstanceVariableReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new InstanceVariable(ctx,br);
    }
    public class TrueReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new True(br);
    }
    public class DataReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Data(ctx,br);
    }
    public class FalseReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new False(br);
    }
    public class FloatReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Float(ctx,br);
    }
    public class ExtendedReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br) => new Extended(ctx,br);
    }
    public class UserDefinedReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br)=>new UserDefined(ctx, br);
    }

    public class RegexReader: BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Regex(ctx, br);
    }
    public class StructReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Struct(ctx, br);
    }

    public class UserClassReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new UserClass(ctx, br);
    }
    public class UserMarshalReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new UserMarshal(ctx,br);
    }
    public class DefaultHashReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new DefaultHash(ctx, br);
    }
    public class ModuleReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Module(ctx, br);
    }
    public class ClassReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br) => new Class(ctx, br);
    }

}
