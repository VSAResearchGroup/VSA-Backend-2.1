using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Network {
        /*
         This is a dummy mockup of a network.The real structure is implemented by Andrue Cashman
         */
        private int[,] sequence; //adjacency list for all jobs

        public Network(ArrayList s) {
            //make dummy lists for the optional courses
            MakeDummyNetwork();
        }

        public void InsertJob(Job s) {

        }

        public void DeleteJob(Job s) {

        }

        private void MakeDummyNetwork() {
            //array filled as row is the main, col is what it is adjacent to;
            sequence = new int[14, 14] {
               //0 1 2 3 4 5 6 7 8 9 0 1 2 3 
                {0,0,0,0,0,0,1,1,0,0,0,0,0,0}, //0
                {0,0,0,0,0,0,0,0,1,0,1,0,0,0}, //1
                {0,1,0,0,0,0,0,0,1,0,0,0,0,0}, //2
                {0,0,1,0,0,0,0,0,0,1,0,0,0,0}, //3
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0}, //4
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1}, //5
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //6
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0}, //7
                {0,0,0,0,0,0,0,0,0,0,0,1,0,0}, //8
                {0,0,0,0,0,0,0,0,0,0,0,0,1,0}, //9
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //10
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //11
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1}, //12
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0}  //13
            };

        }
    }
}
