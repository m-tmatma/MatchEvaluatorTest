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
    }

    class Program
    {
        /// <summary>
        /// https://msdn.microsoft.com/ja-jp/library/system.text.regularexpressions.matchevaluator(v=vs.95).aspx
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string regex = @"Guid\(""([0-9A-Fa-f]{8})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{12})""\)";
            string input1 = @"[Guid(""f86ed29d-8060-485f-acf2-93716ca463b8"")]";
            string input2 = @"[Guid(""f86ed29d-8060-485f-acf2-93716ca463b8"")]";
            string input = input1 + input2;
            Console.WriteLine(input);

            var reg = new Regex(regex);
            var testReg = new TestReg();

            // Assign the replace method to the MatchEvaluator delegate.
            MatchEvaluator myEvaluator = new MatchEvaluator(testReg.ReplaceCC);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            Console.WriteLine(output);

            // Assign the replace method to the MatchEvaluator delegate.
            MatchEvaluator myEvaluator2 = new MatchEvaluator(testReg.ReplaceCC2);

            // Replace matched characters using the delegate method.
            var output2 = reg.Replace(input, myEvaluator2);
            Console.WriteLine(output2);
        }
    }
}
