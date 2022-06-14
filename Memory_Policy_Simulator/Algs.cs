using System.Collections.Generic;

namespace Memory_Policy_Simulator {
    /// <summary>
    /// Abstract Algorithm Class for Algorithms
    /// By Lim Jung Min (Dept. of Computer Engineering, Yeungnam University)
    /// </summary>
    abstract class Algs {
        // algsName : The name of current algorithm
        public string algsName;
        // frameSize : The size of current frame window
        public int frameSize;
        // pageHistory : The latest page that has issued will be added in to the list
        public List<Page> pageHistory;
        // frameWindow : Current working frame window
        public List<Page> frameWindow;

        // hit : hit times
        public int hit;
        // fault : fault times
        public int fault;

        // Constructor for initiating values
        public Algs(int getFrameSize) {
            frameSize = getFrameSize;
            pageHistory = new List<Page>();
            frameWindow = new List<Page>();
        }

        /// <summary>
        /// Operate the algorithm
        /// </summary>
        /// <param name="data">The data which is referencing now</param>
        /// <returns>Referenced page</returns>
        public abstract Page Operate(char data);
    }
}
