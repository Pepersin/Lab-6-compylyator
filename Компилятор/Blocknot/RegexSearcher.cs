using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RegexSearcher
{
    public class MatchInfo
    {
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return $"\"{Value}\" at position {StartIndex} (length {Length})";
        }
    }

    /// <summary>
    /// Находит все подстроки в тексте, соответствующие заданному регулярному выражению
    /// </summary>
    /// <param name="text">Текст для поиска</param>
    /// <param name="pattern">Регулярное выражение</param>
    /// <returns>Список найденных вхождений</returns>
    public static List<MatchInfo> FindMatches(string text, string pattern)
    {
        List<MatchInfo> matches = new List<MatchInfo>();

        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern))
            return matches;

        Regex regex;
        try
        {
            regex = new Regex(pattern);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Некорректное регулярное выражение: {ex.Message}");
        }

        foreach (Match match in regex.Matches(text))
        {
            if (match.Success)
            {
                matches.Add(new MatchInfo
                {
                    Value = match.Value,
                    StartIndex = match.Index,
                    Length = match.Length
                });
            }
        }

        return matches;
    }
}

