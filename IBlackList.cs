using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMPurge
{
        //interface to faciliate block list files, extensions etc
        //keep a xml list of block list files
    interface IBlackList
    {
            //return black list as string IEnumerable
         IEnumerable<string> getBlackList();
    }
}
