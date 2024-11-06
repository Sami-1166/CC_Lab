using System.Text.RegularExpressions;

namespace Lab_5_Task_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string grammar = richTextBox1.Text;

            if (ValidateJavaGrammar(grammar))
            {
                label3.Text = "Valid Java Grammar!";
                label3.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label3.Text = "Invalid Java Grammar!";
                label3.ForeColor = System.Drawing.Color.Red;
            }
        }
        private bool ValidateJavaGrammar(string grammar)
        {
            // Class declaration: public class ClassName { ... }
            string classPattern = @"\bpublic\s+class\s+\w+\s*\{[\s\S]*\}";

            // Method declaration: public/private/protected ReturnType MethodName(...) { ... }
            string methodPattern = @"\b(public|private|protected)\s+[\w<>\[\]]+\s+\w+\s*\(.*\)\s*\{[\s\S]*\}";

            // Validate both patterns
            if (Regex.IsMatch(grammar, classPattern) && Regex.IsMatch(grammar, methodPattern))
            {
                return true;  // Valid Java grammar
            }

            return false;  // Invalid Java grammar
        }

    }
}
