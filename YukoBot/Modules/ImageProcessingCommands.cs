using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Modules
{
    [Name("image processing")]
    public class ImageProcessingCommands : ModuleBase<SocketCommandContext>
    {
        private const string DefaultASCII = "`^\",:;Il!i~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
        private static readonly HashSet<string> imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ ".png", ".jpg", ".jpeg", ".bmp", ".ico" };

        private async Task<Bitmap> GetImage()
        {
            Attachment attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null || !imageExtensions.Contains(Path.GetExtension(attachment.Filename)))
            {
                return null;
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    Bitmap image;
                    using (Stream imageStream = await client.OpenReadTaskAsync(new Uri(attachment.Url)))
                        image = new Bitmap(imageStream);
                    return image;
                }
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        [Command("ascii")]
        [Summary("Create ASCII art from an image, send attachment of image with message.")]
        public async Task ASCII()
        {
            Bitmap image = await GetImage();
            if (image == null)
            {
                await ReplyAsync("Please provide an image attachment.");
                return;
            }

            string pixelCharacters = DefaultASCII;

            List<char> chars = new List<char>(image.Height * image.Width + image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    System.Drawing.Color pixel = image.GetPixel(x, y);
                    int normalizedGrayscale = (int)(pixel.GetBrightness() * (pixelCharacters.Length - 1));
                    chars.Add(pixelCharacters[normalizedGrayscale]);
                }
                chars.Add('\n');
            }

            // Find way to not have the memory being copied around
            using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(new string(chars.ToArray()))))
            {
                await Context.Message.Channel.SendFileAsync(memoryStream, $"${Context.Message.Attachments.First().Filename}ASCII.txt");
            }

            image.Dispose();
        }

        [Command("grayscale"), Alias("gray")]
        [Summary("Convert an image to grayscale.")]
        public async Task Grayscale()
        {
            Bitmap image = await GetImage();
            if (image == null)
            {
                await ReplyAsync("Please provide an image attachment.");
                return;
            }

            Bitmap gray = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    System.Drawing.Color pixel = image.GetPixel(x, y);
                    byte brightness = (byte)(pixel.GetBrightness() * 255);
                    gray.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.A, brightness, brightness, brightness));
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                gray.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, $"{Context.Message.Attachments.First().Filename}-grayscale.png");
            }

            image.Dispose();
            gray.Dispose();
        }
    }
}
