using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using SimpleJSON;
#endif
public class UgData : ScriptableObject
{
#if UNITY_EDITOR
    public static UgData ugData;
    public static List<float> vertices = new List<float>();
    public static List<float> uvs = new List<float>();
    public static List<int> triangles = new List<int>();
    [MenuItem("Assets/UgData")]
    public static void MakeUgData()
    {
        vertices = new List<float>();
        uvs = new List<float>();
        triangles = new List<int>();
        //string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string path = "Assets/bingyao_ske.json";
        string texture_path = "Assets/bingyao_tex.json";
        JSONClass json = JSON.Parse(File.ReadAllText(path).Replace("null", "\"null\"")).AsObject;

        string save = "Assets/d.asset";
        ugData = AssetDatabase.LoadAssetAtPath<UgData>(save);
        if (ugData == null)
        {
            ugData = ScriptableObject.CreateInstance<UgData>();
            AssetDatabase.CreateAsset(ugData, save);
            AssetDatabase.Refresh();
        }

        ugData.name = json["name"].Value;
        ugData.version = json["version"].Value;
        ugData.frameRate = json["frameRate"].AsInt;


        //armature
        JSONArray armature = json["armature"].AsArray;
        ArmatureData[] ads = new ArmatureData[armature.Count];
        for (int i = 0; i < armature.Count; i++)
        {
            ads[i] = DecodeArmature(json, armature[i].AsObject);
        }

        JSONClass json_tex = JSON.Parse(File.ReadAllText(texture_path).Replace("null", "\"null\"")).AsObject;
        TextureData td = new TextureData();
        td.width = json_tex["width"].AsInt;
        td.height = json_tex["height"].AsInt;
        td.imagePath = json_tex["imagePath"].Value;
        td.name = json_tex["name"].Value;
        List<SubTextureData> stds = new List<SubTextureData>();
        JSONArray sub_textures = json_tex["SubTexture"].AsArray;
        for (int i = 0; i < sub_textures.Count; i++)
        {
            SubTextureData std = new SubTextureData();
            JSONClass jc = sub_textures[i].AsObject;
            std.width = jc["width"].AsInt;
            std.height = jc["height"].AsInt;
            std.x = jc["x"].AsInt;
            std.y = jc["y"].AsInt;
            std.name = jc["name"].Value;
            stds.Add(std);
        }
        td.datas = stds.ToArray();
        ads[0].textureDatas = td;
        ugData.armatures = ads;
        EditorUtility.SetDirty(ugData);
        AssetDatabase.SaveAssets();

    }

    static ArmatureData DecodeArmature(JSONClass parent, JSONClass json)
    {
        ArmatureData data = new ArmatureData();
        data.name = json["name"].Value;
        data.frameRate = TryGetInt(json, "frameRate", parent["frameRate"].AsInt);
        data.type = TryGetString(json, "type", "Armature");
        data.vertexDatas = new VertexData();

        //bone
        JSONArray bones = json["bone"].AsArray;
        BoneData[] boneDatas = new BoneData[bones.Count];
        for (int i = 0; i < bones.Count; i++)
        {
            boneDatas[i] = DecodeBone(bones[i].AsObject);
        }
        data.boneDatas = boneDatas;



        //slot
        JSONArray slots = json["slot"].AsArray;
        SlotData[] slotDatas = new SlotData[slots.Count];
        for (int i = 0; i < slots.Count; i++)
        {
            slotDatas[i] = DecodeSlot(slots[i].AsObject);
        }
        data.slotDatas = slotDatas;


        JSONArray skins = json["skin"].AsArray;
        List<DisplayData> displayData = new List<DisplayData>();
        for (int i = 0; i < skins.Count; i++)
        {
            JSONClass skin = skins[i].AsObject;
            JSONArray d_slots = skin["slot"].AsArray;
            for (int j = 0; j < d_slots.Count; j++)
            {
                JSONClass d_slot = d_slots[j].AsObject;
                string d_slot_name = d_slot["name"];
                JSONArray d_displays = d_slot["display"].AsArray;
                for (int k = 0; k < d_displays.Count; k++)
                {
                    displayData.Add(DecodeDisplay(d_displays[k].AsObject, d_slot_name));
                }
            }
        }
        data.displayDatas = displayData.ToArray();
        data.vertexDatas.vertices = vertices.ToArray();
        data.vertexDatas.uvs = uvs.ToArray();
        data.vertexDatas.triangles = triangles.ToArray();

        JSONArray animations = json["animation"].AsArray;
        data.animations = new AnimationData[animations.Count];
        for (int i = 0; i < animations.Count; i++)
        {
            data.animations[i] = DecodeAnimation(animations[i].AsObject);
        }

        return data;
    }

    static BoneData DecodeBone(JSONClass json)
    {
        BoneData data = new BoneData();

        data.name = json["name"];
        data.parent = json["parent"];
        TryGetTransform(json, data);

        return data;
    }

    static SlotData DecodeSlot(JSONClass json)
    {
        SlotData data = new SlotData();

        data.name = json["name"];
        data.parent = json["parent"];
        data.displayIndex = TryGetInt(json, "displayIndex", 0);
        TryGetColor(json, ref data.colorMul, ref data.colorAdd);

        return data;
    }

    static DisplayData DecodeDisplay(JSONClass json, string parent)
    {
        DisplayData data = new DisplayData();

        data.name = json["name"];
        data.parent = parent;

        //data.pivot = TryGetVector2(json, "pivot", Vector2.one * 0.5f);
        TryGetTransform(json, data);
        JSONArray verticesjson = json["vertices"].AsArray;
        JSONArray uvsjson = json["uvs"].AsArray;
        JSONArray trianglesjson = json["triangles"].AsArray;
        if (verticesjson.Count > 0) {
            data.offset = vertices.Count / 2;
            data.count = verticesjson.Count / 2;
            data.triangleOffset = triangles.Count;
            data.triangleCount = trianglesjson.Count;
            for (int i = 0; i < verticesjson.Count; i++)
            {
                vertices.Add(verticesjson[i].AsFloat);
                uvs.Add(uvsjson[i].AsFloat);
            }
            for (int i = 0; i < trianglesjson.Count; i++)
            {
                triangles.Add(trianglesjson[i].AsInt);
            }
            
        }


        return data;
    }

    static AnimationData DecodeAnimation(JSONClass json)
    {
        AnimationData data = new AnimationData();
        data.name = json["name"].Value;
        data.playTimes = TryGetInt(json, "playTimes", 1);
        data.duration = TryGetInt(json, "duration", 1);

        JSONArray bones = json["bone"].AsArray;
        data.boneAnimations = new BoneAnimationCollect[bones.Count];
        for (int i = 0; i < bones.Count; i++)
        {
            data.boneAnimations[i] = DecodeBoneAnimationCollect(bones[i].AsObject);
        }


        JSONArray slots = json["slot"].AsArray;
        data.slotAnimations = new SlotAnimationCollect[slots.Count];
        for (int i = 0; i < slots.Count; i++)
        {
            data.slotAnimations[i] = DecodeSlotAnimationCollect(slots[i].AsObject);
        }

        return data;
    }

    static BoneAnimationCollect DecodeBoneAnimationCollect(JSONClass json)
    {
        BoneAnimationCollect data = new BoneAnimationCollect();
        data.name = json["name"];
        data.scale = TryGetFloat(json, "scale", 1);
        data.offset = TryGetFloat(json, "offset", 1);

        JSONArray frames = json["frame"].AsArray;
        data.frames = new BoneFrameData[frames.Count];
        int start = 0;
        for (int i = 0; i < frames.Count; i++)
        {
            JSONClass jd = frames[i].AsObject;
            BoneFrameData fd = new BoneFrameData();

            fd.frameStart = start;
            fd.duration = TryGetInt(jd, "duration", 1);
            start += fd.duration;
            fd.tweenType = TryGetInt(jd, "tweenType", 0);
            fd.tweenEasing = TryGetFloat(jd, "tweenEasing", 0);
            TryGetTransform(jd, fd);

            JSONArray curve = jd["curve"].AsArray;
            fd.curve = new Vector2[curve.Count / 2];
            for (int j = 0; j < curve.Count; j += 2)
            {
                fd.curve[j / 2] = new Vector2(curve[j].AsFloat, curve[j + 1].AsFloat);
            }

            data.frames[i] = fd;
        }
        return data;
    }


    static SlotAnimationCollect DecodeSlotAnimationCollect(JSONClass json)
    {
        SlotAnimationCollect data = new SlotAnimationCollect();
        data.name = json["name"];

        JSONArray frames = json["frame"].AsArray;
        data.frames = new SlotFrameData[frames.Count];
        int start = 0;
        for (int i = 0; i < frames.Count; i++)
        {
            JSONClass jd = frames[i].AsObject;
            SlotFrameData fd = new SlotFrameData();

            fd.frameStart = start;
            fd.duration = TryGetInt(jd, "duration", 1);
            fd.tweenType = TryGetInt(jd, "tweenType", 0);
            fd.tweenEasing = TryGetFloat(jd, "tweenEasing", 0);
            start += fd.duration;

            JSONArray curve = jd["curve"].AsArray;
            fd.curve = new Vector2[curve.Count / 2];
            for (int j = 0; j < curve.Count; j += 2)
            {
                fd.curve[j / 2] = new Vector2(curve[j].AsFloat, curve[j + 1].AsFloat);
            }
            fd.displayIndex = TryGetInt(jd, "displayIndex", 0);
            TryGetColor(jd, ref fd.colorMul, ref fd.colorAdd);

            data.frames[i] = fd;
        }
        return data;
    }


    static void TryGetColor(JSONClass json, ref Color mul, ref Vector4 add)
    {
        if (json.ContainKey("color"))
        {
            JSONClass c = json["color"].AsObject;
            mul = new Color(TryGetFloat(c, "rM", 100) / 100, TryGetFloat(c, "gM", 100) / 100, TryGetFloat(c, "bM", 100) / 100, TryGetFloat(c, "aM", 100) / 100);
            add = new Vector4(TryGetFloat(c, "rO", 0) / 255, TryGetFloat(c, "gO", 0) / 255, TryGetFloat(c, "bO", 0) / 255, TryGetFloat(c, "aO", 0) / 255);
        }
    }

    static void TryGetTransform(JSONClass json, TransformData data)
    {
        if (json.ContainKey("transform"))
        {
            JSONClass t = json["transform"].AsObject;

            data.pos = new Vector2(TryGetFloat(t, "x", 0), TryGetFloat(t, "y", 0));
            data.rotation = -TryGetFloat(t, "skX", 0);
            data.scale = new Vector2(TryGetFloat(t, "scX", 1), TryGetFloat(t, "scY", 1));
        }
    }

    static void TryGetTransform(JSONClass json, BoneFrameData data)
    {
        if (json.ContainKey("transform"))
        {
            JSONClass t = json["transform"].AsObject;

            data.transPos = t.ContainKey("x") || t.ContainKey("y");
            data.transScale = t.ContainKey("scX") || t.ContainKey("scY");
            data.transRotate = t.ContainKey("skX");

            data.pos = new Vector2(TryGetFloat(t, "x", 0), TryGetFloat(t, "y", 0));
            data.rotation = -TryGetFloat(t, "skX", 0);
            data.scale = new Vector2(TryGetFloat(t, "scX", 1), TryGetFloat(t, "scY", 1));
        }
    }

    static int TryGetInt(JSONClass json, string key, int d)
    {
        if (json.ContainKey(key))
            return json[key].AsInt;
        return d;
    }

    static float TryGetFloat(JSONClass json, string key, float d)
    {
        if (json.ContainKey(key))
            return json[key].AsFloat;
        return d;
    }

    static Vector2 TryGetVector2(JSONClass json, string key, Vector2 d)
    {
        if (json.ContainKey(key))
            return new Vector2(TryGetFloat(json[key].AsObject, "x", d.x), TryGetFloat(json[key].AsObject, "y", d.y));
        return d;
    }

    static string TryGetString(JSONClass json, string key, string d)
    {
        if (json.ContainKey(key))
            return json[key].Value;
        return d;
    }

#endif

    [System.Serializable]
    public class ArmatureData
    {
        public string name;
        public int frameRate;
        public string type;

        public BoneData[] boneDatas;
        public SlotData[] slotDatas;
        public DisplayData[] displayDatas;
        public AnimationData[] animations;
        public VertexData vertexDatas;
        public TextureData textureDatas;
    }

    [System.Serializable]
    public class UgBaseData
    {
        public string name;
        public string parent;
    }

    [System.Serializable]
    public class TransformData : UgBaseData
    {
        public Vector2 pos = Vector2.zero;
        public float rotation = 0;
        public Vector2 scale = Vector2.one;
    }

    [System.Serializable]
    public class BoneData : TransformData
    {

    }

    [System.Serializable]
    public class SlotData : UgBaseData
    {
        public int displayIndex = 0;
        public Color colorMul = Color.white;
        public Vector4 colorAdd = Vector4.zero;
    }
    [System.Serializable]
    public class DisplayData : TransformData
    {
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public int offset = -1;
        public int count = -1;
        public int triangleOffset = -1;
        public int triangleCount = -1;
    }

    [System.Serializable]
    public class AnimationData
    {
        public string name;
        public int playTimes = 1;
        public int duration = 1;
        public BoneAnimationCollect[] boneAnimations;
        public SlotAnimationCollect[] slotAnimations;
    }

    [System.Serializable]
    public class BoneAnimationCollect
    {
        public string name;
        public float scale;
        public float offset;
        public BoneFrameData[] frames = new BoneFrameData[0];
    }

    [System.Serializable]
    public class VertexData 
    {
        public float[] vertices = new float[0];
        public float[] uvs = new float[0];
        public int[] triangles = new int[0];
    }

    [System.Serializable]
    public class SlotAnimationCollect
    {
        public string name;
        public SlotFrameData[] frames = new SlotFrameData[0];
    }

    [System.Serializable]
    public class BoneFrameData : TransformData
    {
        public int frameStart;
        public int duration;
        public int tweenType;
        public float tweenEasing;
        public Vector2[] curve;

        public bool transPos;
        public bool transScale;
        public bool transRotate;

    }

    [System.Serializable]
    public class SlotFrameData
    {
        public int frameStart;
        public int duration;
        public int tweenType;
        public float tweenEasing;
        public Vector2[] curve;
        public int displayIndex = 0;
        public Color colorMul = Color.white;
        public Vector4 colorAdd = Vector4.zero;
    }


    public string name;
    public string version;
    public int frameRate;
    public Texture2D tex;
    public Material matrial;
    public ArmatureData[] armatures;
    [System.Serializable]
    public class TextureData 
    {
        public int width;
        public int height;
        public string imagePath;
        public string name;
        public SubTextureData[] datas;

    }
    [System.Serializable]
    public class SubTextureData {
        public string name;
        public int width;
        public int height;
        public int x;
        public int y;
        
    }
}
