namespace KizhiPart1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    public class Interpreter
    {
        private readonly TextWriter writer;

        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();

        private readonly Dictionary<string, Func<string[], bool>> commands;

        private string errorText;

        public Interpreter(TextWriter writer)
        {
            this.writer = writer;

            commands = new Dictionary<string, Func<string[], bool>>
            {
                ["print"] = Print,
                ["rem"] = Rem,
                ["set"] = Set,
                ["sub"] = Sub,
            };
        }

        public void ExecuteLine(string command)
        {
            if (!TryParseCommand(command, out string[] input) || !commands[input[0]](input))
                writer.WriteLine(errorText);
        }

        private bool Rem(string[] args)
        {
            return InputVerify(args, 2) && variables.Remove(args[1]);
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
            if (!InputVerify(args, 3) || !TryParseValue(args[2], out int value))
                return false;
            if (variables[args[1]] - value < 0)
                return Error($"Вычитаемое {value} больше самого числа");
            variables[args[1]] -= value;
            return true;
        }

        private bool Print(string[] args)
        {
            if (!InputVerify(args, 2))
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

        private bool TryParseValue(string input, out int value)
        {
           return (int.TryParse(input, out value) || Error($"Неккоректная строка для парсинга числа: {input}"))
            && (value >= 0 || Error($"Для ввода доступны только натуральные числа, число {value} отрицательное"));
        }

        private bool InputVerify(string[] args, int countOfArgs)
        {
            return (CheckCountOfArgs(args, countOfArgs) || Error($"Неккоректные количество аргументов функции {args.Length}, ожидалось {countOfArgs}"))
            && (variables.ContainsKey(args[1]) || Error("Переменная отсутствует в памяти"));
        }

        private bool Error(string input)
        {
            errorText = input;
            return false;
        }

        private bool CheckCountOfArgs(string[] args, int countOfArgs) => args.Length == countOfArgs;

        [TestFixture]
        private class TestInterpreter
        {
            private readonly Interpreter interpreter = new Interpreter(new StringWriter());

            [Test]
            public void CheckSet()
            {
                interpreter.ExecuteLine("set a 5");
                CheckValue(5, "a");
                CheckCount(1);
                interpreter.ExecuteLine("set a 2");
                CheckValue(2, "a");
                CheckCount(1);
                interpreter.ExecuteLine("set b 1");
                CheckValue(1, "b");
                CheckCount(2);
            }

            [Test]
            public void CheckRem()
            {
                interpreter.ExecuteLine("set a 5");
                CheckCount(1);
                interpreter.ExecuteLine("rem a");
                CheckCount(0);
                interpreter.ExecuteLine("rem a");
                CheckCount(0);
            }

            [Test]
            public void CheckSub()
            {
                interpreter.ExecuteLine("set a 5");
                CheckValue(5, "a");
                interpreter.ExecuteLine("sub a 3");
                CheckValue(2, "a");
            }

            private void CheckCount(int expectedValue)
                => Assert.AreEqual(expectedValue, interpreter.variables.Count);

            private void CheckValue(int expectedValue, string varName)
                => Assert.AreEqual(expectedValue, interpreter.variables[varName]);
        }
    }
}