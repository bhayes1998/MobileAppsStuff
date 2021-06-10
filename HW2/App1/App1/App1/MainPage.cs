/*
 * Bryan Hayes 
 * CSE 570J
 * 2/27/2020
 * MainPage
 * The main form for the US Tax Infomration app 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using USLocations;
using System.Windows;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace App1
{
    public class MainPage : ContentPage
    {
        USLocation loc;

        ListView listViewOfStates;

        Entry entryAmount;
        Entry entryCity;
        Entry entryState;
        Picker commandPicker;

        Button amountButton;
        Button cityStateButton;

        Label titleLabel;
        Label infoLabel;
        Label amountLabel;
        Label cityLabel;
        Label stateLabel;
        Label commandLabel;

        StackLayout topLevel;
        StackLayout amountBox;
        StackLayout cityBox;
        StackLayout stateBox;
        StackLayout commandBox;

        public List<string> infoList;
        public ObservableCollection<string> observableCollection;

        public MainPage()
        {
            // Loads in all of the data 
            loc = new USLocation();
            string filename = "zipcodes.tsv";
           
            loc.loadData(filename);

            // List to store search results later 
            infoList = new List<string>();

            // Title label 
            titleLabel = new Label {
                Text = "Welcome to US Tax Information!",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 21,
                TextColor = Color.Black,
                BackgroundColor = Color.RoyalBlue
            };

            // Info label 
            infoLabel = new Label {
                Text = "Pick an option from the dropdown below.  Choosing \"Amount\" will allow you to " +
                        "enter a dollar amount to see all the zipcodes in the U.S. with average tax returns within $100 of that amount. " +
                        "Choosing \"City State\" will allow you to choose a city and state in the U.S. and see all of the zipcodes " +
                        "in that are in that city.\n\nHave fun!",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                BackgroundColor = Color.RoyalBlue
            };

            // List view for displaying search results 
            listViewOfStates = new ListView();


            listViewOfStates.ItemsSource = infoList;

            // Several StackLayouts used for the entries, labels, and picker that follow 
            amountBox = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 0, 10, 10) };
            cityBox = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 0, 10, 0) };
            stateBox = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 0, 11, 10) };
            commandBox = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 20, 10, 0) };

            entryAmount = new Entry
            {
                WidthRequest = 200,
                HeightRequest = 45,
                BackgroundColor = Color.LightGreen,
                Placeholder = "How much?"
            };

            amountLabel = new Label { Text = "Enter Dollar Amount:  ",
                TextColor = Color.White,
                FontSize = 18
            };
            amountBox.Children.Add(amountLabel);
            amountBox.Children.Add(entryAmount);

            entryCity = new Entry
            {
                WidthRequest = 200,
                HeightRequest = 45,
                BackgroundColor = Color.LightBlue,
                Placeholder = "Which city?"
            };
            cityLabel = new Label {
                Text = "Enter City Name:         ",
                TextColor = Color.White,
                FontSize = 18
            };
            cityBox.Children.Add(cityLabel);
            cityBox.Children.Add(entryCity);

            entryState = new Entry
            {
                WidthRequest = 200,
                HeightRequest = 45,
                BackgroundColor = Color.LightSalmon,
                Placeholder = "Which state?"
            };
            
            stateLabel = new Label { Text = "Enter State Name:      ", TextColor = Color.White, FontSize = 18 };
            stateBox.Children.Add(stateLabel);
            stateBox.Children.Add(entryState);

            commandPicker = new Picker {
                WidthRequest = 200,
                HeightRequest = 45,
                BackgroundColor = Color.RoyalBlue
            };

            List<string> commandOptions = new List<string>();
            commandOptions.Add("Amount");
            commandOptions.Add("City State");
            commandPicker.ItemsSource = commandOptions;
            commandPicker.SelectedIndexChanged += SelectedIndexChanged;

            commandLabel = new Label {
                Text = "What'll it be?                ",
                TextColor = Color.White,
                FontSize = 18
            };

            commandBox.Children.Add(commandLabel);
            commandBox.Children.Add(commandPicker);

            // Button used for the Amount option 
            amountButton = new Button {
                Text = "Let's do some taxes!",
                WidthRequest = 250,
                HorizontalOptions = LayoutOptions.Center
            };
            amountButton.Clicked += OnClickedAmount;

            // Button used for the City State option 
            cityStateButton = new Button {
                Text = "Let's find those zipcodes!",
                WidthRequest = 250,
                HorizontalOptions = LayoutOptions.Center
            };
            cityStateButton.Clicked += OnClickedCityState;

            // StackLayout that will house everything created above 
            topLevel = new StackLayout();


            topLevel.Children.Add(titleLabel);
            topLevel.Children.Add(infoLabel);
            topLevel.Children.Add(commandBox);
            topLevel.Padding = new Thickness(10, 15, 10, 0);
            topLevel.BackgroundColor = Color.Black;


            Content = topLevel;

        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            var pick = sender as Picker;

            if (pick.SelectedIndex == 0)
            {
                topLevel = new StackLayout();
                topLevel.Children.Add(titleLabel);
                topLevel.Children.Add(infoLabel);
                topLevel.Children.Add(commandBox);

                entryAmount.Text = "";
                topLevel.Children.Add(amountBox);
                topLevel.Children.Add(amountButton);

                listViewOfStates = new ListView();
                var template = new DataTemplate(typeof(TextCell));
                template.SetValue(TextCell.TextColorProperty, Color.White);
                template.SetBinding(TextCell.TextProperty, ".");
                listViewOfStates.ItemTemplate = template;

                topLevel.Children.Add(listViewOfStates);
                topLevel.Padding = new Thickness(10, 15, 10, 0);

                topLevel.BackgroundColor = Color.Black;

                Content = topLevel;
            }
            else
            {
                topLevel = new StackLayout();
                topLevel.Children.Add(titleLabel);
                topLevel.Children.Add(infoLabel);
                topLevel.Children.Add(commandBox);

                entryState.Text = null;
                entryCity.Text = null;
                topLevel.Children.Add(cityBox);
                topLevel.Children.Add(stateBox);
                topLevel.Children.Add(cityStateButton);
                cityStateButton.Clicked += OnClickedCityState;

                listViewOfStates = new ListView();
                var template = new DataTemplate(typeof(TextCell));
                template.SetValue(TextCell.TextColorProperty, Color.White);
                template.SetBinding(TextCell.TextProperty, ".");
                listViewOfStates.ItemTemplate = template;

                topLevel.Children.Add(listViewOfStates);
                topLevel.Padding = new Thickness(10, 15, 10, 0);

                topLevel.BackgroundColor = Color.Black;

                Content = topLevel;
            }
        }

        // Event handler for the Amount button 
        private void OnClickedAmount(object sender, EventArgs e)
        {
            double amount = 0;
            bool result = double.TryParse(entryAmount.Text, out amount);

            if (result)
            {
                infoList = loc.avgTaxWithin(Convert.ToDouble(entryAmount.Text));

                // Displays a message if no results are found 
                if (infoList.Count == 0)
                {
                    infoList.Add("Sorry, but I couldn't find anything.");
                }
                listViewOfStates.ItemsSource = new List<string>(infoList);
            }

            // Displays a message if the dollar amount entry does not contain a number 
            else
            {
                infoList = new List<string>();
                infoList.Add("Make sure you put a number in the dollar amount box!");
                listViewOfStates.ItemsSource = new List<string>(infoList);
            }
        }

        // Event handler for the CityState Button 
        private void OnClickedCityState(object sender, EventArgs e)
        {
            double amount = 0;
            bool result = double.TryParse(entryCity.Text, out amount);
            double stateAmount = 0;
            bool stateResult = double.TryParse(entryState.Text, out stateAmount);
            
            // Displays a message if either the city or state entries are empty 
            if ((entryCity.Text == null) || (entryState.Text == null) || (entryCity.Text == "") || (entryState.Text == ""))
            {
                infoList = new List<string>();
                infoList.Add("Make sure you put a valid city and state!");
                listViewOfStates.ItemsSource = new List<string>(infoList);
            }

            // Displays a message if either the city or state entries are numerical  
            else if (result || stateResult)
            {
                infoList = new List<string>();
                infoList.Add("You can't put numbers for a city or state!");
                listViewOfStates.ItemsSource = new List<string>(infoList);
            }
            else
            {
                // Stores search results 
                infoList = loc.zipsInCityState(entryCity.Text, entryState.Text);

                // Displays a message if no results are found
                if (infoList.Count == 0)
                {
                    infoList.Add("Sorry, but I couldn't find anything.");
                }
                listViewOfStates.ItemsSource = new List<string>(infoList);
            }
        }
    }
}

