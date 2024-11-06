namespace Lab_4_Task_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            string result = LexicalAnalysis(inputText);
            richTextBox2.Text = result;
        }
        // Function to perform lexical analysis
        private string LexicalAnalysis(string input)
        {
            // Buffers for the lexical analyzer
            int bufferSize = 10; // Define the buffer size
            char[] buffer1 = new char[bufferSize];
            char[] buffer2 = new char[bufferSize];
            int currentBuffer = 1;

            string output = ""; // Output result
            int currentPosition = 0;

            while (currentPosition < input.Length)
            {
                // Fill buffer1
                int lengthToRead = Math.Min(bufferSize, input.Length - currentPosition);
                if (currentBuffer == 1)
                {
                    Array.Copy(input.ToCharArray(), currentPosition, buffer1, 0, lengthToRead);
                }
                else
                {
                    Array.Copy(input.ToCharArray(), currentPosition, buffer2, 0, lengthToRead);
                }

                currentPosition += lengthToRead;

                char[] currentBufferData = currentBuffer == 1 ? buffer1 : buffer2;
                currentBuffer = currentBuffer == 1 ? 2 : 1;

                // Lexical analysis on current buffer
                for (int i = 0; i < lengthToRead; i++)
                {
                    char ch = currentBufferData[i];

                    if (char.IsWhiteSpace(ch))
                    {
                        continue; // Skip whitespace
                    }
                    else if (char.IsLetter(ch))
                    {
                        string token = "";
                        // Collect a full token (word)
                        while (i < lengthToRead && char.IsLetter(currentBufferData[i]))
                        {
                            token += currentBufferData[i];
                            i++;
                        }
                        i--; // Step back after reading a word
                        output += "WORD: " + token + "\n";
                    }
                    else if (char.IsDigit(ch))
                    {
                        string token = "";
                        // Collect a full token (number)
                        while (i < lengthToRead && char.IsDigit(currentBufferData[i]))
                        {
                            token += currentBufferData[i];
                            i++;
                        }
                        i--; // Step back after reading a number
                        output += "NUMBER: " + token + "\n";
                    }
                    else
                    {
                        // Handle special characters or symbols
                        output += "SYMBOL: " + ch + "\n";
                    }
                }
            }

            return output;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
