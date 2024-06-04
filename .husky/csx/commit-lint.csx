/// <summary>
/// Commit linter
/// https://www.conventionalcommits.org/en/v1.0.0/
/// https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#type
/// </summary>

#r "System.dll"
#r "System.Linq.dll"

using System;
using System.Linq;
using System.Text.RegularExpressions;

private static var tasksPrefixes = new [] { "BKL" };

private static var allowedCommitTypes = new []
{
    "feat",
    "bug",
    "ci",
    "wip",
    "fix",
    "perf",
    "refactor",
    "docs",
    "build",
    "chore",
    "revert",
    "style",
    "test"
};

private var pattern = $@"^(?=.{{1,90}}$)(?:{string.Join("|", allowedCommitTypes)})(?:\((?<scope>.+)\)|)(|!)(?::)\s.{{4,}}(?<![\.\s])$";

private var msg = File.ReadAllLines(Args[0])[0];

private var match = Regex.Match(msg, pattern);

if (!match.Success)
{
   return Fail(msg);
}

private var scope = match.Groups["scope"]?.Value;

if (string.IsNullOrWhiteSpace(scope))
{
    return 0;
}

private var isTaskScope = tasksPrefixes.Any(tp => scope?.StartsWith($"{tp}-") == true);

if (isTaskScope)
{
   return 0;
}

return Fail(msg);

private static int Fail(string msg)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Invalid commit message: {0}", msg);
    Console.ResetColor();
    Console.Write($"commit type must be one of [{string.Join(", ", allowedCommitTypes)}]");

    if (tasksPrefixes.Length > 0)
    {
        Console.WriteLine($" or [{string.Join("|", tasksPrefixes.Select(tp => tp + "-XXX"))}] ");
        Console.WriteLine($"e.g: '{allowedCommitTypes[0]}({tasksPrefixes[0]}-1234): subject' or '{allowedCommitTypes[0]}: subject'");
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine($"e.g: '{allowedCommitTypes[0]}: subject'");
    }

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("more info: https://www.conventionalcommits.org/en/v1.0.0/");

    return 1;
}
