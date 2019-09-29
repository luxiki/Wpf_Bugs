using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Bugs
{
    public class Matter
    {
        public int x_pos;
        public int y_pos;
        public int Id { get; protected set; }
        public Matter(int id)
        {
            this.Id = id;
        }
    }

    public class Poison:Matter
    {
        public new static readonly string name = "poison";
        public Poison(int id) : base(id)
        {
        }
    }

    public class Food : Matter
    {
        public new static readonly string name = "poison";
        public Food(int id) : base(id)
        {
        }
    }
}
