using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Src.Modules;

namespace Src
{
    internal class Program
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;

        public static async Task Main(string[] args)
        {
            InitializeBotClient();
            SetupReceiverOptions();

            using var cancellationTokenSource = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cancellationTokenSource.Token);

            var botSettings = await _botClient.GetMeAsync(cancellationTokenSource.Token);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"#{botSettings.Username} start working>>>\n");
            Console.ResetColor();

            await Task.Delay(-1, cancellationTokenSource.Token);
        }

        private static void InitializeBotClient()
        {
            _botClient = new TelegramBotClient("TOKEN");
        }

        private static void SetupReceiverOptions()
        {
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                },
                ThrowPendingUpdates = true
            };
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessageUpdate(botClient, update.Message, cancellationToken);
                        break;

                    case UpdateType.CallbackQuery:
                        await KeyboardUpdates(botClient, update, cancellationToken);
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

        private static async Task HandleMessageUpdate(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message == null || message.From == null || message.Chat == null)
                return;

            var chat = message.Chat;
            var from = message.From;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"#{from.Username} type text message: {message.Text}\n");
            Console.ResetColor();

            var bitcoin = new CoinContext();
            var ethereum = new CoinContext();
            var bnb = new CoinContext();
            var toncoin = new CoinContext();

            if (message.Text == "/start")
            {
                await SendMessage(botClient, $"Welcome to the #bozonBot: @{from.Username}\nType - /info for more Information", chat, cancellationToken);
            }
            else if (message.Text == "/info")
            {
                await SendMessage(botClient, $"We are the bozon:io team working on cryptocurrencies,\nAnd our mission is to optimize data acquisition from the CRYPTOS\n\nFunctions:\n\n/follow - Follow on Updates\n/currency - Show all available Currency", chat, cancellationToken);
            }
            else if (message.Text == "/follow")
            {
                await SendMessage(botClient, "\nFollow function is not available at this moment", chat, cancellationToken);
            }
            else if (message.Text == "/currency")
            {
                await SendMessage(botClient, "Top 4 CryptoCurrency Price:\n\n", chat, cancellationToken);
                await SendMessage(botClient, $"#Bitcoin(btc) - {bitcoin.GetPrice("/btc")}\n#Ethereum(eth) - {ethereum.GetPrice("/eth")}\n#Bnbcoin(bnb) - {bnb.GetPrice("/bnb")}\n#Toncoin(ton) - {toncoin.GetPrice("/ton")}\n\nFor more information type - /all", chat, cancellationToken);
            }
            else if (message.Text == "/all")
            {
                await SendCryptoInfoKeyboard(botClient, chat);
            }
        }

        private static async Task SendCryptoInfoKeyboard(ITelegramBotClient botClient, Chat chat)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("bitcoin"),
                        new KeyboardButton("ethereum")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("bnbcoin"),
                        new KeyboardButton("toncoin")
                    }
                })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chat.Id,
                "Crypto currency info:",
                replyMarkup: replyKeyboard
            );
        }

        private static async Task KeyboardUpdates(ITelegramBotClient client, Update update, CancellationToken ctx)
        {
            var callbackQuery = update.CallbackQuery;
            var user = callbackQuery.From;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"#{user.Username} type the button: {callbackQuery.Data}\n");
            Console.ResetColor();
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            var error = ex switch
            {
                ApiRequestException apiRequestException => "Telegram Api Request Exception" + apiRequestException,
                _ => ex.ToString()
            };

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(error);
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private static async Task SendMessage(ITelegramBotClient botClient, string message, Chat chat, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                message,
                cancellationToken: cancellationToken
            );
        }
    }
}