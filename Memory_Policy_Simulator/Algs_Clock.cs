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
            int oldest = 0;

            if (!refBit.Any(x => x.Value == false)) {
                for (int i = 1; i<frameWindow.Count; i++)
                    if (frameWindow[i].pid < frameWindow[oldest].pid)
                        oldest = i;

                return oldest;
            }

            List<Page> tmp = frameWindow.ToList();
            foreach (char c in refBit.Keys)
                if (refBit[c] == true) {
                    Debug.WriteLine(c + "is true");
                    tmp.Remove(tmp.Find(x => x.data == c));
                }

            foreach (Page p in frameWindow)
                if (!tmp.Contains(p)) {
                    Debug.WriteLine(p.data + "will be changed");
                    refBit[p.data] = false;
                }

            for (int i = 0; i<tmp.Count; i++)
                if (tmp[i].pid < tmp[oldest].pid) {
                    Debug.WriteLine("victim is " + tmp[i].data);
                    oldest = i;
                }

            return frameWindow.IndexOf(frameWindow.Find(x => x.data == tmp[oldest].data));
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
                    refBit.Add(newPage.data, false);
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