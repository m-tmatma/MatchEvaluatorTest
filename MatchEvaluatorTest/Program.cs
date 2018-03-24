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
        static private string regex = @"Guid\(""([0-9A-Fa-f]{8})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{12})""\)";
        static private Regex reg = new Regex(regex);

        private string GetGuidString(Match m)
        {
            var builder = new StringBuilder();
            for (int i = 1; i < m.Groups.Count; i++)
            {
                builder.Append(m.Groups[i].Value);
            }
            return builder.ToString();
        }

        public string ReplaceCC(Match m)
        {
            var key = GetGuidString(m);
            Guid guid = new Guid(key);
            Guid newGuid = Guid.NewGuid();
            string x = m.ToString();
            string y = string.Format(@"Guid(""{0}"")", newGuid.ToString("D"));
            return y;
        }

        public string ReplaceCC2(Match m)
        {
            var key = GetGuidString(m);
            Guid guid = new Guid(key);
            if (!dict.ContainsKey(key))
            {
                dict[key] = Guid.NewGuid();
            }
            Guid newGuid = dict[key];
            string y = string.Format(@"Guid(""{0}"")", newGuid.ToString("D"));
            return y;
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
            string input = input1 + input2;
            Console.WriteLine(input);

            var testReg = new TestReg();

            // Replace matched characters using the delegate method.
            var output = testReg.Replace(input);
            Console.WriteLine(output);

            // Replace matched characters using the delegate method.
            var output2 = testReg.Replace(input);
            Console.WriteLine(output2);
        }
    }
}
