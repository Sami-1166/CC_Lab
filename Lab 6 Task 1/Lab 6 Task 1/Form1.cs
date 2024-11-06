namespace Lab_6_Task_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /*
        S → A B C
        A → a | ε
        B → b | A D
        C → c | ε
        D → d | ε

        */

        /*Valid Strings: abc, ab, a */

        private void button1_Click(object sender, EventArgs e)
        {
            string input = richTextBox1.Text.Trim();
            if (ParseS(ref input))
            {
                if (input == "") // Successful parsing
                    MessageBox.Show("Valid String", "Result");
                else
                    MessageBox.Show("Invalid String", "Result");
            }
            else
            {
                MessageBox.Show("Invalid String", "Result");
            }
        }

        // Grammar: S → A B C
        private bool ParseS(ref string input)
        {
            if (ParseA(ref input) && ParseB(ref input) && ParseC(ref input))
            {
                return true;
            }
            return false;
        }

        // Grammar: A → a | ε
        private bool ParseA(ref string input)
        {
            if (input.Length > 0 && input[0] == 'a')
            {
                input = input.Substring(1); // consume 'a'
                return true;
            }
            return true; // ε (empty string) case
        }

        // Grammar: B → b | A D
        private bool ParseB(ref string input)
        {
            if (input.Length > 0 && input[0] == 'b')
            {
                input = input.Substring(1); // consume 'b'
                return true;
            }
            else if (ParseA(ref input) && ParseD(ref input))
            {
                return true;
            }
            return false;
        }

        // Grammar: C → c | ε
        private bool ParseC(ref string input)
        {
            if (input.Length > 0 && input[0] == 'c')
            {
                input = input.Substring(1); // consume 'c'
                return true;
            }
            return true; // ε (empty string) case
        }

        // Grammar: D → d | ε
        private bool ParseD(ref string input)
        {
            if (input.Length > 0 && input[0] == 'd')
            {
                input = input.Substring(1); // consume 'd'
                return true;
            }
            return true; // ε (empty string) case
        }

    }
}
