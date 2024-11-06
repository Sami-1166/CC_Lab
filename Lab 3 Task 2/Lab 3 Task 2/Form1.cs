using System.Text.RegularExpressions;

namespace Lab_3_Task_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the text from the TextBox (replace 'textBox1' with the actual name of your TextBox)
            string input = textBox1.Text;

            // Regular expression to match scientific notation format
            string pattern = @"^-?\d+(\.\d+)?[eE][-+]?\d+$";

            // Validate the input using the regular expression
            if (Regex.IsMatch(input, pattern))
            {
                // Show success message
                MessageBox.Show("Success! The input is in correct scientific notation format.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Show error message
                MessageBox.Show("Invalid input. Please enter a number in scientific notation format like 8e4 or 5e-2.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
