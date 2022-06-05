using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    class Algs_RefBit : Algs {
        Dictionary<char, bool> refBit;

        public Algs_RefBit(int getFrameSize) : base(getFrameSize) {
            this.refBit = new Dictionary<char, bool>();
        }

        public int getVictimIdx() {
            foreach(char c in refBit.Keys)
                if (refBit[c] == false && frameWindow.Any(x => x.data == c))
                    return frameWindow.IndexOf(frameWindow.Find(x => x.data == c));

            return 0;
        }

        public override Page Operate(char data) {
            // Create a new page
            Page newPage = new Page {
                pid = Page.createdAt++,
                data = data
            };

            if (frameWindow.Any(x => x.data == data)) {
                // The case; Page Hit
                newPage.status = Page.STATUS.HIT;
                // Increase the number of hits
                hit++;
                refBit[newPage.data] = true;
            } else {
                if (frameWindow.Count >= frameSize) {
                    newPage.status = Page.STATUS.MIGRATION;
                    int victimIdx = getVictimIdx();
                    newPage.before = frameWindow[victimIdx].data;
                    frameWindow.RemoveAt(victimIdx);
                    refBit.Remove(newPage.before);
                } else {
                    // First fault
                    newPage.status = Page.STATUS.PAGEFAULT;
                }

                refBit.Add(newPage.data, false);

                fault++;
                // New data will be added into the last of the index
                frameWindow.Add(newPage);
            }
            pageHistory.Add(newPage);

            return newPage;
        }
    }
}