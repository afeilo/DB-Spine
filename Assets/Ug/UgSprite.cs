using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgSprite : UgTrans
{
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    public void Init(UgData.DisplayData data)
    {
        base.Init(data);
        this.pivot = data.pivot;
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
