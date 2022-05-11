using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace ImageParser
{
    public class ImageParser
    {
        byte StrSignatureToByte(string strSignature)
        {
            return Byte.Parse(strSignature, NumberStyles.HexNumber);
        }

        Dictionary<byte, string> CreateDictionaryOfSignatures()
        {
            //var strGifSignature1 = "47 49 46 38 39 61";
            //var strGifSignature2 = "47 49 46 38 37 61";
            //var strBmSignature1 = "4D 42";
            //var strBmSignature2 = "42 4D";
            //var strPngSignature = "89 50 4E 47 0D 0A 1A 0A";
            var formats = new Dictionary<byte, string>(4);
            formats[StrSignatureToByte("42")] = "Bmp";
            formats[StrSignatureToByte("47")] = "Gif";
            formats[StrSignatureToByte("89")] = "Png";
            return formats;
        }

        int ConvertFromBytesToNumber (byte[] bytes, int numberOfBytes)
        {
            if (numberOfBytes == 4)
            {
                if (bytes[0] == 0 && bytes[1] == 0)
                    Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
            else
                return BitConverter.ToUInt16(bytes, 2);
        }

        int CalculateValue(Stream stream, int position, int lenght)
        {
            stream.Position = position;
            var readBytes = new byte[4];
            stream.Read(readBytes, 4-lenght, lenght);
            return ConvertFromBytesToNumber(readBytes, lenght);
        }

        void CalculateWidthAndHeight (Stream stream,int startPosition,int lenght, Dictionary<string, object> imageData)
        {
            imageData["Width"] = CalculateValue(stream, startPosition, lenght);
            imageData["Height"] = CalculateValue(stream, startPosition+lenght, lenght);
        }

        void IdentifyWidthAndHeight(Dictionary < string, object > imageData,Stream stream)
        {
            switch (imageData["Format"])
            {
                case "Bmp":
                    CalculateWidthAndHeight(stream, 18, 4,imageData);
                    break;
                case "Gif":
                    CalculateWidthAndHeight(stream, 6, 2, imageData);
                    break;
                case "Png":
                    CalculateWidthAndHeight(stream, 16, 4,imageData);
                    break;
            }
        }

        void IdentifyFormat(Stream stream, Dictionary<string, object> imageData)
        {
            var formats = CreateDictionaryOfSignatures();
            var firstByte = new byte[1];
            stream.Read(firstByte, 0, 1);
            imageData["Format"] = formats[firstByte[0]];
        }

        public string GetImageInfo(Stream stream)
        {
            var imageData = new Dictionary<string, object>(4);
            imageData["Size"] = stream.Length;
            IdentifyFormat(stream,imageData);
            IdentifyWidthAndHeight(imageData,stream);
            return JsonConvert.SerializeObject(imageData);
        }
    }
}