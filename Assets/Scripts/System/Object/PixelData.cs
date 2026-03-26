using System;
using System.Collections.Generic;

[Serializable]
public class PixelData
{
    public float x, y, z;
    public float r, g, b, a;
}

[Serializable]
public class PixelMapJSON
{
    public int width;
    public int height;
    public List<PixelData> pixels = new List<PixelData>();
}