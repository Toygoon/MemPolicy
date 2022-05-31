using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    abstract class Algs {
        public int frameSize;
        public List<Page> pageHistory;
        public List<Page> frameWindow;

        public int hit;
        public int fault;

        public Algs(int get_frame_size) {
            frameSize = get_frame_size;
            pageHistory = new List<Page>();
            frameWindow = new List<Page>();
        }

        public abstract Page Operate(char data);
    }
}
