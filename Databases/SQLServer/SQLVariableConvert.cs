using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VQuery.Databases.SQLServer
{
    public class SQLVariableConvert : VariableConverter
    {
        public string SQLToString(object s)
        {
            return "'" + base.ToString(s).Replace("'", "''") + "'";
        }

        public string SQLToFieldName(object s)
        {
            return "[" + base.ToString(s).Replace("'", "").Replace(" ", "") + "]";
        }

        public string SQLToDate(string s, string Format = "dd-MM-yyyy")
        {
            if (s.ToString().Trim() == "")
            {
                return " NULL ";
            }
            else
            {
                try
                {
                    string str = base.ToString(s).Replace("'", "");
                    DateTime D = base.ToDate(str, Format);
                    return "'" + D.ToString(Format) + "'";
                }
                catch
                {
                    return " NULL ";
                }
            }
        }

        public string SQLToDate(DateTime s, string Format = "dd-MM-yyyy")
        {
            try
            {
                return "'" + s.ToString(Format) + "'";
            }
            catch
            {
                return " NULL ";
            }
        }

        public string SQLToDateTime(DateTime s, string Format = "dd-MM-yyyy HH:mm:ss")
        {
            try
            {
                return "'" + s.ToString(Format) + "'";
            }
            catch
            {
                return " NULL ";
            }
        }

        public string SQLToEscape(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate (System.Text.RegularExpressions.Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }

        public int SQLToInt(object s)
        {
            var str = base.ToString(s).Replace("'", "").Replace(" ", "").Replace(",", "");
            return base.ToInt(str);
        }

        public double SQLToDouble(object s)
        {
            var str = base.ToString(s).Replace("'", "").Replace(" ", "").Replace(",", "");
            return base.ToDouble(str);
        }
    }
}
