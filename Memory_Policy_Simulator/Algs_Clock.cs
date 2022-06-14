using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// Clock (Second Chance) Algorithm Simulation
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    class Algs_Clock : Algs {
        // refBit : Each data indicates key, and reference bit saves the value for it
        private Dictionary<char, bool> refBit;

        public Algs_Clock(int getFrameSize) : base(getFrameSize) {
            this.algsName = "CLOCK";
            this.refBit = new Dictionary<char, bool>();
        }

        public override Page Operate(char data) {
            // Create a new page
            Page newPage = new Page {
                pid = Page.createdAt++,
                data = data
            };

            // Process the current data
            if (frameWindow.Any(x => x.data == data)) {
                // The case; Page Hit
                newPage.status = Page.STATUS.HIT;
                // Increase the number of hits
                hit++;
                // Change reference bit for current data to true
                refBit[newPage.data] = true;
            } else {
                // The case; Page fault
                refBit[newPage.data] = false;

                if (frameWindow.Count >= frameSize) {
                    // The case; Existing page should be replaced
                    newPage.status = Page.STATUS.MIGRATION;
                    
                    while (true) {
                        // The case; All reference bits are false, abort
                        if (!refBit.Any(x => x.Value == false))
                            break;

                        // The case; Only single page in the frame window
                        if (refBit[frameWindow[0].data] == false)
                            break;

                        // The case; Select victim as first index, and add new page to the last
                        if (refBit[frameWindow[0].data] == true) {
                            Page p = frameWindow[0];
                            frameWindow.RemoveAt(0);
                            frameWindow.Add(p);
                            refBit[p.data] = false;
                        }
                    }

                    // Save before data
                    newPage.before = frameWindow[0].data;
                    // Reset the reference bit that is victim
                    refBit.Remove(newPage.before);
                    // Remove first index (Like FIFO)
                    frameWindow.RemoveAt(0);
                } else {
                    // First fault
                    newPage.status = Page.STATUS.PAGEFAULT;
                }

                // Increase the fault
                fault++;
                // New data will be added into the last of the index
                frameWindow.Add(newPage);
            }
            // Record the page history
            pageHistory.Add(newPage);

            return newPage;
        }
    }
}