using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Birds
{
    public partial class Form1 : Form
    {
        private Bird[] b;
        private readonly int size = 2;
        private readonly int n = 2000;
        private readonly int maxSpeed = 2, maxTurn = 1;
        private readonly int maxWSpeed = 30;
        //private readonly int clumpDensity = 3;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size region = new Size(PB_main.Width - size, PB_main.Height - size);
            Random rng = new Random();
            b = new Bird[n];
            for (int i = 0; i <= n - 1; i++)
            {
                b[i] = new Bird(rng.Next(1, maxSpeed + 1), region, rng.Next(), rng.Next(1, maxTurn + 1));
            }
        }

        private void PB_main_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Bird bi in b)
            {
                g.FillRectangle(new SolidBrush(bi.Color), new Rectangle(bi.Pos.X, bi.Pos.Y, size, size));
            }
        }

        private void PB_main_Click(object sender, EventArgs e)
        {
            foreach (Bird bi in b)
            {
                bi.Confuse();
            }
        }

        private void TIM_clock_Tick(object sender, EventArgs e)
        {
            int avgx1, avgy1, avgx2, avgy2;
            int sumx1 = 0, sumy1 = 0, sumx2 = 0, sumy2 = 0;
            Point[] targets = new Point[0];
            Random rng = new Random();
            int n = 0;
            foreach (Bird bi in b)
            {
                if (n % 2 == 0)
                {
                    sumx1 += bi.Pos.X; sumy1 += bi.Pos.Y;
                }
                else
                {
                    sumx2 += bi.Pos.X; sumy2 += bi.Pos.Y;
                }
                n++;
            }
            avgx1 = sumx1 / (n / 2); avgy1 = sumy1 / (n / 2);
            avgx2 = sumx2 / (n / 2); avgy2 = sumy2 / (n / 2);
            //int rad = 1;
            //while (targets.Count() < 3)
            //{
            //    targets = GetTargets(rad, clumpDensity);
            //    rad++;
            //}
            n = rng.Next(0,2);
            foreach (Bird bi in b)
            {
                //double distMin = PB_main.Width;
                Point t;
                if (n % 2 == 0)
                {
                    t = new Point(avgx1, avgy1);
                }
                else
                {
                    t = new Point(avgx2, avgy2);
                }
                //foreach (Point target in targets)
                //{
                //    int diffx, diffy;
                //    double dist;
                //    diffx = bi.Pos.X - target.X; diffy = bi.Pos.Y - target.Y;
                //    dist = Math.Sqrt(Math.Pow(diffx, 2) + Math.Pow(diffy, 2));
                //    if (dist < distMin)
                //    {
                //        distMin = dist;
                //        t = target;
                //    }
                //}
                bi.Step(new Point(t.X, t.Y), new Point(rng.Next(-maxWSpeed, maxWSpeed + 1), rng.Next(-maxWSpeed, maxWSpeed + 1)), rng.Next(-1, 2));
                n++;
            }
            PB_main.Refresh();
        }

        private Point[] GetTargets(int rad, int density)
        {
            List<Point> output = new List<Point>();
            for (int x = 0; x <= PB_main.Width; x++)
            {
                for (int y = 0; y <= PB_main.Height; y++)
                {
                    if (NoInRange(rad, x, y) >= density)
                    {
                        output.Add(new Point(x, y));
                    }
                }
            }
            return output.ToArray();
        }

        private int NoInRange(int range, int x, int y)
        {
            int n = 0;
            foreach (Bird bi in b)
            {
                int diffx, diffy;
                diffx = bi.Pos.X - x; diffy = bi.Pos.Y - y;
                if (Math.Abs(diffx) <= range && Math.Abs(diffy) <= range)
                {
                    n++;
                }
            }
            return n;
        }
    }
    public class Bird
    {
        public Point Pos { get { return new Point(x, y); } }
        public Color Color { get { return c; } }
        private int x, y;
        private int speed, turnSpeed;
        private double dir;
        private Size region;
        private int confTickCount = 0;
        private Color c;
        public Bird(int speedIn, Size regionIn, int seed, int turnSpeedIn)
        {
            Random rng = new Random(seed);
            x = rng.Next(0, regionIn.Width); y = rng.Next(0, regionIn.Height);
            speed = speedIn; turnSpeed = turnSpeedIn;
            region = regionIn;
        }
        public void Step(Point target, Point wind, int dirShift) 
        {
            double anglefromVert = 0;
            int movex, movey;
            double diffx, diffy;
            double clockwise, antic;
            double distance;
            diffx = target.X - x; diffy = target.Y - y;

            if (confTickCount > 0)
            {
                diffx *= -1; diffy *= -1;
                confTickCount--;
            }

            {
                if (diffx > 0 && diffy < 0)
                {
                    anglefromVert = Math.Atan(diffx / (diffy * -1));
                }
                else if (diffx > 0 && diffy == 0)
                {
                    anglefromVert = Math.PI / 2;
                }
                else if (diffx > 0 && diffy > 0)
                {
                    anglefromVert = Math.Atan(diffx / (diffy * -1)) + Math.PI;
                }
                else if (diffx == 0 && diffy > 0)
                {
                    anglefromVert = Math.PI;
                }
                else if (diffx < 0 && diffy > 0)
                {
                    anglefromVert = Math.Atan((diffx * -1) / diffy) + Math.PI;
                }
                else if (diffx < 0 && diffy == 0)
                {
                    anglefromVert = (Math.PI * 3) / 2;
                }
                else if (diffx < 0 && diffy < 0)
                {
                    anglefromVert = (2 * Math.PI) - Math.Atan(diffx / diffy);
                }
                else
                {
                    anglefromVert = 0;
                }
            }
            if (anglefromVert > dir)
            {
                clockwise = anglefromVert - dir;
                antic = (2 * Math.PI) - clockwise;
            }
            else
            {
                antic = dir - anglefromVert;
                clockwise = (2 * Math.PI) - antic;
            }

            if (clockwise < antic)
            {
                dir += (Convert.ToDouble(turnSpeed) / 360) * 2 * Math.PI;
            }
            else
            {
                dir -= (Convert.ToDouble(turnSpeed) / 360) * 2 * Math.PI;
            }

            switch (dirShift)
            {
                case -1:
                    dir += (Convert.ToDouble(turnSpeed) / 360) * 2 * Math.PI;
                    break;
                case 1:
                    dir -= (Convert.ToDouble(turnSpeed) / 360) * 2 * Math.PI;
                    break;
                default:
                    break;
            }



            movex = Convert.ToInt32(speed * Math.Sin(dir));
            movey = Convert.ToInt32(speed * Math.Cos(dir)) * -1;
            x += movex; y += movey;

            if (x > region.Width)
            {
                x = region.Width;
                dir += Math.PI;
            }
            else if (x < 0)
            {
                x = 0;
                dir += Math.PI;
            }

            if (y > region.Height)
            {
                y = region.Height;
                dir += Math.PI;
            }
            else if (y < 0)
            {
                y = 0;
                dir += Math.PI;
            }

            if (dir < 0) { dir = (2 * Math.PI) + dir; }
            if (dir > 2 * Math.PI) { dir -= 2 * Math.PI; }

            distance = Math.Sqrt(Math.Pow(diffx, 2) + Math.Pow(diffy, 2));
            c = Color.FromArgb(Convert.ToInt32(((distance) / region.Width) * 255), 255 ,0 );
        }
        public void Reverse()
        {
            dir += Math.PI;

            if (dir < 0) { dir = (2 * Math.PI) + dir; }
            if (dir > 2 * Math.PI) { dir -= 2 * Math.PI; }
        }
        public void Confuse()
        {
            confTickCount = 300;
        }
        
    }
}
