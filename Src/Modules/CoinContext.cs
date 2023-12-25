using HtmlAgilityPack;

namespace Src.Modules
{
    public class CoinContext
    {
        private static readonly Dictionary<string, string> CoinData = new Dictionary<string, string>
        {
            {"/btc", @"https://coinmarketcap.com/currencies/bitcoin/"},
            {"/eth", @"https://coinmarketcap.com/currencies/ethereum/" },
            {"/ton", @"https://coinmarketcap.com/currencies/toncoin/" },
            {"/bnb", @"https://coinmarketcap.com/currencies/bnb/" }
        };

        private static readonly Dictionary<string, string> CoinHistory = new Dictionary<string, string>
        {
            {"1d", "/html/body/div[1]/div[2]/div/div[2]/div/div/div[2]/div[4]/section[2]/div/div[4]/div[2]/div/div[1]/div[1]/span" },
            {"H", "/html/body/div[1]/div[2]/div/div[2]/div/div/div[2]/div[3]/section[2]/div/div[4]/div[2]/div/div[3]/div/div[2]/span" },
            {"L", "/html/body/div[1]/div[2]/div/div[2]/div/div/div[2]/div[3]/section[2]/div/div[4]/div[2]/div/div[4]/div/div[2]/span" }
        };

        private HtmlDocument GetParser(string url)
        {
            var html = new HtmlWeb();
            return html.Load(url);
        }

        public string GetPrice(string coinName)
        {
            try
            {
                if (CoinData.TryGetValue(coinName, out string url))
                {
                    Thread.Sleep(1000);
                    var coinPriceNode = GetParser(url)?.DocumentNode.SelectSingleNode("/html/body/div[1]/div[2]/div/div[2]/div/div/div[2]/div[1]/div[2]/span"); // Example XPath

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
            catch (HtmlWebException ex)
            {
                LogError($"An error occurred: {ex}");
            }

            return "N/A";
        }

        private string GetHistory(string coinName, string date)
        {
            try
            {
                if (CoinData.TryGetValue(coinName, out string url))
                {
                    Thread.Sleep(1000);
                    var coinHistoryNode = GetParser(url)?.DocumentNode.SelectSingleNode(CoinHistory[date]);

                    if (coinHistoryNode != null)
                    {
                        return coinHistoryNode.InnerText.Trim();
                    }
                    else
                    {
                        LogError($"No history element found for: {coinName}. XPath may need adjustment");
                    }
                }
                else
                {
                    LogError($"Invalid coin name: {coinName}");
                }
            }
            catch (HtmlWebException ex)
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
}