using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SweepingMines
{
    public partial class Form1 : Form
    {
        int nr = 1;
        int posX = 5;
        int posY = 5;

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //foreach (Button btn in groupBox1.Controls.OfType<Button>())
            //   btn.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.Count > 0)
            {
                while (groupBox1.Controls.Count > 0)
                {
                    foreach (Button btn in groupBox1.Controls.OfType<Button>())
                    {
                        groupBox1.Controls.Remove(btn);
                        btn.Dispose();
                    }
                }
            }
            for (int i = 1; i <= nudR.Value; i++)
            {
                for (int j = 1; j <= nudS.Value; j++)
                {
                    add_button(i, j);
                }
            }
        }
        

        private void config_btn(Button btn)
        { 
            
        //btn.Location
        }
        private void add_button(int i, int j)
        {
            Button button = new Button();

            // button.Click += new EventHandler(run_program);
            button.Name = "btnTest"+nr.ToString();
            button.Text = "R" + i.ToString() + "S" + j.ToString();
            button.Height = 40;
            button.Width = 40;
            //.Location.X = 5;
            posX = 40*j;
            posY = 40*i;
            button.Location = new Point(posX, posY);

            nr++;
            

            //GroupBox groupBox = null;

            //if (group == "1") groupBox = groupBox1;//-----this is specific
            //if (group == "2") groupBox = groupBox2;//-----this is specific
            //if (group == "3") groupBox = groupBox3;//-----this is specific

            //if (groupBox != null)
            {
            groupBox1.Controls.Add(button);
                groupBox1.Controls.SetChildIndex(button, 0);
            }
        }
    }
}
