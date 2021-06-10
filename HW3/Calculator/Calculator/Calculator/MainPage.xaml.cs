/* Bryan Hayes
 * CSE 570J
 * 3/14/2020
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Calculator
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // Properties for storing number and operators 
        private static float total = 0;
        private static List<string> ops = new List<string>();
        private static List<float> args = new List<float>();
        private static string tempArg = "";


        public MainPage()
        {
            InitializeComponent();
        }

        // Event handler for numbers, operations, and clear 
        public void Button_Clicked(object sender, EventArgs e)
        {
            Button pressed = sender as Button;
            int temp;
            bool isInt = int.TryParse(pressed.Text, out temp);
            
            if (pressed.Text == "+" || pressed.Text == "x" || pressed.Text == "/" || pressed.Text == "-")
            {
                if (Output.Text[Output.Text.Length-1] == '/' || Output.Text[Output.Text.Length - 1] == '+' || Output.Text[Output.Text.Length - 1] == '-' || Output.Text[Output.Text.Length - 1] == 'x')
                    return;

                float arg2;
                if (tempArg[tempArg.Length - 1] == '.')
                {
                    tempArg += "0";
                    Output.Text += "0";
                }
                bool isNum = float.TryParse(tempArg, out arg2);

                if (isNum || total > 0)
                {
                    args.Add(arg2);
                    tempArg = "";
                    ops.Add(pressed.Text);
                    Output.Text += pressed.Text;
                }
            }

            else if (pressed.Text == ".")
            {
                if (!tempArg.Contains("."))
                {
                    tempArg += ".";
                    Output.Text += ".";
                }
            }

            else if (pressed.Text == "AC")
            {
                Output.Text = "0";
                tempArg = "0";
                ops = new List<string>();
                args = new List<float>();
                total = 0;
            }

            else if (pressed.Text == "=")
            {
                if (Output.Text == "0")
                    return;

                float arg2;
                
                bool isNum = float.TryParse(tempArg, out arg2);

                if (isNum)
                {
                    if (tempArg[tempArg.Length - 1] == '.')
                        tempArg += "0";

                    if (ops.Count == 0)
                    {
                        Output.Text = tempArg;
                    }
                    else
                    {
                        args.Add(arg2);
                        string op = ops[0];
                        if (op == "+")
                            total += args[0] + args[1];
                        else if (op == "-")
                            total += args[0] - args[1];
                        else if (op == "/")
                            total += args[0] / args[1];
                        else if (op == "x")
                            total += args[0] * args[1];

                        for (int i = 1; i < ops.Count; i++)
                        {
                            if (ops[i] == "+")
                                total += args[i + 1];
                            else if (ops[i] == "-")
                                total -= args[i + 1];
                            else if (ops[i] == "/")
                                total = total / args[i + 1];
                            else if (ops[i] == "x")
                                total *= args[i + 1];
                        }
                        if (total.ToString().Contains("Infinity") || total.ToString() == "NaN")
                            total = 0;
                        Output.Text = total.ToString();
                        args = new List<float>();
                        ops = new List<string>();
                        tempArg = total.ToString();
                        total = 0;
                    }
                }
            }

            else if (isInt || (Output.Text == "0" || (Output.Text == "" && pressed.Text == ".")))
            {
                if (Output.Text == "0")
                {
                    Output.Text = pressed.Text;
                    tempArg = pressed.Text;
                }
                else
                {
                    Output.Text += pressed.Text;
                    tempArg += pressed.Text;
                }
            }
        }

        // Event handler for function buttons (Grad portion)
        public void Function_Clicked(object sender, EventArgs e)
        {
            Button pressed = sender as Button;

            if (Output.Text[Output.Text.Length - 1] == '/' || Output.Text[Output.Text.Length - 1] == '+' || Output.Text[Output.Text.Length - 1] == '-' || Output.Text[Output.Text.Length - 1] == 'x')
                return;

            float arg;
            if (tempArg[tempArg.Length - 1] == '.')
                tempArg += "0";
            bool temp = float.TryParse(tempArg, out arg);

            if (!temp)
                return;
            if (pressed.Text == "x^2")
            {
                arg *= arg;

                tempArg = arg.ToString();
                Output.Text += "^2";
            }

            else if (pressed.Text == "10^x")
            {
                arg = (float)Math.Pow(10, arg);

                Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                Output.Text += "10^(" + tempArg + ")";
                tempArg = arg.ToString();
  

            }

            else if (pressed.Text == "1/x")
            {
                if (arg != 0)
                    arg = 1 / arg;

                Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                Output.Text += "1/(" + tempArg + ")";
                tempArg = arg.ToString();
            }

            else if (pressed.Text == "sqrt(x)")
            {
                arg = (float)Math.Sqrt(arg);

                Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                Output.Text += "sqrt(" + tempArg + ")";
                tempArg = arg.ToString();
            }

            else if (pressed.Text == "ln")
            {
                if (arg != 0)
                    arg = (float)Math.Log(arg);

                Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                Output.Text += "ln(" + tempArg + ")";
                tempArg = arg.ToString();
            }

            else if (pressed.Text == "cos")
            {
                arg = (float)Math.Cos(arg);

                Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                Output.Text += "cos(" + tempArg + ")";
                tempArg = arg.ToString();
            }

            else if (pressed.Text == "+/-")
            {
                arg *= -1;

                if (arg < 0)
                {
                    Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                    Output.Text += "(-" + tempArg + ")";
                    tempArg = arg.ToString();
                }
                else if (arg > 0)
                {
                    Output.Text = Output.Text.Substring(0, Output.Text.Length - tempArg.Length);
                    tempArg = arg.ToString();
                    Output.Text += "(" + tempArg + ")";
                }
            }
        }
    }
}
