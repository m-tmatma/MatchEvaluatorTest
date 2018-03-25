using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MatchEvaluatorTest
{
    public class TestReg
    {
        private Dictionary<string, Guid> dict = new Dictionary<string, Guid>();
        static private string raw_guid_string1 = "([0-9A-Fa-f]{8})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{12})";
        static private string raw_guid_string2 = "([0-9A-Fa-f]{32})";
        //static private string raw_head_separater = @"[{""]";
        //static private string raw_tail_separater = @"[}""]";

        // For reference
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/regular-expression-language-quick-reference#backreference_constructs
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/backreference-constructs-in-regular-expressions
        //static private string name_head_separater = @"(?:" + raw_head_separater + ")";
        //static private string name_tail_separater = @"(?:" + raw_tail_separater + ")";
        static private string word_separater = @"\b";
        static private string name_guid_string1 = @"(?<RawHyphenDigits>" + raw_guid_string1 + ")";
        static private string name_guid_string2 = @"(?<Raw32Digits>"     + raw_guid_string2 + ")";
        static private string guid_string1 = word_separater + name_guid_string1 + word_separater;
        static private string guid_string2 = word_separater + name_guid_string2 + word_separater;

        static private string[] elements = new string[] { guid_string1, guid_string2 };
        static private string[] elements_par = Array.ConvertAll(elements, delegate (string elem) { return "(" + elem + ")"; });
        static private string guid_string = string.Join("|", elements_par);
        static private Regex reg = new Regex(guid_string);

        private class ProcessGuid
        {
            /// <summary>
            /// https://msdn.microsoft.com/ja-jp/library/97af8hh4(v=vs.110).aspx
            /// </summary>
            public enum Format
            {
                Unknown,

                /// <summary>
                /// "D": 00000000-0000-0000-0000-000000000000
                /// </summary>
                RawHyphenDigits,

                /// <summary>
                /// "N": 00000000000000000000000000000000
                /// </summary>
                Raw32Digits,
            };

            private class MapFormat
            {
                public string Key;
                public Format GuidFormat;

                public MapFormat(string Key, Format GuidFormat)
                {
                    this.Key = Key;
                    this.GuidFormat = GuidFormat;
                }
            };
            static private MapFormat[] tableFormats = new MapFormat[] {
                new MapFormat("RawHyphenDigits", Format.RawHyphenDigits),
                new MapFormat("Raw32Digits", Format.Raw32Digits),
            };

            private Match m;

            /// <summary>
            /// key for Guid dictionary
            /// </summary>
            public string Key { get; private set; }

            /// <summary>
            /// guid format
            /// </summary>
            public Format GuidFormat { get; private set; }

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="m"></param>
            public ProcessGuid(Match m)
            {
                this.m = m;

                this.Key = string.Empty;
                this.GuidFormat = Format.Unknown;
                foreach (MapFormat mapFormat in tableFormats)
                {
                    if (m.Groups[mapFormat.Key].Success)
                    {
                        var guid = new Guid(m.Groups[mapFormat.Key].Value);
                        this.Key =  guid.ToString("D");
                        this.GuidFormat = mapFormat.GuidFormat;
                        break;
                    }
                }
#if OLDCODE
                var key = string.Empty;
                if (m.Groups["RawHyphenDigits"].Success)
                {
                    key = new Guid(m.Groups["RawHyphenDigits"].Value).ToString("D");
                    this.GuidFormat = Format.RawHyphenDigits;
                }
                else if (m.Groups["Raw32Digits"].Success)
                {
                    key = new Guid(m.Groups["Raw32Digits"].Value).ToString("D");
                    this.GuidFormat = Format.RawHyphenDigits;
                }
                else
                {
                    this.GuidFormat = Format.Unknown;
                }
                this.Key = key;
#endif
            }

            public string Convert(Guid guid)
            {
                switch(this.GuidFormat)
                {
                    case Format.Unknown:
                        break;
                    case Format.RawHyphenDigits:
                        return guid.ToString("D");
                    case Format.Raw32Digits:
                        return guid.ToString("N");
                }
                return string.Empty;
            }
        }

        public string ReplaceCC(Match m)
        {
            var processGuid = new ProcessGuid(m);
            var newGuid = Guid.NewGuid();

            var guid_str = processGuid.Convert(newGuid);
            return guid_str;

        }

        public string ReplaceCC2(Match m)
        {
            var processGuid = new ProcessGuid(m);
            var key = processGuid.Key;
            var guid = new Guid(key);
            if (!dict.ContainsKey(key))
            {
                dict[key] = Guid.NewGuid();
            }
            var newGuid = dict[key];

            var guid_str = processGuid.Convert(newGuid);
            return guid_str;
        }
 
        public string Replace(string input)
        {
            Console.WriteLine(guid_string);
            var myEvaluator = new MatchEvaluator(ReplaceCC);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }
        public string Replace2(string input)
        {
            Console.WriteLine(guid_string);
            var myEvaluator = new MatchEvaluator(ReplaceCC2);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }

        public void Dump()
        {
            foreach (KeyValuePair<string, Guid> keyvalue in dict)
            {
                Console.WriteLine("{0}:{1}", keyvalue.Key, keyvalue.Value.ToString());
            }
        }
    }

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
            string[] array = new string[]
            {
                input1,
                input2,
                input3,
                input4,
                input5,
                input6,
                input7,
            };
            string input = string.Join(Environment.NewLine, array);
            Console.WriteLine("Original");
            Console.WriteLine(input);
            Console.WriteLine("");

            var testReg = new TestReg();

            // Replace matched characters using the delegate method.
            var output = testReg.Replace(input);
            Console.WriteLine("New All");
            Console.WriteLine(output);
            //testReg.Dump();
            Console.WriteLine("");

            // Replace matched characters using the delegate method.
            var output2 = testReg.Replace2(input);
            Console.WriteLine("New for different ones");
            Console.WriteLine(output2);
            //testReg.Dump();
            Console.WriteLine("");
        }
    }
}
