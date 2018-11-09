using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgSprite : UgTrans
{
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    public int offset = -1;
    public int count = -1;
    public int triangleOffset = -1;
    public int triangleCount = -1;
    public void Init(UgData.DisplayData data)
    {
        base.Init(data);
        this.pivot = data.pivot;
        this.offset = data.offset;
        this.count = data.count;
        this.triangleOffset = data.triangleOffset;
        this.triangleCount = data.triangleCount;
    }

    public override GameObject CreateTest(Transform trans)
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.transform.SetParent(trans);
        o.transform.localScale = new Vector3(scale.x * 1, scale.y * 1, 1);
        o.transform.localPosition = pos;
        o.transform.localEulerAngles = new Vector3(0, 0, rotation);
        return o;
    }
}
