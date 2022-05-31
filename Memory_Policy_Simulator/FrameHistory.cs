using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class FrameHistory {
        public int seqAt;
        public char removedData;
        public char insertedData;
        public int issuedIdx;
        public int frameSize;
        public Page.STATUS status;

        public FrameHistory(int seqAt, char removedData, char insertedData, int issuedIdx, int frameSize) {
            this.seqAt = seqAt;
            this.removedData = removedData;
            this.insertedData = insertedData;
            this.issuedIdx = issuedIdx;
            this.frameSize = frameSize;
        }

        public void historyToPage() {
            Page page = new Page();
            page.pid = this.seqAt;
            page.data = this.insertedData;
            return;
        }
    }
}
