using System.Drawing;
// 定义新的最大宽度和高度为4K
int maxWidthOrHeight = 4096;
int blurSize;
// 读取images文件
string path = "images";
Console.WriteLine("请输入要执行的操作：" + "\n" + "1. 重命名文件" + "\n" + "2. 调整图片大小" + "\n" + "3. 模糊图片" + "\n" + "4. 使用 OpenCV 库模糊图片" + "\n" + "5. 使用 OpenCV 库毛玻璃处理图片" + "\n" + "6. 使用自定义算法添加水滴效果");
// 获取输入控制台参数
string Do = Console.ReadLine();
switch (Do)
{
    case "1":
        Rename.RenameFile(path);
        break;
    case "2":
        Resize.ResizeImages(maxWidthOrHeight, path);
        break;
    case "3":
        Console.WriteLine("请输入模糊程度:");
        blurSize = Convert.ToInt32(Console.ReadLine());
        if (blurSize % 2 == 0)
        {
            Console.WriteLine("模糊程度必须是奇数。");
            return;
        }
        Console.WriteLine("请输入高斯模糊的sigma值:");
        float sigma = Convert.ToSingle(Console.ReadLine());
        foreach (string filepath in Directory.GetFiles(path))
        {
            Bitmap sourceBitmap = new Bitmap(filepath);
            Bitmap darkBlurBitmap = ImageProcessor.ApplyDarkBlurEffect(sourceBitmap, blurSize, sigma);

            // 构建输出文件路径
            string outputFileName = $"{blurSize}_{sigma}_{Path.GetFileName(filepath)}";
            string outputFilePath = Path.Combine("out", outputFileName);

            // 确保输出目录存在
            Directory.CreateDirectory("out");

            // 保存处理后的图像
            darkBlurBitmap.Save(outputFilePath);
            Console.WriteLine($"暗色模糊处理完成: {outputFilePath}");
        }
        break;
        case "4":
        {
            OpenCVs openCVs = new OpenCVs();
            openCVs.ApplyGaussianBlur();
            break;
        }
    case "5":
        {
            OpenCVs openCVs = new OpenCVs();
            openCVs.ApplyAcrylicEffect();
            break;
        }
    case "6":
        {
            WaterDropsEffect waterDropsEffect = new WaterDropsEffect();
            Image result = waterDropsEffect.ApplyEffect(new Bitmap("images/test.png"), "filter1.png");
            result.Save("out/result.png");
            break;
        }
    default:
        Console.WriteLine("输入有误");
        break;
}