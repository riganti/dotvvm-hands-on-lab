using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DotvvmBlog.BL.Services
{
    public class HtmlUtils
    {
        public static string GetTextFromHtml(string html)
        {
            // this is not reliable, we should use a proper HTML parser instead
            return Regex.Replace(html, @"\<[^\>]+\>", " ");
        }

        public static string Ellipsis(string text, int maxLength)
        {
            if (text.Length < maxLength - 3)
            {
                return text;
            }
            return text.Substring(0, maxLength - 3) + "...";
        }

        public static string GetHtmlFromText(string text)
        {
            // to preserve line endings, we need to HTML-encode each line separately
            using (var sr = new StringReader(text))
            {
                var output = new StringBuilder();
                output.Append("<p>");

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    output.Append(WebUtility.HtmlEncode(line));
                    output.Append("<br />");
                }

                output.Append("</p>");
                return output.ToString();
            }
        }
    }
}