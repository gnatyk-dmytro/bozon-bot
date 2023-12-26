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
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;

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

            using var cancellationTokenSource = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cancellationTokenSource.Token);

            var botSettings = await _botClient.GetMeAsync(cancellationTokenSource.Token);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"#{botSettings.Username} start working>>>\n");
            Console.ResetColor();

            await Task.Delay(-1, cancellationTokenSource.Token);
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
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("\nUndefined type of Message");
                        Console.ResetColor();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private static async Task HandleMessageUpdate(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
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
                await SendCryptoInfoKeyboard(botClient, chat);
            }
            else if ((message.Text == "/info") || (message.Text != null && message.Text.ToLower() == "info"))
            {
                await SendMessage(botClient, $"We are the bozon:io team working on cryptocurrencies,\nAnd our mission is to optimize data acquisition from the CRYPTOS\n\nFunctions:\n\n/follow - Follow on Updates\n/currency - Show all available Currency", chat, cancellationToken);
            }
            else if ((message.Text == "/follow") || (message.Text != null && message.Text.ToLower() == "follow"))
            {
                await SendMessage(botClient, "\nFollow function is not available at this moment", chat, cancellationToken);
            }
            else if ((message.Text == "/cryptoprice") || (message.Text != null && message.Text.ToLower() == "cryptoprice"))
            {
                await SendMessage(botClient, "Top 4 CryptoCurrency Price:\n\n", chat, cancellationToken);
                await SendMessage(botClient, $"#Bitcoin(btc) - {bitcoin.GetPrice("/btc")}\n#Ethereum(eth) - {ethereum.GetPrice("/eth")}\n#Bnbcoin(bnb) - {bnb.GetPrice("/bnb")}\n#Toncoin(ton) - {toncoin.GetPrice("/ton")}\n\nFor more information type - /all", chat, cancellationToken);
            }
        }

        private static async Task SendCryptoInfoKeyboard(ITelegramBotClient botClient, Chat chat)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("info"),
                        new KeyboardButton("follow")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("cryptoprice"),
                        new KeyboardButton("convert")
                    }
                })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(chat.Id, "Crypto currency info:", replyMarkup: replyKeyboard);
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

        private static async Task SendMessage(ITelegramBotClient botClient, string message, Chat chat, CancellationToken ctx)
        {
            await botClient.SendTextMessageAsync(chat!.Id, message, cancellationToken: ctx);
        }
    }
}