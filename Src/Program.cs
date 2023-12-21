using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Src.Modules;

namespace Src
{
    internal class Program
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;

        public static async Task Main(string[] args)
        {
            _botClient = new TelegramBotClient("TOKEN");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                },
                ThrowPendingUpdates = true
            };

            using var ctx = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, ctx.Token);
            var botSettings = await _botClient.GetMeAsync(ctx.Token);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"#{botSettings.Username} start working>>>\n");
            Console.ResetColor();

            await Task.Delay(-1, ctx.Token);
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken ctx)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var message = update.Message;
                        var chat = message!.Chat;
                        var from = message!.From;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"#{from!.Username} type text message: {message.Text}\n");
                        Console.ResetColor();

                        if (message.Text == "/start")
                        {
                            SendMessage(botClient, $"Welcome to the  #bozonBot:  @{from!.Username}\nType - /info for more Information", chat, ctx);
                        }
                        else if (message.Text == "/info")
                        {
                            SendMessage(botClient, $"We are the bozon:io team working on cryptocurrencies,\nAnd our mission is to optimize data acquisition from the CRYPTOS\n\nFunctions:\n\n/follow - Follow on Updates\n/currency - Show all available Currency", chat, ctx);
                        }
                        else if (message.Text == "/follow")
                        {
                            SendMessage(botClient, "\nFollow function is not available in this moment", chat, ctx);
                        }
                        else if (message.Text == "/currency")
                        {
                            CoinContext bitcoin = new CoinContext();
                            CoinContext ethereum = new CoinContext();
                            CoinContext bnb = new CoinContext();
                            CoinContext toncoin = new CoinContext();

                            SendMessage(botClient, "Top 4 CryptoCurrency Price:\n\n", chat, ctx);
                            SendMessage(botClient, $"#Bitcoin(btc) - {bitcoin.GetPrice("/btc")}\n#Ethereum(eth) - {ethereum.GetPrice("/eth")}\n#Bnbcoin(bnb) - {bnb.GetPrice("/bnb")}\n#Toncoin(ton) - {toncoin.GetPrice("/ton")}\n\nFor more information type - /all", chat, ctx);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken ctx)
        {
            var error = ex switch
            {
                ApiRequestException apiRequestException
                => "Telegram Api Request Exception" + apiRequestException,
                _ => ex.ToString()
            };

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(error);
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private static async void SendMessage(ITelegramBotClient botClient, string message, Chat? chat, CancellationToken ctx) 
        {
            await botClient.SendTextMessageAsync(
                chat!.Id,
                message,
                cancellationToken: ctx
                );
        }
    }
}