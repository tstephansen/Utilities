using System.Text;

namespace CaseChanger;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments specified.");
            return;
        }
        ParseText(args[0]);
    }

    private static void ParseText(string value)
    {
        var sb = new StringBuilder();
        var lowerText = value.ToLower();
        var splitText = lowerText.Split(" ");
        for(var i = 0; i < splitText.Length; i++)
        {
            var charArray = splitText[i].ToCharArray();
            for(var n = 0; n < charArray.Length; n++)
            {
                if (n == 0)
                    sb.Append(char.ToUpper(charArray[n]));
                else
                    sb.Append(charArray[n]);
            }
            sb.Append(' ');
        }
        var newText = sb.ToString();
        Console.WriteLine(newText);
    }
}