using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject
{
    public class Cell
    {
        public int x;
        public int y;
    }

    public class Unit
    {
        public Cell[] members;
        public Cell pivot;

    }
}
