using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB.Shell
{
    public class InputBuffer
    {
        private readonly Stack<char> bracesStack = new();
        private readonly StringBuilder stringBuilder = new();

        public bool TryGetCommands(string input, out List<string> commands)
        {
            commands = null;

            foreach (var c in input
                .Where(ProcessChar))
            {
                (commands ??= new List<string>()).Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }

            return commands != null;
        }

        private bool ProcessChar(char c)
        {
            if (char.IsWhiteSpace(c))
                return false;

            stringBuilder.Append(c);

            switch (c)
            {
                case '{':
                    bracesStack.Push('{');
                    break;
                case '}':
                    bracesStack.Pop();
                    break;
            }

            return bracesStack.Count == 0;
        }
    }
}