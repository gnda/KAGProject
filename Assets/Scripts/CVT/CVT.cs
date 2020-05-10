using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CVT
{
    public static void run(int processNumber = 1, 
        int generatorNumber = 2, int iterationNumber = 100)
    {
        Process process = new Process();
        process.StartInfo.FileName = "mpiexec.exe";
        
        string cvt2DPath = Application.dataPath + "/CVT2D/";
        string processPath = cvt2DPath + "CVT2D.exe";
        string inputPath = cvt2DPath + "input/" + "input.boundary";
        string outputPath = cvt2DPath + "output/";

        process.StartInfo.Arguments = $"-n {processNumber} {processPath} {inputPath} " +
                                      $"{generatorNumber} {outputPath} {iterationNumber}";
    
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
    
        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();

        UnityEngine.Debug.Log(output);
    
        process.WaitForExit();
        process.Close();
    }
}