using System;
using System.Collections.Generic;
using System.Text;

namespace expense.manager
{
    public static class Extensions
    {
        public static string ToMonthId(this DateTime date)
        {
            return $"{date:MM/yyyy}";
        }
    }
}
