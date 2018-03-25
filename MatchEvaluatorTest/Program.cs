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
        static private string raw_head_separater = @"[{""]";
        static private string raw_tail_separater = @"[}""]";

        // For reference
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/regular-expression-language-quick-reference#backreference_constructs
        // https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/backreference-constructs-in-regular-expressions
        static private string name_head_separater = @"(?<head_sep>" + raw_head_separater + ")";
        static private string name_tail_separater = @"(?<tail_sep>" + raw_tail_separater + ")";
        static private string name_guid_string1 = @"(?<raw_guid>" + raw_guid_string1 + ")";
        static private string guid_string = name_head_separater + name_guid_string1 + name_tail_separater;
        static private Regex reg = new Regex(guid_string);
 
        private string GetGuidString(Match m)
        {
            return m.Groups["raw_guid"].Value;
        }

        public string ReplaceCC(Match m)
        {
            var newGuid = Guid.NewGuid();

            if (m.Groups["head_sep"].Success && m.Groups["tail_sep"].Success)
            {
                var head_sep = m.Groups["head_sep"].Value;
                var tail_sep = m.Groups["tail_sep"].Value;
                var guid_str = newGuid.ToString("D");
                return head_sep + guid_str + tail_sep;
            }
            else
            {
                return m.ToString();
            }
       }

        public string ReplaceCC2(Match m)
        {
            var key = GetGuidString(m);
            var guid = new Guid(key);
            if (!dict.ContainsKey(key))
            {
                dict[key] = Guid.NewGuid();
            }
            var newGuid = dict[key];
            if (m.Groups["head_sep"].Success && m.Groups["tail_sep"].Success)
            {
                var head_sep = m.Groups["head_sep"].Value;
                var tail_sep = m.Groups["tail_sep"].Value;
                var guid_str = newGuid.ToString("D");
                return head_sep + guid_str + tail_sep;
            }
            else
            {
                return m.ToString();
            }
        }
 
        public string Replace(string input)
        {
            var myEvaluator = new MatchEvaluator(ReplaceCC);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }
        public string Replace2(string input)
        {
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
