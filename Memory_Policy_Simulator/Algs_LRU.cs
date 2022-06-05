using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_LRU : Algs {
        int currentStrIdx;
        string str;

        public Algs_LRU(int getFrameSize, string str) : base(getFrameSize) {
            this.currentStrIdx = 0;
            this.str = str;
        }

        public int getVictimIdx() {
            string tmp = str.Substring(0, currentStrIdx - 1);
            List<char> windowChars = new List<char>(), strChars = new List<char>();

            foreach (var v in frameWindow)
                windowChars.Add(v.data);

            foreach (char c in tmp)
                strChars.Add(c);

            for (int i = tmp.Length - 1; i >= 0; i--) {
                char c = strChars[i];

                if (windowChars.Count == 1)
                    return frameWindow.IndexOf(frameWindow.Find(x => x.data == windowChars[0]));

                if (windowChars.Contains(c))
                    windowChars.Remove(c);
            }

            return 0;
        }

        public override Page Operate(char data) {
            currentStrIdx++;

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
