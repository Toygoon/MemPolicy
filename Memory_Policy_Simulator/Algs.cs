using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    abstract class Algs {
        protected int cursor;
        public int p_frame_size;
        public Queue<Page> frame_window;
        public List<Page> pageHistory;

        public int hit;
        public int fault;
        public int migration;

        public Algs(int get_frame_size) {
            cursor = 0;
            p_frame_size = get_frame_size;
            frame_window = new Queue<Page>();
            pageHistory = new List<Page>();
        }

        public abstract Page.STATUS Operate(char data);
        public abstract List<Page> GetPageInfo(Page.STATUS status);
    }
}
