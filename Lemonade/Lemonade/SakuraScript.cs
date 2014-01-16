using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemonade
{
    public class SakuraScript
    {
        private readonly List<Command> commands = new List<Command>();

        public SakuraScript(string script)
        {
            Parse(script);
        }

        public enum State
        {
            Normal,

        }

        private void Parse(string script)
        {

        }

        public void Run()
        {
            foreach (Command command in commands)
                command.Run();
        }

        public abstract class Command
        {
            public abstract void Run();
        }
    }
}
