using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_NRD : Algs {
        private Dictionary<char, int> refTimes;

        public Algs_NRD(int getFrameSize) : base(getFrameSize) {
            this.algsName = "NRD";
            refTimes = new Dictionary<char, int>();
            aux = -1;
        }

        public int getNormalVictim() {
            var items = from pair in refTimes orderby pair.Value ascending select pair;

            foreach (KeyValuePair<char, int> item in items)
                if (frameWindow.Any(x => x.data == item.Key))
                    return frameWindow.IndexOf(frameWindow.Find(x => x.data == item.Key));

            return 0;
        }

        public int replaceDual(char before, char after) {
            var asc = from pair in refTimes orderby pair.Value ascending select pair;
            var desc = from pair in refTimes orderby pair.Value descending select pair;

            int victim = 0, target = 0;

            foreach (KeyValuePair<char, int> item in asc)
                if (frameWindow.Any(x => x.data == item.Key) && item.Key != before && item.Key != after) 
                    victim = frameWindow.IndexOf(frameWindow.Find(x => x.data == item.Key));

            Debug.WriteLine("after" + after + ", victim : " + victim);

            foreach (KeyValuePair<char, int> item in desc)
                if (!frameWindow.Any(x => x.data == item.Key) && item.Key != before && item.Key != after)
                    target = frameWindow.IndexOf(frameWindow.Find(x => x.data == item.Key));

            if (victim != target)
                return target;

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
                refTimes[newPage.data]++;
                aux = -1;
            } else {
                if (frameWindow.Count >= frameSize) {
                    newPage.status = Page.STATUS.MIGRATION;
                    int victimIdx = getNormalVictim();
                    newPage.before = frameWindow[victimIdx].data;
                    frameWindow.RemoveAt(victimIdx);

                    if (!refTimes.Any(x => x.Key == data))
                        refTimes[newPage.data] = 0;

                    aux = replaceDual(newPage.before, data);
                } else {
                    // First fault
                    newPage.status = Page.STATUS.PAGEFAULT;
                    refTimes[newPage.data] = 0;

                    aux = -1;
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
