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

            Page victim = frameWindow[0];
            foreach(var v in frameWindow) {
                if (freqs[victim.data] > freqs[v.data])
                    victim = v;
            }

            foreach(var v in frameWindow) {
                freqs.Remove(v.data);
            }


            Page newPage = new Page {
                pid = Page.createdAt,
                data = freqs.Keys.Last()
            };

            newPage.status = Page.STATUS.MIGRATION;
            newPage.before = victim.data;
            frameWindow.Remove(victim);
            frameWindow.Add(newPage);

            return frameWindow.IndexOf(newPage);

            /*
            bool determined = false;
            char victim = '0', replace = '0';

            foreach (var v in freqs.Keys) {
                if (!frameWindow.Any(x => x.data == v)) {
                    victim = v;
                    determined = true;
                    break;
                }
            }

            if (determined == false)
                return -1;

            freqs = tmp.GroupBy(c => c).ToDictionary(c => c.Key, g => g.Count());

            determined = false;
            foreach (var v in freqs.Keys) {
                if (!frameWindow.Any(x => x.data == v)) {
                    replace = v;
                    determined = true;
                    break;
                }
            }

            if (determined == false)
                return -1;

            Page newPage = new Page {
                pid = Page.createdAt,
                data = replace
            };

            newPage.status = Page.STATUS.MIGRATION;
            newPage.before = victim;
            frameWindow.Remove(frameWindow.Find(x => x.data == victim));
            frameWindow.Add(newPage);

            return frameWindow.IndexOf(newPage);

            */
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
                aux = -1;
                refTimes[newPage.data]++;
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
