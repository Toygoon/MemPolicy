using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_LMFU : Algs {
        bool reverseOrder = false;
        public Algs_LMFU(int getFrameSize) : base(getFrameSize) {
            this.algsName = "LMFU";
        }

        public int getVictimIdx() {
            List<Page> tmp = frameWindow.ToList();
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

            reverseOrder = !reverseOrder;

            return frameWindow.IndexOf(tmp[0]);
        }

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
