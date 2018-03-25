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

        /// <summary>
        /// 00000000-0000-0000-0000-000000000000
        /// </summary>
        static private string raw_guid_string1 = "([0-9A-Fa-f]{8})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{12})";

        /// <summary>
        /// 00000000000000000000000000000000
        /// </summary>
        static private string raw_guid_string2 = "([0-9A-Fa-f]{32})";

        /// <summary>
        /// 0x00000000
        /// </summary>
        static private string hex_4byte_string = "(0[xX][0-9A-Fa-f]{1,8})";

        /// <summary>
        /// 0x0000
        /// </summary>
        static private string hex_2byte_string = "(0[xX][0-9A-Fa-f]{1,4})";

        /// <summary>
        /// 0x00
        /// </summary>
        static private string hex_1byte_string = "(0[xX][0-9A-Fa-f]{1,2})";
        static private string spaces = @"\s*";
        static private string comma_spaces = spaces + "," + spaces;
        //static private string raw_head_separater = @"[{""]";
        //static private string raw_tail_separater = @"[}""]";

        /// <summary>
        /// 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        /// </summary>
        static private string hex_8_of_1byte = hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string;

        /// <summary>
        /// {0x00000000, 0x0000, 0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
        /// </summary>
        static private string raw_string_array = "{"
                                               + spaces
                                               + hex_4byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + "{"
                                               + spaces
                                               + hex_8_of_1byte
                                               + spaces
                                               + "}"
                                               + spaces
                                               + "}";

        // For reference
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/regular-expression-language-quick-reference#backreference_constructs
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/backreference-constructs-in-regular-expressions
        //static private string name_head_separater = @"(?:" + raw_head_separater + ")";
        //static private string name_tail_separater = @"(?:" + raw_tail_separater + ")";
        static private string word_separater = @"\b";
        static private string name_guid_string1 = @"(?<RawHyphenDigits>" + raw_guid_string1 + ")";
        static private string name_guid_string2 = @"(?<Raw32Digits>"     + raw_guid_string2 + ")";
        static private string name_guid_string3 = @"(?<GuidVariable>"    + raw_string_array + ")";
        static private string guid_string1 = word_separater + name_guid_string1 + word_separater;
        static private string guid_string2 = word_separater + name_guid_string2 + word_separater;
        static private string guid_string3 = word_separater + name_guid_string3 + word_separater;
        static private string[] elements = new string[]
        {
            guid_string1,
            guid_string2,
            name_guid_string3,
        };
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

                /// <summary>
                /// "X": {0x00000000, 0x0000, 0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
                /// </summary>
                GuidVariable,
            };

            /// <summary>
            /// delegate for converting a guid to a string
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            delegate string ConvertGuid(Guid guid);

            /// <summary>
            /// class for converting GUID
            /// </summary>
            private class MapFormat
            {
                public string Key { get; private set; }
                public Format GuidFormat { get; private set; }
                public ConvertGuid delegateConvertGuid { get; private set; }

                public MapFormat(string Key, Format GuidFormat, ConvertGuid delegateConvertGuid)
                {
                    this.Key = Key;
                    this.GuidFormat = GuidFormat;
                    this.delegateConvertGuid = delegateConvertGuid;
                }
            };
            static private MapFormat[] tableFormats = new MapFormat[] {
                new MapFormat("RawHyphenDigits", Format.RawHyphenDigits, delegate (Guid guid) { return guid.ToString("D"); }),
                new MapFormat("Raw32Digits",     Format.Raw32Digits,     delegate (Guid guid) { return guid.ToString("N"); }),
                new MapFormat("GuidVariable",    Format.GuidVariable,    delegate (Guid guid) { return guid.ToString("X"); }),
            };

            private Match m;

            private MapFormat mapFormat;

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
                this.mapFormat = null;
                this.GuidFormat = Format.Unknown;
                foreach (MapFormat mapFormat in tableFormats)
                {
                    if (m.Groups[mapFormat.Key].Success)
                    {
                        var guid = new Guid(m.Groups[mapFormat.Key].Value);
                        this.Key =  guid.ToString("D");
                        this.mapFormat = mapFormat;
                        this.GuidFormat = mapFormat.GuidFormat;
                        break;
                    }
                }
            }

            /// <summary>
            /// Format the guid in the original format
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public string Convert(Guid guid)
            {
                if (this.mapFormat != null)
                {
                    return this.mapFormat.delegateConvertGuid(guid);
                }

                // return original string
                return this.m.ToString();
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
            string input8 = @"{0xf86ed29d, 0x8060, 0x485f,{0xac,0xf2,0x93,0x71,0x6c,0xa4,0x63,0xb8}}";
            string input9 = @"{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}";
            string input10 = @"{0x00,0x00,0x00,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}";
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
