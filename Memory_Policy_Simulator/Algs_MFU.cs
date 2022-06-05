using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_MFU : Algs {
        public Algs_MFU(int getFrameSize) : base(getFrameSize) {
        }

        public int getVictimIdx() {
            List<Page> tmp = frameWindow.ToList();
            tmp.Sort(delegate (Page x, Page y) {
                if (x.refCount > y.refCount)
                    return 1;
                else if (x.refCount < y.refCount)
                    return -1;

                return x.pid.CompareTo(y.pid);
            });

            foreach (Page p in tmp) {
                Debug.WriteLine(p.data + " : " + p.refCount);
            }
            Debug.WriteLine("");

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
