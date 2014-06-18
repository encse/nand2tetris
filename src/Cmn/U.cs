using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cmn.Compiler;
using JetBrains.Annotations;

namespace Cmn
{
    public class Ufer : Exception
    {
        public Ufer(string ust)
            : base(ust)
        {
        }
    }

    public static class U
    {
        public static string UstMessage(this Exception er)
        {
            if (er is Ufer)
                return er.Message;
            return er.ToString();
        }

        public static string[] ToLines(this string st)
        {
            return st.Split('\n').Select(stT => stT.Replace("\r", "")).ToArray();
        }

        public static T ParseKIgnoreCase<T>(string st) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof (T), st, true);
        }
        
        public static T KFromMnemonic<T>(string st) where T : struct, IConvertible
        {
            foreach (var v in Enum.GetValues(typeof(T)))
                if (ToMnemonic((Enum)v) == st)
                    return (T)v;
            
            throw new Exception();
        }

        public static IEnumerable<T> Enk<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string ToMnemonic(this Enum e)
        {
            var f = e.GetType().GetField(e.ToString());
            if (f == null)
                return e.ToString().ToLowerInvariant();

            var attributes = (MnemonicAttribute[])f.GetCustomAttributes(typeof(MnemonicAttribute), false);
            return attributes.Length > 0 ? attributes[0].st : e.ToString().ToLowerInvariant();
        }

        public static T GetAttribute<T>(this Enum e, bool fInherit = false)
        {
            var f = e.GetType().GetField(e.ToString());
            var attributes = f.GetCustomAttributes(typeof(T), true);
            return (T)attributes.SingleOrDefault();

        }

        public static IEnumerable<string> ToLinesSkipComments(this string st)
        {
            foreach (var stLine in st.ToLines())
            {
                var stLineT = stLine.SkipComment();
                if (!string.IsNullOrWhiteSpace(stLineT))
                    yield return stLineT;
            }
        }

        public static string SkipComment(this string st)
        {
            var ichComment = st.IndexOf("//", StringComparison.Ordinal);
            return ichComment >= 0 ? st.Substring(0, ichComment) : st;
        }

        public static string[] ToWords(this string st)
        {
            var rgst = new List<string>();
            var stT = "";
            foreach (char ch in st)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!string.IsNullOrEmpty(stT))
                        rgst.Add(stT);
                    stT = "";
                }
                else
                {
                    stT += ch;
                }
            }

            if (!string.IsNullOrEmpty(stT))
                rgst.Add(stT);

            return rgst.ToArray();
        }


        public static string StFromResource(Type type, string stFilename)
        {
            var assembly = type.Assembly;

            var respat = assembly.GetManifestResourceNames().SingleOrDefault(
                respatT => respatT.EndsWith("." + stFilename, StringComparison.InvariantCultureIgnoreCase));

            if (respat == null)
                throw new Exception("Could not load resource: {0}".StFormat(stFilename));

            using(var sr = new StreamReader(assembly.GetManifestResourceStream(respat), Encoding.UTF8))
                return sr.ReadToEnd();
        }

        [StringFormatMethod("stFormat")]
        public static string StFormat(this string stFormat, params object[] args)
        {
            return String.Format(stFormat, args);
        }

        public static string StJoinOrDefault(this IEnumerable<string> enst, string stSep, string stDefault = "")
        {
            var rgst = enst.ToArray();
            return rgst.Any() ? rgst.StJoin(stSep) : stDefault;
        }

        public static string StJoin<T>([InstantHandle] this IEnumerable<T> enumerable, string separator, [InstantHandle]Func<T, string> valueSelector)
        {
            return String.Join(separator, enumerable.Select(valueSelector).ToArray());
        }

        public static string StJoin(this IEnumerable<string> enst, char chSep)
        {
            return String.Join(chSep.ToString(CultureInfo.InvariantCulture), enst.ToArray());
        }

        public static string StJoin(this IEnumerable<string> enst, string stSep)
        {
            return String.Join(stSep, enst.ToArray());
        }

        public static bool FIn<T>(this T t, params T[] ent)
        {
            return ent.Contains(t);
        }


        public static IEnumerable<T> EnEnsure<T>(this IEnumerable<T> rgt)
        {
            return rgt ?? Enumerable.Empty<T>();
        }



        public static IEnumerable<T> EnCons<T>(this T tHead)
        {
            yield return tHead;
        }

        public static IEnumerable<T> EnCons<T>(this T tHead, IEnumerable<T> entTail)
        {
            yield return tHead;
            foreach (var tTail in entTail)
            {
                yield return tTail;
            }
        }
    }

}
