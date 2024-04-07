using RubyMarshal.Types;

namespace RubyMarshal.Reader
{
    public abstract class BaseReader
    {
        public abstract Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList);
    }
    public class ArrayReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Types.Array(ctx,br,addToObjectReferenceList);
    }
    public class HashReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Hash(ctx,br,addToObjectReferenceList);
    }
    public class ObjectReferencesReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) =>new ObjectReferences(ctx!,br).Target;
    }
    public class NilReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Nil();
    }
    public class ObjectReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Types.Object(ctx,br,addToObjectReferenceList);
    }
    public class SymbolReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Symbol(ctx,br);
    }
    public class SymbolLinkReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new SymbolLink(ctx!,br).Target;
    }
    public class FixnumReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Fixnum(br);
    }
    public class StringReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Types.String(ctx,br,addToObjectReferenceList);
    }
    public class BignumReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Bignum(ctx, br,addToObjectReferenceList);
    }
    public class InstanceVariableReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new InstanceVariable(ctx,br,addToObjectReferenceList);
    }
    public class TrueReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new True();
    }
    public class DataReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Data(ctx,br,addToObjectReferenceList);
    }
    public class FalseReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new False();
    }
    public class FloatReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Float(ctx,br,addToObjectReferenceList);
    }
    public class ExtendedReader : BaseReader
    {
        public override Base Read(Decoder ctx,BinaryReader br, bool addToObjectReferenceList) => new Extended(ctx,br,addToObjectReferenceList);
    }
    public class UserDefinedReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList)=>new UserDefined(ctx, br,addToObjectReferenceList);
    }
    public class RegexpReader: BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Regexp(ctx, br,addToObjectReferenceList);
    }
    public class StructReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Struct(ctx, br,addToObjectReferenceList);
    }
    public class UserClassReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new UserClass(ctx, br,addToObjectReferenceList);
    }
    public class UserMarshalReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new UserMarshal(ctx,br,addToObjectReferenceList);
    }
    public class DefaultHashReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new DefaultHash(ctx, br,addToObjectReferenceList);
    }
    public class ModuleReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Module(ctx, br,addToObjectReferenceList);
    }
    public class ClassReader : BaseReader
    {
        public override Base Read(Decoder ctx, BinaryReader br,bool addToObjectReferenceList) => new Class(ctx, br,addToObjectReferenceList);
    }

}
