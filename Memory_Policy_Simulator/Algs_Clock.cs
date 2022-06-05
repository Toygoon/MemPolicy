using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Memory_Policy_Simulator {
    class Algs_Clock : Algs {
        Dictionary<char, bool> refBit;

        public Algs_Clock(int getFrameSize) : base(getFrameSize) {
            this.refBit = new Dictionary<char, bool>();
        }

        public int getVictimIdx() {
            List<Page> tmp = frameWindow.ToList();
            tmp.Sort(delegate (Page x, Page y) {
                return x.pid.CompareTo(y.pid);
            });

            for (int i = 0; i<tmp.Count; i++) {
                if (refBit[tmp[i].data])
                    refBit[tmp[i].data] = false;
                else
                    return frameWindow.IndexOf(tmp[i]);
            }

            return -1;
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
                    refBit.Remove(newPage.before);
                    frameWindow.RemoveAt(victimIdx);
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

            foreach(var v in refBit.Keys) {
                Debug.WriteLine(v + " : " + refBit[v]);
          
            }

            Debug.WriteLine("");
            return newPage;
        }
    }
}