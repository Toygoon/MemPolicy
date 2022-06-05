using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    public struct Page {
        public enum STATUS {
            HIT,
            PAGEFAULT,
            MIGRATION
        }
        public static int createdAt;
        public int pid;
        public char data, before;
        public int refCount;
        public STATUS status;

        public Page copy() {
            return (Page) MemberwiseClone();
        }
    }

}
