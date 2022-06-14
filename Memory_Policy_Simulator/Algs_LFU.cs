using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// LFU (Least Frequently Used) or MFU (Most Frequently Used) Algorithm Simulation
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    class Algs_LFU : Algs {
        // isLFU : Determine LFU or MFU
        private bool isLFU;

        public Algs_LFU(int getFrameSize, bool isLFU) : base(getFrameSize) {
            this.algsName = "LFU";
            this.isLFU = isLFU;
        }

        /// <summary>
        /// getVictimIdx : Calculate the victim
        /// </summary>
        /// <returns>The index of selected victim</returns>
        public int getVictimIdx() {
            List<Page> tmp = frameWindow.ToList();
            // LFU : Reference count will be sorted with ascending order
            if (isLFU) {
                tmp.Sort(delegate (Page x, Page y) {
                    if (x.refCount > y.refCount)
                        return 1;
                    else if (x.refCount < y.refCount)
                        return -1;

                    return x.pid.CompareTo(y.pid);
                });
            // LFU : Reference count will be sorted with descending order
            } else {
                this.algsName = "MFU";
                tmp.Sort(delegate (Page x, Page y) {
                    if (x.refCount < y.refCount)
                        return 1;
                    else if (x.refCount > y.refCount)
                        return -1;

                    return x.pid.CompareTo(y.pid);
                });
            }

            return frameWindow.IndexOf(tmp[0]);
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
                // Find current index
                int idx = frameWindow.IndexOf(frameWindow.Find(x => x.data == data));
                // Clone current page (The count will be changed)
                newPage = frameWindow[idx].copy();
                // Increase the referenced count
                newPage.refCount++;
                frameWindow[idx] = newPage;

                // The case; Page Hit
                newPage.status = Page.STATUS.HIT;
                // Increase the number of hits
                hit++;
            } else {
                // The case; Page fault
                if (frameWindow.Count >= frameSize) {
                    // The case; Existing page should be replaced
                    newPage.status = Page.STATUS.MIGRATION;
                    // Get victim index
                    int victimIdx = getVictimIdx();
                    newPage.before = frameWindow[victimIdx].data;
                    frameWindow.RemoveAt(victimIdx);
                } else {
                    // First fault
                    newPage.status = Page.STATUS.PAGEFAULT;
                }

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
