﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Fclp;
using Functional;
using TagCloudCreation;
using TagCloudVisualization;
using Point = System.Drawing.Point;

namespace TagCloudApp
{
    internal class ConsoleUserInterface : UserInterface
    {
        private const string DefaultFont = "Microsoft Sans Serif";
        private const BrushesEnum DefaultBrush = BrushesEnum.Gradient;

        private readonly Dictionary<BrushesEnum, Brush> brushes = new Dictionary<BrushesEnum, Brush>
        {
            [BrushesEnum.Simple] = Brushes.Chartreuse, [BrushesEnum.Tough] = SystemBrushes.ControlDarkDark,
            [BrushesEnum.White] = Brushes.White, [BrushesEnum.Black] = Brushes.Black,
            [BrushesEnum.Gradient] =
                new LinearGradientBrush(Point.Empty, new Point(1000, 1000), Color.CadetBlue, Color.DeepPink)
        };

        private readonly FluentCommandLineParser parser;
        private Brush brush;
        private TagCloudVisualization.Point center;
        private string font;
        private bool isHelpShown;
        private string outputPath;

        private string wordsFile;

        public ConsoleUserInterface(
            TagCloudCreator creator,
            IEnumerable<ITextReader> readers,
            IPathValidator validator) : base(creator, readers)
        {
            Validator = validator;
            parser = new FluentCommandLineParser();
            SetupParser();
        }

        public IPathValidator Validator { get; }

        private static bool ValidateFontName(string name)
        {
            return new InstalledFontCollection().Families.Any(f => f.Name == name);
        }

        public override void Run(string[] startupArgs)
        {
            parser.Parse(startupArgs);
            if (isHelpShown)
            {
                End();
                return;
            }

            var options = new TagCloudCreationOptions(new ImageCreatingOptions(brush, font, center));

            Read(wordsFile)
                .Then(ws => Creator.CreateImage(ws, options))
                .Then(i => i.Save(outputPath))
                .Then(none => End($"Here you go{Environment.NewLine}\tFile is saved successfully into {outputPath}"))
                .OnFail(err => End(error: err));
        }

        private static void End(string message = null, string error = null)
        {
            if (message != null)
                Console.WriteLine(message);
            if (error != null)
                Console.Error.WriteLine(error);
            Console.ReadLine();
        }

        private void SetupParser()
        {
            parser.Setup<string>('f', "words")
                  .Callback(SetWords)
                  .Required();

            parser.SetupHelp()
                  .UseForEmptyArgs()
                  .Callback(ShowHelp);

            parser.Setup<string>('c', "center")
                  .WithDescription("two integers separated by space: x y")
                  .Callback(SetCenter)
                  .SetDefault("250 250");

            parser.Setup<string>('o', "output")
                  .Callback(SetOutput)
                  .SetDefault($"..{Path.DirectorySeparatorChar}tag_cloud.png");

            parser.Setup<string>("font")
                  .WithDescription($"Font name, default is {DefaultFont}")
                  .Callback(SetFontName)
                  .SetDefault(DefaultFont);

            parser.Setup<BrushesEnum>("brush")
                  .WithDescription($"Brush to paint the words with, default is {DefaultBrush} one")
                  .Callback(arg => brush = brushes[arg])
                  .SetDefault(DefaultBrush);
        }

        private void ShowHelp(string t)
        {
            isHelpShown = true;
            Console.WriteLine(t);
        }

        private void SetFontName(string name)
        {
            if (ValidateFontName(name))
                font = name;
            else
                throw new ArgumentException("This font is not installed");
        }

        private void SetWords(string path)
        {
            if (Validator.Validate(path))
                wordsFile = path;
            else
                throw new ArgumentException("Path is invalid");
        }

        private void SetOutput(string path)
        {
            if (Validator.Validate(path))
                outputPath = path;
            else
                throw new ArgumentException("Path is invalid");
        }

        private void SetCenter(string rawCenter)
        {
            var coordinates = rawCenter.Split(null)
                                       .Select(int.Parse)
                                       .ToArray();
            center = new TagCloudVisualization.Point(coordinates[0], coordinates[1]);
        }

        [Flags]
        private enum BrushesEnum
        {
            Simple,
            Tough,
            White,
            Gradient,
            Black
        }
    }
}
