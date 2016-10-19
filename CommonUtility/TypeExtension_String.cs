using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtility
{

    public static partial class TypeExtension_String
    {
        #region 字符串 转 类型
        /// <summary>
        /// 输出为yyyy-MM-dd hh:mm:ss格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static String ToYMD_HMS(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static String ToYMD_HMSM(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss." + dt.Millisecond.ToString());
        }

        public static String ToYMD(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static DateTime ToDateTime(this String str, DateTime? defaultValueIfFail = null)
        {
            if (defaultValueIfFail.HasValue)
            {
                DateTime dt = DateTime.MinValue;
                return DateTime.TryParse(str, out dt) ? dt : defaultValueIfFail.Value;
            }

            return DateTime.Parse(str);
        }

        public static Byte ToByte(this String str, byte? defaultValueIfFail = null)
        {
            byte rst = 0;
            if (defaultValueIfFail.HasValue)
            {
                return byte.TryParse(str, out rst) ? rst : defaultValueIfFail.Value;
            }

            return byte.Parse(str);
        }

        public static int ToInt(this String str, int? defaultValueIfFail = null)
        {
            int rst = 0;
            if (str.Contains(".")) str = str.Mid("", ".", false);
            if (defaultValueIfFail.HasValue)
            {
                return int.TryParse(str, out rst) ? rst : defaultValueIfFail.Value;
            }

            return int.Parse(str);
        }

        public static long ToLong(this String str, long? defaultValueIfFail = null)
        {
            long rst = 0;
            if (str.Contains(".")) str = str.Mid("", ".", false);
            if (defaultValueIfFail.HasValue)
            {
                return long.TryParse(str, out rst) ? rst : defaultValueIfFail.Value;
            }

            return long.Parse(str);
        }

        public static Double ToDouble(this String str, Double? defaultValueIfFail = null, int postfixCount = -1)
        {
            Double rst = 0.0;
            if (defaultValueIfFail.HasValue)
            {
                if (!Double.TryParse(str, out rst))
                {
                    rst = defaultValueIfFail.Value;
                }
            }
            else
            {
                rst = Double.Parse(str);
            }

            if (postfixCount < 0)
            {
                return rst;
            }
            else
            {
                return Math.Round(rst, postfixCount);
            }
        }

        public static Decimal ToDecimal(this String str, Decimal? defaultValueIfFail = null, int postfixCount = -1)
        {
            Decimal rst = new Decimal(0.0);
            if (defaultValueIfFail.HasValue)
            {
                if (!Decimal.TryParse(str, out rst))
                {
                    rst = defaultValueIfFail.Value;
                }
            }
            else
            {
                rst = Decimal.Parse(str);
            }

            if (postfixCount < 0)
            {
                return rst;
            }
            else
            {
                return Math.Round(rst, postfixCount);
            }
        }

        public static Boolean ToBoolean(this String str, bool? defaultValueIfFail = null)
        {
            bool b = false;
            if (defaultValueIfFail.HasValue)
            {
                return bool.TryParse(str, out b) ? b : defaultValueIfFail.Value;
            }
            return bool.Parse(str);
        }
        #endregion

        #region 取子字符串
        public static String Mid(this String str, string startTag, string endTag, bool includeTag = false)
        {
            if (!str.Contains(startTag)) return "";

            if (includeTag)
            {
                str = str.Substring(str.IndexOf(startTag));
                if (endTag != null && str.IndexOf(endTag) >= 0)
                    str = str.Substring(0, str.IndexOf(endTag) + endTag.Length);
            }
            else
            {
                str = str.Substring(str.IndexOf(startTag) + startTag.Length);
                if (endTag != null && str.IndexOf(endTag) >= 0)
                    str = str.Substring(0, str.IndexOf(endTag));
            }
            return str;
        }

        public static String Mid(this String str, string xmlTag_withoutBracket, bool includeTag = false)
        {
            string startTag = "<" + xmlTag_withoutBracket + ">";
            string endTag = "</" + xmlTag_withoutBracket + ">";

            return str.Mid(startTag, endTag, includeTag);
        }

        #endregion

        #region 字符串操作
        public static bool Contains_OneOf(this String str, string[] tags, bool ignoreCase = false)
        {
            if (tags != null)
            {
                foreach (var t in tags)
                {
                    if (str.Contains(t)) return true;
                    if (ignoreCase && str.ToLower().Contains(t)) return true;
                }
            }
            return false;
        }
        public static bool Contains_OneOf(this String str, bool ignoreCase = false, params string[] tags)
        {
            if (tags != null)
            {
                foreach (var t in tags)
                {
                    if (str.Contains(t)) return true;
                    if (ignoreCase && str.ToLower().Contains(t)) return true;
                }
            }
            return false;
        }

        public static bool Contains_AllOf(this String str, string[] tags, bool ignoreCase = false)
        {
            if (tags != null)
            {
                foreach (var t in tags)
                {
                    if (!str.Contains(t)) return false;
                    if (ignoreCase && !str.ToLower().Contains(t)) return false;
                }
            }
            return true;
        }
        public static bool Contains_AllOf(this String str, bool ignoreCase = false, params string[] tags)
        {
            if (tags != null)
            {
                foreach (var t in tags)
                {
                    if (!str.Contains(t)) return false;
                    if (ignoreCase && !str.ToLower().Contains(t)) return false;
                }
            }
            return true;
        }

        public static String Format_ext(this String str, bool strictMatchParamCount, params object[] param)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("\\{[\\d]+\\}");    //匹配{num}
            if (reg.IsMatch(str))
            {
                try
                {
                    str = string.Format(str, param);
                }
                catch (FormatException fe)
                {
                    // 奇葩的Formt函数，有时在处理格式正确的字符串时抛出异常，这里再做一次匹对

                    System.Diagnostics.Debug.WriteLine(str + " catch " + fe);

                    var matches = reg.Matches(str);
                    if (strictMatchParamCount && matches.Count != param.Length) throw fe;

                    string p = "";
                    for (int i = 0; i < param.Length; i++)
                    {
                        p = "{" + i + "}";
                        if (!str.Contains(p)) throw fe;

                        str = str.Replace(p, param[i].ToString());
                    }
                }
            }
            else if (str.Contains("{?}"))
            {
                reg = new System.Text.RegularExpressions.Regex("\\{\\?\\}");    //匹配{?}
                int matchCount = reg.Matches(str).Count;
                if (strictMatchParamCount && matchCount != param.Length) throw new IndexOutOfRangeException();
                if (matchCount - 1 > param.Length) throw new Exception("参数数目小于字符串{?}数目（参数数目可大于{?}数目，多余的参数不作配置）");

                string[] s = str.Split(new string[] { "{?}" }, StringSplitOptions.None);
                StringBuilder sb = new StringBuilder();
                int pIdx = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    sb.Append(s[i] + (pIdx < param.Length ? param[pIdx++] : ""));
                }
                str = sb.ToString();
            }

            return str;
        }
        public static String Format_Ext(this String str, params object[] param)
        {
            return str.Format_ext(false, param);
        }

        public static String Format_ext(this String str, Dictionary<string, string> dic)
        {
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("\\{\\$(\\w)*\\$\\}");  //匹配{$字母数字下划线$}
            //var matches = reg.Matches(str);
            //if (strictMatchParamCount && matches.Count != dic.Count) throw new IndexOutOfRangeException();
            //if (matches.Count > dic.Count) throw new Exception("参数数目小于字符串{$*$}数目（参数数目可大于{$*$}数目，多余的参数不作配置）");

            string p = "";
            for (int i = 0; i < dic.Count; i++)
            {
                p = "{$" + dic.ElementAt(i).Key + "$}";
                if (!str.Contains(p)) throw new Exception("String miss " + p);

                str = str.Replace(p, dic.ElementAt(i).Value);
            }

            return str;
        }

        public static bool IsNullOrWhiteSpace(this String str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static String Remove(this String str, params string[] toWipeOffText)
        {
            foreach (var item in toWipeOffText)
            {
                str = str.Replace(item, "");
            }
            return str;
        }

        public static String[] Split(this String str, bool ignoreBlank, params string[] spliteTags)
        {
            List<string> lst = new List<string>();
            foreach (var item in spliteTags)
            {
                lst.Add(item);
            }
            StringSplitOptions sso = ignoreBlank ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return str.Split(lst.ToArray(), sso);
        }

        public static String[] SimpleSplit(this String str)
        {
            List<string> lStr = new List<string>();
            foreach (var s in str)
            {
                lStr.Add(s.ToString());
            }
            return lStr.ToArray();
        }

        public static String Join<T>(this T[] ts, string joinTag = " ")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in ts)
            {
                sb.Append(joinTag + s.ToString());
            }

            return sb.ToString().IfEmptyThenOutput(".").Substring(1);
        }
        public static String Join<T>(this List<T> lt, string joinTag = " ")
        {
            return Join<T>(lt.ToArray(), joinTag);
        }
        #endregion

        #region 判断字符串，显示指定值
        public static String IfEmptyThenOutput(this String str, string output)
        {
            if (string.IsNullOrWhiteSpace(str)) return output;
            return str;
        }

        public static String IfEqualValueThenOutput(this String str, string value, string output, bool ignoreCase = false)
        {
            if (str == value) return output;
            if (ignoreCase && str.ToLower() == value) return output;
            return str;
        }

        public static String IfContainValueThenOutput(this String str, string value, string output, bool ignoreCase = false)
        {
            if (str.Contains(value)) return output;
            if (ignoreCase && str.ToLower().Contains(value)) return output;
            return str;
        }

        public static String IfContainOneOfValueThenOutput(this String str, string[] values, string output, bool ignoreCase = false)
        {
            if (str.Contains_OneOf(values)) return output;
            if (ignoreCase && str.ToLower().Contains_OneOf(values)) return output;
            return str;
        }

        public static String IfStartWithValueThenOutput(this String str, string value, string output, bool ignoreCase = false)
        {
            if (str.StartsWith(value)) return output;
            if (ignoreCase && str.ToLower().StartsWith(value)) return output;
            return str;
        }

        public static String IfEndWithValueThenOutput(this String str, string value, string output, bool ignoreCase = false)
        {
            if (str.EndsWith(value)) return output;
            if (ignoreCase && str.ToLower().EndsWith(value)) return output;
            return str;
        }
        #endregion

    }

}
