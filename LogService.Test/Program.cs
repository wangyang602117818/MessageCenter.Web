﻿using MessageCenter.Business;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogService.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Log4Net.InfoLog("start...");
            new Processor().StartWork();

            Console.WriteLine("ok");
            Console.ReadKey();
        }
    }
}
