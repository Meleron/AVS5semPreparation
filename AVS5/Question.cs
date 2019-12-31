using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVS5
{
    class Question
    {
        public string Text { get; set; }
        public string Variants { get; set; }
        public int ChosenAnswer { get; set; } = 0;
        public int RightAnswer { get; set; }
        public bool IsRight { get; set; } = false;
        public bool IsRandomized { get; set; } = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Text);
            if (!Program.RANDOMIZEANSWERS || IsRandomized)
            {
                //  Do not randomize possible answers
                foreach (string s in Variants.Split(';'))
                    sb.AppendLine(s.Trim());
                return sb.ToString();
            }
            else
            {
                //  Randomize possible answers
                string[] splittedVariants = Variants.Split(';').Where(s => s != "").Select(s => s).ToArray();
                string rightAnswerText = splittedVariants[RightAnswer - 1];
                splittedVariants = new List<string>(splittedVariants).Shuffle().ToArray();
                RightAnswer = Array.IndexOf(splittedVariants, rightAnswerText) + 1;
                Variants = "";
                for (int i = 0; i < splittedVariants.Length; i++)
                {
                    sb.AppendLine($"{i + 1}) {splittedVariants[i].Trim().Substring(3)}");
                    Variants += string.Format($"{ i + 1}) { splittedVariants[i].Trim().Substring(3)};");
                }
                IsRandomized = true;
                return sb.ToString();
            }
        }
    }
}
