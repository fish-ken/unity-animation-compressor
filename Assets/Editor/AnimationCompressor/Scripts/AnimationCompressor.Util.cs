using System.Text.RegularExpressions;

namespace AnimationCompressor
{
    public static class Util
    {
        public static int GetDepth(string path)
        {
            var matches = Regex.Matches(path, "/");
            return matches.Count;
        }
    }
}