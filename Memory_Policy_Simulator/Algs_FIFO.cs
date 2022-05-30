using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator {
    class Algs_FIFO : Algs { 
        public Algs_FIFO(int get_frame_size) : base(get_frame_size) {
        }

        public override Page.STATUS Operate(char data) {
            Page newPage;

            if (frame_window.Any<Page>(x => x.data == data)) {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                hit++;
                int i;

                for (i = 0; i < frame_window.Count; i++) {
                    if (frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;
            } else {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size) {
                    newPage.status = Page.STATUS.MIGRATION;
                    frame_window.Dequeue();
                    cursor = p_frame_size;
                    migration++;
                    fault++;
                } else {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    cursor++;
                    fault++;
                }

                newPage.loc = cursor;
                frame_window.Enqueue(newPage);
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }

        public override List<Page> GetPageInfo(Page.STATUS status) {
            List<Page> pages = new List<Page>();

            foreach (Page page in pageHistory) {
                if (page.status == status) {
                    pages.Add(page);
                }
            }

            return pages;
        }
    }


}