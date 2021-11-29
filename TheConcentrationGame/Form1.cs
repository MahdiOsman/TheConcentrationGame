using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheConcentrationGame.Properties;

namespace TheConcentrationGame
{
    public partial class MainForm : Form
    {
        List<Button> buttons = new List<Button>(); // List of buttons
        Random random = new Random();
        private Button firstGuess, secondGuess;
        private int cardMatchRule = 2; // 2 = Match 2   /   2 = Match 3
        private int numberOfUserGuesses = 0;
        private int numberOfMatchedCards = 0;

        public MainForm()
        {
            InitializeComponent();
            StartSmallRule(); // Start with small rule
            smallToolStripMenuItem.Checked = true;
        }

        // About box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        // Win menu
        void Win()
        {   
            MessageBox.Show($"Well done! You beat the game in {numberOfUserGuesses} clicks.", "You Won!");
            ResetCards();
        }

        // Create the Small Rule version of the game
        // Add buttons to the flowLayoutPanel
        private void StartSmallRule()
        {
            for (int i = 0; i < 5 * cardMatchRule; i++)
            {
                buttons.Add(new Button());
            }

            foreach (var b in buttons)
            {
                b.Size = new Size(60, 60);
                b.Click += B_Click;
                flowLayoutPanel.Controls.Add(b);
            }

            ResetCards();
        }

        // Create the Medium Rule version of the game
        // Add buttons to the flowLayoutPanel
        private void StartMediumRule()
        {
            for (int i = 0; i < 11 * cardMatchRule; i++)
            {
                buttons.Add(new Button());
            }

            foreach (var b in buttons)
            {
                b.Size = new Size(60, 60);
                b.Click += B_Click;
                flowLayoutPanel.Controls.Add(b);
            }

            ResetCards();
        }

        // Create the Large Rule version of the game
        // Add buttons to the flowLayoutPanel
        private void StartLargeRule()
        {
            for (int i = 0; i < 17 * cardMatchRule; i++)
            {
                buttons.Add(new Button());
            }

            foreach (var b in buttons)
            {
                b.Size = new Size(60, 60);
                b.Click += B_Click;
                flowLayoutPanel.Controls.Add(b);
            }

            ResetCards();
        }

        // Validation for matching 2 cards of the same type
        private async void MatchTwoValidation(object sender)
        {
            Button b = (Button)sender;
            ResourceManager rm = Resources.ResourceManager;
            Bitmap cardImage = (Bitmap)rm.GetObject($"image{b.Tag}");

            // Game logic
            if (firstGuess == null)
            {
                firstGuess = b;
                firstGuess.Enabled = false;
                firstGuess.BackgroundImage = cardImage;
            }
            else if (firstGuess.Tag.ToString() == b.Tag.ToString())
            {
                // Increase number of matched cards
                numberOfMatchedCards += 2;

                b.BackgroundImage = cardImage;
                b.Enabled = false;
                firstGuess = null;
            }
            else if (firstGuess.Tag.ToString() != b.Tag.ToString())
            {
                b.BackgroundImage = cardImage;
                b.Enabled = false;

                // Show images for x ms
                // the delay time
                this.Enabled = false;
                await Task.Delay(200);
                this.Enabled = true;

                // Revert cards back to default
                b.BackgroundImage = Properties.Resources.Question_Mark;
                b.Enabled = true;
                firstGuess.BackgroundImage = Properties.Resources.Question_Mark;
                firstGuess.Enabled = true;
                firstGuess = null;
            }

            // Check if player has won
            if (numberOfMatchedCards == (buttons.Count()))
                Win();
        }

        // Validation for matching 3 cards of the same type 
        private async void MatchThreeValidation(object sender)
        {
            Button b = (Button)sender;
            ResourceManager rm = Resources.ResourceManager;
            Bitmap cardImage = (Bitmap)rm.GetObject($"image{b.Tag}");

            if (firstGuess == null)
            {
                firstGuess = b;
                firstGuess.Enabled = false;
                firstGuess.BackgroundImage = cardImage;
            }
            else if (secondGuess == null && firstGuess.Tag.ToString() == b.Tag.ToString())
            {
                secondGuess = b;
                secondGuess.Enabled = false;
                secondGuess.BackgroundImage = cardImage;
            }
            else if (secondGuess != null && secondGuess.Tag.ToString() == b.Tag.ToString())
            {
                numberOfMatchedCards += 3;

                b.Enabled = false;
                b.BackgroundImage = cardImage;
                firstGuess = null;
                secondGuess = null;
            }
            else if (secondGuess == null && firstGuess.Tag.ToString() != b.Tag.ToString())
            {
                b.BackgroundImage = cardImage;
                b.Enabled = false;

                this.Enabled = false;
                await Task.Delay(200);
                this.Enabled = true;

                // Revert cards back to default
                b.BackgroundImage = Properties.Resources.Question_Mark;
                b.Enabled = true;
                firstGuess.BackgroundImage = Properties.Resources.Question_Mark;
                firstGuess.Enabled = true;
                firstGuess = null;
            }
            else if (secondGuess != null && secondGuess.Tag.ToString() != b.Tag.ToString())
            {
                b.BackgroundImage = cardImage;
                b.Enabled = false;

                this.Enabled = false;
                await Task.Delay(200);
                this.Enabled = true;

                // Revert cards back to default
                b.BackgroundImage = Properties.Resources.Question_Mark;
                b.Enabled = true;
                firstGuess.BackgroundImage = Properties.Resources.Question_Mark;
                firstGuess.Enabled = true;
                firstGuess = null;
                secondGuess.BackgroundImage = Properties.Resources.Question_Mark;
                secondGuess.Enabled = true;
                secondGuess = null;
            }

            // Check if player has won
            if (numberOfMatchedCards == (buttons.Count()))
                Win();
        }

        private void B_Click(object sender, EventArgs e)
        {
            // Increase player numberOfUserGuesses
            numberOfUserGuesses++;

            if (cardMatchRule == 2)
                MatchTwoValidation(sender);
            else if (cardMatchRule == 3)
                MatchThreeValidation(sender);
        }

        // Reset cards back to their initial state
        private void ResetCards()
        {
            numberOfUserGuesses = 0; // Reset player guesses
            numberOfMatchedCards = 0; // Reset number of matched cards

            foreach(var b in buttons)
            {
                b.Tag = null;
                b.Visible = true;
                b.Enabled = true;
            }

            HideCards();
            if (cardMatchRule == 2)
                SetRandomCardsMatchTwo();
            else if (cardMatchRule == 3)
                SetRandomCardsMatchThree();
        }

        // Organise cards randomly by assigning each pair of cards a specific number
        // through the Tag property 
        // 2 cards needed for match 
        private void SetRandomCardsMatchTwo()
        {
            // Array of ids for buttons with size of buttons
            int[] id = new int[buttons.Count()];
            int i = 0;

            // Initialize array e.g. 0 1 2 3 4 4 3 2 1 0
            for (int k = 0; k < buttons.Count() / 2; k++)
            {
                id[k] = k;
                id[buttons.Count() - k - 1] = k;
            }

            // Shuffle array
            id = id.OrderBy(x => random.Next()).ToArray();

            foreach (var b in buttons) 
            {
                b.Tag = id[i].ToString();
                i++;
            }
        }

        // Organise cards randomly by assigning each pair of cards a specific number
        // through the Tag property 
        // 3 cards needed for match 
        private void SetRandomCardsMatchThree()
        {
            // Array of ids for buttons with size of buttons
            int[] id = new int[buttons.Count()];
            int i = 0;

            // Initialize array e.g. 0 0 1 1 2 2 3 3 4 4 3 2 1 0
            for (int k = 0; k < buttons.Count(); k++)
            {
                if (k % 3 == 0)
                    i++;
                
                id[k] = i;
            }
            i = 0; // Reset i

            // Shuffle array
            id = id.OrderBy(x => random.Next()).ToArray();

            foreach (var b in buttons)
            {
                b.Tag = id[i].ToString();
                i++;
            }
        }

        // Hide all cards
        private void HideCards()
        {
            foreach (var b in buttons)
            {
                b.BackgroundImage = Properties.Resources.Question_Mark;
                b.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        // Exit game
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Start new game
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetCards();
        }

        // Small game size
        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Uncheck other menu items
            mediumToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;

            // Clear all previous buttons
            foreach (var b in buttons)
            {
                flowLayoutPanel.Controls.Clear();
            }
            buttons.Clear(); // Empty List
            StartSmallRule(); // Start rule
        }

        // Medium game size
        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Uncheck other menu items
            smallToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;

            // Clear all previous buttons
            foreach (var b in buttons)
            {
                flowLayoutPanel.Controls.Clear();   
            }
            buttons.Clear(); // Empty List
            StartMediumRule(); // Start rule
        }

        // Large game size
        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Uncheck other menu items
            smallToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;

            // Clear all previous buttons
            foreach (var b in buttons)
            {
                flowLayoutPanel.Controls.Clear();
            }
            buttons.Clear(); // Empty List
            StartLargeRule(); // Start rule
        }

        private void match2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            match3ToolStripMenuItem.Checked = false;
            cardMatchRule = 2;

            // Clear all previous buttons
            foreach (var b in buttons)
            {
                flowLayoutPanel.Controls.Clear();
            }
            buttons.Clear(); // Empty List

            if (smallToolStripMenuItem.Checked == true)
                StartSmallRule();
            else if (mediumToolStripMenuItem.Checked == true)
                StartMediumRule();
            else if (largeToolStripMenuItem.Checked == true)
                StartLargeRule();
        }

        private void match3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            match2ToolStripMenuItem.Enabled = true;
            cardMatchRule = 3;

            // Clear all previous buttons
            foreach (var b in buttons)
            {
                flowLayoutPanel.Controls.Clear();
            }
            buttons.Clear(); // Empty List

            if (smallToolStripMenuItem.Checked == true)
                StartSmallRule();
            else if (mediumToolStripMenuItem.Checked == true)
                StartMediumRule();
            else if (largeToolStripMenuItem.Checked == true)
                StartLargeRule();
        }
    }
}
