namespace KizhiPart2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    public class Interpreter
    {
        private readonly TextWriter writer;

        private readonly Dictionary<string, Func<string[], bool>> commands;

        private readonly Dictionary<string, Function> functions = new Dictionary<string, Function>();

        private readonly List<string> userScript = new List<string>();

        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();

        private InterpreterMod interpreterMod = InterpreterMod.Run;

        private string errorText;

        private int currentLine = 0;

        public Interpreter(TextWriter writer)
        {
            this.writer = writer;

            commands = new Dictionary<string, Func<string[], bool>>
            {
                ["print"] = Print,
                ["rem"] = Rem,
                ["set"] = SwitchSet,
                ["sub"] = Sub,
                ["run"] = Run,
                ["call"] = Call,
                ["def"] = Def,
            };
        }

        private enum InterpreterMod
        {
            Write,
            Run,
        }

        public void ExecuteLine(string command)
        {
            bool isSuccess = false;
            switch (interpreterMod)
            {
                case InterpreterMod.Write:
                    isSuccess = command is null ? Error("Ввод отсутсвует") : WriteScript(command);
                    break;
                case InterpreterMod.Run:
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
                    interpreterMod = InterpreterMod.Run;
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
            while (currentLine < userScript.Count)
            {
                ExecuteLine(userScript[currentLine]);
                currentLine++;
            }

            if (currentLine == userScript.Count)
                ClearMemory();

            return true;
        }

        private void ClearMemory()
        {
            variables.Clear();
            currentLine = 0;
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
                    interpreterMod = InterpreterMod.Write;
                    return true;
                case 3:
                    return Set(input);
                default:
                    return Error($"Неверное количетсво аргументов \"{input.Length}\" у команды set");
            }
        }

        private bool Call(string[] args)
        {
            if (!InputVerify(args, 2, functions))
                return false;
            var start = functions[args[1]].Start + 1;
            var end = functions[args[1]].End;
            for (int i = start; i < end; i++)
                ExecuteLine(userScript[i]);
            return true;
        }

        private bool Rem(string[] args)
        {
            return InputVerify(args, 2, variables) && variables.Remove(args[1]);
        }

        private bool Set(string[] args)
        {
            if (!CheckCountOfArgs(args, 3))
                return Error($"Неверное число аргументов {args.Length}");
            if (!TryParseValue(args[2], out int value))
                return false;
            variables[args[1]] = value;
            return true;
        }

        private bool Sub(string[] args)
        {
            if (!InputVerify(args, 3, variables) || !TryParseValue(args[2], out int value))
                return false;
            if (variables[args[1]] - value < 0)
                return Error($"Вычитаемое число {value} больше самого числа");
            variables[args[1]] -= value;
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
                return Error("Аргументы отсуствуют");
            return commands.ContainsKey(input[0]) || Error($"Несуществующая команда: {input[0]}");
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

        private bool Error(string input)
        {
            errorText = input;
            return false;
        }

        private bool CheckCountOfArgs(string[] args, int countOfArgs) => args.Length == countOfArgs;

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
        private class TestInterpreter
        {
            private Interpreter interpreter;

            [SetUp]
            public void InitData()
            {
                interpreter = new Interpreter(new StringWriter());
                interpreter.ExecuteLine("set code");
                interpreter.ExecuteLine(
                    "def test\n" +
                    "    rem a\n" +
                    "set a 12\n" +
                    "call test\n" +
                    "print a");
                interpreter.ExecuteLine("end set code");
                interpreter.ExecuteLine("run");
            }

            [Test(Description = "Проверка неизменности исходного кода")]
            public void CheckUserCodeSafety()
            {
                Assert.AreEqual(5, interpreter.userScript.Count);
                Assert.AreEqual(new List<string> { "def test", "rem a", "set a 12", "call test", "print a" }, interpreter.userScript);
            }

            [Test(Description = "Првоерка записи команд функции")]
            public void CheckFuncWriter()
            {
                var func = interpreter.functions["test"];
                Assert.AreEqual(2, func.End - func.Start);
                Assert.AreEqual("rem a", interpreter.userScript[func.Start + 1]);
            }

            [Test(Description = "Проверка записи функций")]
            public void CheckFuncInit()
            {
                Assert.AreEqual(1, interpreter.functions.Count);
                Assert.True(interpreter.functions.ContainsKey("test"));
            }
        }
    }
}