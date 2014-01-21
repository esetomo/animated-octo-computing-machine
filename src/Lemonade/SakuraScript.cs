using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lemonade
{
    public class SakuraScript
    {
        private readonly IEnumerable<IToken> tokens;

        public SakuraScript(string script)
        {
            this.tokens = GetTokens(script);
        }

        private static readonly TokenDefinition[] tokenDefinitions = new TokenDefinition[]{
            new TokenDefinition(@"[^\%\\]+", (m) => new TextToken(m.Value)),
            new TokenDefinition(@"\\([01])", (m) => new ChangeScopeToken(m.Groups[1].Value)),
            new TokenDefinition(@"\\p\[(\d+)\]", (m) => new ChangeScopeToken(m.Groups[1].Value)),
            new TokenDefinition(@"\\n\[(\d+)\]", (m) => new LineFeedToken(m.Groups[1].Value)),
            new TokenDefinition(@"\\n\[half\]", (m) => new LineFeedToken(50)),
            new TokenDefinition(@"\\n", (m) => new LineFeedToken(100)),
            new TokenDefinition(@"\\e", (m) => new EndToken()),
            new TokenDefinition(@"\\_w\[(\d+)\]", (m) => new WaitToken(m.Groups[1].Value)),
            new TokenDefinition(@"\\w(\d)", (m) => WaitToken.Simple(m.Groups[1].Value)),
            new TokenDefinition(@"[\%\\]", (m) => new TextToken(m.Value))
        };

        private static IEnumerable<IToken> GetTokens(string text)
        {
            while (text.Length > 0)
            {
                foreach (var def in tokenDefinitions)
                {
                    Match match = def.Match(text);
                    if (match.Success)
                    {
                        yield return def.Func(match);
                        text = text.Substring(match.Length);
                        break;
                    }
                }
            }
        }

        public class TokenDefinition
        {
            private readonly Regex regex;
            public readonly Func<Match, IToken> Func;

            public TokenDefinition(string regex, Func<Match, IToken> func)
            {
                this.regex = new Regex(string.Format("^{0}", regex));
                this.Func = func;
            }

            public Match Match(string text)
            {
                return regex.Match(text);
            }
        }

        public interface IToken
        {
        }

        [DebuggerDisplay("Text: {text}")]
        public class TextToken : IToken
        {
            private readonly string text;

            public TextToken(string text)
            {
                this.text = text;    
            }
        }

        [DebuggerDisplay("ChangeScope: {scope}")]
        public class ChangeScopeToken : IToken
        {
            private readonly int scope;

            public ChangeScopeToken(string scope)
            {
                this.scope = int.Parse(scope);
            }
        }

        [DebuggerDisplay("LineFeed: {amount}")]
        public class LineFeedToken : IToken
        {
            private readonly int amount; // percent

            public LineFeedToken(string amount)
            {
                this.amount = int.Parse(amount);
            }

            public LineFeedToken(int amount)
            {
                this.amount = amount;
            }
        }

        [DebuggerDisplay("End")]
        public class EndToken : IToken
        {
        }

        [DebuggerDisplay("Wait: {wait}")]
        public class WaitToken : IToken
        {
            private readonly int wait; // msec

            public WaitToken(string wait)
            {
                this.wait = int.Parse(wait);
            }

            public WaitToken(int wait)
            {
                this.wait = wait;
            }

            public static WaitToken Simple(string w)
            {
                return new WaitToken(int.Parse(w) * 50);
            }
        }
    }
}
