/* Bryan Hayes
 * CSE 570J
 * 3/14/2020
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;


namespace Calculator
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage(ViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
