using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Calculator
{
    class ViewModel : ViewModelBase
    {
        private static float total = 0;
        private static List<string> ops = new List<string>();
        private static List<float> args = new List<float>();
        private static string tempArg = "";
        private static string outputString = "0";

        public ViewModel()
        {
            ClearCommand = new Command(
                execute: () =>
                {
                    outputString = "0";
                    tempArg = "0";
                    ops = new List<string>();
                    args = new List<float>();
                    total = 0;
                });

        }

        public string OutputString
        {
            private set { SetProperty(ref outputString, value); }
            get { return outputString; }
        }

        public ICommand ClearCommand { private set; get; }

        public ICommand ClearEntryCommand { private set; get; }

        public ICommand BackspaceCommand { private set; get; }

        public ICommand NumericCommand { private set; get; }
        public ICommand PlusMinusCommand { private set; get; }

        public ICommand DecimalPointCommand { private set; get; }
        public ICommand FactCommand { private set; get; }

        public ICommand AddCommand { private set; get; }
    }

}