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
        public int ChosenAnswer { get; set; }
        public int RightAnswer { get; set; }
        public bool IsRight { get; set; } = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!Program.RANDOMIZEANSWERS)
            {
                //  Do not randomize possible answers
                sb.AppendLine(Text);
                foreach (string s in Variants.Split(';'))
                    sb.AppendLine(s.Trim());
                return sb.ToString();
            }
            else
            {
                //  Randomize possible answers
                string[] splittedVariants = Variants.Split(';').Where(s => s != "").Select(s => s).ToArray();
                string rightAnswerText = splittedVariants[RightAnswer - 1];
                RightAnswer = Array.IndexOf(splittedVariants, rightAnswerText) + 1;
                splittedVariants = new List<string>(splittedVariants).Shuffle().ToArray();
                for (int i = 0; i < splittedVariants.Length; i++)
                    sb.AppendLine($"{i + 1}) {splittedVariants[i].Trim().Substring(3)}");
                return sb.ToString();
            }
        }
    }
}
