using System;
using System.Drawing;
using System.Windows.Forms;

namespace TSP
{
    public partial class mainform : Form
    {
        private ProblemAndSolver CityData;

        // Switch case strings for TSP run modes
        private int DEFAULT = 0;
        private int BRANCH_AND_BOUND = 1;
        private int GREEDY = 2;
        private int FANCY = 3;

        public mainform()
        {
            InitializeComponent();

            CityData = new ProblemAndSolver();
            this.tbSeed.Text = CityData.Seed.ToString();
        }

        /*
         * GUI methods & event handlers
         */

        private void AlgorithmMenu2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            AlgorithmMenu2.Text = e.ClickedItem.Text;
            AlgorithmMenu2.Tag = e.ClickedItem;
        }

        private void AlgorithmMenu2_ButtonClick_1(object sender, EventArgs e)
        {
            if (AlgorithmMenu2.Tag != null)
            {
                (AlgorithmMenu2.Tag as ToolStripMenuItem).PerformClick();
            }
            else
            {
                AlgorithmMenu2.ShowDropDown();
            }
        }

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reset();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.reset();
        }

        // overloaded to call the redraw method for CityData. 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetClip(new Rectangle(0, 0, this.Width, this.Height - this.toolStrip1.Height - 35));
            CityData.Draw(e.Graphics);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void generate_Click(object sender, EventArgs e)
        {
            this.reset();
        }

        private void randomProblem_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int seed = random.Next(1000); // 3-digit random number
            this.reset(seed, getProblemSize(), getTimeLimit());
        }

        // Please note that clicking the New Problem button and
        // the Random Problem button do the same thing
        private void resetToDefaults_Click(object sender, EventArgs e)
        {
            reset(ProblemAndSolver.DEFAULT_SEED,
                  ProblemAndSolver.DEFAULT_PROBLEM_SIZE,
                  ProblemAndSolver.DEFAULT_TIME_LIMIT
                  );
        }

        private void tbProblemSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.reset();
        }

        private void tbProblemSize_Leave(object sender, EventArgs e)
        {
            this.reset();
        }

        private void tbSeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.reset();
        }

        private void tbSeed_Leave(object sender, EventArgs e)
        {
            this.reset();
        }

        private void tbTimeLimit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.reset();
        }

        private void tbTimeLimit_Leave(object sender, EventArgs e)
        {
            this.reset();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /*
         * GUI - Button clicks to run TSP in various modes
         */

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(DEFAULT);
        }

        private void bBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(BRANCH_AND_BOUND);
        }

        private void greedyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(GREEDY);
        }

        private void myTSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(FANCY);
        }

        private void handleToolStripMenuClick(int originCode)
        {
            string[] results;
            this.reset();

            tbElapsedTime.Text = " Running...";
            tbCostOfTour.Text = " Running...";
            Refresh();

            if (originCode == BRANCH_AND_BOUND)
            {
                results = CityData.solveBranchAndBound();
            }
            else if (originCode == GREEDY)
            {
                results = CityData.solveGreedy();
            }
            else if (originCode == FANCY)
            {
                results = CityData.solveFancy();
            }
            else  // originCode == DEFAULT
            {
                results = CityData.solveDefault();
            }

            results = CityData.solveDefault();

            tbCostOfTour.Text = results[ProblemAndSolver.COST];
            tbElapsedTime.Text = results[ProblemAndSolver.TIME];
            tbNumSolutions.Text = results[ProblemAndSolver.COUNT];
            Invalidate();                          // force a refresh.
        }

        /*
         * Model manipulation methods
         */

        private HardMode.Modes getMode()
        {
            return HardMode.getMode(cboMode.Text);
        }

        // If the tbSeed box doesn't contain a valid integer,
        // returns the default value
        private int getSeed()
        {
            int seed;
            return int.TryParse(this.tbSeed.Text, out seed) ? int.Parse(this.tbSeed.Text)
                : ProblemAndSolver.DEFAULT_SEED;
        }

        // If the tbProblemSize box doesn't contain a valid integer,
        // returns the default value
        private int getProblemSize()
        {
            int size;
            return int.TryParse(this.tbProblemSize.Text, out size) ? int.Parse(this.tbProblemSize.Text) 
                : ProblemAndSolver.DEFAULT_PROBLEM_SIZE;
        }        

        // If the tbTimeLimit box doesn't contain a valid integer,
        // returns the default value
        private int getTimeLimit()
        {
            int timeLimit;
            return int.TryParse(this.tbTimeLimit.Text, out timeLimit) ? int.Parse(this.tbTimeLimit.Text) 
                : ProblemAndSolver.DEFAULT_TIME_LIMIT;
        }

        // Calls the reset(int, int, int) function using the current state values
        private void reset()
        {
            reset(getSeed(), getProblemSize(), getTimeLimit());
        }

        private void reset(int seed, int problemSize, int timeLimit) {
            this.toolStrip1.Focus();  // Not sure why this is here; leftover from previous code
            HardMode.Modes mode = getMode();

            CityData = new ProblemAndSolver(seed, problemSize, timeLimit, mode);
            //CityData.GenerateProblem(problemSize, mode, timeLimit);

            tbSeed.Text = seed.ToString();
            tbProblemSize.Text = problemSize.ToString();
            tbTimeLimit.Text = timeLimit.ToString();
            tbCostOfTour.Text = " --";
            tbElapsedTime.Text = " --";
            tbNumSolutions.Text = " --";              // re-blanking the text boxes that may have been modified by a solver
            this.Invalidate();
        }
    }
}