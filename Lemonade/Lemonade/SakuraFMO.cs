using System;
using System.Collections.Generic;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Lemonade
{
    // http://usada.sakura.vg/contents/objects.html#fmo
    // http://bottle.mikage.to/doc/inside/fmo.html
    // http://emily.shillest.net/specwiki/index.php?SSP%2F%E4%BB%95%E6%A7%98%E6%9B%B8%2FFMO
    public static class SakuraFMO
    {
        private static MemoryMappedFile mmf = null;
        private static Dictionary<string, Dictionary<string, string>> data = null;

        public static void Open()
        {
            mmf = MemoryMappedFile.CreateOrOpen("Sakura", 65536);
        }

        public static void Close()
        {
            mmf.Dispose();
        }

        private static string MyId()
        {
            return string.Format("lemonade_{0}", Process.GetCurrentProcess().Id);
        }

        public static ICollection<string> Entries()
        {
            if (data == null)
                Load();

            if (data == null)
                return new string[] { };

            return data.Keys;
        }

        public static IReadOnlyDictionary<string, string> GetEntry(string id)
        {
            return data[id];
        }

        public static void Reset()
        {
            data = null;
        }

        private static void Load()
        {
            byte[] buf;

            using (Mutex mutex = new Mutex(false, "SakuraFMO"))
            {
                if (!mutex.WaitOne(1000))
                    return;

                try
                {
                    buf = Read();
                    if (buf == null)
                        return;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            Parse(buf);
        }

        private static byte[] Read()
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                int len = accessor.ReadInt32(0);
                if (len < 5)
                    return null;
                byte[] buf = new byte[len - 4];
                accessor.ReadArray(4, buf, 0, len - 4);
                return buf;
            }
        }

        private static void Parse(byte[] buf)
        {
            string str = Encoding.GetEncoding(932).GetString(buf);
            Regex regex = new Regex("^([^.]+)\\.([^\u0001]+)\u0001([^\r\n]+)\r$", RegexOptions.Multiline);

            data = new Dictionary<string, Dictionary<string, string>>();

            foreach(Match match in regex.Matches(str))
            {
                string id = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                string val = match.Groups[3].Value;

                if (!data.ContainsKey(id))
                    data[id] = new Dictionary<string, string>();

                data[id][key] = val;
            }
        }

        public static void Save(IReadOnlyDictionary<string, string> dic)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> item in dic){
                items[item.Key] = item.Value;
            }

            using (Mutex mutex = new Mutex(false, "SakuraFMO"))
            {
                if (!mutex.WaitOne(1000))
                    return;

                try
                {
                    byte[] buf = Read();
                    if (buf != null)
                    {
                        Parse(buf);
                    }
                    else
                    {
                        data = new Dictionary<string, Dictionary<string, string>>();
                    }

                    data[MyId()] = items;

                    buf = Format();
                    Write(buf);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            SakuraAPI.SendBroadcastGhostchange();
        }

        private static byte[] Format()
        {
            StringBuilder buf = new StringBuilder();

            foreach (string id in Entries())
            {
                foreach (KeyValuePair<string, string> item in GetEntry(id))
                {
                    buf.AppendFormat("{0}.{1}\u0001{2}\r\n", id, item.Key, item.Value);
                }
            }

            string str = buf.ToString();
            byte[] bytes = new byte[65536 - 4];
            Encoding.GetEncoding(932).GetBytes(str, 0, str.Length, bytes, 0);
            return bytes;
        }

        private static void Write(byte[] buf)
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                accessor.Write(0, buf.Length);
                accessor.WriteArray(4, buf, 0, buf.Length);
            }
        }
    }
}
