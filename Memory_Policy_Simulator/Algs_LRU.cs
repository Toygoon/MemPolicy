using System.Collections.Generic;
using System.Linq;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// LRU (Least Recently Used) Algorithm Simulation
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    class Algs_LRU : Algs {
        // currentStrIdx : For the substring
        private int currentStrIdx;
        // str : Reference string
        private string str;

        public Algs_LRU(int getFrameSize, string str) : base(getFrameSize) {
            this.algsName = "LRU";
            this.currentStrIdx = 0;
            this.str = str;
        }

        /// <summary>
        /// getVictimIdx : Calculate the victim
        /// </summary>
        /// <returns>The index of selected victim</returns>
        public int getVictimIdx() {
            // Substring for entire reference string given
            string tmp = str.Substring(0, currentStrIdx - 1);
            // Separate current existing page, or not for list
            List<char> windowChars = new List<char>(), strChars = new List<char>();

            // Make lists
            foreach (var v in frameWindow)
                windowChars.Add(v.data);

            foreach (char c in tmp)
                strChars.Add(c);

            // Remove the data referenced before
            for (int i = tmp.Length - 1; i >= 0; i--) {
                char c = strChars[i];

                if (windowChars.Count == 1)
                    return frameWindow.IndexOf(frameWindow.Find(x => x.data == windowChars[0]));

                if (windowChars.Contains(c))
                    windowChars.Remove(c);
            }

            // If there's no victim, make a result like the FIFO
            return 0;
        }

        public override Page Operate(char data) {
            // Increase the current index of string
            currentStrIdx++;

            // Create a new page
            Page newPage = new Page {
                pid = Page.createdAt++,
                data = data
            };

            // Process the current data
            if (frameWindow.Any(x => x.data == data)) {
                // The case; Page Hit
                newPage.status = Page.STATUS.HIT;
                // Increase the number of hits
                hit++;
            } else {
                if (frameWindow.Count >= frameSize) {
                    // The case; Existing page should be replaced
                    newPage.status = Page.STATUS.MIGRATION;
                    // Get the victim index
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
