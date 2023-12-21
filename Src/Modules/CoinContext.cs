using HtmlAgilityPack;

namespace Src.Modules;

public class CoinContext
{
    public static readonly Dictionary<string, string> CoinData = new Dictionary<string, string>
        {
            {"/btc", @"https://coinmarketcap.com/currencies/bitcoin/"},
            {"/eth", @"https://coinmarketcap.com/currencies/ethereum/" },
            {"/ton", @"https://coinmarketcap.com/currencies/toncoin/" },
            {"/bnb", @"https://coinmarketcap.com/currencies/bnb/" }
        };

    public string GetPrice(string coinName)
    {
        try
        {
            if (CoinData.TryGetValue(coinName, out string url))
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                var htmlDoc = htmlWeb.Load(url);

                var coinPriceNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div[2]/div/div[2]/div/div/div[2]/div[1]/div[2]/span"); // Example XPath

                Thread.Sleep(100);

                if (coinPriceNode != null)
                {
                    return coinPriceNode.InnerText.Trim();
                }
                else
                {
                    LogError($"No price element found for {coinName}. XPath may need adjustment.");
                }
            }
            else
            {
                LogError($"Invalid coin name: {coinName}");
            }
        }
        catch (Exception ex)
        {
            LogError($"An error occurred: {ex}");
        }

        return "N/A";
    }

    private static void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}