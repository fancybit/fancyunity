using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FancyCSharp;

namespace FancyUnity
{
    public class StringTurple : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof((string, string));
        }

        private static Regex regSS = new Regex(@"\[\s*(.+)\s*\]", RegexOptions.Compiled);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            return Json2Turple((string)reader.Value);
        }

        public static object Json2Turple(string txt)
        {
            ValueTuple<string, string> ret;
            var match = regSS.Match(txt);
            var pair = match.Groups[0].Value.Split(',');
            ret.Item1 = pair[0];
            ret.Item2 = pair[1];
            return ret;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Turple2Json(writer, value);
        }

        public static void Turple2Json(JsonWriter writer, object value)
        {
            var v = (ValueTuple<string, string>)value;
            writer.WriteValue($"[{v.Item1},{v.Item2}]");
        }
    }

    public class StringTurpleSet : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<(string, string)>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ret = new HashSet<(string, string)>();
            var str = reader.Value as string;
            str = str.Slice(1, -2);
            var jsonElems = str.Split(',');
            foreach (var txt in jsonElems)
            {
                ret.Add((ValueTuple<string, string>)StringTurple.Json2Turple(txt));
            }
            return ret;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var turpleSet = (HashSet<(string, string)>)value;
            if (turpleSet.Count == 0)
            {
                writer.WriteValue("[]");
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var t in turpleSet)
                {
                    sb.AppendLine($"[{t.Item1},{t.Item2}],");
                }
                var txt = sb.ToString();
                txt = txt.Slice(0, -2);
                writer.WriteValue(txt);
            }
        }
    }
}

