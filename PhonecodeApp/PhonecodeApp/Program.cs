using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PhonecodeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var lookup = new Dictionary<string, List<string>>();

            var lines = File.ReadAllLines("test.w");

            foreach (var line in lines)
            {
                var intString = EncodeToNumberString(line.Replace("\"", ""));
                List<string> currAliases;

                if (lookup.TryGetValue(intString, out currAliases))
                    currAliases.Add(line);
                else
                    lookup[intString] = new List<string> { line };
            }

            foreach (var k in lookup.Keys)
                Console.WriteLine($"{k}: {string.Join(", ", lookup[k])}");

            var testLines = File.ReadAllLines("test.t");

            Console.WriteLine();

            foreach (var line in testLines)
            {
                var cleanedLine = Regex.Replace(line, "[/-]", "");
                var encodings = GetEncodings(cleanedLine, lookup, true);

                foreach (var enc in encodings)
                    Console.WriteLine($"{line}: {enc}");
            }

            Console.ReadKey(false);
        }

        private static List<string> GetEncodings(string s, Dictionary<string, List<string>> lookup, bool allowSkip)
        {
            var result = new List<string>();

            for (int i = 1; i <= s.Length; i++)
            {
                var left = s.Substring(0, i);
                var right = s.Substring(i);

                List<string> leftEncodings;
                bool skipWasMade = false;

                if (!lookup.TryGetValue(left, out leftEncodings) && ((i == 0) && allowSkip))
                {
                    leftEncodings = new List<string> { left };
                    skipWasMade = true;
                }

                if (leftEncodings == null || !leftEncodings.Any())
                    continue;

                if (string.IsNullOrWhiteSpace(right))
                {
                    result.AddRange(leftEncodings);
                }
                else
                {
                    var rightEncodings = GetEncodings(right, lookup, skipWasMade);

                    if(i == s.Length-1 && !skipWasMade)
                        rightEncodings.Add(right);

                    foreach (var s2 in rightEncodings)
                        result.AddRange(leftEncodings.Select(s1 => s1 + " " + s2));
                }
            }

            return result;
        }

        private static string EncodeToNumberString(string s)
        {
            var sb = new StringBuilder();

            foreach (var c in s)
                sb.Append(EncodeChar(c));

            return sb.ToString();
        }

        private static char EncodeChar(char c)
        {
            switch (Char.ToLower(c))
            {
                case 'e':
                    return '0';

                case 'j':
                case 'n':
                case 'q':
                    return '1';

                case 'r':
                case 'w':
                case 'x':
                    return '2';

                case 'd':
                case 's':
                case 'y':
                    return '3';

                case 'f':
                case 't':
                    return '4';

                case 'a':
                case 'm':
                    return '5';

                case 'c':
                case 'i':
                case 'v':
                    return '6';

                case 'b':
                case 'k':
                case 'u':
                    return '7';

                case 'l':
                case 'o':
                case 'p':
                    return '8';

                case 'g':
                case 'h':
                case 'z':
                    return '8';

                default:
                    throw new ArgumentException("Unsupported char");
            }
        }
    }
}
