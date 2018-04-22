using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParseVariable
{
    public class ProcessVariable
    {
        const string regexStrLeftSep = @"(?<={)";
        const string regexStrkeyword = @"(?<keyword>\w+)";
        const string regexStrIndex = @"(?<index>\d+)";
        const string regexStrRightSep = @"(?=})";
        const string regexStr = regexStrLeftSep + regexStrkeyword + @"(" + @"\(" + regexStrIndex + @"\)" + @")?" + regexStrRightSep;
        static private Regex reg = new Regex(regexStr);

        /// <summary>
        /// class to provide a delegate of MatchEvaluator
        /// </summary>
        internal class MatchEvaluatorHandler
        {
            private Dictionary<string, string> m_translationTable;

            public MatchEvaluatorHandler(Dictionary<string, string> translationTable)
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
                var groupKeyword = m.Groups["keyword"];
                var groupIndex   = m.Groups["index"];
                if (!groupKeyword.Success)
                {
                    return m.Groups[0].Value;
                }

                // get matched keyword
                var keyword = groupKeyword.Value;

                // replace a keyword with newer one.
                if (this.m_translationTable.ContainsKey(keyword))
                {
                    keyword = this.m_translationTable[keyword];
                }

                if (groupIndex.Success)
                {
                    // with index
                    var index = groupIndex.Value;
                    return keyword + "(" + index + ")";
                }
                else
                {
                    // without index
                    return keyword;
                }
            }
        }

        /// <summary>
        /// public interface to replace keyword
        /// </summary>
        /// <param name="input"></param>
        /// <param name="translationTable"></param>
        /// <returns></returns>
        public static string ReplaceVariable(string input, Dictionary<string, string> translationTable)
        {
            var evaluateHander = new MatchEvaluatorHandler(translationTable);
            var myEvaluator = new MatchEvaluator(evaluateHander.delegateReplace);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }
    }
}
