namespace FishSpinDays.Web.Helpers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Linq;

    /// <summary>
    /// HTML Sanitization helper for safe rich text content
    /// Protects against XSS while preserving safe HTML formatting from Summernote editor
    /// </summary>
    public static class HtmlSanitizerHelper
    {
        private static readonly HtmlSanitizer sanitizer = new HtmlSanitizer();

        static HtmlSanitizerHelper()
        {
            // Configure allowed HTML tags for rich text (Summernote compatible)
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("h1");
            sanitizer.AllowedTags.Add("h2");
            sanitizer.AllowedTags.Add("h3");
            sanitizer.AllowedTags.Add("h4");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("ol");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("img");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("table");
            sanitizer.AllowedTags.Add("tr");
            sanitizer.AllowedTags.Add("td");
            sanitizer.AllowedTags.Add("th");
            sanitizer.AllowedTags.Add("thead");
            sanitizer.AllowedTags.Add("tbody");
            sanitizer.AllowedTags.Add("div");
            sanitizer.AllowedTags.Add("span");
            sanitizer.AllowedTags.Add("blockquote");

            // Added Video support
            sanitizer.AllowedTags.Add("iframe");
            sanitizer.AllowedTags.Add("video");
            sanitizer.AllowedTags.Add("source");
            sanitizer.AllowedTags.Add("embed");

            // Configure allowed attributes
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowedAttributes.Add("alt");
            sanitizer.AllowedAttributes.Add("title");
            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("style");
            sanitizer.AllowedAttributes.Add("width");
            sanitizer.AllowedAttributes.Add("height");

            // Added Video attributes
            sanitizer.AllowedAttributes.Add("frameborder");
            sanitizer.AllowedAttributes.Add("allowfullscreen");
            sanitizer.AllowedAttributes.Add("controls");
            sanitizer.AllowedAttributes.Add("autoplay");
            sanitizer.AllowedAttributes.Add("loop");
            sanitizer.AllowedAttributes.Add("muted");
            sanitizer.AllowedAttributes.Add("poster");
            sanitizer.AllowedAttributes.Add("type");

            // Configure allowed URL schemes (security)
            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedSchemes.Add("mailto");

            // Added Video URL schemes
            sanitizer.AllowedSchemes.Add("data"); // For data URLs

            // Configure allowed CSS properties (limited for security)
            sanitizer.AllowedCssProperties.Clear();
            sanitizer.AllowedCssProperties.Add("width");
            sanitizer.AllowedCssProperties.Add("height");
            sanitizer.AllowedCssProperties.Add("margin");
            sanitizer.AllowedCssProperties.Add("padding");
            sanitizer.AllowedCssProperties.Add("text-align");
            sanitizer.AllowedCssProperties.Add("color");
            sanitizer.AllowedCssProperties.Add("background-color");
            sanitizer.AllowedCssProperties.Add("float");
            sanitizer.AllowedCssProperties.Add("display");
            sanitizer.AllowedCssProperties.Add("border");
            sanitizer.AllowedCssProperties.Add("border-radius");

            // Added Video-specific URL validation
            sanitizer.FilterUrl += (sender, args) =>
            {
                // Allow trusted video domains
                var trustedDomains = new[]
                {
                    "youtube.com", "www.youtube.com", "youtu.be",
                    "vimeo.com", "www.vimeo.com",
                    "dailymotion.com", "www.dailymotion.com"
                };

                var uri = args.OriginalUrl;
                if (uri.StartsWith("//"))
                {
                    uri = "https:" + uri;
                }

                if (Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
                {
                    var host = parsed.Host.ToLowerInvariant();
                    if (!trustedDomains.Any(domain => host == domain || host.EndsWith("." + domain)))
                    {
                        // Block untrusted video domains
                        args.SanitizedUrl = null;
                    }
                }
            };

            // Remove dangerous attributes automatically
            sanitizer.RemovingAttribute += (sender, args) =>
            {
                // Log removed attributes for security monitoring if needed
                // This prevents onerror, onclick, onload, etc.
            };
        }

        /// <summary>
        /// Sanitizes HTML content to prevent XSS while preserving safe formatting
        /// </summary>
        /// <param name="html">Raw HTML content from rich text editor</param>
        /// <returns>Sanitized HTML safe for display</returns>
        public static string Sanitize(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return sanitizer.Sanitize(html);
        }

        /// <summary>
        /// Extension method for IHtmlHelper to sanitize and render HTML content safely
        /// </summary>
        /// <param name="htmlHelper">HTML Helper instance</param>
        /// <param name="html">Raw HTML content</param>
        /// <returns>Sanitized HTML content ready for rendering</returns>
        public static IHtmlContent SanitizeHtml(this IHtmlHelper htmlHelper, string html)
        {
            var sanitized = Sanitize(html);
            return new HtmlString(sanitized);
        }
    }
}