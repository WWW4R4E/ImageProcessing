using OpenCvSharp;

public class OpenCVs
{
    int blurSize;
    float sigma;
    public void ApplyGaussianBlur()
    {
        // string filepath = "images/test.png";
        string filepath = "images/1.png";
        // 加载图像
        Mat sourceMat = Cv2.ImRead(filepath);

        System.Console.WriteLine("请输入模糊程度:");
        blurSize = Convert.ToInt32(System.Console.ReadLine());
        System.Console.WriteLine("请输入高斯模糊的sigma值:");
        sigma = Convert.ToSingle(System.Console.ReadLine());
        // 应用高斯模糊
        Mat blurredMat = new Mat();
        Cv2.GaussianBlur(sourceMat, blurredMat, new Size(blurSize, blurSize), sigma);
        string outputFilePath = $"out/{blurSize}_{sigma}_{Path.GetFileName(filepath)}";
        // 保存处理后的图像
        Cv2.ImWrite(outputFilePath, blurredMat);
    }
    public void ApplyAcrylicEffect()
    {
        string filepath = "images/test.png";
        // 加载图像
        Mat sourceMat = Cv2.ImRead(filepath);



        while (true)
        {
            Console.WriteLine("请输入模糊程度 (必须是大于 0 的奇数):");
            string blurInput = Console.ReadLine();
            if (int.TryParse(blurInput, out blurSize) && blurSize > 0 && blurSize % 2 == 1)
            {
                break;
            }
            Console.WriteLine("输入无效，请输入一个大于 0 的奇数。");
        }

        while (true)
        {
            Console.WriteLine("请输入高斯模糊的 sigma 值:");
            string sigmaInput = Console.ReadLine();
            if (float.TryParse(sigmaInput, out sigma) && sigma > 0)
            {
                break;
            }
            Console.WriteLine("输入无效，请输入一个大于 0 的数值。");
        }

        // 应用高斯模糊
        Mat blurredMat = new Mat();
        Cv2.GaussianBlur(sourceMat, blurredMat, new Size(blurSize, blurSize), sigma);

        // 调整透明度
        double alpha = 1; // 透明度系数，范围 0.0 到 1.0
        Mat blendedMat = new Mat();
        Cv2.AddWeighted(blurredMat, alpha, sourceMat, 1 - alpha, 0, blendedMat);

        string outputFilePath = $"out/{blurSize}_{sigma}_{Path.GetFileName(filepath)}";
        // 确保输出目录存在
        Directory.CreateDirectory("out");

        // 保存处理后的图像
        Cv2.ImWrite(outputFilePath, blendedMat);
        Console.WriteLine($"毛玻璃效果处理完成: {outputFilePath}");
    }
}