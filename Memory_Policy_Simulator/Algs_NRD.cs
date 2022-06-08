using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_NRD : Algs {
        private Dictionary<char, int> refTimes;
        private int currentStrIdx;
        private string str;

        public Algs_NRD(int getFrameSize, string str) : base(getFrameSize) {
            this.algsName = "NRD";
            this.refTimes = new Dictionary<char, int>();
            this.aux = -1;
            this.str = str;
            this.currentStrIdx = 0;
        }

        public int getNormalVictim() {
            var items = from pair in refTimes orderby pair.Value ascending select pair;

            foreach (KeyValuePair<char, int> item in items)
                if (frameWindow.Any(x => x.data == item.Key))
                    return frameWindow.IndexOf(frameWindow.Find(x => x.data == item.Key));

            return 0;
        }

        public int replaceDual(char before, char after) {
            string tmp = str.Substring(0, currentStrIdx);
            var freqs = tmp.GroupBy(c => c).OrderBy(c => c.Count()).ToDictionary(c => c.Key, g => g.Count());
            freqs.Remove(before);
            freqs.Remove(after);

            bool determined = false;
            char victim = '0', replace = '0';

            foreach (var v in freqs.Keys) {
                if (!frameWindow.Any(x => x.data == v)) {
                    victim = v;
                    determined = true;
                    break;
                }
            }

            Debug.Write("before : ");
            foreach(var v in freqs.Keys) {
                Debug.Write(v + "(" + freqs[v] + "), ");
            }

            freqs = tmp.GroupBy(c => c).ToDictionary(c => c.Key, g => g.Count());

            foreach (var v in freqs.Keys) {
                if (!frameWindow.Any(x => x.data == v)) {
                    victim = v;
                    determined = true;
                    break;
                }
            }


            Debug.Write("after : ");
            foreach (var v in freqs.Keys) {
                Debug.Write(v + "(" + freqs[v] + "), ");
            }

            if (determined)
                Debug.WriteLine("determined : " + victim + ", times : " + freqs[victim]);

            return -1;
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
