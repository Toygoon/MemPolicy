using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Memory_Policy_Simulator {
    class Algs_Clock : Algs {
        Dictionary<char, bool> refBit;

        public Algs_Clock(int getFrameSize) : base(getFrameSize) {
            this.refBit = new Dictionary<char, bool>();
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
                refBit[newPage.data] = false;

                if (frameWindow.Count >= frameSize) {
                    newPage.status = Page.STATUS.MIGRATION;
                    

                    while (true) {
                        if (!refBit.Any(x => x.Value == false))
                            break;

                        Debug.WriteLine("data : " + frameWindow[0].data);

                        if (refBit[frameWindow[0].data] == false)
                            break;

                        if (refBit[frameWindow[0].data] == true) {
                            Page p = frameWindow[0];
                            frameWindow.RemoveAt(0);
                            frameWindow.Add(p);
                            refBit[p.data] = false;
                        }
                    }

                    newPage.before = frameWindow[0].data;
                    refBit.Remove(newPage.before);
                    frameWindow.RemoveAt(0);
                } else {
                    // First fault
                    newPage.status = Page.STATUS.PAGEFAULT;
                }

                fault++;
                // New data will be added into the last of the index
                frameWindow.Add(newPage);
            }
            pageHistory.Add(newPage);

            foreach (var v in refBit.Keys) {
                Debug.WriteLine(v + " : " + refBit[v]);

            }

            Debug.WriteLine("");
            return newPage;
        }
    }
}