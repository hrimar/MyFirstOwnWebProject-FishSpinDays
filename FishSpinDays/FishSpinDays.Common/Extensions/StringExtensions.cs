namespace FishSpinDays.Common.Extensions
{
    using FishSpinDays.Common.Constants;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        /// <summary>
        /// Extracts only text content from HTML description for search results and previews
        /// This method is used when we need plain text (e.g., search results)
        /// For full display, use HTML sanitization instead
        /// </summary>
        /// <param name="description">HTML description from rich text editor</param>
        /// <returns>Plain text with truncation for preview</returns>
        public static string GetOnlyTextFromDescription(this string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            // Remove HTML tags more comprehensively
            var htmlTagPattern = @"<[^>]*>";
            var result = Regex.Replace(description, htmlTagPattern, " ");

            // Clean up multiple spaces and normalize whitespace
            result = Regex.Replace(result, @"\s+", " ").Trim();

            // Decode common HTML entities
            result = result.Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&#39;", "'");

            string shortResult = Truncate(result, WebConstants.DescriptinMaxLength);
            return shortResult;
        }
    }
}
