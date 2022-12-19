using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class PlotRenderer : MonoBehaviour
{
    private GraphicsBuffer _buffer;
    private static int s_BufferID = Shader.PropertyToID("pointBuffer");
    
    [SerializeField] ComputeShader plotRendererShader;
    [SerializeField] GraphData container;

    // Start is called before the first frame update
    void Start()
    {
        _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, container.graphData.Count, PointData.GetSize());
        GetComponent<VisualEffect>().SetGraphicsBuffer(s_BufferID, _buffer);
    }

    // Update is called once per frame
    void Update()
    {
        PointData[] pointData = new PointData[container.graphData.Count];

        for (int i = 0; i < container.graphData.Count; i++)
        {
            PointData p = new PointData()
            {
                x = container.graphData[i].x,
                y = container.graphData[i].y,
                z = container.graphData[i].z
            };

            pointData[i] = p;
        }
        
        _buffer.SetData(pointData);
        
        plotRendererShader.SetBuffer(0, s_BufferID, _buffer);
    }

    // free buffer on destroy
    void OnDestroy()
    {
        _buffer?.Dispose();
        _buffer = null;
    }
}

public struct PointData
{
    public float x;
    public float y;
    public float z;

    public static int GetSize()
    {
        return sizeof(float) * 3;
    }
}
