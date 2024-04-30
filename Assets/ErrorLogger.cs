using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ErrorLogger : MonoBehaviour
{
    private string filePath;
    private StreamWriter writer;

    /*void Start()
    {
        *//*filePath = Application.dataPath + "/Rigidbody2D_Log.csv";*//*
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        filePath = Path.Combine(desktopPath, "Rigidbody2D_ErrorsLog.csv");
        writer = new StreamWriter(filePath);
        writer.WriteLine("Err_Bridge,Err_Size");
        Dictionary<string, string> errors = FindObjectOfType<CarController>().errorInBridge;
        print(errors.FirstOrDefault());
        string data = errors.FirstOrDefault().Key + "," + errors.FirstOrDefault().Value;
        // Close the StreamWriter
        writer.Close();
    }*/

    /*private void OnDestroy()
    {
        *//*filePath = Application.dataPath + "/Rigidbody2D_Log.csv";*//*
        
    }*/

    public void logErrors(string map, string bridge, string errSize)
    {
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        filePath = Path.Combine(desktopPath, "Rigidbody2D_ErrorsLog_" + map + ".csv");
        writer = new StreamWriter(filePath);
        writer.WriteLine("Err_Bridge,Err_Size");
        /*Dictionary<string, string> errors = FindObjectOfType<CarController>().errorInBridge;*/
        string data = bridge + "," + errSize;
        // Close the StreamWriter
        writer.Close();
    }
}
