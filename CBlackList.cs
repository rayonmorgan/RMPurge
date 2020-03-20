/*
 * Author: Rayon Morgan
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMPurge
{
    //implements IBlackList
    class CBlackList : IBlackList
    {
        //implements getBlackList
        public IEnumerable<string> getBlackList()
        {
            List<string> bList = new List<string>();
            //bList.Add(".exe");
            //bList.Add(".bat");
            bList.Add("virus");

            return bList;
        }
    }
}
