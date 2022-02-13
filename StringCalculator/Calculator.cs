using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
                
                try
                {   
                    // check for single custom delimiters like ';'               
                    delimiters.Add(splitOnFirstNewLine[0].Replace("//", string.Empty).Single().ToString());
                }
                catch (InvalidOperationException)
                {
                    // Delimiter sequence contains more than one element.
                    StringBuilder delimiterSequence = new StringBuilder(splitOnFirstNewLine[0]).Replace("//", string.Empty);

                    // check for custom delimiters of any length enclosed by "[]" e.g. "[*][%%][---]".
                    Regex regexForCustomDelimiters = new Regex($"{Regex.Escape("[")}[^{Regex.Escape("]")}]+{Regex.Escape("]")}");
                    
                    // Extract delimiters.
                    delimiters.AddRange(from Match customDelimiter in regexForCustomDelimiters.Matches(delimiterSequence.ToString())
                                        select customDelimiter.Value.Substring(1, customDelimiter.Value.Length - 2));
                }

                // split the number string at all known delimiters.
                numbers = splitOnFirstNewLine[1];
            }

            // Get a list of integers from the number string.
            var splitNumbers = numbers
                .Split(delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            // Get all negative numbers.
            var negativeNumbers = splitNumbers.Where(x => x < 0).ToList();

            // Throw an exception, if negative numbers are included
            // or return sum of integers, ignoring numbers greater than 1000.
            return negativeNumbers.Any()
                ? throw new Exception("Negatives not allowed: " + string.Join(",", negativeNumbers))
                : splitNumbers.Where(x => x <= 1000).Sum();
        }
    }
}