namespace CommandLineCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class StatefulInterpreter : Interpreter, IDisposable
    {
        private readonly Queue<string> commandsLines = new Queue<string>();

        private Storage storage;

        private MemoryStream memory;

        private int ignoreCounter;

        private int previousIgnoreCounter;

        private long x;

        private static CultureInfo Culture => CultureInfo.InvariantCulture;

        public void Dispose()
        {
            memory.Dispose();
        }

        public override void Run(UserConsole userConsole, Storage storage)
        {
            userConsole = userConsole ?? throw new ArgumentNullException(nameof(userConsole));
            InitializeData(storage);
            while (true)
            {
                if (commandsLines.Count == 0)
                    ClearCommandInfo();
                if (ExecuteLine(userConsole, TakeNewLine(userConsole)))
                    break;
            }
        }

        private void InitializeData(Storage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            memory = new MemoryStream();
            var oldData = storage.Read();
            if (oldData.Length > 0)
                ReadOldData(oldData);
            else
                WriteNewData();
        }

        private void ReadOldData(byte[] data)
        {
            x = BitConverter.ToInt64(data, 0);
            ignoreCounter = BitConverter.ToInt32(data, 8);
            previousIgnoreCounter = ignoreCounter;
            var strData = Encoding.Unicode.GetString(data, 12, data.Length - 12);
            var strArrData = strData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Array.ForEach(strArrData, x => commandsLines.Enqueue(x));
            memory.Write(data, 0, data.Length);
        }

        private void WriteNewData()
        {
            memory.Write(BitConverter.GetBytes(x = 420L), 0, 8);
        }

        private void ClearCommandInfo()
        {
            ignoreCounter = 0;
            memory.SetLength(8);
        }

        private void ClearAllData()
        {
            storage.Write(Array.Empty<byte>());
        }

        private void AddToData(string line)
        {
            var input = Encoding.Unicode.GetBytes(line + "\n");
            memory.Position = memory.Length < 12 ? 12 : memory.Length;
            memory.Write(input, 0, input.Length);
            storage.Write(memory.ToArray());
        }

        private void UpgradeIgnore()
        {
            memory.Position = 8;
            memory.Write(BitConverter.GetBytes(++ignoreCounter), 0, 4);
            storage.Write(memory.ToArray());
        }

        private void UpgrageX()
        {
            memory.Position = 0;
            memory.Write(BitConverter.GetBytes(x), 0, 8);
        }

        private string TakeNewLine(UserConsole console)
        {
            string input;
            if (commandsLines.Count > 0)
                input = commandsLines.Dequeue();
            else
            {
                input = console.ReadLine().Trim();
                AddToData(input);
            }

            return input;
        }

        private void WriteInConsole(UserConsole console, string line)
        {
            if (previousIgnoreCounter > 0)
                previousIgnoreCounter--;
            else
            {
                console.WriteLine(line);
                UpgradeIgnore();
            }
        }

        private int ReadNumber(UserConsole console)
        {
            return int.Parse(TakeNewLine(console), Culture);
        }

        private bool ExecuteLine(UserConsole userConsole, string input)
        {
            switch (input)
            {
                case "exit":
                    ClearAllData();
                    return true;
                case "add":
                    Add(userConsole);
                    break;
                case "median":
                    Median(userConsole);
                    break;
                case "help":
                    Help(userConsole);
                    break;
                case "rand":
                    Random(userConsole);
                    UpgrageX();
                    break;
                default:
                    WriteInConsole(userConsole, "Такой команды нет, используйте help для списка команд");
                    break;
            }

            return false;
        }

        private void Random(UserConsole console)
        {
            const int a = 16807;
            const int m = 2147483647;

            var count = ReadNumber(console);
            for (var i = 0; i < count; i++)
            {
                WriteInConsole(console, x.ToString(Culture));
                x = (a * x) % m;
            }
        }

        private void Add(UserConsole console)
        {
            var a = ReadNumber(console);
            var b = ReadNumber(console);
            WriteInConsole(console, (a + b).ToString(Culture));
        }

        private void Median(UserConsole console)
        {
            var count = ReadNumber(console);
            var numbers = new List<int>();

            for (var i = 0; i < count; i++)
                numbers.Add(ReadNumber(console));

            var result = CalculateMedian(numbers);

            WriteInConsole(console, result.ToString(Culture));
        }

        private double CalculateMedian(List<int> numbers)
        {
            numbers.Sort();
            var count = numbers.Count;
            if (count == 0)
                return 0;

            if (count % 2 == 1)
                return numbers[count / 2];

            return (numbers[(count / 2) - 1] + numbers[count / 2]) / 2.0;
        }

        private void Help(UserConsole console)
        {
            const string exitMessage = "Чтобы выйти из режима помощи введите end";
            const string commands = "Доступные команды: add, median, rand";

            WriteInConsole(console, "Укажите команду, для которой хотите посмотреть помощь");
            WriteInConsole(console, commands);
            WriteInConsole(console, exitMessage);

            while (true)
            {
                string command = TakeNewLine(console);
                if (ExecuteHelp(console, command))
                    break;
            }
        }

        private bool ExecuteHelp(UserConsole console, string command)
        {
            const string exitMessage = "Чтобы выйти из режима помощи введите end";
            const string commands = "Доступные команды: add, median, rand";

            switch (command)
            {
                case "end":
                    return true;
                case "add":
                    WriteInConsole(console, "Вычисляет сумму двух чисел");
                    WriteInConsole(console, exitMessage);
                    break;
                case "median":
                    WriteInConsole(console, "Вычисляет медиану списка чисел");
                    WriteInConsole(console, exitMessage);
                    break;
                case "rand":
                    WriteInConsole(console, "Генерирует список случайных чисел");
                    WriteInConsole(console, exitMessage);
                    break;
                default:
                    WriteInConsole(console, "Такой команды нет");
                    WriteInConsole(console, commands);
                    WriteInConsole(console, exitMessage);
                    break;
            }

            return false;
        }
    }
}