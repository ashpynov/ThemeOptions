using System;

namespace ThemeOptions.Tools
{
    public class VersionComparer
    {
        public static int CompareVersions(string version1, string version2)
        {
            // Split the version strings into arrays of integers
            var v1 = Array.ConvertAll(version1.Split('.'), int.Parse);
            var v2 = Array.ConvertAll(version2.Split('.'), int.Parse);

            // Compare each part of the version
            for (int i = 0; i < Math.Max(v1.Length, v2.Length); i++)
            {
                int part1 = i < v1.Length ? v1[i] : 0;
                int part2 = i < v2.Length ? v2[i] : 0;

                if (part1 > part2)
                    return 1; // version1 is greater
                if (part1 < part2)
                    return -1; // version1 is lower
            }

            return 0; // versions are equal
        }

        public static bool MinimalVersion(string minVersion, string actualVersion)
        {
            return CompareVersions( minVersion, actualVersion) <= 0;
        }
    }
}