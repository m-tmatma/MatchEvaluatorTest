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
        static private string name_head_separater = @"\b";
        static private string name_tail_separater = @"\b";
        static private string name_guid_string1 = @"(?<raw_guid1>" + raw_guid_string1 + ")";
        static private string name_guid_string2 = @"(?<raw_guid2>" + raw_guid_string2 + ")";
        static private string guid_string = name_head_separater + name_guid_string1 + name_tail_separater;
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
                var key = string.Empty;
                if (m.Groups["raw_guid1"].Success)
                {
                    var guid = new Guid(m.Groups["raw_guid1"].Value);
                    this.GuidFormat = Format.RawHyphenDigits;
                    key = guid.ToString("D");
                }
                else if (m.Groups["raw_guid2"].Success)
                {
                    var guid = new Guid(m.Groups["raw_guid2"].Value);
                    this.GuidFormat = Format.RawHyphenDigits;
                    key = guid.ToString("D");
                }
                else
                {
                    this.GuidFormat = Format.Unknown;
                }
                this.Key = key;
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
            string[] array = new string[]
            {
                input1,
                input2,
                input3,
                input4,
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
