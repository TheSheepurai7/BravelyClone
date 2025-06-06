using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    class JobInfo
    {
        public string name { get; protected set; }
        public int lvl { get; private set; }

        public JobInfo()
        {
            //At the very least the job level should be set to 1 by default
            lvl = 1;
        }
    }
}
