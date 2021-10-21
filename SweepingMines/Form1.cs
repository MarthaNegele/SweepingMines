using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Text;

namespace SweepingMines
{

	//ideen:
	/*Felder reihenweise in arrays speichern
	 * 
	 gamemode:
	100 zeilen, 15 breit, 3reihen zugänglich
	nächste reihe wird enabled, wenn alle felder angeklickt wurden, die keine bombe sind
	punkte für feldaufdecken nach benötigter zeit
	freischalten der nächsten reihe durch punkte möglich
	startpunkte für 1-3 weitere reihen herschenken
	
	 */

	public partial class Form1 : Form
	{
		//int nr = 1; //für btn.Name (btnTest+nr) buttons werden jetzt nach position benannt...
		int posX = 5;//die Werte sind mittlerweile irrelevant
		int posY = 5;
		int iX, iY; //brauch ich für reveal()
		int iBomb, iAllBombs, iFaehnchen;
		int neighbB;
		int iBtSz;
		int iFtSize;
		List<Button> liBu, liAllBtn, lib;
		List<string> liBtnToCheck;
		Random cGen;
		bool bMoB, bGameActive, bFirstTurn;


		Color[] col = { /*0*/ Color.LightGreen,
		/*1*/Color.DarkBlue,
		/*2*/Color.Green,
		/*3*/Color.DarkCyan,
		/*4*/Color.DarkOliveGreen,
		/*5*/Color.MediumSeaGreen,
		/*6*/Color.Chocolate,
		/*7*/Color.Teal,
		/*8*/Color.Fuchsia,
		//*9*/Color.Red
		};


		public Form1()
		{
			InitializeComponent();
			
			liBu = new List<Button>();
			liAllBtn = new List<Button>();
			liBtnToCheck = new List<string>();
			cGen = new Random();
			lib = new List<Button>();

		}

		

		private void button1_Click(object sender, EventArgs e)  //btn "Start"
		{
			iBtSz   = Convert.ToInt32(nudBtnSz.Value);
			iFtSize = iBtSz / 2;

			decimal allFields = nudS.Value * nudR.Value;   //für die Warnung für große Felder

			if (allFields > 2000)
				if (allFields > 7000)
					MessageBox.Show("Die eingegeben Werte erzeugen mehr als 7000 Felder." + Environment.NewLine
						+ "Dies führt zu längeren Ladezeiten."
						+ Environment.NewLine+" Möglicherweise wird das Programm abstürzen - Sorry!");
				else
					MessageBox.Show("Die eingegeben Werte erzeugen mehr als 2000 Felder." + Environment.NewLine
						+ "Dies führt zu längeren Ladezeiten");

			liAllBtn.Clear();
			//falls buttons in der grpbx sind, alle löschen
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
			//nr = 1; buttons werden nicht mehr durchnummeriert, deshalb ist das jetzt irrelevant

			for (int j = 1; j <= nudS.Value; j++)
			{
				for (int i = 1; i <= nudR.Value; i++)
				{
					add_button(i, j);   //Zeile 98
				}
			}
			iBomb = 0;
			placeBombs();				//Zeile 134
			
			bGameActive = true;
			bFirstTurn = true;
		}

		private void add_button(int i, int j)
		{
			Button button = new Button();

			// button.Click = eigBtn_Click;
			button.Name = "S" + j.ToString() + "R" + i.ToString();
			button.Text = "";
			button.Tag = "";
			button.Height = iBtSz;
			button.Width = iBtSz;
			button.TabStop = false;
			//.Location.X = 5;
			posX = iBtSz * j - 20;
			posY = iBtSz * i - 15;
			button.Location = new Point(posX, posY);
			button.Font = new Font("Monotype Corsiva", iFtSize, FontStyle.Bold);
			button.BackColor = Color.DarkGray;
			button.FlatAppearance.BorderColor = Color.DarkGray;
			//button.Margin = new Padding(10);

			//button.ForeColor = col[9];

			button.Click += new EventHandler(this.eigBtn_Click);
			button.MouseEnter += new EventHandler(this.button4_MouseEnter);
			button.MouseLeave += new EventHandler(this.button4_MouseLeave);
			button.MouseUp += new MouseEventHandler(this.button4_MouseUp);
			liAllBtn.Add(button);

			//nr++; brauch ich nimmer, weil der btn ja nimmer TestBtn+nr heißt

			{
				groupBox1.Controls.Add(button);
				groupBox1.Controls.SetChildIndex(button, 0);
			}
		}

		private void placeBombs()
		{

			iAllBombs = liAllBtn.Count * Convert.ToInt16(nudB.Value) / 100;
			//label1.Text = iAllBombs.ToString();
			while (iBomb < iAllBombs)
			{
				foreach (Button btn in liAllBtn)
				{
					if (btn.Tag.ToString() != "Bomb")
					{
						if (cGen.Next(1, 11) <= 1 && iBomb < iAllBombs) //rand von 0-100
																		//weil ich ja über AllBombs kommen könnte, während ich innerhalb der foreach-Schleife bin
						{
							btn.Tag = "Bomb";
							//btn.Text = ".";
							//Feld als Bombe kennzeichnen
							iBomb++;
						}
						//else
						//{
						//	btn.Tag = "noBomb";
						//	//btn.Text = "no Bomb";
						//}
					}
				}
			}
			nudF.Value = iAllBombs;
			iFaehnchen = iAllBombs;

			placeNumberTags();          //Zeile 167
		}

		private void placeNumberTags()
		{
			foreach (Button btn in liAllBtn)
			{
				if (btn.Tag.ToString() != "Bomb")
				{
					btn.Tag = countBombs(btn).ToString();		//Zeile 178
				}
	//btn.Text = btn.Tag.ToString();  //zur Überprüfung
			}
		}

		private int countBombs(Button button)
		{
			neighbB = 0;
			string sBtnName;
liBtnToCheck.Clear();

			iX = extractCoords(button)[0];		//Zeile 216
			iY = extractCoords(button)[1];
												//ist auch super effizient, dass ich ihn die liste zwei mal erstellen lass, um dann jeweils nur einen wert zu nehmen...
			for (int i = iX - 1; i <= iX + 1; i++)
			{
				for (int j = iY - 1; j <= iY + 1; j++)
				{
					sBtnName = "S" + i.ToString() + "R" + j.ToString();
					if (sBtnName != button.Name)
					{
						liBtnToCheck.Add(sBtnName);
					}
				}
			}
			foreach (string s in liBtnToCheck)
			{
				foreach (Button bu in liAllBtn)
				{
					if (s == bu.Name)
					{
						//liBu.Add(bu);
						if (bu.Tag.ToString() == "Bomb")
							neighbB++;
					}
				}
			}

			
			//liBu.Clear();

			return neighbB;
		}

		private List<int> extractCoords(Button bu)  //gibt x und y als liste mit zwei einträgen zurück
		{
			string sNr = "";

			bool bStart = true;


			foreach (char c in bu.Name)
			{
				if (bStart && char.IsDigit(c))
				{
					sNr += c;
				}
				else if (c == 'R')
				{
					bStart = false;
					iX = Convert.ToInt32(sNr);
					sNr = "";
				}
				else if (char.IsNumber(c))
				{
					sNr += c;
				}
			}
			iY = Convert.ToInt32(sNr);

			List<int> li = new List<int>();
			li.Add(iX);
			li.Add(iY);

			//label1.Text = bu.Name + Environment.NewLine + "X = " + iX.ToString() + ", Y = " + iY.ToString();

			return li;

		}

		
		
		private void eigBtn_Click(object sender, EventArgs e)
		{
			Button btn = sender as Button;
			if (bGameActive && btn.Text == "")
			{

				//MouseEventArgs me = (MouseEventArgs)e;
				//if ( == MouseButtons.Right)
				//	label1.Text = "R";
				//else
				//	label1.Text = "L";

				if (btn.Tag.ToString() == "Bomb")
				{
                    if (bFirstTurn)							//weil die leute nicht beim ersten zug auf einer bombe landen wollten
                    {                                       //führt aber zu fehlern. glaub ich
						while (btn.Tag.ToString() == "Bomb") //damit net wieder eine Bombe draufgesetzt wird
						{
							btn.Tag = "";
							iBomb--;
							placeBombs();   //Zeile 134
						}
                        reveal(btn);    //Zeile 289

                    }
                    else
                        bakuhatsu(btn);	//Zeile 329
				}

				else
					
				{
					reveal(btn);        //Zeile 289
					if (btn.Text == "0")
						checkForMore(btn);
					CheckForVictory();	//Zeile 350
				}
				bFirstTurn = false;
			}
		}


		private void reveal(Button button) //eigentlich muss ich den Button da mitschicken oder nicht?
		{
			neighbB = Convert.ToInt16(button.Tag);
			button.Text = Convert.ToString(button.Tag);
			button.ForeColor = col[neighbB];


			if (neighbB == 0)
			{
				button.BackColor = Color.LightGreen;
			}

			else
			{
				button.BackColor = Color.LightGray;
			}

			//liBu.Clear();
		}

		private void checkForMore(Button button)
		{
			liBu = createLiBu(button);
			

			while (liBu.Count > 0)
			{
				if (liBu[0].Tag.ToString() != liBu[0].Text)
				{
					reveal(liBu[0]);
					if (liBu[0].Text=="0")
                    {
						foreach(Button bu in createLiBu(liBu[0]))
                        {
							liBu.Add(bu);
                        }
					}
				}
				liBu.RemoveAt(0);
			}
		}

		private List<Button> createLiBu(Button button)
		{
			List<Button> liBu2 = new List<Button>();
			liBtnToCheck.Clear();
			string sBtnName;

			iX = extractCoords(button)[0];      //Zeile 216
			iY = extractCoords(button)[1];

			for (int i = iX - 1; i <= iX + 1; i++)
			{
				for (int j = iY - 1; j <= iY + 1; j++)
				{
					sBtnName = "S" + i.ToString() + "R" + j.ToString();
					if (sBtnName != button.Name)
					{
						liBtnToCheck.Add(sBtnName);
					}
				}
			}
			foreach (string s in liBtnToCheck)
			{
				foreach (Button bu in liAllBtn)
				{
					if (s == bu.Name && bu.Text == "") //da kommen nur unrevealed btns rein
					{
						liBu2.Add(bu);
					}
				}
			}
			return liBu2;
		}

		private void bakuhatsu(Button bu)
		{

			neighbB = 0;
			foreach (Button b in liAllBtn)
			{
				//b.Enabled = false;
				if (b.Tag.ToString() == "Bomb")
				{
					if (b.Text == "!")
						b.BackColor = Color.Black;
				
					b.Text = "B";
				}
				
			}
			bu.Text = "X";
			bGameActive = false;
		}


		private void CheckForVictory()
		{
			int iCount = 0;
			foreach (Button bu in liAllBtn)
			{
				if (bu.Text != "")
					iCount++;
			}
			if (iCount == liAllBtn.Count)
			{
				//foreach (Button bu in liAllBtn)
				//	bu.Enabled = false;
				MessageBox.Show("Herzlichen Glückwunsch!");
				bGameActive = false;
			}
		}



		//Zeug für den Rechtsklick
		private void button4_MouseEnter(object sender, EventArgs e)
        {
			//label1.Text = "Mouse on button";
			bMoB = true;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
			//label1.Text = "";
			bMoB = false;
        }

		private void button4_MouseUp(object sender, MouseEventArgs e)
		{
			if (bGameActive)
			{
				Button btn = sender as Button;
				if (e.Button == MouseButtons.Right)
				{
					if (btn.Text == "" && iFaehnchen > 0)
					{
						btn.Text = "!";
						btn.BackColor = Color.Red;
						btn.ForeColor = Color.White;
						iFaehnchen--;
						CheckForVictory();

					}
					else if (btn.Text == "!")
					{
						btn.Text = "";
						btn.BackColor = Color.DarkGray;
						btn.ForeColor = Color.Black;
						iFaehnchen++;
					}
					nudF.Value = iFaehnchen;
				}
			}
		}

       
		

        #region useless stuff
        private void button3_Click(object sender, EventArgs e)
		{
			//foreach (Button btn in groupBox1.Controls.OfType<Button>())
			//   btn.
		}

		private void config_btn(Button btn)
		{

			//btn.Location
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
		#endregion
	}
}
