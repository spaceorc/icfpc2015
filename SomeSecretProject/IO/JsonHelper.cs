﻿using Newtonsoft.Json;

namespace SomeSecretProject.IO
{
    public static class JsonHelper
    {
        public static T ParseAsJson<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
