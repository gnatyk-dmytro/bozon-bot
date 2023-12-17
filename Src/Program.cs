using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Src
{
    internal class Program
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;

        public static async Task Main(string[] args)
        {
            _botClient = new TelegramBotClient("YOUR_TOKEN");
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
                            await botClient.SendTextMessageAsync
                                (
                                chat.Id,
                                $"Welcome to the  #bozonBot:  @{from!.Username}\nType - /info for more Information",
                                cancellationToken: ctx
                                );
                        }
                        else if (message.Text == "/info")
                        {
                            await botClient.SendTextMessageAsync
                                (
                                chat!.Id,
                                $"We are the bozon:io team working on cryptocurrencies,\nAnd our mission is to optimize data acquisition from the CRYPTOS\n\nFunctions:\n\n/follow - Follow on Updates\n/currency - Show all available Currency",
                                cancellationToken: ctx
                                );
                        }
                        else if (message.Text == "/currency")
                        {
                            await botClient.SendTextMessageAsync(
                                chat!.Id,
                                $"All Currency:\n\nBitcoin - /bitcoin\nEthereum - /eth\nToncoin - /ton\nBNB - /bnb",
                                cancellationToken: ctx
                            );
                        }
                        else if (message.Text == "/follow")
                        {
                            await botClient.SendTextMessageAsync(
                                chat!.Id,
                                "\nFollow function is not available in this moment",
                            cancellationToken: ctx
                            );
                        }
                        else if (message.Text == "/bitcoin")
                        {
                            await botClient.SendTextMessageAsync(
                                chat!.Id,
                                "Bitcoin price:",
                            cancellationToken: ctx
                                );
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
    }
}