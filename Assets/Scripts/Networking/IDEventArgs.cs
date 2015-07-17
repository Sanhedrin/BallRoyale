using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Player_Scripts
{
    public class IDEventArgs : EventArgs
    {
        public int ID { get; private set; }

        public IDEventArgs(int i_ID)
        {
            ID = i_ID;
        }
    }
}
