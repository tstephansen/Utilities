using System.Text.RegularExpressions;
using TextCopy;

namespace AsteriskFixer;

class Program
{
    static async Task Main(string[] args)
    {
        var printText = false;
        if (args.Length > 0)
        {
            if (args[0] == "-o")
                printText = true;
        }

        var origText = await GetClipboardText();
        if (string.IsNullOrEmpty(origText))
        {
            Console.WriteLine("Please provide a string to clean.");
            return;
        }
        var result = RemoveInnerAsterisks(FixMismatchedDelimiters(origText));
        if (printText)
            Console.WriteLine(result);
        await SetClipboardText(result);
    }

    private static async Task<string?> GetClipboardText()
    {
        try
        {
            return await ClipboardService.GetTextAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read from clipboard: {ex.Message}");
            return string.Empty;
        }
    }

    private static async Task SetClipboardText(string text)
    {
        try
        {
            await ClipboardService.SetTextAsync(text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to copy to clipboard: {ex.Message}");
        }
    }

    static string FixMismatchedDelimiters(string input)
    {
        return Regex.Replace(
            input,
            @"(?<=^|[\s""(])(?<open>[_*])(?<content>[^_*\r\n]+?)(?<close>[_*])(?=[\s"".,!?;:)]|$)",
            match =>
            {
                string open = match.Groups["open"].Value;
                string close = match.Groups["close"].Value;
                if (open == close)
                    return match.Value;

                return $"{open}{match.Groups["content"].Value}{open}";
            }
        );
    }

    static string RemoveInnerAsterisks(string input)
    {
        return Regex.Replace(
            input,
            @"\*(.*?)(?<!\*)\*(?=\s*[""\r\n]|$)",
            match =>
            {
                string content = match.Groups[1].Value;
                // Remove any nested *...* emphasis.
                content = Regex.Replace(
                    content,
                    @"\*([^*]+)\*",
                    "$1"
                );

                return $"*{content}*";
            },
            RegexOptions.Singleline
        );
    }
}
