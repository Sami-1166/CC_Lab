using System.Text.RegularExpressions;

namespace Lab_3_Task_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            string pattern = @"\b[tTmM]\w*";

            MatchCollection matches = Regex.Matches(inputText, pattern);

            if (matches.Count > 0)
            {
                string result = "Success! Words starting with 't' or 'm':\n";
                foreach (Match match in matches)
                {
                    result += match.Value + "\n";
                }
                MessageBox.Show(result);
            }
            else
            {
                MessageBox.Show("Condition not satisfied: No words starting with 't' or 'm' found.");
            }
        }
    }
}
