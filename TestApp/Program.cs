namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach(var file in Directory.EnumerateFiles(@"E:\Documents\RPGMAKER\RPGVXACE\Project1\Data", "*.rvdata2", SearchOption.TopDirectoryOnly))
            {
                RubyMarshal.Decoder.Decode(File.OpenRead(file));
            }
        }
    }
}
