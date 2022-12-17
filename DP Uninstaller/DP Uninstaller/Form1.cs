/*
    DELAPORTATION (DP) Uninstaller

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DP_Uninstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION\command", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\*\shell\DELAPORTATION", false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
