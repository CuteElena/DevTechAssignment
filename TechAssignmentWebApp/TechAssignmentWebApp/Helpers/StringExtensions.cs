using System.Text.RegularExpressions;

namespace TechAssignmentWebApp.Helpers
{
    public static class StringExtensions
    {
        public static string Titleize(this string text)
        {
            //var returnValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
            var returnValue = text;
            returnValue = returnValue.ToSentenceCase();
            return returnValue;
        }

        public static string ToSentenceCase(this string str)
        {
            var returnValue = Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToUpper(m.Value[1]));
            return returnValue;
        }
    }
}