using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewGraphDataContainer", menuName = "Graph Data/Graph Data Container")]
public class GraphData : ScriptableObject
{
    private int _currentDataIndex = 0;
    
    public List<TextAsset> rawData;

    public List<Vector3> graphData;

    // process all raw data to unity compatible form
    public void OnEnable()
    {
        ReadData();
    }
    
    // invoke when trying to access another text asset file
    public void OnIndexChange(int index)
    {
        _currentDataIndex = index;
        ReadData();
    }

    // func to read csv and store as vector3
    void ReadData()
    {
        if (rawData.Count < 1 || !rawData[_currentDataIndex])
        {
            Debug.Log("Error Loading File");
            return;
        }
        
        graphData = new List<Vector3>();
        List<Dictionary<string, float>> processedData = CSVReader.Read(rawData[_currentDataIndex]);
        for (int j = 0; j < processedData.Count; j++)
        {
            graphData.Add(new Vector3(processedData[j]["x"], processedData[j]["y"], processedData[j]["z"]));
        }
    }
}

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load (file) as TextAsset;

        var lines = Regex.Split (data.text, LINE_SPLIT_RE);

        if(lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for(var i=1; i < lines.Length; i++) {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if(values.Length == 0 ||values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for(var j=0; j < header.Length && j < values.Length; j++ ) {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if(int.TryParse(value, out n)) {
                    finalvalue = n;
                } else if (float.TryParse(value, out f)) {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add (entry);
        }
        return list;
    }
    
    public static List<Dictionary<string, float>> Read(TextAsset file)
    {
        var list = new List<Dictionary<string, float>>();
        TextAsset data = file;

        var lines = Regex.Split (data.text, LINE_SPLIT_RE);

        if(lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for(var i=1; i < lines.Length; i++) {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if(values.Length == 0 ||values[0] == "") continue;

            var entry = new Dictionary<string, float>();
            for(var j=0; j < header.Length && j < values.Length; j++ ) {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                float finalvalue = float.Parse(value);
                int n;
                float f;
                if(int.TryParse(value, out n)) {
                    finalvalue = n;
                } else if (float.TryParse(value, out f)) {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add (entry);
        }
        return list;
    }
}
