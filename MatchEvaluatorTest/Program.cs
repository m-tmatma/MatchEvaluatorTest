using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MatchEvaluatorTest
{
    class Program
    {
        /// <summary>
        /// https://msdn.microsoft.com/ja-jp/library/system.text.regularexpressions.matchevaluator(v=vs.95).aspx
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string input1 = @"[Guid(""f86ed29d-8060-485f-acf2-93716ca463b8"")]";
            string input2 = @"[Guid(""f86ed29d-8060-485f-acf2-93716ca463b8"")]";
            string input3 = @"""f86ed29d-8060-485f-acf2-93716ca463b8""";
            string input4 = @"{f86ed29d-8060-485f-acf2-93716ca463b8}";
            string input5 = @"{f86ed29d8060485facf293716ca463b8}";
            string input6 = @"f86ed29d8060485facf293716ca463b8";
            string input7 = @"F86ED29D8060485FACF293716CA463B8";
            string input8 = @"{0xf86ed29d, 0x8060, 0x485f,{0xac,0xf2,0x93,0x71,0x6c,0xa4,0x63,0xb8}}";
            string input9 = @"{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}";
            string input10 = @"{0x00,0x00,0x00,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}";
            string input11 = @"IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x00000000,0x0000,0x0000,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);";
            string input12 = @"DEFINE_GUID(<<name>>, 0x00000000,0x0000,0x0000,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);";
            string[] array = new string[]
            {
                input1,
                input2,
                input3,
                input4,
                input5,
                input6,
                input7,
                input8,
                input9,
                input10,
                input11,
                input12,
            };
            string input = string.Join(Environment.NewLine, array);
            Console.WriteLine("Original");
            Console.WriteLine(input);
            Console.WriteLine("");

            var replaceWithNewGuid = new ReplaceWithNewGuid();

            // Replace matched characters using the delegate method.
            var output = replaceWithNewGuid.Replace(input);
            Console.WriteLine("New All");
            Console.WriteLine(output);
            //testReg.Dump();
            Console.WriteLine("");

            // Replace matched characters using the delegate method.
            var output2 = replaceWithNewGuid.Replace2(input);
            Console.WriteLine("New for different ones");
            Console.WriteLine(output2);
            //testReg.Dump();
            Console.WriteLine("");
        }
    }
}
