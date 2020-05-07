using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileScript : MonoBehaviour
{
    private bool isSpanish = false;


    public bool spanish
    {
        get
        {
            return isSpanish;
        }
        set
        {
            isSpanish = value;
        }
    }

    public void toggleLang()
    {
        isSpanish = !isSpanish;
    }


    public void writeFile(string fileName, Vector3[] clickPoints, Vector3 uiScale, bool spanish)
    {
        StreamWriter sr = File.CreateText(fileName);//overwrites if exists

        sr.WriteLine("{0},{1},{2}", clickPoints[0].x, clickPoints[0].y, clickPoints[0].z);
        sr.WriteLine("{0},{1},{2}", clickPoints[1].x, clickPoints[1].y, clickPoints[1].z);
        sr.WriteLine("{0},{1},{2}", clickPoints[2].x, clickPoints[2].y, clickPoints[2].z);
        sr.WriteLine("{0},{1},{2}", clickPoints[3].x, clickPoints[3].y, clickPoints[3].z);

        sr.WriteLine("{0},{1},{2}", uiScale.x, uiScale.y, uiScale.z);

        if (spanish)
        {
            sr.WriteLine("s");
        }
        else
        {
            sr.WriteLine("e");
        }

        sr.Close();
    }


    //Reads the config file - lines 1-4 are the corner points, line 5 is the localScale of the UICanvas
    //line 6 is e or s for english or spanish
    public Vector3[] readConfigFile()
    {
        StreamReader sr = File.OpenText("screenLoc.txt");
        Vector3[] points = new Vector3[5];

        string line;
        int index = 0;

        while (index < 5 && (line = sr.ReadLine()) != null)
        {
            string[] p = line.Split(','); //x,y,z
            points[index] = new Vector3(float.Parse(p[0]), float.Parse(p[1]), float.Parse(p[2]));
            index++;
        }

        //get line 6 - eng/span
        line = sr.ReadLine();
        isSpanish = line == "s" ? true : false;

        sr.Close();
        return points;
    }
}