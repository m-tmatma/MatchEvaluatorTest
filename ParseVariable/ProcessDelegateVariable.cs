using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParseVariable
{
    public class ProcessDelegateVariable
    {
        const string regexStrLeftSep = @"(?<={)";
        const string regexStrkeyword = @"(?<keyword>\w+)";
        const string regexStrIndex = @"(?<index>\d+)";
        const string regexStrRightSep = @"(?=})";
        const string regexStr = regexStrLeftSep + regexStrkeyword + @"(" + @"\(" + regexStrIndex + @"\)" + @")?" + regexStrRightSep;
        static private Regex reg = new Regex(regexStr);

        public delegate string GetNewText(string keyword, int index);

        /// <summary>
        /// class to provide a delegate of MatchEvaluator
        /// </summary>
        internal class MatchEvaluatorHandler
        {
            private Dictionary<string, GetNewText> m_translationTable;

            public MatchEvaluatorHandler(Dictionary<string, GetNewText> translationTable)
            {
                this.m_translationTable = translationTable;
            }

            /// <summary>
            /// delegate for ReplaceVariable
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            public string delegateReplace(Match m)
            {
                if (!m.Groups["keyword"].Success)
                {
                    return m.Groups[0].Value;
                }

                // get matched keyword
                var keyword = m.Groups["keyword"].Value;
                var outData = keyword;
                var index = -1;

                if (m.Groups["index"].Success)
                {
                    // with index
                    index = int.Parse(m.Groups["index"].Value);
                }

                if (this.m_translationTable.ContainsKey(keyword))
                {
                    outData = this.m_translationTable[keyword](keyword, index);
                }
                return outData;
            }
        }

        /// <summary>
        /// public interface to replace keyword
        /// </summary>
        /// <param name="input"></param>
        /// <param name="translationTable"></param>
        /// <returns></returns>
        public static string ReplaceVariable(string input, Dictionary<string, GetNewText> translationTable)
        {
            var evaluateHander = new MatchEvaluatorHandler(translationTable);
            var myEvaluator = new MatchEvaluator(evaluateHander.delegateReplace);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }
    }
}
