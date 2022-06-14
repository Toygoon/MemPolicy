using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// LMFU (Least Most Frequent Used) Algorithm Simulation
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    class Algs_LMFU : Algs {
        // Reverse bit
        bool reverseOrder = false;
        public Algs_LMFU(int getFrameSize) : base(getFrameSize) {
            this.algsName = "LMFU";
        }

        /// <summary>
        /// Algorithm Operations
        /// First, get the victim index using LFU algorithm
        /// Second, if LFU algorithm is used before, use MFU algorithm now
        /// Third, Repeat this
        /// </summary>
        /// <returns>The index of selected victim</returns>
        public int getVictimIdx() {
            List<Page> tmp = frameWindow.ToList();
            // Sort
            if (reverseOrder) {
                tmp.Sort(delegate (Page x, Page y) {
                    if (x.refCount > y.refCount)
                        return 1;
                    else if (x.refCount < y.refCount)
                        return -1;

                    return x.pid.CompareTo(y.pid);
                });
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

            // Change the order
            reverseOrder = !reverseOrder;

            // First index is Least, or Most
            return frameWindow.IndexOf(tmp[0]);
        }

        // Operation is identical with LFU, MFU
        public override Page Operate(char data) {
            // Create a new page
            Page newPage = new Page {
                pid = Page.createdAt++,
                data = data
            };

            if (frameWindow.Any(x => x.data == data)) {
                int idx = frameWindow.IndexOf(frameWindow.Find(x => x.data == data));
                newPage = frameWindow[idx].copy();
                newPage.refCount++;
                frameWindow[idx] = newPage;

                // The case; Page Hit
                newPage.status = Page.STATUS.HIT;
                // Increase the number of hits
                hit++;
            } else {
                if (frameWindow.Count >= frameSize) {
                    newPage.status = Page.STATUS.MIGRATION;
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
            pageHistory.Add(newPage);

            return newPage;
        }
    }
}
