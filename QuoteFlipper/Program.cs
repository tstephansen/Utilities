using TextCopy;

namespace QuoteFlipper;

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
            Console.WriteLine("Please provide a string to flip.");
            return;
        }
        var flippedText = origText.Replace("\"", "^");
        flippedText = flippedText.Replace("*", "\"");
        flippedText = flippedText.Replace("^", "*");
        if (printText)
            Console.WriteLine(flippedText);
        await SetClipboardText(flippedText);
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
}
