using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// Reference Bit Algorithm Simulation
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    class Algs_RefBit : Algs {
        // refBit : Each data indicates key, and reference bit saves the value for it
        private Dictionary<char, bool> refBit;

        public Algs_RefBit(int getFrameSize) : base(getFrameSize) {
            this.algsName = "REFBIT";
            this.refBit = new Dictionary<char, bool>();
        }

        /// <summary>
        /// getVictimIdx : Calculate the victim
        /// </summary>
        /// <returns>The index of selected victim</returns>
        public int getVictimIdx() {
            // Find the page which has false reference bit
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
                // Change reference bit for current data to true
                refBit[newPage.data] = true;
            } else {
                if (frameWindow.Count >= frameSize) {
                    // The case; Existing page should be replaced
                    newPage.status = Page.STATUS.MIGRATION;
                    // Get the victim
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