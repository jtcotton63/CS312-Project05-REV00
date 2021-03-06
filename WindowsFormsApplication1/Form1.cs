using System;
using System.Drawing;
using System.Windows.Forms;

namespace TSP
{
    public partial class mainform : Form
    {
        private Problem CityData;

        // Used to define the different kind of ways that the TSP can be solved
        // DEFAULT is used to indicate that the given problem should be solved
        // using the default implementation (DefaultSolver.cs).
        // BRANCH_AND_BOUND is used to indicate that the given problem should be solved
        // using the branch and bound method that you write (BranchAndBoundSolver.cs).
        // GREEDY --> The greedy method you will have to write for the group project (GreedySolver.cs)
        // FANCY --> Your own implementation that you will have to write for the 
        // group project (FancySolver.cs).
        // If you are wondering where these are used, see the handleToolStripMenuClick method
        private enum RunType { DEFAULT, BRANCH_AND_BOUND, GREEDY, FANCY }

        public mainform()
        {
            InitializeComponent();

            CityData = new Problem();
            this.tbSeed.Text = CityData.seed.ToString();
        }

        /*
         * GUI methods & event handlers
         */

        private void Form1_Load(object sender, EventArgs e)
        {
            this.reset();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        // overloaded to call the redraw method for CityData. 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetClip(new Rectangle(0, 0, this.Width, this.Height - this.toolStrip1.Height - 35));
            DrawCities(e.Graphics);
        }

        // draw the cities in the problem.  if the bssf member is defined, then
        // draw that too. 
        // <param name="g">where to draw the stuff</param>
        private void DrawCities(Graphics g)
        {
            // Take care of the brushes
            Brush cityBrushStyle = new SolidBrush(Color.Black);
            Brush cityBrushStartStyle = new SolidBrush(Color.Red);
            Pen routePenStyle = new Pen(Color.Blue, 1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            int CITY_ICON_SIZE = 5;
            float width = g.VisibleClipBounds.Width - 45F;
            float height = g.VisibleClipBounds.Height - 45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (CityData.bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[CityData.bssf.route.Count];
                int index = 0;
                foreach (City c in CityData.bssf.route)
                {
                    if (index < CityData.bssf.route.Count - 1)
                        g.DrawString(" " + index + "(" + c.costToGetTo(CityData.bssf.route[index + 1] as City) + ")",
                            labelFont,
                            cityBrushStartStyle,
                            new PointF((float)c.x * width + 3F,
                            (float)c.y * height));
                    else
                        g.DrawString(" " + index + "(" + c.costToGetTo(CityData.bssf.route[0] as City) + ")",
                            labelFont,
                            cityBrushStartStyle,
                            new PointF((float)c.x * width + 3F,
                            (float)c.y * height));
                    ps[index++] = new Point((int)(c.x * width) + CITY_ICON_SIZE / 2,
                        (int)(c.y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)CityData.cities[0].x * width - 1,
                        (float)CityData.cities[0].y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in CityData.cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.x * width, (float)c.y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }
        }

        /*
         * Button click handlers
         */

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

        private void resetToDefaults_Click(object sender, EventArgs e)
        {
            this.reset(Problem.DEFAULT_SEED, Problem.DEFAULT_PROBLEM_SIZE, Problem.DEFAULT_TIME_LIMIT);
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

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /*
         * TSP mode selection click - allows TSP to run in various modes
         */

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(RunType.DEFAULT);
        }

        private void bBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(RunType.BRANCH_AND_BOUND);
        }

        private void greedyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(RunType.GREEDY);
        }

        private void myTSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleToolStripMenuClick(RunType.FANCY);
        }

        private void handleToolStripMenuClick(RunType runType)
        {
            this.reset();

            tbElapsedTime.Text = " Running...";
            tbCostOfTour.Text = " Running...";
            Refresh();

            Solver solver;
            if (runType == RunType.BRANCH_AND_BOUND)
            {
                BranchAndBoundSolver babs = new BranchAndBoundSolver(CityData);
                CityData = babs.solve();
            }
            else if (runType == RunType.GREEDY)
            {
                CityData = GreedySolver.solve(CityData);
            }
            else if (runType == RunType.FANCY)
            {
                solver = new FancySolver();
                CityData = solver.solve(CityData);
            }
            else  // runType == RunType.DEFAULT
            {
                solver = new DefaultSolver();
                CityData = solver.solve(CityData);
            }

            tbCostOfTour.Text = CityData.bssf.costOfRoute.ToString();
            tbElapsedTime.Text = CityData.timeElasped.ToString();
            tbNumSolutions.Text = CityData.solutions.ToString();
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
                : Problem.DEFAULT_SEED;
        }

        // If the tbProblemSize box doesn't contain a valid integer,
        // returns the default value
        private int getProblemSize()
        {
            int size;
            return int.TryParse(this.tbProblemSize.Text, out size) ? int.Parse(this.tbProblemSize.Text) 
                : Problem.DEFAULT_PROBLEM_SIZE;
        }        

        // If the tbTimeLimit box doesn't contain a valid integer,
        // returns the default value
        private int getTimeLimit()
        {
            int timeLimit;
            return int.TryParse(this.tbTimeLimit.Text, out timeLimit) ? int.Parse(this.tbTimeLimit.Text) 
                : Problem.DEFAULT_TIME_LIMIT;
        }

        // Calls the reset(int, int, int) function using the current state values
        private void reset()
        {
            reset(getSeed(), getProblemSize(), getTimeLimit());
        }

        private void reset(int seed, int problemSize, int timeLimit) {
            this.toolStrip1.Focus();  // Not sure why this is here; leftover from previous code
            HardMode.Modes mode = getMode();

            CityData = new Problem(seed, problemSize, timeLimit, mode);

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