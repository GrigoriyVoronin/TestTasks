namespace Refactoring
{
    using NUnit.Framework;

    public class RemoteController
    {
        private const string UnknownCommand = "Unknown command";

        private const string InvalidNumberOfArguments = "Invalid number of arguments";

        private int volume = 20;

        private bool isOnline = false;

        private int brightness = 20;

        private int contrast = 20;

        public string Call(string command)
        {
            if (command == null)
                return "Command is null";
            var input = command.ToLowerInvariant().Split();
            if (!LengthsNotEqual(input, 0))
                return InvalidNumberOfArguments;
            switch (input[0])
            {
                case "tv":
                    return TurnTVMode(input);
                case "volume":
                    return ChangeNumericParametr(input, 2, ref volume);
                case "options":
                    return SwitchOptions(input);
                default:
                    return UnknownCommand;
            }
        }

        private string TurnTVMode(string[] input)
        {
            if (LengthsNotEqual(input, 2))
                return InvalidNumberOfArguments;
            switch (input[1])
            {
                case "on":
                    isOnline = true;
                    return string.Empty;
                case "off":
                    isOnline = false;
                    return string.Empty;
                default:
                    return UnknownCommand;
            }
        }

        private string ChangeNumericParametr(string[] input, int length, ref int parametr)
        {
            if (LengthsNotEqual(input, length))
                return InvalidNumberOfArguments;
            switch (input[length - 1])
            {
                case "up":
                    parametr = (parametr + 1) % 101;
                    return string.Empty;
                case "down":
                    parametr = (parametr - 1) >= 0 ? parametr - 1 : 0;
                    return string.Empty;
                default:
                    return UnknownCommand;
            }
        }

        private string SwitchOptions(string[] input)
        {
            if (input.Length < 2)
                return InvalidNumberOfArguments;
            switch (input[1])
            {
                case "change":
                    return SwitchOptionsParametr(input);
                case "show":
                    return ShowOptions();
                default:
                    return UnknownCommand;
            }
        }

        private string SwitchOptionsParametr(string[] input)
        {
            if (LengthsNotEqual(input, 4))
                return InvalidNumberOfArguments;
            switch (input[2])
            {
                case "brightness":
                    return ChangeNumericParametr(input, 4, ref brightness);
                case "contrast":
                    return ChangeNumericParametr(input, 4, ref contrast);
                default:
                    return UnknownCommand;
            }
        }

        private string ShowOptions()
        {
            return
                $"Options: \n" +
                $"Volume {volume}\n" +
                $"IsOnline {isOnline}\n" +
                $"Brightness {brightness}\n" +
                $"Contrast {contrast}";
        }

        private bool LengthsNotEqual(string[] input, int length) => input.Length != length;

        [TestFixture]
        private class TestRemoteController
        {
            private readonly RemoteController remoteController = new RemoteController();

            [Test]
            public void ChangeVolume()
            {
                remoteController.Call("volume up");
                Assert.AreEqual(21, remoteController.volume);
                remoteController.Call("volume down");
                Assert.AreEqual(20, remoteController.volume);
                remoteController.Call("volume abra");
                Assert.AreEqual(20, remoteController.volume);
            }

            [Test]
            public void ChangeBright()
            {
                remoteController.Call("options change brightness up");
                Assert.AreEqual(21, remoteController.brightness);
                remoteController.Call("options change brightness down");
                Assert.AreEqual(20, remoteController.brightness);
                remoteController.Call("options change brightness abra");
                Assert.AreEqual(20, remoteController.brightness);
            }

            [Test]
            public void ChangeContrast()
            {
                remoteController.Call("options change contrast up");
                Assert.AreEqual(21, remoteController.contrast);
                remoteController.Call("options change contrast down");
                Assert.AreEqual(20, remoteController.contrast);
                remoteController.Call("options change contrast abra");
                Assert.AreEqual(20, remoteController.contrast);
            }

            [Test]
            public void ChangeTVMode()
            {
                remoteController.Call("TV on");
                Assert.AreEqual(true, remoteController.isOnline);
                remoteController.Call("TV off");
                Assert.AreEqual(false, remoteController.isOnline);
                remoteController.Call("TV abra");
                Assert.AreEqual(false, remoteController.isOnline);
            }

            [Test]
            public void ShowOptions()
            {
                Assert.AreEqual(
                    $"Options: \n" +
                    $"Volume 20\n" +
                    $"IsOnline False\n" +
                    $"Brightness 20\n" +
                    $"Contrast 20",
                    remoteController.Call("options show"));
            }
        }
    }
}