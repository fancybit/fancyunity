using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public class FileSys
    {
        protected static Regex regPrefabPath = new Regex(@"(.*)Resources/(.*)",RegexOptions.Compiled);

        public static int Traverse<T>(string path,Func<T,bool> visitor) 
            where T:UnityEngine.Object
        {
            var count = 0;
            foreach (var item in Directory.GetFiles($"{Application.dataPath}/{path}"))
            {
                if (item.EndsWith(".meta"))
                {
                    continue;
                }
                var match = regPrefabPath.Match(item);
                if (match.Success)
                {
                    var str = match.Groups[2].Value;
                    str = str.Replace(Path.GetExtension(item), string.Empty);//去掉后缀
                    if (visitor(Resources.Load<T>(str)))
                    {
                        ++count;
                    };
                }
            }
            return count;
        }
    }
}
