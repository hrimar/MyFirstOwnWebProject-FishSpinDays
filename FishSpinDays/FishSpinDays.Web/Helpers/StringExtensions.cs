using FishSpinDays.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FishSpinDays.Web.Helpers
{
    public static class StringExtensions
    {
        public static string Truncate( this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static string GetOnlyTextFromDescription(this string description)
        {
            var imgTemplate = @"<img.*?\\?.*?>";

            var regex = new Regex(imgTemplate);
            var matched = regex.Match(description);

            var image = matched.Groups[0].Value;
            var result = Regex.Replace(description, imgTemplate, "");

            var youtubeTemplate = @"<iframe.*?<\/iframe>";

            var regexY = new Regex(imgTemplate);
            var matchedY = regexY.Match(result);

            var youtube = matched.Groups[0].Value;
            var finalResult = Regex.Replace(result, youtubeTemplate, "");

            string shortResult = Truncate(finalResult, WebConstants.DescriptinMaxLength);
            return shortResult;
        }
    }
}
