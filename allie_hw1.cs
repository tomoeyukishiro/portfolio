using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace allie_hw_1
{
    class MainClass : Form
    {
        List<int> init_r_list = new List<int>(); //stores radii of randomly generated bubbles
        List<int> init_x_list = new List<int>(); //stores radii of randomly generated bubbles
        List<int> init_y_list = new List<int>(); //stores radii of randomly generated bubbles
        int aisle_num = 19; //my row num from sfo to jfk
        List<double> time_tracking = new List<double>(); //stores times of successful selections
        List<double> time_interval = new List<double>(); //stores time intervals of successful selections
        int successes = 0; //num of successful selections
        int fails = 0; // number of failed selections
        double avg_time = 0; //avg time of successful selections
        int target_bubble = 0; //targeted bubble\
        int selected_bubble = 0; //selected bubble
        Label Score; //screen to show results
        Button Restart; //click to start over
        DateTime Begin = System.DateTime.Now; //starting time

        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainClass());
        }

        public MainClass()
        {
            this.ClientSize = new Size(700, 500);
            CenterToScreen();

            Random rnd_bubbles = new Random();

            //stores only one randomly generated bubble inside each list for radius, x-coordinate, and y-coordinate

            init_r_list.Add(rnd_bubbles.Next(10, 50));
            init_x_list.Add(rnd_bubbles.Next(0, this.Width));
            init_y_list.Add(rnd_bubbles.Next(0, this.Height));

            int counter = 1;

            while (counter <= aisle_num)
            {
                int next_r = rnd_bubbles.Next(10, 50);
                int next_x = rnd_bubbles.Next(0, this.Width);
                int next_y = rnd_bubbles.Next(0, this.Height);

                for (int i = 0; i < counter; i++)
                {
                    double cond_1 = Math.Sqrt((double)(next_x - init_x_list[i]) * (next_x - init_x_list[i]) + (double)(next_y - init_y_list[i]) * (next_y - init_y_list[i]));
                    double cond_2 = next_r + init_r_list[i];

                    if (cond_1 < cond_2) //euclidean distance between newly drawn circles and existing cicles need to be greater than the sum of their radii
                        break;

                    else
                    {
                        if (i < counter - 1) continue;
                        else
                            init_r_list.Add(next_r);
                            init_x_list.Add(next_x);
                            init_y_list.Add(next_y);
                            counter++;
                            break;
                    }
                }
            }

                Score = new Label();
                Score.Width = 700;
                Score.Height = 50;
                Score.Text = "Successes: " + successes + " Fails: " + fails + " Avg Time: " + avg_time;
                Score.BackColor = Color.LightYellow;
                Score.Font = new Font("Verdana", 12);
                Score.Location = new Point(50, 50);
                this.Controls.Add(Score);

                Restart = new Button();
                Restart.Width = 50;
                Restart.Width = 25;
                Restart.Text = "Reset";
                Restart.Location = new Point(300, 50);
                this.Controls.Add(Restart);
            
                this.MouseClick += new MouseEventHandler(onMouseClick);
                this.MouseMove += new MouseEventHandler(onMouseMove);
                this.Paint += new System.Windows.Forms.PaintEventHandler(this.Draw_Bubbles);
                this.Restart.MouseClick += new MouseEventHandler(onMouseClickButton);
        }

        private void Draw_Bubbles(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle bground = this.ClientRectangle;
            g.FillRectangle(Brushes.Gold, bground);

            for (int i = 0; i < aisle_num; i++)
            {
                g.FillEllipse(Brushes.Blue, init_x_list[i] - init_r_list[i], init_y_list[i] - init_r_list[i], 2 * init_r_list[i], 2 * init_r_list[i]);
            }

            Rectangle rect = new Rectangle(init_x_list[target_bubble] - init_r_list[target_bubble], init_y_list[target_bubble] - init_r_list[target_bubble], 2 * init_r_list[target_bubble], 2 * init_r_list[target_bubble]);
            g.FillEllipse(Brushes.Green, rect);

            //use point structure to locate x and y coordinates of the mouse
            System.Drawing.Point mouse = Control.MousePosition;
            int mouse_x = this.PointToClient(mouse).X;
            int mouse_y = this.PointToClient(mouse).Y;


            List<int> target_distance = new List<int>();
            for (int i = 0; i < aisle_num; i++)
            {
                int center_distance = (int)Math.Sqrt((mouse_x - init_x_list[i]) * (mouse_x - init_x_list[i]) + (mouse_y - init_y_list[i]) * (mouse_y - init_y_list[i]));
                int t_distance = center_distance - init_r_list[i];
                target_distance.Add(t_distance);
            }

            int smallest = target_distance[0];
            int smallest_z = 0;

            for (int i = 0; i < aisle_num; i++)
            {
                if (target_distance[i] < smallest)
                {
                    smallest = target_distance[i];
                    smallest_z = i;
                }
            }

            int second_smallest = 1000;
            int second_z = 100;

            for (int i = 0; i < aisle_num; i++)
            {
                if (target_distance[i] < second_smallest && target_distance[i] > smallest)
                {
                    second_smallest = target_distance[i];
                    second_z = i;
                }
            }

            int cond_a = smallest + 2 * init_r_list[smallest_z];
            int cond_b = second_smallest;
            int mouse_r = 0;
            if (cond_a > cond_b)
            {
                mouse_r = cond_b;
                selected_bubble = second_z;
            }
            else
            {
                mouse_r = cond_a;
                selected_bubble = smallest_z;
            }

            rect = new Rectangle(mouse_x - mouse_r, mouse_y - mouse_r, 2 * mouse_r, 2 * mouse_r);
            g.FillEllipse(Brushes.Azure, rect);

            rect = new Rectangle(init_x_list[selected_bubble] - init_r_list[selected_bubble] - 5, init_y_list[selected_bubble] - init_r_list[selected_bubble] - 5, 2 * init_r_list[selected_bubble] + 10, 2 * init_r_list[selected_bubble] + 10);
            g.FillEllipse(Brushes.AliceBlue, rect);

            rect = new Rectangle(init_x_list[selected_bubble] - init_r_list[selected_bubble], init_y_list[selected_bubble] - init_r_list[selected_bubble], 2 * init_r_list[selected_bubble], 2 * init_r_list[selected_bubble]);
            g.FillEllipse(Brushes.Pink, rect);
        }
        
        void onMouseClick(object sender, MouseEventArgs args)
        {        
            if(selected_bubble == target_bubble)
            {
                successes++;
            
                int prev_bubble = target_bubble;
                while(prev_bubble == target_bubble)
                {
                    Random rnd_target = new Random();
                    target_bubble = rnd_target.Next(0, init_r_list.Count-1);//target bubble is restricted to the number of randomly generated bubbles
                }
                TimeSpan start_time = new TimeSpan(Begin.Ticks);
                TimeSpan time_now = new TimeSpan(System.DateTime.Now.Ticks);
                TimeSpan time = start_time.Subtract(time_now).Duration();
                time_tracking.Add(time.TotalSeconds);
            
                if(time_tracking.Count == 1) 
                {
                    time_interval.Add(time_tracking[0]);
                }
                else
                {
                    time_interval.Add(time_tracking[time_tracking.Count - 1] - time_tracking[time_tracking.Count - 2]);
                }
                avg_time = time.TotalSeconds / successes;
            }
            else
            {
                fails++;
            }

            Score.Text = "Successes: " + successes + " Fails: " + fails + " Avg Time: " + avg_time;//increment each time this works

            Invalidate();
        }
        
        void onMouseClickButton(object sender, MouseEventArgs args)
        {
            Application.Restart();
        }
    
        void onMouseMove(object sender, MouseEventArgs args)
        {
            Invalidate();
        }
    }
}