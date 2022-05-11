namespace KizhiPart3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    public class Debugger
    {
        private readonly TextWriter writer;

        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();

        private readonly Dictionary<string, Func<string[], bool>> commands;

        private readonly Dictionary<string, Function> functions = new Dictionary<string, Function>();

        private readonly Stack<CalledFunction> stackTrace = new Stack<CalledFunction>();

        private readonly HashSet<int> breakPoints = new HashSet<int>();

        private readonly Dictionary<string, int> variablesChangesLine = new Dictionary<string, int>();

        private readonly List<string> userScript = new List<string>();

        private string errorText;

        private int currentLine = 0;

        private int breakOnLine = -1;

        private DebuggerMod debuggerMod = DebuggerMod.Run;

        public Debugger(TextWriter writer)
        {
            this.writer = writer;

            commands = new Dictionary<string, Func<string[], bool>>
            {
                ["print"] = SwitchPrint,
                ["step"] = SwitchStep,
                ["set"] = SwitchSet,
                ["rem"] = Rem,
                ["sub"] = Sub,
                ["run"] = Run,
                ["call"] = Call,
                ["def"] = Def,
                ["add"] = AddBreak,
            };
        }

        private enum DebuggerMod
        {
            Write,
            Run,
        }

        public void ExecuteLine(string command)
        {
            var isSuccess = false;
            switch (debuggerMod)
            {
                case DebuggerMod.Write:
                    isSuccess = command is null ? Error("Ввод отсутсвует") : WriteScript(command);
                    break;
                case DebuggerMod.Run:
                    isSuccess = TryParseCommand(command, out string[] input) && commands[input[0]](input);
                    break;
            }

            if (!isSuccess)
                writer.WriteLine(errorText);
        }

        private bool WriteScript(string input)
        {
            var isSuccess = true;
            var lines = input.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                isSuccess &= AddLineToScript(lines, ref i);
            return isSuccess;
        }

        private bool AddLineToScript(string[] lines, ref int i)
        {
            var command = lines[i].Split(' ');
            switch (command[0])
            {
                case "def":
                    return AddFunctionToScript(lines, command, ref i);
                case "end":
                    debuggerMod = DebuggerMod.Run;
                    return true;
                default:
                    if (!TryParseCommand(command[0], out _))
                        return Error($"Попытка добавить в скрипт неизветсную команду: {command[0]}");
                    userScript.Add(lines[i]);
                    return true;
            }
        }

        private bool AddFunctionToScript(string[] lines, string[] command, ref int i)
        {
            var funcName = command[1];
            functions[funcName] = new Function(userScript.Count);
            userScript.Add(lines[i++]);
            while (i < lines.Length && lines[i].StartsWith("    ", StringComparison.InvariantCulture))
            {
                userScript.Add(lines[i++].TrimStart());
                functions[funcName].AddLine();
            }

            i--;
            return true;
        }

        private bool Run(string[] _)
        {
            var isSuccess = false;
            while (IsNotStop() && currentLine < userScript.Count)
            {
                if (stackTrace.Count > 0)
                    isSuccess = Call(null);
                else
                {
                    ExecuteLine(userScript[currentLine]);
                    if (stackTrace.Count > 0)
                        return true;

                    isSuccess = true;
                }

                UpCurrentLine();
            }

            return isSuccess && BreakProgram();
        }

        private bool IsNotStop()
        {
            return currentLine == breakOnLine || !breakPoints.Contains(currentLine) || stackTrace.Count > 0;
        }

        private bool BreakProgram()
        {
            breakOnLine = TakeCurrentLine();
            if (currentLine == userScript.Count)
                ClearMemory();
            return true;
        }

        private void UpCurrentLine()
        {
            if (stackTrace.Count == 0)
                currentLine++;
        }

        private void ClearMemory()
        {
            variables.Clear();
            variablesChangesLine.Clear();
            currentLine = 0;
            breakOnLine = -1;
        }

        private CalledFunction CallFunction(string[] args)
        {
            if (args == null)
            {
                ExecuteFuncLine();
                return stackTrace.Peek();
            }

            if (!InputVerify(args, 2, functions))
                return null;
            var calledFunc = new CalledFunction(functions[args[1]], TakeCurrentLine(), args[1]);
            stackTrace.Push(calledFunc);
            return calledFunc;
        }

        private bool Call(string[] args)
        {
            var calledFunc = CallFunction(args);
            if (calledFunc == null)
                return Error("Вызов не существующей функции");
            while (!breakPoints.Contains(calledFunc.RunningOnLine) && calledFunc.LinesToOut > 0)
                ExecuteFuncLine();
            if (calledFunc.LinesToOut == 0)
                stackTrace.Pop();
            return true;
        }

        private void ExecuteFuncLine()
        {
            var func = stackTrace.Peek();
            ExecuteLine(userScript[func.RunningOnLine]);
            func.ExecuteLine();
        }

        private bool AddBreak(string[] args)
        {
            return CheckCountOfArgs(args, 3) && TryParseValue(args[2], out int value) && breakPoints.Add(value);
        }

        private bool SwitchStep(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    return Step();
                case 2:
                    return StepOver();
                default:
                    return Error($"Неверное количетсво аргументов \"{args.Length}\" у команды step");
            }
        }

        private bool Step()
        {
            if (stackTrace.Count > 0)
                ExecuteFuncLine();
            else
                ExecuteLine(userScript[currentLine]);
            return StepNextLine() && BreakProgram();
        }

        private bool StepNextLine()
        {
            if (stackTrace.Count > 0)
            {
                var func = stackTrace.Peek();
                if (func.LinesToOut == 0)
                {
                    stackTrace.Pop();
                    StepNextLine();
                }
            }
            else
                currentLine++;
            return true;
        }

        private bool StepOver()
        {
            var lineData = userScript[TakeCurrentLine()].Split();
            Function func = null;
            if (InputVerify(lineData, 2, functions))
                func = functions[lineData[1]];
            if (func == null)
                return Step();
            for (int i = func.Start + 1; i < func.End; i++)
                ExecuteLine(userScript[i]);
            return StepNextLine() && BreakProgram();
        }

        private bool SwitchPrint(string[] args)
        {
            if (!CheckCountOfArgs(args, 2))
                return false;
            switch (args[1])
            {
                case "mem":
                    return PrintMem();
                case "trace":
                    return PrintTrace();
                default:
                    return Print(args);
            }
        }

        private bool PrintTrace()
        {
            foreach (var func in stackTrace)
                writer.WriteLine(func.CalledInLine + " " + func.Name);
            return true;
        }

        private bool PrintMem()
        {
            foreach (var varPair in variables)
                writer.WriteLine(varPair.Key + " " + varPair.Value + " " + variablesChangesLine[varPair.Key]);
            return true;
        }

        private bool Def(string[] args)
        {
            if (!InputVerify(args, 2, functions))
                return false;
            currentLine += functions[args[1]].Length - 1;
            return true;
        }

        private bool SwitchSet(string[] input)
        {
            switch (input.Length)
            {
                case 2:
                    debuggerMod = DebuggerMod.Write;
                    return true;
                case 3:
                    return Set(input);
                default:
                    return Error($"Неверное количетсво аргументов \"{input.Length}\" у команды set");
            }
        }

        private bool Rem(string[] args)
        {
            if (!InputVerify(args, 2, variables))
                return false;
            variables.Remove(args[1]);
            variablesChangesLine.Remove(args[1]);
            return true;
        }

        private bool Set(string[] args)
        {
            if (!CheckCountOfArgs(args, 3) || !TryParseValue(args[2], out int value))
                return false;
            variables[args[1]] = value;
            variablesChangesLine[args[1]] = TakeCurrentLine();
            return true;
        }

        private bool Sub(string[] args)
        {
            if (!InputVerify(args, 3, variables) || !TryParseValue(args[2], out int value))
                return false;
            if (variables[args[1]] - value < 0)
                return Error($"Вычитаемое число {value} больше самого числа");
            variables[args[1]] -= value;
            variablesChangesLine[args[1]] = TakeCurrentLine();
            return true;
        }

        private bool Print(string[] args)
        {
            if (!InputVerify(args, 2, variables))
                return false;
            writer.WriteLine(variables[args[1]]);
            return true;
        }

        private bool TryParseCommand(string command, out string[] input)
        {
            input = null;
            if (command == null)
                return Error("Ввод отсутствует");
            input = command.Split();
            if (input.Length == 0)
                return Error("Отсутсвуют аргументы");
            return commands.ContainsKey(input[0]) || Error($"Несуществующая команда: {input[0]}");
        }

        private bool Error(string input)
        {
            errorText = input;
            return false;
        }

        private bool TryParseValue(string input, out int value)
        {
            return (int.TryParse(input, out value) || Error($"Неккоректная строка для парсинга числа: {input}"))
            && (value >= 0 || Error($"Для ввода доступны только натуральные числа, число {value} отрицательное"));
        }

        private bool InputVerify(string[] args, int countOfArgs, IDictionary memory)
        {
            return (CheckCountOfArgs(args, countOfArgs) || Error($"Неккоректные количество аргументов функции {args.Length}, ожидалось {countOfArgs}"))
            && (memory.Contains(args[1]) || Error("Переменная отсутствует в памяти"));
        }

        private int TakeCurrentLine()
        {
            return stackTrace.Count > 0 ? stackTrace.Peek().RunningOnLine : currentLine;
        }

        private bool CheckCountOfArgs(string[] args, int countOfArgs)
        {
            return args.Length == countOfArgs || Error($"Некорректное число аргументов: {args.Length}");
        }

        private class CalledFunction
        {
            public CalledFunction(Function func, int line, string name)
            {
                CalledInLine = line;
                Name = name;
                LinesToOut = func.Length - 1;
                RunningOnLine = func.Start + 1;
            }

            public string Name { get; }

            public int CalledInLine { get; }

            public int LinesToOut { get; private set; }

            public int RunningOnLine { get; private set; }

            public void ExecuteLine()
            {
                LinesToOut--;
                RunningOnLine++;
            }
        }

        private class Function
        {
            public Function(int start)
            {
                Start = start;
                End = start + 1;
                Length = 1;
            }

            public int Start { get; }

            public int End { get; private set; }

            public int Length { get; private set; }

            public void AddLine()
            {
                End++;
                Length++;
            }
        }

        [TestFixture]
        private class TestDebugger
        {
            private Debugger debugger;

            [TestCase(
                "def test\n" +
                "    rem a\n" +
                "set a 12\n" +
                "call test\n" +
                "print a")]
            [TestCase(
                "def test\n" +
                "    set a 5\n" +
                "    sub a 3\n" +
                "    print b\n" +
                "call test")]
            [TestCase(
                "def test\n" +
                "    set a 4\n" +
                "set t 5\n" +
                "call test\n" +
                "sub a 3\n" +
                "call test\n" +
                "print a")]
            [TestCase(
                "def test\n" +
                "    print a\n" +
                "    call test\n" +
                "set a 5\n" +
                "call test")]
            [TestCase(
                "def test\n" +
                "    set a 4\n" +
                "    set b 5\n" +
                "set t 5\n" +
                "call test\n" +
                "print a")]
            public void TestPlots(string input)
            {
                debugger = new Debugger(new StringWriter());
                debugger.ExecuteLine("set code");
                debugger.ExecuteLine(input);
                debugger.ExecuteLine("end set code");
                debugger.ExecuteLine("add break 1");
                debugger.ExecuteLine("add break 4");
                debugger.ExecuteLine("run");
                debugger.ExecuteLine("step over");
                debugger.ExecuteLine("step");
            }
        }
    }
}