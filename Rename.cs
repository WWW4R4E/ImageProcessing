using System;

public class Rename
{
    public static string RenameFile(string folder)
    {
        int fileCount = 1;
        foreach (string file in System.IO.Directory.GetFiles(folder))
        {
            string extension = System.IO.Path.GetExtension(file);
            string newFileName = fileCount.ToString("D3") + extension; // 生成三位数的编号，不足三位前面补0
            string newFilePath = System.IO.Path.Combine(folder, newFileName);

            System.IO.File.Move(file, newFilePath); // 重命名文件
            fileCount++;
        }
        return "ok";
    }
}
