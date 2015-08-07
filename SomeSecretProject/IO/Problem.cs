using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.IO
{
    public class Problem
    {
        public int id;
        public Unit[] units;
        public int width, height;

        public Cell[] filled;
        public int sourceLength;
        public int[] sourceSeeds;
    }
}
