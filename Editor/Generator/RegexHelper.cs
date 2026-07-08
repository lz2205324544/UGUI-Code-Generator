using System.Collections.Generic;

namespace UGUICodeGenerator
{
    public class RegexHelper
    {
        /// <summary>
        /// 获取匹配(xxx)的标记
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<string> GetMarksFromMatchParentheses(string name)
        {
            List<string> matchesNames = new();
            var matches = CodeConst.ParenthesesRegex.Matches(name);
            for (int i = 0; i < matches.Count; i++)
            {
                matchesNames.Add(matches[i].Groups[1].Value);
            }
            return matchesNames;
        }
        
        /// <summary>
        /// 获取匹配尖括号的标记
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetMarksFromMatchAngleBracketsRegex(string name)
        {
            var match = CodeConst.AngleBracketsRegex.Match(name);
            if (match.Success)
                return match.Groups[1].Value;
            return string.Empty;
        }
    }
}