﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    class Program
    {
        static void Main(string[] args)
        {
            Server uut = new Server();
            uut.Start();
        }
    }
}
