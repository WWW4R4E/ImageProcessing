using System;using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

public class Resize
{
    public static void ResizeImages(int maxWidthOrHeight, string path)
    {
        string[] files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            try
            {
                using (Image image = Image.Load(file))
                {
                    // 计算新的尺寸，保持宽高比
                    var newSize = CalculateNewSize(image.Size, maxWidthOrHeight);

                    // 调整大小
                    image.Mutate(x => x.Resize(newSize.Width, newSize.Height));

                    // 保存调整大小后的图像到文件，指定文件扩展名
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    Console.WriteLine("fileName:" + fileName);
                    string extension = Path.GetExtension(file);
                    Console.WriteLine("extension:" + extension);
                    string newName = fileName + "_4k" + extension;
                    Console.WriteLine("newName:" + newName);
                    EnsureDirectoryExists("newImages");
                    image.Save("newImages/" + newName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    private static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static Size CalculateNewSize(Size size, int maxLength)
    {
        if (size.Width > maxLength || size.Height > maxLength)
        {
            if (size.Width > size.Height)
            {
                return new Size(maxLength, (int)((float)size.Height / size.Width * maxLength));
            }
            else
            {
                return new Size((int)((float)size.Width / size.Height * maxLength), maxLength);
            }
        }
        // 如果原始尺寸已经小于或等于最大长度，则返回原始尺寸
        return size;
    }
}