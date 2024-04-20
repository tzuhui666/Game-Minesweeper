using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1102056_Final_Project
{
    public partial class Form1 : Form
    {
        Button[] btns;

        int flagnumber;//計算標記數量
        int clicknumber;//計算點擊數量
        int onex;
        int oney;
        int[,] numbers;
        bool[,] isBomb;//炸彈位置確認是不是炸彈
        bool[,] isClicked;//確認點擊開了嗎
        bool[,] isFlaged;//確認是否有放旗子
        bool oneclick = true;
        
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)//生成踩地雷按鍵
        {
            numbers = new int[10,10];
            isBomb = new bool[10, 10];
            isClicked = new bool[10, 10];
            isFlaged = new bool[10, 10];
            clicknumber = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    numbers[i, j] = 0;
                    isBomb[i, j] = false;
                    isClicked[i, j] = false;
                    isFlaged[i, j] = false;
                }
            }
            btns = new Button[10*10];//100buttons
            flagnumber = 0;

            int x = 15, y = 15, z = 30, w = z + 5;

            for (int i = 0; i < btns.Length; i++)//panel可以放控制屬性的東西，用這個東西生成按紐
            {
                Button btn = new Button();

                int row = i / 10;  
                int column = i % 10;

                btn.Width = z;  //格子寬
                btn.Height = z;  //格子高
                btn.Left = x + column * w; //設定橫排按紐
                btn.Top = y + row * w;  //設定直排按紐
                btn.Font = new Font("新細明體", 15);//設定按鈕內的文字
                btn.Visible = true;
                btns[i] = btn;
                this.panel.Controls.Add(btn);//在panel加入按紐
                btn.MouseDown += new System.Windows.Forms.MouseEventHandler(click);//按鈕全部都用同一個事件
                btn.Name = i.ToString();
            }
        }
        public void click(object sender, MouseEventArgs e)//點擊格子
        {
            Button button = (Button)sender;
            int mouseplace = int.Parse(button.Name);
            int x = mouseplace / 10;
            int y = mouseplace % 10;

            if (e.Button == MouseButtons.Right)//放旗子
            {
                if (isClicked[x, y] == true)
                    return;

                if (isFlaged[x, y] == false)
                {
                    button.BackgroundImage = imageList1.Images[1];
                    isFlaged[x, y] = true;
                    flagnumber++;
                }
                else
                {
                    button.BackgroundImage = null;
                    isFlaged[x, y] = false;
                    flagnumber--;
                }

                if (flagnumber == 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (isFlaged[i, j] == false)
                                continue;
                            if (isBomb[i, j] == false)
                                return;
                        }
                    }
                    win();
                }
            }

            else if (e.Button == MouseButtons.Left)//開格子
            {
                if (oneclick)//第一次按才會擺放炸彈
                {
                    onex = x;
                    oney = y;
                    isBomb[x, y] = true;
                    newmap();
                    isBomb[x, y] = false;
                    oneclick = false;
                }

                if (isClicked[x, y])
                    return;

                clicknumber++;

                if (isBomb[x, y])
                {
                    gameover();
                    return;
                }
                isClicked[x, y] = true;

                button.BackColor = Color.DarkGray;

                if (numbers[x, y] != 0)
                    button.Text = numbers[x, y].ToString();

                map(x, y);

                if (clicknumber == 10 * 10 - 10)
                    win();
            }

        }
        public void newmap()//生成10*10的大地圖，並將炸彈隨機擺放
        {
            Random r = new Random();
            int x, y;
            for (int i = 0; i < 10; i++)
            {
                do
                {
                    x = r.Next(10);
                    y = r.Next(10);
                }
                while (isBomb[x, y] == true);
                    isBomb[x, y] = true;
                //以炸彈為中心的八個方位，確認炸彈周圍格子的數字
                if (x != 0 && y != 0)
                    numbers[x - 1, y - 1]++;
                if (x != 0)
                    numbers[x - 1, y]++;
                if (x != 0 && y != 10 - 1)
                    numbers[x - 1, y + 1]++;
                if (x != 10 - 1 && y != 0)
                    numbers[x + 1, y - 1]++;
                if (x != 10 - 1)
                    numbers[x + 1, y]++;
                if (x != 10 - 1 && y != 10 - 1)
                    numbers[x + 1, y + 1]++;
                if (y != 0)
                    numbers[x, y - 1]++;
                if (y != 10 - 1)
                    numbers[x, y + 1]++;
            }

        }
        public void map(int x, int y)//以點擊為中心的八個方位，判斷是否為數字，不是數字的話就擴散出去，直到碰到數字
        {
            if (numbers[x, y] > 0)//有大於0的數字就顯示
                btns[x * 10 + y].Text = numbers[x, y].ToString();
            clicknumber++;
            if (numbers[x, y] == 0)//有等於0的數字表示空白要擴張
            {
                if (x != 0 && y != 0 && isBomb[x - 1, y - 1] == false && isClicked[x - 1, y - 1] == false)//先判斷所選格子是否為100方格子中的邊界
                {
                    isClicked[x - 1, y - 1] = true;
                    btns[(x - 1) * 10 + y - 1].BackColor = Color.DarkGray;
                    clicknumber++;//點擊次數增加
                    if (isFlaged[x - 1, y - 1] == true)
                    {
                        isFlaged[x - 1, y - 1] = false;
                        btns[(x - 1) * 10 + y - 1].BackgroundImage = null;//刪掉該格旗子
                    }
                    if (numbers[x - 1, y - 1] != 0)
                        btns[(x - 1) * 10 + y - 1].Text = numbers[x - 1, y - 1].ToString();//顯示數字
                    else
                    {
                        map(x - 1, y - 1);//空白的繼續擴張空白
                    }
                }
                if (x != 0 && isBomb[x - 1, y] == false && isClicked[x - 1, y] == false)
                {
                    isClicked[x - 1, y] = true;
                    btns[(x - 1) * 10 + y].BackColor = Color.DarkGray;
                    clicknumber++;

                    if (isFlaged[x - 1, y] == true)
                    {
                        isFlaged[x - 1, y - 1] = false;
                        btns[(x - 1) * 10 + y].BackgroundImage = null;
                    }

                    if (numbers[x - 1, y] != 0)
                        btns[(x - 1) * 10 + y].Text = numbers[x - 1, y].ToString();
                    else
                    {
                        map(x - 1, y);
                    }
                }
                if (x != 0 && y != 10 - 1 && isBomb[x - 1, y + 1] == false && isClicked[x - 1, y + 1] == false)
                {
                    isClicked[x - 1, y + 1] = true;
                    btns[(x - 1) * 10 + y + 1].BackColor = Color.DarkGray;
                    clicknumber++;

                    if (isFlaged[x - 1, y + 1] == true)
                    {
                        isFlaged[x - 1, y + 1] = false;
                        btns[(x - 1) * 10 + y + 1].BackgroundImage = null;
                    }

                    if (numbers[x - 1, y + 1] != 0)
                        btns[(x - 1) * 10 + y + 1].Text = numbers[x - 1, y + 1].ToString();
                    else
                    {
                        map(x - 1, y + 1);
                    }
                }
                if (y != 0 && isBomb[x, y - 1] == false && isClicked[x, y - 1] == false)
                {
                    isClicked[x, y - 1] = true;
                    btns[x * 10 + y - 1].BackColor = Color.DarkGray;

                    if (isFlaged[x, y - 1] == true)
                    {
                        isFlaged[x, y - 1] = false;
                        btns[x * 10 + y - 1].BackgroundImage = null;
                    }

                    if (numbers[x, y - 1] != 0)
                        btns[x * 10 + y - 1].Text = numbers[x, y - 1].ToString();
                    else
                    {
                        map(x, y - 1);
                    }
                }
                if (y != 10 - 1 && isBomb[x, y + 1] == false && isClicked[x, y + 1] == false)
                {
                    isClicked[x, y + 1] = true;
                    btns[x * 10 + y + 1].BackColor = Color.DarkGray;

                    if (isFlaged[x, y + 1] == true)
                    {
                        isFlaged[x, y + 1] = false;
                        btns[x * 10 + y + 1].BackgroundImage = null;
                    }

                    if (numbers[x, y + 1] != 0)
                        btns[x * 10 + y + 1].Text = numbers[x, y + 1].ToString();
                    else
                    {
                        map(x, y + 1);
                    }
                }
                if (x != 10 - 1 && y != 0 && isBomb[x + 1, y - 1] == false && isClicked[x + 1, y - 1] == false)
                {
                    isClicked[x + 1, y - 1] = true;
                    btns[(x + 1) * 10 + y - 1].BackColor = Color.DarkGray;

                    if (isFlaged[x + 1, y - 1] == true)
                    {
                        isFlaged[x + 1, y - 1] = false;
                        btns[(x + 1) * 10 + y - 1].BackgroundImage = null;
                    }

                    if (numbers[x + 1, y - 1] != 0)
                        btns[(x + 1) * 10 + y - 1].Text = numbers[x + 1, y - 1].ToString();
                    else
                    {
                        map(x + 1, y - 1);
                    }
                }
                if (x != 10 - 1 && isBomb[x + 1, y] == false && isClicked[x + 1, y] == false)
                {
                    isClicked[x + 1, y] = true;
                    btns[(x + 1) * 10 + y].BackColor = Color.DarkGray;

                    if (isFlaged[x + 1, y] == true)
                    {
                        isFlaged[x + 1, y] = false;
                        btns[(x + 1) * 10 + y].BackgroundImage = null;
                    }

                    if (numbers[x + 1, y] != 0)
                        btns[(x + 1) * 10 + y].Text = numbers[x + 1, y].ToString();
                    else
                    {
                        map(x + 1, y);
                    }
                }
                if (x != 10 - 1 && y != 10 - 1 && isBomb[x + 1, y + 1] == false && isClicked[x + 1, y + 1] == false)
                {
                    isClicked[x + 1, y + 1] = true;
                    btns[(x + 1) * 10 + y + 1].BackColor = Color.DarkGray;

                    if (isFlaged[x + 1, y + 1] == true)
                    {
                        isFlaged[x + 1, y + 1] = false;
                        btns[(x + 1) * 10 + y + 1].BackgroundImage = null;
                    }

                    if (numbers[x + 1, y + 1] != 0)
                        btns[(x + 1) * 10 + y + 1].Text = numbers[x + 1, y + 1].ToString();
                    else
                    {
                        map(x + 1, y + 1);
                    }
                }
                return;
            }
        }

        public void win()//遊戲贏後要把炸彈給他們看，然後除了炸彈其他位置要顯示灰格子跟數字
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (isFlaged[i, j] == true)
                    {
                        isFlaged[i, j] = false;
                        btns[i * 10 + j].BackgroundImage = null;
                    }

                    if (isClicked[i, j] == true)
                        continue;

                    btns[i * 10 + j].BackColor = Color.DarkGray;

                    isClicked[i, j] = true;

                    if (isBomb[i, j])
                    {
                        btns[i * 10 + j].BackgroundImage = imageList1.Images[0];
                        continue;
                    }

                    if (numbers[i, j] != 0)
                        btns[i * 10 + j].Text = numbers[i, j].ToString();
                }
            }
            MessageBox.Show("你贏了");
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    btns[i].Enabled = false;
        }
        public void gameover()//遊戲輸後要把炸彈給他們看，然後除了炸彈其他位置要顯示灰格子跟數字
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (isFlaged[i, j] == true)
                    {
                        isFlaged[i, j] = false;
                        btns[i * 10 + j].BackgroundImage = null;
                    }

                    if (isClicked[i, j] == true)
                        continue;

                    btns[i * 10 + j].BackColor = Color.DarkGray;

                    isClicked[i, j] = true;

                    if (isBomb[i, j])
                    {
                        btns[i * 10 + j].BackgroundImage = imageList1.Images[0];
                        continue;
                    }
                    if (numbers[i, j] != 0)
                        btns[i * 10 + j].Text = numbers[i, j].ToString();

                }
            }

            MessageBox.Show("你輸了！");
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    btns[i].Enabled = false;
        }


        

       
        
        private void 新遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void 再玩一次AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
