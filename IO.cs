using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyUnity
{
    public static class IO
    {
        public static bool hasExtName(this string self,params string[] extNames)
        {
            var ext = self.Split('.').Last();
            foreach (var n in extNames)
                if (ext.Trim().ToLower() == n.Trim().ToLower())
                    return true;
            return false;
        }
        public static bool IsImageFile(this string self)
        {
            return self.hasExtName("jpg","jpeg","psd","psb","png","bmp","gif");
        }

        public static string MainName(this string self)
        {
            var str = self.Split('.');
            var len = str.Length;
            if (len == 1) return self;
            if (len == 2) return str[0];
            return str[len - 2];
        }
    }
}
