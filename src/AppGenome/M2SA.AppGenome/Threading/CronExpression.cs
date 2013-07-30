using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace M2SA.AppGenome.Threading
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CronExpression : ICloneable, IDeserializationCallback
    {
        #region Const
        /// <summary>
        /// Field specification for second.
        /// </summary>
        protected const int Second = 0;

        /// <summary>
        /// Field specification for minute.
        /// </summary>
        protected const int Minute = 1;

        /// <summary>
        /// Field specification for hour.
        /// </summary>
        protected const int Hour = 2;

        /// <summary>
        /// Field specification for day of month.
        /// </summary>
        protected const int DayOfMonth = 3;

        /// <summary>
        /// Field specification for month.
        /// </summary>
        protected const int Month = 4;

        /// <summary>
        /// Field specification for day of week.
        /// </summary>
        protected const int DayOfWeek = 5;

        /// <summary>
        /// Field specification for year.
        /// </summary>
        protected const int Year = 6;

        /// <summary>
        /// Field specification for all wildcard value '*'.
        /// </summary>
        protected const int AllSpecValue = 99; // '*'

        /// <summary>
        /// Field specification for not specified value '?'.
        /// </summary>
        protected const int NoSpecValue = 98; // '?'

        /// <summary>
        /// Field specification for wildcard '*'.
        /// </summary>
        protected const int AllSpec = AllSpecValue;

        /// <summary>
        /// Field specification for no specification at all '?'.
        /// </summary>
        protected const int NoSpec = NoSpecValue;

        #endregion

        #region Static

        private static readonly Hashtable monthMap = new Hashtable(20);
        private static readonly Hashtable dayMap = new Hashtable(60);

        static CronExpression()
        {
            monthMap.Add("JAN", 0);
            monthMap.Add("FEB", 1);
            monthMap.Add("MAR", 2);
            monthMap.Add("APR", 3);
            monthMap.Add("MAY", 4);
            monthMap.Add("JUN", 5);
            monthMap.Add("JUL", 6);
            monthMap.Add("AUG", 7);
            monthMap.Add("SEP", 8);
            monthMap.Add("OCT", 9);
            monthMap.Add("NOV", 10);
            monthMap.Add("DEC", 11);

            dayMap.Add("SUN", 1);
            dayMap.Add("MON", 2);
            dayMap.Add("TUE", 3);
            dayMap.Add("WED", 4);
            dayMap.Add("THU", 5);
            dayMap.Add("FRI", 6);
            dayMap.Add("SAT", 7);
        }

        #endregion

        #region Instance Fields

        private readonly string cronExpressionString = null;
        private TimeZone timeZone = null;


        [NonSerialized]
        private ITreeList seconds;

        [NonSerialized]
        private ITreeList minutes;

        [NonSerialized]
        private ITreeList hours;

        [NonSerialized]
        private ITreeList daysOfMonth;

        [NonSerialized]
        private ITreeList months;

        [NonSerialized]
        private ITreeList daysOfWeek;

        [NonSerialized]
        private ITreeList years;

        [NonSerialized]
        private bool lastdayOfWeek = false;

        [NonSerialized]
        private int nthdayOfWeek = 0;

        [NonSerialized]
        private bool lastdayOfMonth = false;

        [NonSerialized]
        private bool nearestWeekday = false;

        [NonSerialized]
        private bool calendardayOfWeek = false;

        [NonSerialized]
        private bool calendardayOfMonth = false;

        #endregion

        #region Instance Properties

        #endregion

        ///<summary>
        /// Constructs a new <see cref="CronExpressionString" /> based on the specified 
        /// parameter.
        /// </summary>
        /// <param name="cronExpression">
        /// String representation of the cron expression the new object should represent
        /// </param>
        /// <see cref="CronExpressionString" />
        public CronExpression(string cronExpression)
        {
            if (cronExpression == null)
            {
                throw new ArgumentException("cronExpression cannot be null");
            }

            cronExpressionString = cronExpression.ToUpper(CultureInfo.InvariantCulture);
            BuildExpression(cronExpression);
        }

        /// <summary>
        /// Indicates whether the given date satisfies the cron expression. 
        /// </summary>
        /// <remarks>
        /// Note that  milliseconds are ignored, so two Dates falling on different milliseconds
        /// of the same second will always have the same result here.
        /// </remarks>
        /// <param name="dateUtc">The date to evaluate.</param>
        /// <returns>a boolean indicating whether the given date satisfies the cron expression</returns>
        public virtual bool IsSatisfiedBy(DateTime dateUtc)
        {
            DateTime test =
                new DateTime(dateUtc.Year, dateUtc.Month, dateUtc.Day, dateUtc.Hour, dateUtc.Minute, dateUtc.Second).AddSeconds(-1);

            Nullable<DateTime> timeAfter = GetTimeAfter(test);

            if (timeAfter.HasValue && timeAfter.Value.Equals(dateUtc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the next date/time <i>after</i> the given date/time which
        /// satisfies the cron expression.
        /// </summary>
        /// <param name="dateTime">the date/time at which to begin the search for the next valid date/time</param>
        /// <returns>the next valid date/time</returns>
        public virtual Nullable<DateTime> GetNextValidTimeAfter(DateTime dateTime)
        {
            return GetTimeAfter(dateTime);
        }

        /// <summary>
        /// Returns the next date/time <i>after</i> the given date/time which does
        /// <i>not</i> satisfy the expression.
        /// </summary>
        /// <param name="dateTime">the date/time at which to begin the search for the next invalid date/time</param>
        /// <returns>the next valid date/time</returns>
        public virtual Nullable<DateTime> GetNextInvalidTimeAfter(DateTime dateTime)
        {
            long difference = 1000;

            //move back to the nearest second so differences will be accurate
            DateTime lastDate =
                new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).AddSeconds(-1);

            //TODO: IMPROVE THIS! The following is a BAD solution to this problem. Performance will be very bad here, depending on the cron expression. It is, however A solution.

            //keep getting the next included time until it's farther than one second
            // apart. At that point, lastDate is the last valid fire time. We return
            // the second immediately following it.
            while (difference == 1000)
            {
                DateTime newDate = GetTimeAfter(lastDate).Value;

                difference = (long)(newDate - lastDate).TotalMilliseconds;

                if (difference == 1000)
                {
                    lastDate = newDate;
                }
            }

            return lastDate.AddSeconds(1);
        }

        /// <summary>
        /// Sets or gets the time zone for which the <see cref="CronExpression" /> of this        
        /// </summary>
        public virtual TimeZone TimeZone
        {
            set
            {
                timeZone = value;
            }
            get
            {
                if (timeZone == null)
                {
                    timeZone = TimeZone.CurrentTimeZone;
                }

                return timeZone;
            }
        }

        /// <summary>
        /// Returns the string representation of the <see cref="CronExpression" />
        /// </summary>
        /// <returns>The string representation of the <see cref="CronExpression" /></returns>
        public override string ToString()
        {
            return cronExpressionString;
        }

        /// <summary>
        /// Indicates whether the specified cron expression can be parsed into a 
        /// valid cron expression
        /// </summary>
        /// <param name="cronExpression">the expression to evaluate</param>
        /// <returns>a boolean indicating whether the given expression is a valid cron
        ///         expression</returns>
        public static bool IsValidExpression(string cronExpression)
        {
            try
            {
                new CronExpression(cronExpression);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        // Expression Parsing Functions
        //
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Builds the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected void BuildExpression(string expression)
        {
            if (null == expression)
                throw new ArgumentNullException("expression");
            try
            {
                if (seconds == null)
                {
                    seconds = new TreeList();
                }
                if (minutes == null)
                {
                    minutes = new TreeList();
                }
                if (hours == null)
                {
                    hours = new TreeList();
                }
                if (daysOfMonth == null)
                {
                    daysOfMonth = new TreeList();
                }
                if (months == null)
                {
                    months = new TreeList();
                }
                if (daysOfWeek == null)
                {
                    daysOfWeek = new TreeList();
                }
                if (years == null)
                {
                    years = new TreeList();
                }

                int exprOn = Second;

                string[] exprsTok = expression.Trim().Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string expr in exprsTok)
                {
                    if (expr.Trim().Length == 0)
                    {
                        continue;
                    }
                    if (exprOn > Year)
                    {
                        break;
                    }
                    string[] vTok = expr.Trim().Split(',');
                    foreach (string v in vTok)
                    {
                        StoreExpressionVals(0, v, exprOn);
                    }

                    exprOn++;
                }

                if (exprOn <= DayOfWeek)
                {
                    throw new FormatException("Unexpected end of expression.");
                }

                if (exprOn <= Year)
                {
                    StoreExpressionVals(0, "*", Year);
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Illegal cron expression format ({0})", e));
            }
        }

        /// <summary>
        /// Stores the expression values.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="values">The string to traverse.</param>
        /// <param name="type">The type of value.</param>
        /// <returns></returns>
        protected virtual int StoreExpressionVals(int pos, string values, int type)
        {
            if (null == values)
                throw new ArgumentNullException("values");
            int incr = 0;
            int i = SkipWhiteSpace(pos, values);
            if (i >= values.Length)
            {
                return i;
            }
            char c = values[i];
            if ((c >= 'A') && (c <= 'Z') && (!values.Equals("L")) && (!values.Equals("LW")))
            {
                String sub = values.Substring(i, 3);
                int sval;
                int eval = -1;
                if (type == Month)
                {
                    sval = GetMonthNumber(sub) + 1;
                    if (sval <= 0)
                    {
                        throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid Month value: '{0}'", sub));
                    }
                    if (values.Length > i + 3)
                    {
                        c = values[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = values.Substring(i, 3);
                            eval = GetMonthNumber(sub) + 1;
                            if (eval <= 0)
                            {
                                throw new FormatException(
                                    string.Format(CultureInfo.InvariantCulture, "Invalid Month value: '{0}'", sub));
                            }
                        }
                    }
                }
                else if (type == DayOfWeek)
                {
                    sval = GetDayOfWeekNumber(sub);
                    if (sval < 0)
                    {
                        throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid Day-of-Week value: '{0}'", sub));
                    }
                    if (values.Length > i + 3)
                    {
                        c = values[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = values.Substring(i, 3);
                            eval = GetDayOfWeekNumber(sub);
                            if (eval < 0)
                            {
                                throw new FormatException(
                                    string.Format(CultureInfo.InvariantCulture, "Invalid Day-of-Week value: '{0}'", sub));
                            }
                            if (sval > eval)
                            {
                                throw new FormatException(
                                    string.Format(CultureInfo.InvariantCulture, "Invalid Day-of-Week sequence: {0} > {1}", sval, eval));
                            }
                        }
                        else if (c == '#')
                        {
                            try
                            {
                                i += 4;
                                nthdayOfWeek = Convert.ToInt32(values.Substring(i), CultureInfo.InvariantCulture);
                                if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                                {
                                    throw new ArgumentNullException("values");
                                }
                            }
                            catch (Exception)
                            {
                                throw new FormatException(
                                    "A numeric value between 1 and 5 must follow the '#' option");
                            }
                        }
                        else if (c == 'L')
                        {
                            lastdayOfWeek = true;
                            i++;
                        }
                    }
                }
                else
                {
                    throw new FormatException(
                        string.Format(CultureInfo.InvariantCulture, "Illegal characters for this position: '{0}'", sub));
                }
                if (eval != -1)
                {
                    incr = 1;
                }
                AddToTreeList(sval, eval, incr, type);
                return (i + 3);
            }

            if (c == '?')
            {
                i++;
                if ((i + 1) < values.Length
                    && (values[i] != ' ' && values[i + 1] != '\t'))
                {
                    throw new FormatException("Illegal character after '?': "
                                              + values[i]);
                }
                if (type != DayOfWeek && type != DayOfMonth)
                {
                    throw new FormatException(
                        "'?' can only be specfied for Day-of-Month or Day-of-Week.");
                }
                if (type == DayOfWeek && !lastdayOfMonth)
                {
                    int val = (int)daysOfMonth[daysOfMonth.Count - 1];
                    if (val == NoSpecValue)
                    {
                        throw new FormatException(
                            "'?' can only be specfied for Day-of-Month -OR- Day-of-Week.");
                    }
                }

                AddToTreeList(NoSpecValue, -1, 0, type);
                return i;
            }

            if (c == '*' || c == '/')
            {
                if (c == '*' && (i + 1) >= values.Length)
                {
                    AddToTreeList(AllSpecValue, -1, incr, type);
                    return i + 1;
                }
                else if (c == '/'
                         && ((i + 1) >= values.Length || values[i + 1] == ' ' || values[i + 1] == '\t'))
                {
                    throw new FormatException("'/' must be followed by an integer.");
                }
                else if (c == '*')
                {
                    i++;
                }
                c = values[i];
                if (c == '/')
                {
                    // is an increment specified?
                    i++;
                    if (i >= values.Length)
                    {
                        throw new FormatException("Unexpected end of string.");
                    }

                    incr = GetNumericValue(values, i);

                    i++;
                    if (incr > 10)
                    {
                        i++;
                    }
                    if (incr > 59 && (type == Second || type == Minute))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 60 : {0}", incr));
                    }
                    else if (incr > 23 && (type == Hour))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 24 : {0}", incr));
                    }
                    else if (incr > 31 && (type == DayOfMonth))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 31 : {0}", incr));
                    }
                    else if (incr > 7 && (type == DayOfWeek))
                    {
                        throw new FormatException(
                            string.Format(CultureInfo.InvariantCulture, "Increment > 7 : {0}", incr));
                    }
                    else if (incr > 12 && (type == Month))
                    {
                        throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Increment > 12 : {0}", incr));
                    }
                }
                else
                {
                    incr = 1;
                }

                AddToTreeList(AllSpecValue, -1, incr, type);
                return i;
            }
            else if (c == 'L')
            {
                i++;
                if (type == DayOfMonth)
                {
                    lastdayOfMonth = true;
                }
                if (type == DayOfWeek)
                {
                    AddToTreeList(7, 7, 0, type);
                }
                if (type == DayOfMonth && values.Length > i)
                {
                    c = values[i];
                    if (c == 'W')
                    {
                        nearestWeekday = true;
                        i++;
                    }
                }
                return i;
            }
            else if (c >= '0' && c <= '9')
            {
                int val = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= values.Length)
                {
                    AddToTreeList(val, -1, -1, type);
                }
                else
                {
                    c = values[i];
                    if (c >= '0' && c <= '9')
                    {
                        ValueSet vs = GetValue(val, values, i);
                        val = vs.TheValue;
                        i = vs.Pos;
                    }
                    i = CheckNext(i, values, val, type);
                    return i;
                }
            }
            else
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unexpected character: {0}", c));
            }

            return i;
        }

        /// <summary>
        /// Checks the next value.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="checks">The string to check.</param>
        /// <param name="val">The value.</param>
        /// <param name="type">The type to search.</param>
        /// <returns></returns>
        protected virtual int CheckNext(int pos, string checks, int val, int type)
        {
            if (null == checks)
                throw new ArgumentNullException("checks");
            int end = -1;
            int i = pos;

            if (i >= checks.Length)
            {
                AddToTreeList(val, end, -1, type);
                return i;
            }

            char c = checks[pos];

            if (c == 'L')
            {
                if (type == DayOfWeek)
                {
                    lastdayOfWeek = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'L' option is not valid here. (pos={0})", i));
                }
                ITreeList data = GetTreeList(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == 'W')
            {
                if (type == DayOfMonth)
                {
                    nearestWeekday = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'W' option is not valid here. (pos={0})", i));
                }
                ITreeList data = GetTreeList(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == '#')
            {
                if (type != DayOfWeek)
                {
                    throw new FormatException(
                        string.Format(CultureInfo.InvariantCulture, "'#' option is not valid here. (pos={0})", i));
                }
                i++;
                try
                {
                    nthdayOfWeek = Convert.ToInt32(checks.Substring(i), CultureInfo.InvariantCulture);
                    if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                    {
                        throw new ArgumentNullException("checks");
                    }
                }
                catch (Exception)
                {
                    throw new FormatException(
                        "A numeric value between 1 and 5 must follow the '#' option");
                }

                ITreeList data = GetTreeList(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == 'C')
            {
                if (type == DayOfWeek)
                {
                    calendardayOfWeek = true;
                }
                else if (type == DayOfMonth)
                {
                    calendardayOfMonth = true;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'C' option is not valid here. (pos={0})", i));
                }
                ITreeList data = GetTreeList(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == '-')
            {
                i++;
                c = checks[i];
                int v = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                end = v;
                i++;
                if (i >= checks.Length)
                {
                    AddToTreeList(val, end, 1, type);
                    return i;
                }
                c = checks[i];
                if (c >= '0' && c <= '9')
                {
                    ValueSet vs = GetValue(v, checks, i);
                    int v1 = vs.TheValue;
                    end = v1;
                    i = vs.Pos;
                }
                if (i < checks.Length && ((c = checks[i]) == '/'))
                {
                    i++;
                    c = checks[i];
                    int v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                    i++;
                    if (i >= checks.Length)
                    {
                        AddToTreeList(val, end, v2, type);
                        return i;
                    }
                    c = checks[i];
                    if (c >= '0' && c <= '9')
                    {
                        ValueSet vs = GetValue(v2, checks, i);
                        int v3 = vs.TheValue;
                        AddToTreeList(val, end, v3, type);
                        i = vs.Pos;
                        return i;
                    }
                    else
                    {
                        AddToTreeList(val, end, v2, type);
                        return i;
                    }
                }
                else
                {
                    AddToTreeList(val, end, 1, type);
                    return i;
                }
            }

            if (c == '/')
            {
                i++;
                c = checks[i];
                int v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= checks.Length)
                {
                    AddToTreeList(val, end, v2, type);
                    return i;
                }
                c = checks[i];
                if (c >= '0' && c <= '9')
                {
                    ValueSet vs = GetValue(v2, checks, i);
                    int v3 = vs.TheValue;
                    AddToTreeList(val, end, v3, type);
                    i = vs.Pos;
                    return i;
                }
                else
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}' after '/'", c));
                }
            }

            AddToTreeList(val, end, 0, type);
            i++;
            return i;
        }

        /// <summary>
        /// Gets the cron expression string.
        /// </summary>
        /// <value>The cron expression string.</value>
        public string CronExpressionString
        {
            get
            {
                return cronExpressionString;
            }
        }

        /// <summary>
        /// Gets the expression summary.
        /// </summary>
        /// <returns></returns>
        public virtual string GetExpressionSummary()
        {
            StringBuilder buf = new StringBuilder();

            buf.Append("@seconds: ");
            buf.Append(GetExpressionTreeListSummary(seconds));
            buf.Append(" \n");
            buf.Append("@minutes: ");
            buf.Append(GetExpressionTreeListSummary(minutes));
            buf.Append(" \n");
            buf.Append("@hours: ");
            buf.Append(GetExpressionTreeListSummary(hours));
            buf.Append(" \n");
            buf.Append("@daysOfMonth: ");
            buf.Append(GetExpressionTreeListSummary(daysOfMonth));
            buf.Append(" \n");
            buf.Append("@months: ");
            buf.Append(GetExpressionTreeListSummary(months));
            buf.Append(" \n");
            buf.Append("@daysOfWeek: ");
            buf.Append(GetExpressionTreeListSummary(daysOfWeek));
            buf.Append(" \n");
            buf.Append("@lastdayOfWeek: ");
            buf.Append(lastdayOfWeek);
            buf.Append(" \n");
            buf.Append("@nearestWeekday: ");
            buf.Append(nearestWeekday);
            buf.Append(" \n");
            buf.Append("@NthDayOfWeek: ");
            buf.Append(nthdayOfWeek);
            buf.Append(" \n");
            buf.Append("@lastdayOfMonth: ");
            buf.Append(lastdayOfMonth);
            buf.Append(" \n");
            buf.Append("@calendardayOfWeek: ");
            buf.Append(calendardayOfWeek);
            buf.Append(" \n");
            buf.Append("@calendardayOfMonth: ");
            buf.Append(calendardayOfMonth);
            buf.Append(" \n");
            buf.Append("@years: ");
            buf.Append(GetExpressionTreeListSummary(years));
            buf.Append(" \n");

            return buf.ToString();
        }

        /// <summary>
        /// Gets the expression set summary.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected virtual string GetExpressionTreeListSummary(ITreeList data)
        {
            if (null == data)
                throw new ArgumentNullException("data");

            if (data.Contains(NoSpec))
            {
                return "?";
            }
            if (data.Contains(AllSpec))
            {
                return "*";
            }

            StringBuilder buf = new StringBuilder();

            bool first = true;
            foreach (int iVal in data)
            {
                string val = iVal.ToString(CultureInfo.InvariantCulture);
                if (!first)
                {
                    buf.Append(",");
                }
                buf.Append(val);
                first = false;
            }

            return buf.ToString();
        }

        /// <summary>
        /// Skips the white space.
        /// </summary>
        /// <param name="index">The i.</param>
        /// <param name="val">The s.</param>
        /// <returns></returns>
        protected virtual int SkipWhiteSpace(int index, string val)
        {
            if (null == val)
                throw new ArgumentNullException("val");
            for (; index < val.Length && (val[index] == ' ' || val[index] == '\t'); index++)
            {
                ;
            }

            return index;
        }

        /// <summary>
        /// Finds the next white space.
        /// </summary>
        /// <param name="index">The i.</param>
        /// <param name="val">The s.</param>
        /// <returns></returns>
        protected virtual int FindNextWhiteSpace(int index, string val)
        {
            if (null == val)
                throw new ArgumentNullException("val");
            for (; index < val.Length && (val[index] != ' ' || val[index] != '\t'); index++)
            {
                ;
            }

            return index;
        }

        /// <summary>
        /// Adds to set.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="endIndex">The end.</param>
        /// <param name="incr">The incr.</param>
        /// <param name="type">The type.</param>
        protected virtual void AddToTreeList(int val, int endIndex, int incr, int type)
        {
            ITreeList data = GetTreeList(type);

            if (type == Second || type == Minute)
            {
                if ((val < 0 || val > 59 || endIndex > 59) && (val != AllSpecValue))
                {
                    throw new FormatException(
                        "Minute and Second values must be between 0 and 59");
                }
            }
            else if (type == Hour)
            {
                if ((val < 0 || val > 23 || endIndex > 23) && (val != AllSpecValue))
                {
                    throw new FormatException(
                        "Hour values must be between 0 and 23");
                }
            }
            else if (type == DayOfMonth)
            {
                if ((val < 1 || val > 31 || endIndex > 31) && (val != AllSpecValue)
                    && (val != NoSpecValue))
                {
                    throw new FormatException(
                        "Day of month values must be between 1 and 31");
                }
            }
            else if (type == Month)
            {
                if ((val < 1 || val > 12 || endIndex > 12) && (val != AllSpecValue))
                {
                    throw new FormatException(
                        "Month values must be between 1 and 12");
                }
            }
            else if (type == DayOfWeek)
            {
                if ((val == 0 || val > 7 || endIndex > 7) && (val != AllSpecValue)
                    && (val != NoSpecValue))
                {
                    throw new FormatException(
                        "Day-of-Week values must be between 1 and 7");
                }
            }

            if ((incr == 0 || incr == -1) && val != AllSpecValue)
            {
                if (val != -1)
                {
                    data.Add(val);
                }
                else
                {
                    data.Add(NoSpec);
                }
                return;
            }

            int startAt = val;
            int stopAt = endIndex;
            bool appendAllSpec = false;

            if (val == AllSpecValue && incr <= 0)
            {
                if (type == Year)
                {
                    data.Add(AllSpec); // put in a marker, but also fill values
                }
                else
                {
                    appendAllSpec = true;
                }
                incr = 1;
            }

            if (type == Second || type == Minute)
            {
                if (stopAt == -1)
                {
                    stopAt = 59;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 0;
                }
            }
            else if (type == Hour)
            {
                if (stopAt == -1)
                {
                    stopAt = 23;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 0;
                }
            }
            else if (type == DayOfMonth)
            {
                if (stopAt == -1)
                {
                    stopAt = 31;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 1;
                }
            }
            else if (type == Month)
            {
                if (stopAt == -1)
                {
                    stopAt = 12;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 1;
                }
            }
            else if (type == DayOfWeek)
            {
                if (stopAt == -1)
                {
                    stopAt = 7;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 1;
                }
            }
            else if (type == Year)
            {
                if (stopAt == -1)
                {
                    stopAt = 2099;
                }
                if (startAt == -1 || startAt == AllSpecValue)
                {
                    startAt = 1970;
                }
            }

            for (int i = startAt; i <= stopAt; i += incr)
            {
                data.Add(i);
            }

            if (appendAllSpec)
            {
                data.Add(AllSpec); // put in a marker, but also fill values
            }
        }

        /// <summary>
        /// Gets the set of given type.
        /// </summary>
        /// <param name="type">The type of set to get.</param>
        /// <returns></returns>
        protected virtual ITreeList GetTreeList(int type)
        {
            switch (type)
            {
                case Second:
                    return seconds;
                case Minute:
                    return minutes;
                case Hour:
                    return hours;
                case DayOfMonth:
                    return daysOfMonth;
                case Month:
                    return months;
                case DayOfWeek:
                    return daysOfWeek;
                case Year:
                    return years;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="vc">The v.</param>
        /// <param name="str">The s.</param>
        /// <param name="index">The i.</param>
        /// <returns></returns>
        protected virtual ValueSet GetValue(int vc, string str, int index)
        {
            if (null == str)
                throw new ArgumentNullException("str");
            char c = str[index];
            string s1 = vc.ToString(CultureInfo.InvariantCulture);
            while (c >= '0' && c <= '9')
            {
                s1 += c;
                index++;
                if (index >= str.Length)
                {
                    break;
                }
                c = str[index];
            }
            ValueSet val = new ValueSet();
            if (index < str.Length)
            {
                val.Pos = index;
            }
            else
            {
                val.Pos = index + 1;
            }
            val.TheValue = Convert.ToInt32(s1, CultureInfo.InvariantCulture);
            return val;
        }

        /// <summary>
        /// Gets the numeric value from string.
        /// </summary>
        /// <param name="value">The string to parse from.</param>
        /// <param name="index">The i.</param>
        /// <returns></returns>
        protected virtual int GetNumericValue(string value, int index)
        {
            if (null == value)
                throw new ArgumentNullException("value");
            int endOfVal = FindNextWhiteSpace(index, value);
            string val = value.Substring(index, endOfVal - index);
            return Convert.ToInt32(val, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <param name="str">The string to map with.</param>
        /// <returns></returns>
        protected virtual int GetMonthNumber(string str)
        {
            if (monthMap.ContainsKey(str))
            {
                return (int)monthMap[str];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the day of week number.
        /// </summary>
        /// <param name="str">The s.</param>
        /// <returns></returns>
        protected virtual int GetDayOfWeekNumber(string str)
        {
            if (dayMap.ContainsKey(str))
            {
                return (int)dayMap[str];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the time from given time parts.
        /// </summary>
        /// <param name="sc">The seconds.</param>
        /// <param name="mn">The minutes.</param>
        /// <param name="hr">The hours.</param>
        /// <param name="dayofmn">The day of month.</param>
        /// <param name="mon">The month.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual Nullable<DateTime> GetTime(int sc, int mn, int hr, int dayofmn, int mon)
        {
            try
            {
                if (sc == -1)
                {
                    sc = 0;
                }
                if (mn == -1)
                {
                    mn = 0;
                }
                if (hr == -1)
                {
                    hr = 0;
                }
                if (dayofmn == -1)
                {
                    dayofmn = 0;
                }
                if (mon == -1)
                {
                    mon = 0;
                }
                return new DateTime(DateTime.UtcNow.Year, mon, dayofmn, hr, mn, sc);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the next fire time after the given time.
        /// </summary>
        /// <param name="afterTimeUtc">The UTC time to start searching from.</param>
        /// <returns></returns>
        public virtual Nullable<DateTime> GetTimeAfter(DateTime afterTimeUtc)
        {
            // move ahead one second, since we're computing the time *after/// the
            // given time
            afterTimeUtc = afterTimeUtc.AddSeconds(1);

            // CronTrigger does not deal with milliseconds
            DateTime d = CreateDateTimeWithoutMillis(afterTimeUtc);

            // change to specified time zone
            d = TimeZone.ToLocalTime(d);

            bool gotOne = false;
            // loop until we've computed the next time, or we've past the endTime
            while (!gotOne)
            {
                ITreeList st;
                int t;
                int sec = d.Second;

                // get second.................................................
                st = seconds.TailList(sec);
                if (st != null && st.Count != 0)
                {
                    sec = (int)st.First;
                }
                else
                {
                    sec = ((int)seconds.First);
                    d = d.AddMinutes(1);
                }
                d = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, sec, d.Millisecond);

                int min = d.Minute;
                int hr = d.Hour;
                t = -1;

                // get minute.................................................
                st = minutes.TailList(min);
                if (st != null && st.Count != 0)
                {
                    t = min;
                    min = ((int)st.First);
                }
                else
                {
                    min = (int)minutes.First;
                    hr++;
                }
                if (min != t)
                {
                    d = new DateTime(d.Year, d.Month, d.Day, d.Hour, min, 0, d.Millisecond);
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTime(d.Year, d.Month, d.Day, d.Hour, min, d.Second, d.Millisecond);

                hr = d.Hour;
                int day = d.Day;
                t = -1;

                // get hour...................................................
                st = hours.TailList(hr);
                if (st != null && st.Count != 0)
                {
                    t = hr;
                    hr = (int)st.First;
                }
                else
                {
                    hr = (int)hours.First;
                    day++;
                }
                if (hr != t)
                {
                    int daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
                    if (day > daysInMonth)
                    {
                        d = new DateTime(d.Year, d.Month, daysInMonth, d.Hour, 0, 0, d.Millisecond).AddDays(day - daysInMonth);
                    }
                    else
                    {
                        d = new DateTime(d.Year, d.Month, day, d.Hour, 0, 0, d.Millisecond);
                    }
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTime(d.Year, d.Month, d.Day, hr, d.Minute, d.Second, d.Millisecond);

                day = d.Day;
                int mon = d.Month;
                t = -1;
                int tmon = mon;

                // get day...................................................
                bool dayOfMSpec = !daysOfMonth.Contains(NoSpec);
                bool dayOfWSpec = !daysOfWeek.Contains(NoSpec);
                if (dayOfMSpec && !dayOfWSpec)
                {
                    // get day by day of month rule
                    st = daysOfMonth.TailList(day);
                    if (lastdayOfMonth)
                    {
                        if (!nearestWeekday)
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);
                        }
                        else
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);

                            DateTime tcal = new DateTime(d.Year, mon, day, 0, 0, 0);

                            int ldom = GetLastDayOfMonth(mon, d.Year);
                            DayOfWeek dow = tcal.DayOfWeek;

                            if (dow == System.DayOfWeek.Saturday && day == 1)
                            {
                                day += 2;
                            }
                            else if (dow == System.DayOfWeek.Saturday)
                            {
                                day -= 1;
                            }
                            else if (dow == System.DayOfWeek.Sunday && day == ldom)
                            {
                                day -= 2;
                            }
                            else if (dow == System.DayOfWeek.Sunday)
                            {
                                day += 1;
                            }

                            DateTime nTime = new DateTime(tcal.Year, mon, day, hr, min, sec, d.Millisecond);
                            if (nTime < afterTimeUtc)
                            {
                                day = 1;
                                mon++;
                            }
                        }
                    }
                    else if (nearestWeekday)
                    {
                        t = day;
                        day = (int)daysOfMonth.First;

                        DateTime tcal = new DateTime(d.Year, mon, day, 0, 0, 0);

                        int ldom = GetLastDayOfMonth(mon, d.Year);
                        DayOfWeek dow = tcal.DayOfWeek;

                        if (dow == System.DayOfWeek.Saturday && day == 1)
                        {
                            day += 2;
                        }
                        else if (dow == System.DayOfWeek.Saturday)
                        {
                            day -= 1;
                        }
                        else if (dow == System.DayOfWeek.Sunday && day == ldom)
                        {
                            day -= 2;
                        }
                        else if (dow == System.DayOfWeek.Sunday)
                        {
                            day += 1;
                        }

                        tcal = new DateTime(tcal.Year, mon, day, hr, min, sec);
                        if (tcal < afterTimeUtc)
                        {
                            day = ((int)daysOfMonth.First);
                            mon++;
                        }
                    }
                    else if (st != null && st.Count != 0)
                    {
                        t = day;
                        day = (int)st.First;

                        // make sure we don't over-run a short month, such as february
                        int lastDay = GetLastDayOfMonth(mon, d.Year);
                        if (day > lastDay)
                        {
                            day = (int)daysOfMonth.First;
                            mon++;
                        }
                    }
                    else
                    {
                        day = ((int)daysOfMonth.First);
                        mon++;
                    }

                    if (day != t || mon != tmon)
                    {
                        if (mon > 12)
                        {
                            d = new DateTime(d.Year, 12, day, 0, 0, 0).AddMonths(mon - 12);
                        }
                        else
                        {
                            d = new DateTime(d.Year, mon, day, 0, 0, 0);
                        }
                        continue;
                    }
                }
                else if (dayOfWSpec && !dayOfMSpec)
                {
                    // get day by day of week rule
                    if (lastdayOfWeek)
                    {
                        // are we looking for the last XXX day of
                        // the month?
                        int dow = ((int)daysOfWeek.First); // desired
                        // d-o-w
                        int cDow = ((int)d.DayOfWeek) + 1; // current d-o-w
                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        int lDay = GetLastDayOfMonth(mon, d.Year);

                        if (day + daysToAdd > lDay)
                        {
                            // did we already miss the
                            // last one?
                            d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            // we are promoting the month
                            continue;
                        }

                        // find date of last occurance of this day in this month...
                        while ((day + daysToAdd + 7) <= lDay)
                        {
                            daysToAdd += 7;
                        }

                        day += daysToAdd;

                        if (daysToAdd > 0)
                        {
                            d = new DateTime(d.Year, mon, day, 0, 0, 0);
                            // we are not promoting the month
                            continue;
                        }
                    }
                    else if (nthdayOfWeek != 0)
                    {
                        // are we looking for the Nth XXX day in the month?
                        int dow = ((int)daysOfWeek.First); // desired
                        // d-o-w
                        int cDow = ((int)d.DayOfWeek) + 1; // current d-o-w
                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        else if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        bool dayShifted = false;
                        if (daysToAdd > 0)
                        {
                            dayShifted = true;
                        }

                        day += daysToAdd;
                        int weekOfMonth = day / 7;
                        if (day % 7 > 0)
                        {
                            weekOfMonth++;
                        }

                        daysToAdd = (nthdayOfWeek - weekOfMonth) * 7;
                        day += daysToAdd;
                        if (daysToAdd < 0 || day > GetLastDayOfMonth(mon, d.Year))
                        {
                            d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            // we are promoting the month
                            continue;
                        }
                        else if (daysToAdd > 0 || dayShifted)
                        {
                            d = new DateTime(d.Year, mon, day, 0, 0, 0);
                            // we are NOT promoting the month
                            continue;
                        }
                    }
                    else
                    {
                        int cDow = ((int)d.DayOfWeek) + 1; // current d-o-w
                        int dow = ((int)daysOfWeek.First); // desired
                        // d-o-w
                        st = daysOfWeek.TailList(cDow);
                        if (st != null && st.Count > 0)
                        {
                            dow = ((int)st.First);
                        }

                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        int lDay = GetLastDayOfMonth(mon, d.Year);

                        if (day + daysToAdd > lDay)
                        {
                            // will we pass the end of
                            // the month?
                            d = new DateTime(d.Year, mon + 1, 1, 0, 0, 0);
                            // we are promoting the month
                            continue;
                        }
                        else if (daysToAdd > 0)
                        {
                            // are we swithing days?
                            d = new DateTime(d.Year, mon, day + daysToAdd, 0, 0, 0);
                            continue;
                        }
                    }
                }
                else
                {
                    // dayOfWSpec && !dayOfMSpec
                    throw new Exception(
                        "Support for specifying both a day-of-week AND a day-of-month parameter is not implemented.");
                }

                d = new DateTime(d.Year, d.Month, day, d.Hour, d.Minute, d.Second);
                mon = d.Month;
                int year = d.Year;
                t = -1;

                // test for expressions that never generate a valid fire date,
                // but keep looping...
                if (year > 2099)
                {
                    return null;
                }

                // get month...................................................
                st = months.TailList((mon));
                if (st != null && st.Count != 0)
                {
                    t = mon;
                    mon = ((int)st.First);
                }
                else
                {
                    mon = ((int)months.First);
                    year++;
                }
                if (mon != t)
                {
                    d = new DateTime(year, mon, 1, 0, 0, 0);
                    continue;
                }
                d = new DateTime(d.Year, mon, d.Day, d.Hour, d.Minute, d.Second);
                year = d.Year;
                t = -1;

                // get year...................................................
                st = years.TailList((year));
                if (st != null && st.Count != 0)
                {
                    t = year;
                    year = ((int)st.First);
                }
                else
                {
                    return null;
                } // ran out of years...

                if (year != t)
                {
                    d = new DateTime(year, 1, 1, 0, 0, 0);
                    continue;
                }
                d = new DateTime(year, d.Month, d.Day, d.Hour, d.Minute, d.Second);

                gotOne = true;
            } // while( !done )

            return d.ToUniversalTime();
        }

        /// <summary>
        /// Creates the date time without milliseconds.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        protected static DateTime CreateDateTimeWithoutMillis(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }


        /// <summary>
        /// Advance the calendar to the particular hour paying particular attention
        /// to daylight saving problems.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="hour">The hour.</param>
        /// <returns></returns>
        protected static DateTime SetCalendarHour(DateTime date, int hour)
        {
            // Java version of Quartz uses lenient calendar
            // so hour 24 creates day increment and zeroes hour
            int hourToSet = hour;
            if (hourToSet == 24)
            {
                hourToSet = 0;
            }
            DateTime d =
                new DateTime(date.Year, date.Month, date.Day, hourToSet, date.Minute, date.Second, date.Millisecond);
            if (hour == 24)
            {
                // inrement day
                d = d.AddDays(1);
            }
            return d;
        }

        /// <summary>
        /// Gets the time before.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        public virtual Nullable<DateTime> GetTimeBefore(Nullable<DateTime> endTime)
        {
            // TODO: implement
            return null;
        }

        /// <summary>
        /// NOT YET IMPLEMENTED: Returns the final time that the 
        /// <see cref="CronExpression" /> will match.
        /// </summary>
        /// <returns></returns>
        public virtual Nullable<DateTime> GetFinalFireTime()
        {
            // TODO: implement QUARTZ-423
            return null;
        }

        /// <summary>
        /// Determines whether given year is a leap year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        /// 	<c>true</c> if the specified year is a leap year; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        /// <summary>
        /// Gets the last day of month.
        /// </summary>
        /// <param name="monthNum">The month num.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        protected virtual int GetLastDayOfMonth(int monthNum, int year)
        {
            return DateTime.DaysInMonth(year, monthNum);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            CronExpression copy;
            try
            {
                copy = new CronExpression(CronExpressionString);
                copy.TimeZone = TimeZone;
            }
            catch (FormatException)
            {
                // never happens since the source is valid...
                throw new Exception("Not Cloneable.");
            }
            return copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnDeserialization(object sender)
        {
            BuildExpression(cronExpressionString);
        }
    }

    /// <summary>
    /// Helper class for cron expression handling.
    /// </summary>
    public class ValueSet
    {
        /// <summary>
        /// The value.
        /// </summary>
        public int TheValue { get; set; }

        /// <summary>
        /// The position.
        /// </summary>
        public int Pos { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITreeList : IList<int>
    {
        /// <summary>
        /// 获取列表中大于limit的元素列表
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        ITreeList TailList(int limit);

        /// <summary>
        /// 获取列表中第一个元素
        /// </summary>
        /// <returns></returns>
        int First
        {
            get;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TreeList : List<int>, ITreeList
    {
        #region ISortedList Members

        /// <summary>
        /// 
        /// </summary>
        public int First
        {
            get
            {
                return this[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ITreeList TailList(int limit)
        {
            var newList = new TreeList();

            var i = 0;
            while (i < Count && this[i] < limit)
            {
                i++;
            }
            for (; i < Count; i++)
            {
                newList.Add(this[i]);
            }

            return newList;
        }

        #endregion
    }
}
