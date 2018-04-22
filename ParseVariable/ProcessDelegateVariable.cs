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
            private GetNewText delegateTranslate;

            public MatchEvaluatorHandler(GetNewText delegateTranslate)
            {
                this.delegateTranslate = delegateTranslate;
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
                var outData = keyword;
                var index = -1;

                if (groupIndex.Success)
                {
                    // with index
                    index = int.Parse(groupIndex.Value);
                }

                outData = this.delegateTranslate(keyword, index);
                return outData;
            }
        }

        /// <summary>
        /// public interface to replace keyword
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delegateTranslate"></param>
        /// <returns></returns>
        public static string ReplaceVariable(string input, GetNewText delegateTranslate)
        {
            var evaluateHander = new MatchEvaluatorHandler(delegateTranslate);
            var myEvaluator = new MatchEvaluator(evaluateHander.delegateReplace);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }
    }
}
