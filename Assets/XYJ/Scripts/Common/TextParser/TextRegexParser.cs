using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Config;

namespace xys
{
    /// <summary>
    /// 文字规则解析
    /// </summary>
    public class TextRegexParser
    {
        private const string chineseRegular = "^[^\x00-\xFF]";
        private const string numberRegular = "^[0-9]";
        private const string characterRegular = @"[a-zA-Z]+";

        static Regex regChinese;
        static Regex regNumber;
        static Regex regCharacter;

        static ParserSymbol sensitiveRegex;

        static TextRegexParser()
        {
            regChinese = new Regex(chineseRegular);
            regNumber = new Regex(numberRegular);
            regCharacter = new Regex(characterRegular);

            //初始化屏蔽字的正则表达式
            Dictionary<int, SensitiveWords> sensitiveWords = SensitiveWords.GetAll();
            HashSet<string> keys = new HashSet<string>();
            sensitiveRegex = new ParserSymbol();
            foreach (var itor in sensitiveWords)
            {
                if (keys.Contains(itor.Value.desc))
                    continue;

                keys.Add(itor.Value.desc);
                sensitiveRegex.Add(itor.Value.desc, (string key, object obj) => { return new string('*', key.Length); });
            }
        }

        /// <summary>
        /// 是否包含屏蔽字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ContainsSensitiveWord(string text)
        {
            if (sensitiveRegex.To(text) != text)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 过滤屏蔽词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FilterSensitiveWord(string text)
        {
            return sensitiveRegex.To(text);
        }

        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckStringChineseReg(char text)
        {
            return regChinese.IsMatch(text.ToString());
        }

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckStringNumReg(char text)
        {
            return regNumber.IsMatch(text.ToString());
        }

        /// <summary>
        /// 判断是否是字母
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckStringCharacterReg(char text)
        {
            return regCharacter.IsMatch(text.ToString());
        }
    }
}