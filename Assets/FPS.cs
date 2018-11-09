// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;

namespace Hugula.Utils
{
public class FPS : MonoBehaviour 
{
    public float updateInterval = 0.5F;
    private float lastInterval;
    private int frames = 0;
    private float fps;
    GUIStyle bb = new GUIStyle();
    void Start() 
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1, 0, 0);   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小
    }
    
#if UNITY_EDITOR
    void OnGUI()
    {
        GUILayout.Label("fps:" + fps, bb);
    }
#endif

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval) 
        {
            fps = frames/(timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
}
