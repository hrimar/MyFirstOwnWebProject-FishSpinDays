namespace FishSpinDays.Common.Extensions
{
    using FishSpinDays.Common.Constants;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Truncate( this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static string GetOnlyTextFromDescription(this string description)
        {
            var imgTemplate = @"<.*?>";
            var regex = new Regex(imgTemplate);
            var matched = regex.Match(description);

            var image = matched.Groups[0].Value;
            var result = Regex.Replace(description, imgTemplate, "");

            string shortResult = Truncate(result, WebConstants.DescriptinMaxLength);
            return shortResult;            
        }
    }
}
