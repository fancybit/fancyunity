using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugWriter : TextWriter
{
    private readonly Action<string> _Write;
    private readonly Action<string> _WriteLine;
    private readonly StringBuilder _sb = new StringBuilder();
    private static TextWriter _originOut;
    private static bool _enabled;

    static DebugWriter()
    {
        _originOut = Console.Out;
    }

    public static bool Enabled
    {
        set{
            if (value)
            {
                Console.SetOut(new DebugWriter());
            }
            else
            {
                Console.SetOut(_originOut);
            }
            _enabled = value;
        }
        get
        {
            return _enabled;
        }
    }

    public static bool EnableTime = true;

    // 使用 UTF-16 避免不必要的编码转换
    public override Encoding Encoding => Encoding.Unicode;

    // 最低限度需要重写的方法
    public override void Write(string value)
    {
        if (EnableTime)
        {
            _sb.Append($"{DateTime.Now}:");
        }
        _sb.Append(value);
        Debug.Log(_sb.ToString());
    }

    // 为提高效率直接处理一行的输出
    public override void WriteLine(string value)
    {
        lock(_sb)
        {
            if (EnableTime)
            {
                _sb.Append($"{DateTime.Now}:");
            }
            _sb.Append(value);
            Debug.Log(_sb.ToString());
            _sb.Clear();
        }
    }
}
