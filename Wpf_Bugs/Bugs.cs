using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Wpf_Bugs
{
    public class Bugs:Matter
    {
        const int brainLengt = 64;
        private static readonly int[] xShiftMatrix = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private static readonly int[] yShiftMatrix = new int[8] { 1, 1, 1, 0, -1, -1, -1, 0 };
        int[] brain = new int[brainLengt];
        int ibrain;
        int ibrainTemp;
        int rotation;
        public int life;
        static Random rand = new Random();
        public static List<Bugs>  arrBugs = new List<Bugs>(256);


        public Bugs(int id) : base(id)
        {
            this.ibrain = 0;
            this.life = 80;
        }

        public static void CreateBugs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                arrBugs.Add(new Bugs(i));
                BrainRandom();
                
            }
        }

        public static void CloneBug(int b)
        {
            arrBugs[b].Id = b;
            Bugs clone = new Bugs((int)arrBugs.LongCount()+1);
            clone.CloneBrain (arrBugs[b].brain);
            arrBugs[b].life = 80;
            arrBugs[b].ibrain = 0;
            World.world[arrBugs[b].x_pos, arrBugs[b].y_pos] = World.matter.empty;
            World.GetRandomPosition(ref arrBugs[b].x_pos, ref arrBugs[b].y_pos );
            World.world[arrBugs[b].x_pos, arrBugs[b].y_pos] = World.matter.insect;

            for (int i = 0; i < brainLengt; i++)
            {
                int random = rand.Next(0, brainLengt * 200);
                if (random < 64) { clone.brain[i] = random; }
            }

            World.GetRandomPosition(ref clone.x_pos, ref clone.y_pos);
            World.world[clone.x_pos, clone.y_pos] = World.matter.insect;
            arrBugs.Add(clone);
        }

        public bool Step()
        {
                     
                int step = 10;
                World.world[x_pos, y_pos] =World.matter.empty;
                while (step > 0) {
                ibrainTemp =ibrain;
                
                if (brain[ibrainTemp] < 8) { NextCommand(brain[ibrainTemp] + rotation); Move(brain[ibrainTemp] + rotation); step = 0;  }
                if (brain[ibrainTemp] > 7 && brain[ibrainTemp] < 16) { NextCommand(brain[ibrainTemp] + rotation); Grab(brain[ibrainTemp] + rotation); step = 0; }
                if (brain[ibrainTemp] > 15 && brain[ibrainTemp] < 24) { NextCommand(brain[ibrainTemp] + rotation);  }
                if (brain[ibrainTemp] > 23 && brain[ibrainTemp] < 32) { rotation += brain[ibrain]; rotation %= 8; ibrain++; }
                if (brain[ibrainTemp] > 31) { ibrain += brain[ibrainTemp]; }
                
                step--;
                ibrain %= brainLengt;
                                }
                life--;
                if (life > 0) { World.world[x_pos, y_pos] = World.matter.insect;  return true; }
                else { return false; }
            
        }

        private void BrainInit()
        {
            for (int i = 0; i < brainLengt; i++)
            {
                brain[i] = BrainRandom();
            }
         }

        private void CloneBrain(int[] br )
        {
            for (int i = 0; i < brainLengt; i++)
            {
                this.brain[i] = br[i];
            }
        }

        public static void BugsLanding()
        {
            foreach (var i in Bugs.arrBugs)
            {
                i.BrainInit();
                World.GetRandomPosition(ref i.x_pos, ref i.y_pos);
            }
        }
        
        private static int BrainRandom()
        {
            return rand.Next(0,63);
        }

        private void Move(int command)
        {
            World.matter m = Look(command);
            command %= 8;
              if ( m == World.matter.empty || m== World.matter.food)
            {
                x_pos += xShiftMatrix[command];
                y_pos += yShiftMatrix[command];
            }
            if ( m == World.matter.food)
            {
                life +=15;
                World.ReSetRandomPosition(x_pos,y_pos);
            }
            if (m == World.matter.poison) { life = 0; }
            
        }

        private void Grab(int command)
        {
            command %= 8;
            World.matter m = Look(command);
            if (m == World.matter.food)
            {
                life += 15;
                World.ReSetRandomPosition(x_pos + xShiftMatrix[command], y_pos + yShiftMatrix[command]);
            }
            if (m == World.matter.poison)
            {
                life += 15;
                World.ReSetRandomPosition(x_pos + xShiftMatrix[command], y_pos + yShiftMatrix[command]); }
        }

        private void NextCommand( int command)
        {
            command %= 8;
            switch (World.world[x_pos + xShiftMatrix[command], y_pos + yShiftMatrix[command]])
            {
                case World.matter.empty: ibrain += 5; break;
                case World.matter.food: ibrain += 4; break;
                case World.matter.insect: ibrain += 3; break;
                case World.matter.poison: ibrain++; break;
                case World.matter.wall: ibrain += 2; break;

            }
        }

        private World.matter Look(int command)
        { 
            command %= 8;
            return World.world[x_pos + xShiftMatrix[command], y_pos + yShiftMatrix[command]];
        }

    }
}
