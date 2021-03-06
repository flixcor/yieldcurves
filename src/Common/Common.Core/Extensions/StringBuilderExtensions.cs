﻿using System.Linq;
using System.Text;

namespace Common.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendFormatNonEmptyString(this StringBuilder stringBuilder, string? format, params object?[] values)
        {
            var valuesToUse = values.Where(s => !string.IsNullOrWhiteSpace(s?.ToString()));

            foreach (var value in valuesToUse)
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    stringBuilder.AppendFormat(format, value);
                }
                else
                {
                    stringBuilder.Append(value);
                }
            }
        }
    }
}
