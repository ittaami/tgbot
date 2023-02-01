using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("5852401878:AAGe0M87gz5Z6T4X7uAlLx5VTKEiu7EmG4I");

            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message != null)
            {
                if (message.Text != null)
                {
                    Console.WriteLine($" {message.Chat.Username} |  {message.Text}");

                    if (message.Text != null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отправьте мне фотографию без сжатия (если с телефона - нажмите при отправке \"как файл\")");
                        return;
                    }

                }
            }

            if (message != null)
            {
                if (message.Document != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Обрабатываю... (~20сек)");

                    var fileId = update.Message.Document.FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;

                    string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";

                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await botClient.DownloadFileAsync(filePath, fileStream);

                    fileStream.Close();

                    Process.Start(new ProcessStartInfo(destinationFilePath)
                    {
                        FileName = @"D:\Коды\tgbot\tgbot\Безымянный.exe",
                        UseShellExecute = true,
                        Arguments = destinationFilePath
                    });

                    await Task.Delay(25000);

                    await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                    await botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, message.Document.FileName.Replace(message.Document.FileName, DateTime.Now.ToString() + ".png")));

                    return;
                }
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}