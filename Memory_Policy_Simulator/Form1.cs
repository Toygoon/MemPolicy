using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Memory_Policy_Simulator {
    public partial class Form1 : Form {
        Graphics g;
        PictureBox pbPlaceHolder;
        Bitmap bResultImage;

        public Form1() {
            InitializeComponent();
            this.pbPlaceHolder = new PictureBox();
            this.bResultImage = new Bitmap(2048, 2048);
            this.pbPlaceHolder.Size = new Size(2048, 2048);
            g = Graphics.FromImage(this.bResultImage);
            pbPlaceHolder.Image = this.bResultImage;
            this.pImage.Controls.Add(this.pbPlaceHolder);
        }

        private void DrawGrid(int x, int y) {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawRectangle(new Pen(Color.White), new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridHighlight(int x, int y, Page.STATUS status) {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            SolidBrush highlighter = new SolidBrush(Color.LimeGreen);

            if (status != Page.STATUS.HIT)
                highlighter.Color = Color.Red;

            g.FillRectangle(highlighter, new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridText(int x, int y, char value) {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawString(
                value.ToString(),
                new Font(FontFamily.GenericMonospace, 8),
                new SolidBrush(Color.White),
                new PointF(
                    gridBaseX + (x * gridSpace) + gridSize / 3,
                    gridBaseY + gridSize / 4));
        }

        private void btnOperate_Click(object sender, EventArgs e) {
            this.tbConsole.Clear();

            if (this.tbQueryString.Text != "" || this.tbWindowSize.Text != "") {
                string data = this.tbQueryString.Text;
                int windowSize = int.Parse(this.tbWindowSize.Text);

                /* initalize */
                var window = new Algs_Optimal(windowSize, data);

                g.Clear(Color.Black);

                int i = 0;
                foreach (char element in data) {
                    Page p = window.Operate(element);
                    this.tbConsole.Text += "DATA " + element + " is " +
                        (p.status == Page.STATUS.HIT ? "Hit" : p.status == Page.STATUS.PAGEFAULT ? "Page fault (New)" : "Page fault (" + p.before + "→" + p.data + ")")
                        + "\r\n";
                    DrawBase(window, p, windowSize, i++);
                }


                this.pbPlaceHolder.Refresh();

                /* 차트 생성 */
                chart1.Series.Clear();
                Series resultChartContent = chart1.Series.Add("Statics");
                resultChartContent.ChartType = SeriesChartType.Pie;
                resultChartContent.IsVisibleInLegend = true;
                resultChartContent.Points.AddXY("Hit", window.hit);
                resultChartContent.Points.AddXY("Page Fault", window.fault);
                resultChartContent.Points[0].IsValueShownAsLabel = true;
                resultChartContent.Points[1].IsValueShownAsLabel = true;

                this.lbPageFaultRatio.Text = Math.Round(((float)window.fault / (window.fault + window.hit)), 2) * 100 + "%";
            }
        }

        private void DrawBase(Algs core, Page page, int windowSize, int currentSeq) {
            // Set current frame
            var frameWindow = core.frameWindow;

            // Print current step
            DrawGridText(currentSeq, 0, page.data);

            // Draw empty grids
            for (int i = 1; i <= windowSize; i++)
                DrawGrid(currentSeq, i);

            // Highlight current issued page
            DrawGridHighlight(currentSeq, frameWindow.IndexOf(frameWindow.Find(x => x.data == page.data)) + 1, page.status);

            // Draw existing texts in the grid
            for (int i = 1; i <= frameWindow.Count; i++)
                DrawGridText(currentSeq, i, frameWindow[i - 1].data);
        }



        private void pbPlaceHolder_Paint(object sender, PaintEventArgs e) {
        }

        private void chart1_Click(object sender, EventArgs e) {

        }

        private void tbWindowSize_KeyDown(object sender, KeyEventArgs e) {

        }

        private void tbWindowSize_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8) {
                e.Handled = true;
            }
        }

        private void btnRand_Click(object sender, EventArgs e) {
            Random rd = new Random();

            int count = rd.Next(5, 50);
            StringBuilder sb = new StringBuilder();


            for (int i = 0; i < count; i++) {
                sb.Append((char)rd.Next(65, 90));
            }

            this.tbQueryString.Text = sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            bResultImage.Save("./result.jpg");
        }

        private void tbQueryString_TextChanged(object sender, EventArgs e) {

        }
    }
}
