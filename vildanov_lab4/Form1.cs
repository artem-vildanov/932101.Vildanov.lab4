using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vildanov_lab4
{
    public partial class Form1 : Form
    {
        const int CellsCount = 25;
        const int CellsSize = 20;

        int[,] CurrentState = new int[CellsCount, CellsCount];  //текущее поколение
        int[,] NextState = new int[CellsCount, CellsCount];    //следующее поколение

        Button[,] Cells = new Button[CellsCount, CellsCount];

        bool Drawing = false;

        Timer Timer1;

        int Indent = 25;

        public Form1()
        {
            InitializeComponent();
            FormSize();
            Menu();
            Initialization();
        }

        void FormSize()
        {
            this.Width = (CellsCount + 1) * CellsSize;
            this.Height = (CellsCount + 1) * CellsSize + 40;
        }

        public void Initialization()
        {
            Drawing = false;
            Timer1 = new Timer();
            Timer1.Interval = 100;
            Timer1.Tick += new EventHandler(UpdateStates);
            CurrentState = FieldInit();
            NextState = FieldInit();
            CellsInit();
        }

        void FieldClear()
        {
            Drawing = false;
            Timer1 = new Timer();
            Timer1.Interval = 100;
            Timer1.Tick += new EventHandler(UpdateStates);
            CurrentState = FieldInit();
            NextState = FieldInit();
            CellsReset();
        }

        void CellsReset()
        {
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    Cells[i, j].BackColor = Color.White;
                }
            }
        }

        void Menu()
        {
            var menu = new MenuStrip();

            var restart = new ToolStripMenuItem("Restart game");
            restart.Click += new EventHandler(Restart);

            var play = new ToolStripMenuItem("Start game");
            play.Click += new EventHandler(Play);

            menu.Items.Add(play);
            menu.Items.Add(restart);

            this.Controls.Add(menu);
        }

        private void Restart(object sender, EventArgs e)
        {
            Timer1.Stop();
            FieldClear();
        }

        private void Play(object sender, EventArgs e)
        {
            if (!Drawing)
            {
                Drawing = true;
                Timer1.Start();
            }
        }

        private void UpdateStates(object sender, EventArgs e)
        {
            CalculateNextState();
            DisplayField();
            if (CheckIsFieldEmpty())
            {
                Timer1.Stop();
                MessageBox.Show("Restart please, the field is empty");
            }
        }

        bool CheckIsFieldEmpty()
        {
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    if (CurrentState[i, j] == 1)
                        return false;
                }
            }
            return true;
        }

        void CalculateNextState()
        {
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    var countNeighboors = CountNeighboors(i, j);
                    if (CurrentState[i, j] == 0 && countNeighboors == 3)
                    {
                        NextState[i, j] = 1;
                    }
                    else if (CurrentState[i, j] == 1 && (countNeighboors < 2 && countNeighboors > 3))
                    {
                        NextState[i, j] = 0;
                    }
                    else if (CurrentState[i, j] == 1 && (countNeighboors >= 2 && countNeighboors <= 3))
                    {
                        NextState[i, j] = 1;
                    }
                    else
                    {
                        NextState[i, j] = 0;
                    }
                }
            }
            CurrentState = NextState;
            NextState = FieldInit();
        }

        void DisplayField()
        {
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    if (CurrentState[i, j] == 1)
                        Cells[i, j].BackColor = Color.Green;
                    else Cells[i, j].BackColor = Color.White;
                }
            }
        }

        int CountNeighboors(int i, int j)
        {
            var count = 0;
            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (!IsInsideField(k, l))
                        continue;
                    if (k == i && l == j)
                        continue;
                    if (CurrentState[k, l] == 1)
                        count++;
                }
            }
            return count;
        }

        bool IsInsideField(int i, int j)
        {
            if (i < 0 || i >= CellsCount || j < 0 || j >= CellsCount)
            {
                return false;
            }
            return true;
        }

        int[,] FieldInit()
        {
            int[,] arr = new int[CellsCount, CellsCount];
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    arr[i, j] = 0;
                }
            }
            return arr;
        }

        void CellsInit()
        {
            for (int i = 0; i < CellsCount; i++)
            {
                for (int j = 0; j < CellsCount; j++)
                {
                    Button button = new Button();
                    button.Size = new Size(CellsSize, CellsSize);
                    button.BackColor = Color.White;
                    button.Location = new Point(j * CellsSize, (i * CellsSize) + Indent);
                    button.Click += new EventHandler(OnCellClick);
                    this.Controls.Add(button);
                    Cells[i, j] = button;
                }
            }
        }

        private void OnCellClick(object sender, EventArgs e)
        {
            var pressedButton = sender as Button;
            if (!Drawing)
            {
                var i = (pressedButton.Location.Y - Indent) / CellsSize;
                var j = pressedButton.Location.X / CellsSize;

                if (CurrentState[i, j] == 0)
                {
                    CurrentState[i, j] = 1;
                    Cells[i, j].BackColor = Color.Green;
                }
                else
                {
                    CurrentState[i, j] = 0;
                    Cells[i, j].BackColor = Color.White;
                }
            }
        }
    }
}
