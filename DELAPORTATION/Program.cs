/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;

namespace DELAPORTATION
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Params)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SplashWin(Params));
        }
    }
}
