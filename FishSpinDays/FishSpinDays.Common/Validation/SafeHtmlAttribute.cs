namespace FishSpinDays.Common.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Custom validation attribute for HTML content from rich text editors
    /// Ensures that dangerous content is rejected during model binding
    /// </summary>
    public class SafeHtmlAttribute : ValidationAttribute
    {
        public int MaxLength { get; set; } = 10000;
        public bool AllowEmpty { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return AllowEmpty ? ValidationResult.Success :  new ValidationResult("Content cannot be empty.");
            }

            var html = value.ToString();

            // Check length before processing
            if (html.Length > MaxLength)
            {
                return new ValidationResult($"Content too long. Maximum {MaxLength} characters allowed.");
            }

            // Basic dangerous content detection
            var dangerousPatterns = new[]
            {
                 "<script",
                 "javascript:",
                 "vbscript:",
                 "data:text/html",
                 "onload=",
                 "onerror=",
                 "onclick=",
                 "onmouseover=",
                 "<iframe[^>]+src=\"javascript:",
                 "<iframe[^>]+src=\"data:",
                 "<object",
                 "<embed[^>]+src=\"javascript:",
                 "<form[^>]*action=\"javascript:"
            };

            var lowerHtml = html.ToLowerInvariant();
            foreach (var pattern in dangerousPatterns)
            {
                if (lowerHtml.Contains(pattern.ToLowerInvariant()))
                {
                    return new ValidationResult($"Content contains potentially dangerous elements and cannot be saved.");
                }
            }

            // Additional validation: Check for excessive script-like content
            var scriptLikeRatio = CountScriptLikeContent(html);
            if (scriptLikeRatio > 0.3) // More than 30% script-like content
            {
                return new ValidationResult("Content appears to contain too much script-like content.");
            }

            // Basic HTML tag validation without full sanitization
            var htmlTagCount = Regex.Matches(html, @"<[^>]*>").Count;
            var suspiciousTagRatio = (double)htmlTagCount / html.Length;

            if (suspiciousTagRatio > 0.1) // More than 10% of content is HTML tags
            {
                return new ValidationResult("Content contains too much markup and may be invalid.");
            }

            return ValidationResult.Success;
        }

        private static double CountScriptLikeContent(string html)
        {
            var scriptIndicators = new[] { "function", "var ", "let ", "const ", "=", ";", "{", "}", "(", ")" };
            var count = scriptIndicators.Sum(indicator => html.Split(new[] { indicator }, StringSplitOptions.None).Length - 1);

            return Math.Min(1.0, (double)count / html.Length * 10);
        }
    }
}