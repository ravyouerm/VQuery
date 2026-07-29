using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VQuery
{
    public class VariableConverter
    {

        public string ToString(object s)
        {
            try
            {
                if (s != null)
                {
                    return s.ToString();
                }
                else { return string.Empty; }
            }
            catch
            {
                return "";
            }
        }

        public string ToDoubleString(object s)
        {
            try
            {
                return this.ToString(this.ToDouble(s));
            }
            catch
            {
                return "0";
            }
        }

        public string ToIntString(object s)
        {
            try
            {
                return this.ToString(this.ToInt(s));
            }
            catch
            {
                return "0";
            }
        }


        public string ToString(DateTime s, string Format = "dd-MM-yyyy")
        {
            try
            {
                return s.ToString(Format);
            }
            catch
            {
                return DateTime.Now.ToString(Format);
            }
        }

        public string ToStringDate(DateTime s, string Format = "dd-MM-yyyy")
        {
            try
            {
                return s.ToString(Format);
            }
            catch
            {
                return DateTime.Now.ToString(Format);
            }
        }

        public string ToStringDateTime(DateTime s, string Format = "dd-MM-yyyy HH:mm:ss")
        {
            try
            {
                return s.ToString(Format);
            }
            catch
            {
                return DateTime.Now.ToString(Format);
            }
        }

        // FIX: ParseExact only accepts the single Format given, so any caller who
        // passes a value in a different (but common) shape than the default
        // "dd-MM-yyyy" - e.g. ISO "yyyy-MM-dd", or a value that already carries a
        // time component - fell through to the catch block and silently returned
        // DateTime.Today. That masks real bugs (wrong data, wrong format string)
        // behind a plausible-looking date. Now we try the caller's Format first
        // (unchanged behavior for existing callers), then fall back to a short list
        // of common formats and finally a culture-invariant DateTime.TryParse before
        // giving up.
        private static readonly string[] FallbackDateFormats =
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "dd-MM-yyyy",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "dd-MM-yyyy HH:mm:ss",
            "MM/dd/yyyy HH:mm:ss"
        };

        private static bool TryParseAnyFormat(string input, string preferredFormat, out DateTime result)
        {
            if (DateTime.TryParseExact(
                    input,
                    preferredFormat,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out result))
            {
                return true;
            }

            foreach (string format in FallbackDateFormats)
            {
                if (DateTime.TryParseExact(
                        input,
                        format,
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out result))
                {
                    return true;
                }
            }

            return DateTime.TryParse(
                input,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }

        public DateTime ToDate(object? s, string Format = "dd-MM-yyyy")
        {
            try
            {
                if (s == null)
                {
                    return DateTime.Today;
                }
                else
                {
                    if (TryParseAnyFormat(ToString(s), Format, out DateTime dt))
                    {
                        return dt.Date;
                    }

                    return DateTime.Today;
                }
            }
            catch
            {
                return DateTime.Today;
            }
        }

        public DateTime ToDateTime(object? s, string Format = "dd-MM-yyyy HH:mm:ss")
        {
            try
            {
                if (s == null)
                {
                    return DateTime.Today;
                }
                else
                {
                    if (TryParseAnyFormat(ToString(s), Format, out DateTime dt))
                    {
                        return dt;
                    }

                    return DateTime.Today;
                }
            }
            catch
            {
                return DateTime.Today;
            }
        }

        // FIX: Int32.Parse rejects any string with a decimal point ("88.90"),
        // throwing a FormatException that was silently swallowed and turned into 0.
        // Now we parse as a decimal first (handles "88.90", "42.33", "1,234.5" after
        // the existing comma-stripping) and truncate to an int, matching what most
        // callers expect from a "ToInt" conversion. Plain integer strings still work
        // exactly as before.
        public int ToInt(object? s)
        {
            try
            {
                if (s == null)
                {
                    return 0;
                }
                else
                {
                    string ss = this.ToString(s).Replace("'", "").Replace(" ", "").Replace(",", "");

                    if (Int32.TryParse(ss, out int intResult))
                    {
                        return intResult;
                    }

                    if (decimal.TryParse(
                            ss,
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out decimal decResult))
                    {
                        return (int)decResult;
                    }

                    return 0;
                }
            }
            catch
            {
                return 0;
            }

        }

        public double ToDouble(object? s)
        {
            try
            {
                if (s == null)
                {
                    return 0;
                }
                else
                {
                    string ss = this.ToString(s).Replace("'", "").Replace(" ", "").Replace(",", "");
                    return Double.Parse(ss);
                }
            }
            catch
            {
                return 0;
            }

        }


        public string NumberToText(Int64 number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToText(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToText(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToText(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToText(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public string NumberToTextKH(Int64 number)
        {
            if (number == 0)
                return "សូន្យ";

            if (number < 0)
                return "ដក" + NumberToTextKH(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToTextKH(number / 1000000) + "លាន ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToTextKH(number / 1000) + "ពាន់ ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToTextKH(number / 100) + "រយ ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                {
                    words += "";

                }


                var unitsMap = new[] { "សូន្យ", "មួយ", "ពីរ", "បី", "បួន", "ប្រាំ", "ប្រាំមួយ", "ប្រាំពីរ", "ប្រាំបី", "ប្រាំបួន", "ដប់", "ដប់មួយ", "ដប់ពីរ", "ដប់បី", "ដប់បួន", "ដប់ប្រាំ", "ដប់ប្រាំមួយ", "ដប់ប្រាំពីរ", "ដប់ប្រាំបី", "ដប់ប្រាំបួន" };
                var tensMap = new[] { "សូន្យ", "ដប់", "ម្ភៃ", "សាមសិប", "សែសិប", "ហាសិប", "ហុកសិប", "ចិតសិប", "ប៉ែតសិប", "កៅសិប" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public string NumberToKhNumber(double number)
        {
            string n = number.ToString();

            return n.Replace("0", "០")
                        .Replace("1", "១")
                        .Replace("2", "២")
                        .Replace("3", "៣")
                        .Replace("4", "៤")
                        .Replace("5", "៥")
                        .Replace("6", "៦")
                        .Replace("7", "៧")
                        .Replace("8", "៨")
                        .Replace("9", "៩");

        }

        public bool IsEmpty(DataRow row)
        {
            return row == null || row.ItemArray.All(i => i is DBNull);
        }


    }
}