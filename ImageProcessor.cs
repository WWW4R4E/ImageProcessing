using System.Drawing;
/// <summary>
/// 在实现高斯模糊时，模糊程度和 sigma 值的选择对最终效果有重要影响。下面是对这两个参数的详细解释和建议的取值范围：

// 模糊程度（blurSize）
// 定义：模糊程度通常指的是高斯卷积核的大小。卷积核越大，模糊效果越明显。
// 取值范围：模糊程度必须是奇数。常见的取值范围是 3 到 21。例如，3、5、7、9、11、13、15、17、19、21。
// 注意事项：卷积核越大，计算量越大，处理时间也会相应增加。因此，在实际应用中需要根据具体需求和性能要求来选择合适的卷积核大小。
// Sigma 值
// 定义：sigma 是高斯分布的标准差，决定了高斯核的形状和模糊程度。较小的 sigma 值会导致更尖锐的高斯分布，模糊效果较弱；较大的 sigma 值会导致更平滑的高斯分布，模糊效果更强。
// 取值范围：sigma 的取值范围通常是 0.5 到 10。例如，0.5、1.0、1.5、2.0、2.5、3.0、3.5、4.0、4.5、5.0、5.5、6.0、6.5、7.0、7.5、8.0、8.5、9.0、9.5、10.0。
// 注意事项：sigma 值的选择应该与卷积核大小相匹配。通常，较大的卷积核可以使用较大的 sigma 值，以获得更明显的模糊效果。
// 示例
// 假设你选择的卷积核大小为 7，那么你可以选择 sigma 值为 1.5 到 3.0 之间，以获得合适的模糊效果。
/// </summary>
public static class ImageProcessor
{
    public static float[,] CreateGaussianKernel(int size, float sigma)
    {
        // 确保 size 是奇数
        if (size % 2 == 0)
        {
            throw new ArgumentException("Size must be an odd number.");
        }

        float[,] kernel = new float[size, size];
        float sum = 0.0f;

        int halfSize = size / 2;
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                kernel[x + halfSize, y + halfSize] = (float)Math.Exp(-(x * x + y * y) / (2.0 * sigma * sigma)) / (2 * (float)Math.PI * sigma * sigma);
                sum += kernel[x + halfSize, y + halfSize];
            }
        }

        // 归一化
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                kernel[x, y] /= sum;
            }
        }

        return kernel;
    }

    public static Bitmap ApplyGaussianBlur(Bitmap sourceBitmap, int blurSize, float sigma)
    {
        float[,] kernel = CreateGaussianKernel(blurSize, sigma);
        int width = sourceBitmap.Width;
        int height = sourceBitmap.Height;
        int halfSize = blurSize / 2;

        Bitmap blurredBitmap = new Bitmap(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color blurredColor = CalculateBlurredPixelColor(sourceBitmap, x, y, kernel, blurSize);
                blurredBitmap.SetPixel(x, y, blurredColor);
            }
        }

        return blurredBitmap;
    }

    private static Color CalculateBlurredPixelColor(Bitmap sourceBitmap, int x, int y, float[,] kernel, int blurSize)
    {
        int width = sourceBitmap.Width;
        int height = sourceBitmap.Height;
        int halfSize = blurSize / 2;

        float red = 0, green = 0, blue = 0;

        for (int ky = -halfSize; ky <= halfSize; ky++)
        {
            for (int kx = -halfSize; kx <= halfSize; kx++)
            {
                int pixelX = x + kx;
                int pixelY = y + ky;

                if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height)
                {
                    Color color = sourceBitmap.GetPixel(pixelX, pixelY);
                    red += color.R * kernel[ky + halfSize, kx + halfSize];
                    green += color.G * kernel[ky + halfSize, kx + halfSize];
                    blue += color.B * kernel[ky + halfSize, kx + halfSize];
                }
            }
        }

        return Color.FromArgb(
            Math.Min((int)red, 255),
            Math.Min((int)green, 255),
            Math.Min((int)blue, 255)
        );
    }

    public static Bitmap ApplyDarkBlurEffect(Bitmap sourceBitmap, int blurSize, float sigma)
    {
        Bitmap blurredBitmap = ApplyGaussianBlur(sourceBitmap, blurSize, sigma);
        int width = sourceBitmap.Width;
        int height = sourceBitmap.Height;

        Bitmap darkBlurBitmap = new Bitmap(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color originalColor = sourceBitmap.GetPixel(x, y);
                Color blurredColor = blurredBitmap.GetPixel(x, y);

                // 计算亮度
                int originalBrightness = (originalColor.R + originalColor.G + originalColor.B) / 3;
                int blurredBrightness = (blurredColor.R + blurredColor.G + blurredColor.B) / 3;

                // 如果原始亮度小于模糊亮度，则保留原始颜色
                if (originalBrightness < blurredBrightness)
                {
                    darkBlurBitmap.SetPixel(x, y, originalColor);
                }
                else
                {
                    darkBlurBitmap.SetPixel(x, y, blurredColor);
                }
            }
        }

        return darkBlurBitmap;
    }
}