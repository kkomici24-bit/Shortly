using System.Text;
using System.Text.RegularExpressions;

var links = new List<ShortLink>
{
    new("openai-docs", "https://platform.openai.com/docs", 12),
    new("aspnet-course", "https://github.com/kkomici24-bit/Shortly", 4)
};

Console.WriteLine("SlugLab");
Console.WriteLine("Small side project for practicing slug generation and short-link management.");
Console.WriteLine();

while (true)
{
    Console.WriteLine("1. List links");
    Console.WriteLine("2. Create slug from URL title");
    Console.WriteLine("3. Add new short link");
    Console.WriteLine("4. Simulate click");
    Console.WriteLine("5. Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            ListLinks(links);
            break;
        case "2":
            CreateSlugPreview();
            break;
        case "3":
            AddLink(links);
            break;
        case "4":
            SimulateClick(links);
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Unknown option.");
            break;
    }

    Console.WriteLine();
}

static void ListLinks(IEnumerable<ShortLink> links)
{
    foreach (var link in links.OrderByDescending(x => x.ClickCount))
    {
        Console.WriteLine($"{link.Slug,-20} {link.ClickCount,3} clicks  ->  {link.DestinationUrl}");
    }
}

static void CreateSlugPreview()
{
    Console.Write("Enter a title or phrase: ");
    var input = Console.ReadLine() ?? string.Empty;
    Console.WriteLine($"Generated slug: {SlugGenerator.Create(input)}");
}

static void AddLink(List<ShortLink> links)
{
    Console.Write("Destination URL: ");
    var url = Console.ReadLine() ?? string.Empty;

    Console.Write("Display title: ");
    var title = Console.ReadLine() ?? string.Empty;

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        Console.WriteLine("Invalid URL.");
        return;
    }

    var slug = SlugGenerator.Create(title);
    if (links.Any(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase)))
    {
        slug = $"{slug}-{links.Count + 1}";
    }

    links.Add(new ShortLink(slug, url, 0));
    Console.WriteLine($"Added: /{slug}");
}

static void SimulateClick(List<ShortLink> links)
{
    Console.Write("Slug to click: ");
    var slug = Console.ReadLine() ?? string.Empty;

    var link = links.FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    if (link is null)
    {
        Console.WriteLine("Link not found.");
        return;
    }

    link.ClickCount++;
    Console.WriteLine($"Redirecting to: {link.DestinationUrl}");
    Console.WriteLine($"New click count: {link.ClickCount}");
}

sealed class ShortLink
{
    public ShortLink(string slug, string destinationUrl, int clickCount)
    {
        Slug = slug;
        DestinationUrl = destinationUrl;
        ClickCount = clickCount;
    }

    public string Slug { get; }
    public string DestinationUrl { get; }
    public int ClickCount { get; set; }
}

static class SlugGenerator
{
    public static string Create(string input)
    {
        var normalized = input.Trim().ToLowerInvariant();
        normalized = Regex.Replace(normalized, "[^a-z0-9\\s-]", string.Empty);
        normalized = Regex.Replace(normalized, "\\s+", " ");

        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            builder.Append(character == ' ' ? '-' : character);
        }

        var slug = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "new-link" : slug;
    }
}
