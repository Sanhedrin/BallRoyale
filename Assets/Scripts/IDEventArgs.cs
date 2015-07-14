using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class IDEventArgs : EventArgs
    {
        private int m_ID;
        public int ID
        {
            get { return m_ID; }
        }

        public IDEventArgs(int i_ID)
        {
            m_ID = i_ID;
        }
    }
}
