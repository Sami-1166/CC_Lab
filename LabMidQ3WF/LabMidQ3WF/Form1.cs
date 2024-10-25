namespace LabMidQ3WF
{
    public partial class Form1 : Form
    {
        // Define states
        private const string InitialState = "S0"; // Before starting the car
        private const string StartedState = "S1";  // After starting the car

        // Create a dictionary to define the state transitions
        private static readonly Dictionary<string, Dictionary<string, string>> stateTransition = new Dictionary<string, Dictionary<string, string>>
        {
            { InitialState, new Dictionary<string, string> { { "start", StartedState } } },
            { StartedState, new Dictionary<string, string>
                {
                    { "stop", InitialState },  // Can stop the car, goes back to initial state
                    { "accelerate", StartedState },
                    { "brake", StartedState },
                    { "right", StartedState },
                    { "left", StartedState }
                }
            }
        };

        private string currentState;

        public Form1()
        {
            InitializeComponent();
            currentState = InitialState; // Start in the initial state
            textBox1.ReadOnly = true; // Make the output textbox read-only
        }


        private void ExecuteCommand(string command)
        {
            // Check if the current state has a valid transition for the given command
            if (stateTransition.ContainsKey(currentState) && stateTransition[currentState].ContainsKey(command))
            {
                currentState = stateTransition[currentState][command]; // Move to the next state
                PrintCommandOutput(command); // Output the command effect
            }
            else
            {
                textBox1.AppendText($"Invalid command '{command}' for the current state.\n");
            }
        }

        // Output the effect of each command
        private void PrintCommandOutput(string command)
        {
            switch (command)
            {
                case "start":
                    textBox1.Clear();
                    textBox1.AppendText("Car started.\n");
                    break;
                case "stop":
                    textBox1.Clear();
                    textBox1.AppendText("Car stopped.\n");
                    break;
                case "accelerate":
                    textBox1.Clear();
                    textBox1.AppendText("Car is accelerating.\n");
                    break;
                case "brake":
                    textBox1.Clear();
                    textBox1.AppendText("Car is braking.\n");
                    break;
                case "right":
                    textBox1.Clear();
                    textBox1.AppendText("Car turned right.\n");
                    break;
                case "left":
                    textBox1.Clear();
                    textBox1.AppendText("Car turned left!\n");
                    break;
                default:
                    textBox1.AppendText($"Unknown command: {command}\n");
                    break;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExecuteCommand("start");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ExecuteCommand("stop");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ExecuteCommand("accelerate");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExecuteCommand("brake");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExecuteCommand("right");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ExecuteCommand("left");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
