using System;
using System.Text;

public static class D
{
    private static bool IsLogEnable
    {
        get
        {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR && !DEBUG
            return false;
#endif
            return true;
        }
    }

    private static string NowTimeStr()
    {
        return DateTime.Now.ToString("[HH:mm:ss.fff]: ");
    }

    public static string BytesToStr(byte[] bytes)
    {
        StringBuilder buff = new StringBuilder();
        buff.Append("[");
        for (var i = 0; i < bytes.Length; i++)
        {
            if (buff.Length > 1)
            {
                buff.Append(" ");
            }

            buff.AppendFormat("{0:x2}", bytes[i]);
        }

        buff.Append("]>");

        buff.Insert(0, " ");
        buff.Insert(0, bytes.Length);
        buff.Insert(0, "<bytes len: ");

        return buff.ToString();
    }

    private static string ObjectToStr(object obj)
    {
        if (obj.GetType().Name == "Byte[]")
        {
            return BytesToStr((byte[])obj);
        }

        return obj.ToString();
    }

    private static string ObjectsToString(object message0, object message1 = null, object message2 = null, object message3 = null,
        object message4 = null, object message5 = null, object message6 = null, object message7 = null,
        object message8 = null, object message9 = null, object message10 = null, object message11 = null,
        object message12 = null, object message13 = null, object message14 = null, object message15 = null)
    {
        if (!IsLogEnable) return "";

        StringBuilder result = new StringBuilder();
        result.Append(NowTimeStr());
        if (message0 != null) result.Append(ObjectToStr(message0));
        if (message1 != null) result.Append(ObjectToStr(message1));
        if (message2 != null) result.Append(ObjectToStr(message2));
        if (message3 != null) result.Append(ObjectToStr(message3));
        if (message4 != null) result.Append(ObjectToStr(message4));
        if (message5 != null) result.Append(ObjectToStr(message5));
        if (message6 != null) result.Append(ObjectToStr(message6));
        if (message7 != null) result.Append(ObjectToStr(message7));
        if (message8 != null) result.Append(ObjectToStr(message8));
        if (message9 != null) result.Append(ObjectToStr(message9));
        if (message10 != null) result.Append(ObjectToStr(message10));
        if (message11 != null) result.Append(ObjectToStr(message11));
        if (message12 != null) result.Append(ObjectToStr(message12));
        if (message13 != null) result.Append(ObjectToStr(message13));
        if (message14 != null) result.Append(ObjectToStr(message14));
        if (message15 != null) result.Append(ObjectToStr(message15));
        return "";
    }

    public static void Log(object message0, object message1 = null, object message2 = null, object message3 = null,
        object message4 = null, object message5 = null, object message6 = null, object message7 = null,
        object message8 = null, object message9 = null, object message10 = null, object message11 = null,
        object message12 = null, object message13 = null, object message14 = null, object message15 = null)
    {
        if (!IsLogEnable) return;

        var result = ObjectsToString(message0, message1, message2, message3,
                 message4, message5, message6, message7,
                 message8, message9, message10, message11,
                 message12, message13, message14, message15);

#if UNITY_STANDALONE_WIN
        Plugins.WinDebugLog.OutputDebugString(result);
#endif
        UnityEngine.Debug.Log(result);
    }

    public static void Warning(object message0, object message1 = null, object message2 = null,
        object message3 = null,
        object message4 = null, object message5 = null, object message6 = null, object message7 = null,
        object message8 = null, object message9 = null, object message10 = null, object message11 = null,
        object message12 = null, object message13 = null, object message14 = null, object message15 = null)
    {
        if (!IsLogEnable) return;

        var result = ObjectsToString(message0, message1, message2, message3,
                 message4, message5, message6, message7,
                 message8, message9, message10, message11,
                 message12, message13, message14, message15);

#if UNITY_STANDALONE_WIN
        Plugins.WinDebugLog.OutputDebugString(result);
#endif
        UnityEngine.Debug.LogWarning(result);
    }

    public static void Error(object message0, object message1 = null, object message2 = null, object message3 = null,
        object message4 = null, object message5 = null, object message6 = null, object message7 = null,
        object message8 = null, object message9 = null, object message10 = null, object message11 = null,
        object message12 = null, object message13 = null, object message14 = null, object message15 = null)
    {
        if (!IsLogEnable) return;

        var result = ObjectsToString(message0, message1, message2, message3,
                         message4, message5, message6, message7,
                         message8, message9, message10, message11,
                         message12, message13, message14, message15);

#if UNITY_STANDALONE_WIN
        Plugins.WinDebugLog.OutputDebugString(result);
#endif
        UnityEngine.Debug.LogError(result);
    }
}