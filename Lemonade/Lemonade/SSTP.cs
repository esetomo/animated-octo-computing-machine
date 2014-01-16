using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lemonade
{
    public class SSTP
    {
        public static void Proc(byte[] bytes)
        {
            Request req = new Request(bytes);
            if (req.Headers.ContainsKey("Script"))
            {
                SakuraScript script = new SakuraScript(req.Headers["Script"]);
                script.Run();
            }
        }

        public class Request
        {
            private string method;
            public string Method { get { return method; } }

            private string version;
            public string Version { get { return version;  } }

            private Dictionary<string, string> headers;
            public IReadOnlyDictionary<string, string> Headers { get { return headers; } }

            public Request(byte[] bytes)
            {
                Parse(bytes);
            }

            private void Parse(byte[] bytes)
            {
                string str = Encoding.ASCII.GetString(bytes);
                Regex regex = new Regex("^Charset\\:\\s*(.+)\r$", RegexOptions.Multiline);
                Match match = regex.Match(str);
                Encoding encoding = Encoding.Default;
                if (match.Success)
                {
                    encoding = Encoding.GetEncoding(match.Groups[1].Value);
                }
                Parse(encoding.GetString(bytes));
            }

            private void Parse(string str)
            {
                Regex regex = new Regex("^(\\S+) SSTP\\/([0-9\\.]+)\r");
                Match match = regex.Match(str);
                if (match.Success)
                {
                    method = match.Groups[1].Value;
                    version = match.Groups[2].Value;
                }

                headers = new Dictionary<string, string>();
                regex = new Regex("^(\\S+)\\:\\s*(.+)\r$", RegexOptions.Multiline);
                foreach (Match m in regex.Matches(str))
                {
                    headers[m.Groups[1].Value] = m.Groups[2].Value;
                }
            }
        }
    }
}
