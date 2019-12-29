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
            sb.AppendLine(Text);
            foreach (string s in Variants.Split(';'))
                sb.AppendLine(s.Trim());
            return sb.ToString();
        }
    }
}
