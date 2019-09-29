using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Wpf_Bugs
{
   public  static class World
    {

        public enum matter { empty, wall, food, poison, insect };
        public const int width = 42;
        public const int height = 32;
        public const int foodCount = 80;
        public const int poisonCount = 60;
        public static int generation = 1;
        public static int step = 0;
        public static matter[,] world = new matter[width, height];
        public static bool isStared = false;
        private static Random rand = new Random();
        private  static Food[] food = new Food[100];
        private static Poison[] poison =new Poison[100];
        public static event EventHandler<MoveEventArgs> Move;
        public static event EventHandler<ReloadEventArgs> ReloadWorld;

       

        static World.matter Getworld( int x, int y)
        {
            return world[x,y];
        }

        static World.matter Getworld(int i)
        {
            int y = i / width;
            int x = i - (y * width);
            return world[x, y];
        }

        public static int _2to1(int x, int y)
        {
            return (y * width) + x;
        }

        public static void _1to2(in int i, out int x, out int y)
        {
            y = i / width;
            x = i - (y* width);
        }

        public static void Create()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || i == (height - 1) || j == 0 || j == (width - 1)) { world[j, i] = World.matter.wall; }
                    else { world[j, i] = World.matter.empty; } 
                }
            }

            Bugs.CreateBugs(100);

            Bugs.BugsLanding();
            for (int i = 0; i < foodCount; i++)
            {
                food[i] = new Food(i+100);
                World.SetRandom(food[i]);
                World.world[food[i].x_pos, food[i].y_pos] = matter.food;
            }
            for (int i = 0; i < poisonCount; i++)
            {
                poison[i] = new Poison(i+200);
                World.SetRandom(poison[i]);
                World.world[poison[i].x_pos, poison[i].y_pos] = matter.poison;
            }
            

            Task.Factory.StartNew(() =>
            {
                isStared = true;
                while (isStared)
                {
                    Step();
                    Thread.Sleep(Vm.Slider);
                }
            });

        }

        public static void Reload()
        {
            ReloadWorld(null,new ReloadEventArgs(step,generation) );
            step = 0;
            generation++;

            for (int i = 0; i < 100; i++)
            {
                Move(null, new MoveEventArgs(i, 100, 100, 1,step));
            }

            for (int i = 100; i < 300; i++)
            {
                Move(null, new MoveEventArgs(i, 100, 100, 0,step));
            }

            for (int i = 1; i < World.height-1; i++)
            {
                for (int j = 1; j < World.width-1; j++)
                {
                    World.world[j,i] = World.matter.empty;
                }
            }

            for (int i = 0 ; i < 7; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Bugs.CloneBug(i);
                    }
                }

            foreach (var item in Bugs.arrBugs)
            {
                World.world[item.x_pos,item.y_pos] = World.matter.insect;
            }

            for (int i = 0; i < foodCount; i++)
            {
                World.SetRandom(food[i]);
                World.world[food[i].x_pos, food[i].y_pos] = matter.food;
            }

            for (int i = 0; i < poisonCount; i++)
            {
                World.SetRandom(poison[i]);
                World.world[poison[i].x_pos, poison[i].y_pos] = matter.poison;
            }

        }

        public static void Step()
        {
            for (int i = 0; i< Bugs.arrBugs.LongCount(); i++)
            {
                if (!Bugs.arrBugs[i].Step())
                {
                    Move(null, new MoveEventArgs(Bugs.arrBugs[i].Id, 100, 100,1,step));
                    Bugs.arrBugs.RemoveAt(i);
                    
                }
                else
                {
                    Move(null, new MoveEventArgs(Bugs.arrBugs[i].Id, Bugs.arrBugs[i].x_pos, Bugs.arrBugs[i].y_pos,Bugs.arrBugs[i].life,step));
                }
                //Thread.Sleep(2);
                if (Bugs.arrBugs.LongCount() >= 8) continue;
                World.Reload(); break;
               
            }
            
            step++;
        }

        public static void SetRandom(Matter m)
        {
            GetRandomPosition(ref m.x_pos,ref m.y_pos);
            Move(null, new MoveEventArgs(m.Id, m.x_pos, m.y_pos,0,step));
        }

        public static void ReSetRandomPosition(int oldX, int oldY)
        {
            
            matter m = World.world[oldX, oldY];
            for (int i = 0; i < foodCount; i++)
            {
                if (food[i].x_pos == oldX && food[i].y_pos == oldY)
                {
                    World.SetRandom(food[i]);
                    World.world[oldX, oldY] = World.matter.empty;
                    World.world[food[i].x_pos, food[i].y_pos] = m;
                    
                    return;
                }
            }

            for (int i = 0; i < poisonCount; i++)
            {
                if (poison[i].x_pos == oldX && poison[i].y_pos == oldY)
                {
                    World.SetRandom(poison[i]);
                    World.world[oldX, oldY] = World.matter.empty;
                    World.world[poison[i].x_pos, poison[i].y_pos] = m;
                    return;
                }
            }


        }

        public static void GetRandomPosition(ref int x, ref int y)
        {
            x = y = 1;
            int random = rand.Next(1, World.GetEmptyCount());
            for (y = 1; y < World.height-1; y++)
            {
                for (x = 1; x < World.width-1; x++)
                {
                    if (World.world[x, y] == World.matter.empty) { random--; }
                    if (random <= 0)
                    {
                        return;
                    }
                   
                }
            }
        }

        private static int GetEmptyCount()
        {
            int result = 0;
            for (int y = 1; y < World.height-1; y++)
            {
                for (int x = 1; x < World.width-1; x++)
                {
                    if (World.world[x, y] == World.matter.empty) { result++; }
                }
            }

            return result;
        }


    }


   public class MoveEventArgs : EventArgs
   {
       public int Id { get; private set; }
       public int X { get; private set; }
       public int Y { get; private set; }
       public int Life { get; private set; }
       public int Step { get; private set; }

       public MoveEventArgs(int id, int x, int y, int life, int step)
       {
           this.Id = id;
           this.X = x;
           this.Y = y;
           this.Life = life;
           this.Step = step;
       }
   }

   public class ReloadEventArgs : EventArgs
   {
       public int Step { get; private set; }
       public int Generation { get; private set; }

       public ReloadEventArgs(int step,int generation)
       {
           this.Step = step;
           this.Generation = generation;
       }

   }

}




