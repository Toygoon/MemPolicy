using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_Optimal : Algs {
        int currentStrIdx;
        string str;

        public Algs_Optimal(int get_frame_size, string str) : base(get_frame_size) {
            this.currentStrIdx = 0;
            this.str = str;
        }

        public override Page Operate(char data) {
            // Create a new page
            Page newPage = new Page {
                pid = Page.CREATE_ID++,
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
                    string tmp = str.Substring(currentStrIdx);
                    Debug.WriteLine(tmp);
                    Dictionary<char, int> freq = tmp.Select(c => char.ToUpperInvariant(c)).GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
                    foreach (char c in freq.Keys) {
                        Debug.Write(c + "(" + freq[c] + ") ");
                    }
                    Debug.WriteLine("");
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

            currentStrIdx++;
            return newPage;
        }
    }
}
