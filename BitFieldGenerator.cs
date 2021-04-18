using System;
using System.IO;

namespace BitFields.CodeGeneration
{
    public static class BitFieldGenerator
    {
        public static void Generate(string templatePath, string outputDir, string extention, int maxWords = 8)
        {        
             const string FileTypeToken = @"${TYPE}";
             const string WordCountToken = @"${WORDCOUNT}";
             const string BitCountToken = @"${BITCOUNT}";
             
            for (var i = 1; i <= maxWords; i++)
            {
                var template = File.ReadAllText(templatePath);
                var type = $"BitField{32*i}";
                var wordCount = $"{i}";
                var bitCount = $"{32*i}";

                var fileContents = template
                    .Replace(FileTypeToken, type)
                    .Replace(WordCountToken, wordCount)
                    .Replace(BitCountToken, bitCount);
                
                File.WriteAllText(outputDir + type + extention, fileContents);
                Console.WriteLine($"Generated File: {type}");
            }
        }
    }
}
