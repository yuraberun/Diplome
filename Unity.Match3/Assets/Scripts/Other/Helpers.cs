using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static partial class Helpers
{
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

    public static void SetAlpha(this Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    public static void SetAlpha(this TextMeshProUGUI txt, float alpha)
    {
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alpha);
    }

    public static void SetAlpha(this TextMeshPro txt, float alpha)
    {
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alpha);
    }

    public static List<string> LayerList
    {
        get
        {
            List<string> list = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                string l = LayerMask.LayerToName(i);
                if (l.Length != 0)
                    list.Add(l);
            }
            return list;
        }
    }

    public static void SetOnGround(Transform current, Vector3 pos)
    {
        WheelCollider[] cols = current.GetComponentsInChildren<WheelCollider>();
        Vector3 min = new Vector3(100000, 100000);
        foreach (WheelCollider c in cols)
        {
            if ((c as WheelCollider).isTrigger) continue;
            Vector3 curMin = (c as WheelCollider).bounds.min;
            min.x = Mathf.Min(min.x, curMin.x);
            min.y = Mathf.Min(min.y, curMin.y - (c as WheelCollider).suspensionDistance - (c as WheelCollider).radius);

        }
        int mask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Obs")) | (1 << LayerMask.NameToLayer("Default"));
        RaycastHit r;
        Physics.Linecast(pos, pos + Vector3.down * 30, out r, mask);
        if (r.point == Vector3.zero) return;
        current.position = pos + Vector3.up * (r.point.y - (pos.y - (current.position.y - min.y)));
    }

    public static bool IsNullOrEmpty(this string value)
    {
        if (value != null)
        {
            return value.Length == 0;
        }

        return true;
    }

    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Int32 GetTimeHash(int offset = 0)
    {
        DateTime when = DateTime.Now;
        when = when.AddSeconds(offset);
        Int32 kind = (Int32)when.Kind;
        return (kind << 31) | (Int32)when.Ticks;
    }

    public static T ReadJson<T>(string json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        var deserializeSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Newtonsoft.Json.Formatting.Indented
        };
        return JsonConvert.DeserializeObject<T>(json, deserializeSettings);
    }

    public static T ReadJson<T>(TextAsset json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        var deserializeSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Newtonsoft.Json.Formatting.Indented
        };
        return JsonConvert.DeserializeObject<T>(json.text, deserializeSettings);
    }

    public static string CreateJson(object obj)
    {
        if (obj is null)
            return string.Empty;
        var serializedSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Newtonsoft.Json.Formatting.Indented };
        return JsonConvert.SerializeObject(obj, serializedSettings);
    }
}
