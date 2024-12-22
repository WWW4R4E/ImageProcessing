using System.Drawing;

public class WaterDropsEffect
{
    private const double Index0 = 1.0; // 空气
    private const double Index1 = 1.3333; // 水

    public Bitmap ApplyEffect(Bitmap inputImage, string filterImagePath)
    {
        // 读取图像
        var img = new Bitmap(inputImage);
        var filterImg = new Bitmap(filterImagePath);

        // 生成滤波器
        var filter = MakeFilter(img.Width, img.Height, filterImg);

        // 计算折射光线
        var raysOut = ComputeRefraction(filter);

        // 映射图像
        var finalImg = Mapping(raysOut, img, 500);

        return finalImg;
    }

    private double[,,] Dot(double[,,] a, double[,,] b)
    {
        int height = a.GetLength(0);
        int width = a.GetLength(1);
        double[,,] res = new double[height, width, 1];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                res[y, x, 0] = a[y, x, 0] * b[y, x, 0] + a[y, x, 1] * b[y, x, 1] + a[y, x, 2] * b[y, x, 2];
            }
        }

        return res;
    }

    private double[,,] Normalize(double[,,] arr)
    {
        int height = arr.GetLength(0);
        int width = arr.GetLength(1);
        double[,,] normArr = new double[height, width, 3];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double length = Math.Sqrt(arr[y, x, 0] * arr[y, x, 0] + arr[y, x, 1] * arr[y, x, 1] + arr[y, x, 2] * arr[y, x, 2]);
                if (length != 0)
                {
                    normArr[y, x, 0] = arr[y, x, 0] / length;
                    normArr[y, x, 1] = arr[y, x, 1] / length;
                    normArr[y, x, 2] = arr[y, x, 2] / length;
                }
            }
        }

        return normArr;
    }

    private double[,,] MakeFilter(int width, int height, Bitmap filterImg)
    {
        double[,,] arr = new double[filterImg.Height, filterImg.Width, 3];

        for (int y = 0; y < filterImg.Height; y++)
        {
            for (int x = 0; x < filterImg.Width; x++)
            {
                Color pixel = filterImg.GetPixel(x, y);
                arr[y, x, 0] = pixel.R - 128;
                arr[y, x, 1] = pixel.G - 128;
                arr[y, x, 2] = pixel.B - 128;
            }
        }

        double[,,] normal = Normalize(arr);

        int tileY = (int)Math.Ceiling((double)height / normal.GetLength(0));
        int tileX = (int)Math.Ceiling((double)width / normal.GetLength(1));

        double[,,] tiledNormal = new double[height, width, 3];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int ty = y % normal.GetLength(0);
                int tx = x % normal.GetLength(1);
                tiledNormal[y, x, 0] = normal[ty, tx, 0];
                tiledNormal[y, x, 1] = normal[ty, tx, 1];
                tiledNormal[y, x, 2] = normal[ty, tx, 2];
            }
        }

        return tiledNormal;
    }

    private double[,,] ComputeRefraction(double[,,] normImg)
    {
        int height = normImg.GetLength(0);
        int width = normImg.GetLength(1);
        double[,,] raysIn = new double[height, width, 3];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                raysIn[y, x, 2] = -1.0;
            }
        }

        double[,,] dn = Dot(raysIn, normImg);

        double[,,] raysOut0 = new double[height, width, 3];
        double[,,] raysOut1 = new double[height, width, 3];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                raysOut0[y, x, 0] = (raysIn[y, x, 0] - (normImg[y, x, 0] * dn[y, x, 0])) * (Index0 / Index1);
                raysOut0[y, x, 1] = (raysIn[y, x, 1] - (normImg[y, x, 1] * dn[y, x, 0])) * (Index0 / Index1);
                raysOut0[y, x, 2] = (raysIn[y, x, 2] - (normImg[y, x, 2] * dn[y, x, 0])) * (Index0 / Index1);

                raysOut1[y, x, 0] = normImg[y, x, 0] * Math.Sqrt(1 - ((Index0 * Index0) / (Index1 * Index1)) * (1 - dn[y, x, 0] * dn[y, x, 0]));
                raysOut1[y, x, 1] = normImg[y, x, 1] * Math.Sqrt(1 - ((Index0 * Index0) / (Index1 * Index1)) * (1 - dn[y, x, 0] * dn[y, x, 0]));
                raysOut1[y, x, 2] = normImg[y, x, 2] * Math.Sqrt(1 - ((Index0 * Index0) / (Index1 * Index1)) * (1 - dn[y, x, 0] * dn[y, x, 0]));
            }
        }

        // 创建一个新的三维数组来存储逐元素相减的结果
        double[,,] raysOutDiff = new double[height, width, 3];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    raysOutDiff[y, x, z] = raysOut0[y, x, z] - raysOut1[y, x, z];
                }
            }
        }

        return Normalize(raysOutDiff);
    }

    private Bitmap Mapping(double[,,] rays, Bitmap img, double distance)
    {
        int height = img.Height;
        int width = img.Width;
        Bitmap outImg = new Bitmap(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double depth = distance / (-1 * rays[y, x, 2]);
                double mappedX = x + rays[y, x, 0] * depth;
                double mappedY = y + rays[y, x, 1] * depth;

                if (mappedY < 0 || mappedY >= height || mappedX < 0 || mappedX >= width)
                {
                    outImg.SetPixel(x, y, Color.Black);
                }
                else
                {
                    outImg.SetPixel(x, y, img.GetPixel((int)mappedX, (int)mappedY));
                }
            }
        }

        return outImg;
    }

}