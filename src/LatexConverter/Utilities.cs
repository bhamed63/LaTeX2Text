namespace LatexConverter
{
    public static class Utilities
    {
        public static string SanitizeCommandType(string type)
        {
            return type.Replace(" ", "").Replace("/", "");
        }
    }
}
