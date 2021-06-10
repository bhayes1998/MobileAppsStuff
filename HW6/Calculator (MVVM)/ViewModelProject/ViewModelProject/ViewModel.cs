using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Calculator;

namespace ViewModelProject
{
    public class ViewModel : ViewModelBase
    {
        private static float total = 0;
        private static List<string> ops = new List<string>();
        private static List<float> args = new List<float>();
        private static string currentEntry = "0";
        string outputString = "0";

        public ViewModel()
        {
            ClearCommand = new Command(
                execute: () =>
                {
                    OutputString = "0";
                    currentEntry = "0";
                    ops = new List<string>();
                    args = new List<float>();
                    total = 0;
                    RefreshCanExecutes();
                });

            NumericCommand = new Command<string>(
                execute: (string parameter) =>
                {
                    if (OutputString == "0")
                    {
                        OutputString = parameter;
                        CurrentEntry = parameter;
                    }
                    else
                    {
                        OutputString += parameter;
                        CurrentEntry += parameter;
                    }
                    RefreshCanExecutes();
                });

            DecimalPointCommand = new Command(
                execute: () =>
                {
                    CurrentEntry += ".";
                    OutputString += ".";
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !CurrentEntry.Contains(".");
                });

            OperationCommand = new Command<string>(
                execute: (string parameter) =>
                {
                    float arg2;
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                    {
                        CurrentEntry += "0";
                        OutputString += "0";
                    }
                    bool isNum = float.TryParse(CurrentEntry, out arg2);

                    if (isNum || Total > 0)
                    {
                        Args.Add(arg2);
                        CurrentEntry = "";
                        Ops.Add(parameter);
                        OutputString += parameter;
                    }
                    RefreshCanExecutes();
                },
                canExecute: (string parameter) =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            EqualsCommand = new Command(
                execute: () =>
                {
                    if (OutputString == "0")
                        return;

                    float arg2;

                    bool isNum = float.TryParse(CurrentEntry, out arg2);

                    if (isNum)
                    {
                        if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                            CurrentEntry += "0";

                        if (Ops.Count == 0)
                        {
                            OutputString = CurrentEntry;
                        }
                        else
                        {
                            Args.Add(arg2);
                            string op = Ops[0];
                            if (op == "+")
                                Total += Args[0] + Args[1];
                            else if (op == "-")
                                Total += Args[0] - Args[1];
                            else if (op == "/")
                                Total += Args[0] / Args[1];
                            else if (op == "x")
                                Total += Args[0] * Args[1];

                            for (int i = 1; i < Ops.Count; i++)
                            {
                                if (Ops[i] == "+")
                                    Total += Args[i + 1];
                                else if (Ops[i] == "-")
                                    Total -= Args[i + 1];
                                else if (Ops[i] == "/")
                                    Total = Total / Args[i + 1];
                                else if (Ops[i] == "x")
                                    Total *= Args[i + 1];
                            }
                            if (Total.ToString().Contains("Infinity") || Total.ToString() == "NaN")
                                Total = 0;
                            OutputString = Total.ToString();
                            Args = new List<float>();
                            Ops = new List<string>();
                            CurrentEntry = Total.ToString();
                            Total = 0;
                        }
                    }
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            SquaredCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg *= arg;

                    CurrentEntry = arg.ToString();
                    OutputString += "^2";
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            TenToTheXCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Pow(10, arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "10^(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            OneOverXCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    if (arg != 0)
                        arg = 1 / arg;

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "1/(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && CurrentEntry != "0";
                });

            SquareRootCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Sqrt(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "sqrt(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    float arg;
                    if (float.TryParse(CurrentEntry, out arg))
                        return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && arg < 0;
                    return false;
                });

            NaturalLogCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    if (arg != 0)
                        arg = (float)Math.Log(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "ln(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    float arg;
                    if (float.TryParse(CurrentEntry, out arg))
                        return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && arg > 0;
                    return false;
                });

            CosCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Cos(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "cos(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            SignCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg *= -1;

                    if (arg < 0)
                    {
                        OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                        OutputString += "(-" + CurrentEntry + ")";
                        CurrentEntry = arg.ToString();
                    }
                    else if (arg > 0)
                    {
                        OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                        CurrentEntry = arg.ToString();
                        OutputString += "(" + CurrentEntry + ")";
                    }
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            FactorialCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    if (arg != 0)
                        arg = factorial(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "(" + CurrentEntry + "!)";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    float arg;
                    if (float.TryParse(CurrentEntry, out arg))
                        return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && arg > 0;
                    return false;
                });

            ThirdRootCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Pow(arg, (1.0 / 3));

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "3rt(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    float arg;
                    if (float.TryParse(CurrentEntry, out arg))
                        return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && arg < 0;
                    return false;
                });

            Log10Command = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    if (arg != 0)
                        arg = (float)Math.Log10(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);          
                    OutputString += "log10(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    float arg;
                    if (float.TryParse(CurrentEntry, out arg))
                        return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') && arg > 0;
                    return false;
                });

            EToTheXCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = Convert.ToInt32(Math.Exp(arg));

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "log10(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                });

            SinCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Sin(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "sin(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                 canExecute: () =>
                 {
                     return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                 });

            TanCommand = new Command(
                execute: () =>
                {
                    if (CurrentEntry[CurrentEntry.Length - 1] == '.')
                        CurrentEntry += "0";
                    float arg = float.Parse(CurrentEntry);
                    arg = (float)Math.Tan(arg);

                    OutputString = OutputString.Substring(0, OutputString.Length - CurrentEntry.Length);
                    OutputString += "tan(" + CurrentEntry + ")";
                    CurrentEntry = arg.ToString();
                    RefreshCanExecutes();
                },
                 canExecute: () =>
                 {
                     return !(OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x');
                 });

            RandCommand = new Command(
                execute: () =>
                {
                    Random rand = new Random();
                    float arg = (float)rand.NextDouble();
                    if (CurrentEntry == "0")
                    {
                        if (OutputString == "0")
                            OutputString = "";
                        else
                            OutputString = OutputString.Substring(0, OutputString.Length - 2);
                    }
                    CurrentEntry = arg.ToString();
                    OutputString += CurrentEntry;
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return (OutputString[OutputString.Length - 1] == '/' || OutputString[OutputString.Length - 1] == '+' || OutputString[OutputString.Length - 1] == '-' || OutputString[OutputString.Length - 1] == 'x') || CurrentEntry == "0";
                });

            SecondCommand = new Command(
                execute: () =>
                {
                    if ()
                });
        }

        private float factorial(float arg)
        {
            if (arg == 1)
                return 1;
            else
                return (arg * factorial(arg - 1));
        }

        void RefreshCanExecutes()
        {
            ((Command)NumericCommand).ChangeCanExecute();
            ((Command)DecimalPointCommand).ChangeCanExecute();
            ((Command)OperationCommand).ChangeCanExecute();
            ((Command)SquaredCommand).ChangeCanExecute();
            ((Command)TenToTheXCommand).ChangeCanExecute();
            ((Command)OneOverXCommand).ChangeCanExecute();
            ((Command)SquareRootCommand).ChangeCanExecute();
            ((Command)NaturalLogCommand).ChangeCanExecute();
            ((Command)CosCommand).ChangeCanExecute();
            ((Command)SignCommand).ChangeCanExecute();
            ((Command)FactorialCommand).ChangeCanExecute();
            ((Command)ThirdRootCommand).ChangeCanExecute();
            ((Command)Log10Command).ChangeCanExecute();
            ((Command)EToTheXCommand).ChangeCanExecute();
            ((Command)SinCommand).ChangeCanExecute();
            ((Command)TanCommand).ChangeCanExecute();
            ((Command)RandCommand).ChangeCanExecute();
        }

        public string OutputString
        {
            private set { SetProperty(ref outputString, value); }
            get { return outputString; }
        }

        public string CurrentEntry
        {
            private set { SetProperty(ref currentEntry, value); }
            get { return currentEntry; }
        }

        public List<string> Ops
        {
            private set { SetProperty(ref ops, value); }
            get { return ops; }
        }

        public List<float> Args
        {
            private set { SetProperty(ref args, value); }
            get { return args; }
        }

        public float Total
        {
            private set { SetProperty(ref total, value); }
            get { return total; }
        }

        public ICommand ClearCommand { private set; get; }
        public ICommand ClearEntryCommand { private set; get; }
        public ICommand BackspaceCommand { private set; get; }
        public ICommand NumericCommand { private set; get; }
        public ICommand PlusMinusCommand { private set; get; }
        public ICommand DecimalPointCommand { private set; get; }
        public ICommand EqualsCommand { private set; get; }
        public ICommand OperationCommand { private set; get; }
        public ICommand SquaredCommand { private set; get; }
        public ICommand TenToTheXCommand { private set; get; }
        public ICommand OneOverXCommand{ private set; get; }
        public ICommand SquareRootCommand{ private set; get; }
        public ICommand NaturalLogCommand{ private set; get; }
        public ICommand CosCommand{ private set; get; }
        public ICommand SignCommand{ private set; get; }
        public ICommand FactorialCommand{ private set; get; }
        public ICommand ThirdRootCommand{ private set; get; }
        public ICommand Log10Command{ private set; get; }
        public ICommand EToTheXCommand{ private set; get; }
        public ICommand SinCommand{ private set; get; }
        public ICommand TanCommand{ private set; get; }
        public ICommand RandCommand{ private set; get; }
        public ICommand SecondCommand{ private set; get; }


        public void RestoreState(IDictionary<string, object> dictionary)
        {
            OutputString = GetDictionaryEntry(dictionary, "OutputString", "0");
            Args = GetDictionaryEntry(dictionary, "Args", new List<float>());
            Ops = GetDictionaryEntry(dictionary, "Ops", new List<string>());
            CurrentEntry = GetDictionaryEntry(dictionary, "CurrentEntry", "0");
            Total = GetDictionaryEntry(dictionary, "Total", 0);

            RefreshCanExecutes();
        }

        public void SaveState(IDictionary<string, object> dictionary)
        {
            dictionary["OutputString"] = OutputString;
            dictionary["Args"] = Args;
            dictionary["Ops"] = Ops;
            dictionary["CurrentEntry"] = CurrentEntry;
            dictionary["Total"] = Total;
        }

        public T GetDictionaryEntry<T>(IDictionary<string, object> dictionary,
                                      string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defaultValue;
        }
    }

   

}