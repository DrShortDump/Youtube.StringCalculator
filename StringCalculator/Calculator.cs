using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringCalculator
{
    public class Calculator
    {
        public int Add(string numbers)
        {
            // Add default delimiters.
            var delimiters = new List<string> { ",", "\n" };

            // check for custom delimiters.
            if (numbers.StartsWith("//"))
            {
                // split the delimiter defnition(s) and the numbers.
                var splitOnFirstNewLine = numbers.Split(new[] { '\n' }, 2);

                // check for single custom delimiter like ';'
                try
                {
                    var customDelimiter = splitOnFirstNewLine[0].Replace("//", string.Empty).Single();
                    delimiters.Add($"{customDelimiter}");
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                    {
                        // Sequence contains more than one element.

                        StringBuilder sb = new StringBuilder(splitOnFirstNewLine[0]);
                        sb.Replace("//", string.Empty);

                        // custom delitmiter of any length e.g. "[*][%%][---]".
                        Regex reg = new Regex($"{Regex.Escape("[")}[^{Regex.Escape("]")}]+{Regex.Escape("]")}");
                        MatchCollection matches = reg.Matches(sb.ToString());
                        if (matches.Count > 0)
                        {
                            foreach (Match m in matches)
                            {
                                delimiters.Add(m.Value.Substring(1, m.Value.Length - 2));
                            }
                        }
                    }
                }

                // split the number string with all known delimiters.
                numbers = splitOnFirstNewLine[1];
            }

            // Get a list of integers from the number string.
            var splitNumbers = numbers
                .Split(delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            // Get all negative numberts.
            var negativeNumbers = splitNumbers.Where(x => x < 0).ToList();

            // Throw an exception, if negative numbers are included
            // or return sum of integers, ignoring numbers greater than 1000.
            return negativeNumbers.Any()
                ? throw new Exception("Negatives not allowed: " + string.Join(",", negativeNumbers))
                : splitNumbers.Where(x => x <= 1000).Sum();
        }
    }
}