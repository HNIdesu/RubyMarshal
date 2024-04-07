using RubyMarshal;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var decoder = new Decoder();
            var obj = decoder.Decode(File.OpenRead(@"E:\Documents\VSCode\Code\Ruby\Test\Test\test.bin"));
            Console.WriteLine(obj.ToJson());
        }
    }
}
