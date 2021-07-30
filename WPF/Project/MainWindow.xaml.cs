using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Threading;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer menuTimer = new Timer(50); //Menu Timer to animate the side menu
        bool shrink; //Wether to shrink or enlarge the side menu

        XmlSerializer xs = new XmlSerializer(typeof(Game)); //Read the xml files

        List<Game> allGames = new List<Game>(); //All games available
        List<Game> popularGames = new List<Game>(); //All popular games
        List<Game> newGames = new List<Game>(); //All new games
        HashSet<Game> cartGames = new HashSet<Game>(); //All distinct games added to cart
        HashSet<Game> wishList = new HashSet<Game>(); //All distinct games added to wishlist
        GameGenre gameGenre = new GameGenre(); //List of all genres available
        Game selectedGame = new Game(); //Selected Game
        double cartTotalPrice = 0; // Total prices of games added to cart

        string populargamesPath = "Games/Popular"; //Path containing all the popular games
        string newgamesPath = "Games/New"; //Path containing all the new games

        string allGamesFilter = "All Games"; //Filter that includes all the games

        //Sorting options
        string sortTitleA_Z = "Title (A-Z)"; 
        string sortTitleZ_A = "Title (Z-A)";
        string sortPriceH_L = "Price (High-Low)";
        string sortPriceL_H = "Price (Low-High)";

        //Different Pages
        UserControl currentPage;
        UserControl compare;
        HomePage homePage = new HomePage();
        GamePage gamePage = new GamePage();
        SearchPage searchPage = new SearchPage();
        CartPage cartPage = new CartPage();

        public MainWindow()
        {
            InitializeComponent();

            //Load All the games
            LoadGame(populargamesPath, popularGames);
            LoadGame(newgamesPath, newGames);

            sideMenu.Width = 0; // Close the side menu

            //Add a background to the search box
            searchBox.Background = LoadImageBrush("Icons/Search.png");

            //Add a background to user password/name and set the text to empty
            userPassword.Password = "";
            userPassword.Background = LoadImageBrush("Icons/Password.png");
            userName.Text = "";
            userName.Background = LoadImageBrush("Icons/Username.png");


            //Add the allgames filter to the combobox
            ComboBoxItem item = new ComboBoxItem();
            item.Content = allGamesFilter;
            cmbGenre.Items.Add(item);

            //Add each game genre to the game genre
            foreach (string genre in gameGenre.Genres)
            {
                item = new ComboBoxItem();
                item.Content = genre;

                cmbGenre.Items.Add(item);
            }

            cmbGenre.SelectionChanged += (object sender , SelectionChangedEventArgs e) => FilterGames(); //Call filter on selection changed
            cmbSort.SelectionChanged += (object sender , SelectionChangedEventArgs e) => SortGames(); //Call sort on selection changed

            //Add the sort modes to the combobox
            item = new ComboBoxItem();
            item.Content = sortTitleA_Z;
            cmbSort.Items.Add(item);
            item = new ComboBoxItem();
            item.Content = sortTitleZ_A;
            cmbSort.Items.Add(item);
            item = new ComboBoxItem();
            item.Content = sortPriceL_H;
            cmbSort.Items.Add(item);
            item = new ComboBoxItem();
            item.Content = sortPriceH_L;
            cmbSort.Items.Add(item);

            LoadHomePage(); //Open the homepage
        }

        public void LoadGame(string path , List<Game> games)
        {
            //Read all xml files in path and add it to the games list and all games
            foreach (string file in Directory.GetFiles(path))
            {
                StreamReader reader = new StreamReader(file);
                Game currentGame = (Game)xs.Deserialize(reader);
                games.Add(currentGame);
                allGames.Add(currentGame);
                gameGenre.Genres.Add(currentGame.Genre); //Add the current game genre to the hash set
            }
        }

        #region PAGE FUNCTIONS

        private void UpdateCurrentPage(UserControl control)
        {
            //Remove the current page update it and add it
            screenDock.Children.Remove(currentPage);
            currentPage = control;
            screenDock.Children.Add(currentPage);
        }

        private void OpenHomePage(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        public void LoadHomePage()
        {
            //Show the filtering sorting searching widgets and hide the compare button
            cmbGenre.Visibility = Visibility.Visible;
            cmbSort.Visibility = Visibility.Visible;
            searchBox.Visibility = Visibility.Visible;
            compareButton.Visibility = Visibility.Hidden;
            searchBox.Text = ""; //Empty the search box text

            //Remove the current page from the screen
            if (currentPage != null)
            {
                screenDock.Children.Remove(currentPage);
            }

            //Remove the compare page from the screen and set it to null
            if (compare != null)
            {
                screenDock.Children.Remove(compare);
                compare = null;
            }

            //Set the current page to the homepage and add it to the screen
            UpdateCurrentPage(homePage);

            //Loop through all games
            foreach (Game game in allGames)
            {
                //Add game to the popular panel
                if (popularGames.Contains(game))
                {
                    CreateGameButton(game, homePage.panelPopular);
                }

                //Add game to the new panel
                else if (newGames.Contains(game))
                {
                    CreateGameButton(game, homePage.panelNew);
                }
            }

            //Filter and sort games in the homepage
            FilterGames();
            SortGames();
        }

        private void Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && searchBox.Text != "")
            {
                //The user is in the homepage
                if (compare as SearchPage != searchPage && compare == null)
                {
                    UpdateCurrentPage(searchPage);
                    searchPage.Panel.MaxWidth = 1920;
                }

                //The user opened a game in the compare page
                else if (compare as SearchPage != searchPage)
                {
                    screenDock.Children.Remove(compare);
                    compare = searchPage;
                    screenDock.Children.Add(compare);
                }

                //Clear the search page
                searchPage.Panel.Children.Clear();

                foreach (Game game in allGames)
                {
                    //Checks that the title contains the search text
                    if (game.Title.ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        GameCell cell = CreateGameCell(game);
                        searchPage.Panel.Children.Add(cell);
                    }
                }
            }
        }

        private void GamePage(Game game)
        {
            //Hide the filter sort and search widget
            cmbGenre.Visibility = Visibility.Hidden;
            cmbSort.Visibility = Visibility.Hidden;
            searchBox.Visibility = Visibility.Hidden;

            //Show the compare button add its click event and update its content to compare
            compareButton.Visibility = Visibility.Visible;
            compareButton.Click += OpenComparePage;
            compareButton.Content = "Compare";

            selectedGame = game; //Update the selected game

            gamePage.gameScroll.ScrollToHome(); //Always force the game page to start at the top

            //Add the game cover and icon image
            gamePage.coverImage.Source = new BitmapImage(new Uri(game.SourceImage, UriKind.Relative));
            gamePage.gameIcon.Source = new BitmapImage(new Uri(game.SourceImage, UriKind.Relative));

            //Update the game info
            gamePage.gameName.Content = game.Title;
            gamePage.gameCompany.Content = game.Company;

            //Update description and scroll it to the top
            gamePage.gameDescription.Text = game.Description;
            gamePage.descriptionScroll.ScrollToHome();

            //Update the buy label and price add the click event
            gamePage.lblBuy.Content = $"Buy Game: {game.Title}";
            gamePage.btnBuy.Content = game.Price != 0 ? $"Buy : ${game.Price}" : "Free";
            gamePage.btnBuy.Click += (object sender, RoutedEventArgs e) => cartGames.Add(selectedGame); //Add game to cart

            //Update the gift label and price add the click event
            gamePage.lblGift.Content = $"Gift Game: {game.Title}";
            gamePage.btnGift.Content = game.Price != 0 ? $"Gift : ${game.Price}" : "Free";
            gamePage.btnGift.Click += (object sender, RoutedEventArgs e) => cartGames.Add(selectedGame); //Add game to cart

            //Add the click event on the wish list
            gamePage.gameWishList.Click += (object sender, RoutedEventArgs e) => wishList.Add(selectedGame); //Add game to wish list

            //Update the game rating
            gamePage.gameRating.Children.Clear();
            Label label = new Label();
            label.Content = "Rating : ";
            gamePage.gameRating.Children.Add(label);

            //Add a full star
            for (int i = 0; i < Math.Truncate(game.Rating); i++)
            {
                Image Star = new Image();
                Star.Width = 50;
                Star.Height = 50;
                Star.Source = new BitmapImage(new Uri("Icons/Star.png", UriKind.Relative));
                gamePage.gameRating.Children.Add(Star);
            }

            //Add half a star
            if (game.Rating - Math.Truncate(game.Rating) == 0.5)
            {
                Image HalfStar = new Image();
                HalfStar.Width = 50;
                HalfStar.Height = 50;
                HalfStar.Source = new BitmapImage(new Uri("Icons/Half Star.png", UriKind.Relative));
                gamePage.gameRating.Children.Add(HalfStar);
            }

            UpdateCurrentPage(gamePage);
        }

        private void OpenComparePage(object sender, RoutedEventArgs e)
        {
            //Update current page add it to the left
            UpdateCurrentPage(CreateComparePage(selectedGame));
            DockPanel.SetDock(currentPage, Dock.Left);

            //Add the compare page
            compare = searchPage;
            screenDock.Children.Add(compare);
            searchPage.Panel.MaxWidth = 1300;

            //Empty the search box and show all the games availabe
            searchBox.Text = "";
            searchPage.Panel.Children.Clear();
            foreach (Game game in allGames)
            {
                GameCell cell = CreateGameCell(game);
                searchPage.Panel.Children.Add(cell);
            }

            //Show the search filter and sort widgets
            searchBox.Visibility = Visibility.Visible;
            cmbGenre.Visibility = Visibility.Visible;
            cmbSort.Visibility = Visibility.Visible;

            //Update compare click event and content
            compareButton.Content = "Stop Compare";
            compareButton.Click -= OpenComparePage;
            compareButton.Click += StopCompare;
        }



        private void StopCompare(object sender, RoutedEventArgs e)
        {
            //Remove the compare page and set it to null
            screenDock.Children.Remove(compare);
            compare = null;

            //Update the compare button content and remove its click event
            compareButton.Content = "Compare";
            compareButton.Click -= StopCompare;

            //Add the game page
            GamePage(selectedGame);
        }

        private void OpenCart(object sender, RoutedEventArgs e)
        {
            //Remove the compare from the screen and set it to null
            if (compare != null)
            {
                screenDock.Children.Remove(compare);
                compare = null;
            }

            UpdateCurrentPage(cartPage);

            //Add the games to the cart and wish list
            UpdateCartGames();
            UpdateWishList();

            //Hide the search filtering and sort widgets
            cmbGenre.Visibility = Visibility.Hidden;
            cmbSort.Visibility = Visibility.Hidden;
            searchBox.Visibility = Visibility.Hidden;

            //Remove the click events from the compare button and update its content to checkout
            compareButton.Click -= StopCompare;
            compareButton.Click -= OpenComparePage;
            compareButton.Content = "Checkout";
        }

        #endregion

        #region SIGN IN/SIGN UP

        private void ExpandSignIn(object sender, RoutedEventArgs e)
        {
            //Open the sign in sub menu
            if (userName.Height == 0)
            {
                btnSignIn.Content = "SIGN IN ▼";
                userName.Height = 70;
                userPassword.Height = 70;
                dockSignIn.Height = 70;
                btnSignUp.Height = 70;
            }

            //Close the sign in sub menu
            else if (userName.Height == 70)
            {
                btnSignIn.Content = "SIGN IN ►";
                userName.Height = 0;
                userPassword.Height = 0;
                dockSignIn.Height = 0;
                btnSignUp.Height = 0;
            }
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            //Check that the user name and password is not empty
            if (userName.Text != "" && userPassword.Password != "")
            {
                //Update the username label
                lbluserName.Content = userName.Text;

                //Empty the user name and password field
                userName.Text = "";
                userPassword.Password = "";
            }

            //Show message that user name is empty
            else if (userName.Text == "")
            {
                MessageBox.Show("Please Enter Username");
            }

            //Show message that password is empty
            else if (userPassword.Password == "")
            {
                MessageBox.Show("Please Enter Password");
            }
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            //Open sign up page
            SignUpWindow signUp = new SignUpWindow();
            signUp.Show();
        }

        #endregion

        #region  CART FUNCTIONS

        private void UpdateCartGames()
        {
            //Clear the cart and set the total price to 0
            cartPage.cartPanel.Children.Clear();
            cartTotalPrice = 0;

            //Add the games to the panel and add the price to the total
            foreach (Game game in cartGames)
            {
                GameCell cell = CreateGameCell(game);
                cartPage.cartPanel.Children.Add(cell);
                cartTotalPrice += game.Price;
            }

            //Update the price label
            cartPage.cartTotal.Content = $"Price: ${cartTotalPrice}";
        }

        private void UpdateWishList()
        {
            //Clear the wishlist
            cartPage.wishPanel.Children.Clear();

            //Add the games to the cart
            foreach (Game game in wishList)
            {
                GameCell cell = CreateGameCell(game);
                cartPage.wishPanel.Children.Add(cell);
            }
        }

        #endregion

        #region SIDE MENU

        private void OpenSideMenu(object sender, RoutedEventArgs e)
        {
            //Check if the timer is running
            if (!menuTimer.Enabled)
            {
                shrink = sideMenu.Width != 0 ? true : false; //Set wether to shrink or enlarge
                menuTimer.Interval = 25; //Interval = 25ms
                menuTimer.Elapsed += MenuTimer_Elapsed; // Add tick event
                menuTimer.Start(); //Start Timer
            }
        }

        private void MenuTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Call the function to update the width
            Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
              new Action(() => UpdateMenuWidth(sender)));
        }

        void UpdateMenuWidth(object sender)
        {
            //Shrink menu
            if (shrink)
            {
                sideMenu.Width -= 20;

                //Stop the timer set the width to 0 
                if (sideMenu.Width < 20)
                {
                    sideMenu.Width = 0;
                    ((Timer)sender).Stop();
                    menuTimer.Elapsed -= MenuTimer_Elapsed; //Remove the event
                }
            }

            //Enlarge menu
            else
            {
                sideMenu.Width += 20;

                //Stop the timer set the width to 500
                if (sideMenu.Width > 500)
                {
                    ((Timer)sender).Stop();
                    sideMenu.Width = 500;
                    menuTimer.Elapsed -= MenuTimer_Elapsed; //Remove the event
                }
            }

        }

        #endregion

        #region SEARCH FUNCTIONS

        private void ScrollGames(object sender, KeyEventArgs e)
        {
            //If we're in the homepage scroll the popular games and new games
            if (currentPage as HomePage == homePage)
            {
                if (e.Key == Key.Up)
                {
                    homePage.scrollPopular.ScrollToHorizontalOffset(homePage.scrollPopular.HorizontalOffset - 20);
                }
                else if (e.Key == Key.Down)
                {
                    homePage.scrollPopular.ScrollToHorizontalOffset(homePage.scrollPopular.HorizontalOffset + 20);
                }
                else if (e.Key == Key.Left)
                {
                    homePage.scrollNew.ScrollToHorizontalOffset(homePage.scrollNew.HorizontalOffset - 20);
                }
                else if (e.Key == Key.Right)
                {
                    homePage.scrollNew.ScrollToHorizontalOffset(homePage.scrollNew.HorizontalOffset + 20);
                }
            }
        }

        private void FilterGames()
        {
            ComboBoxItem genre = (ComboBoxItem)cmbGenre.SelectedItem;

            if (currentPage as HomePage == homePage)
            {
                //Clear the panels
                homePage.panelPopular.Children.Clear();
                homePage.panelNew.Children.Clear();

                foreach (Game game in allGames)
                {
                    //Add all the games to the panel
                    if (genre == null || genre.Content.ToString() == allGamesFilter)
                    {
                        if (popularGames.Contains(game))
                        {
                            CreateGameButton(game, homePage.panelPopular);
                        }

                        else if (newGames.Contains(game))
                        {
                            CreateGameButton(game, homePage.panelNew);
                        }
                    }

                    //Add all the games that matches the current genre
                    else if (game.Genre == genre.Content.ToString())
                    {
                        if (popularGames.Contains(game))
                        {
                            CreateGameButton(game, homePage.panelPopular);
                        }

                        else if (newGames.Contains(game))
                        {
                            CreateGameButton(game, homePage.panelNew);
                        }
                    }
                }
            }

            //The user in either searching or comparing games and searching
            else if (currentPage as SearchPage == searchPage || compare as SearchPage == searchPage)
            {
                //Clear the search page
                searchPage.Panel.Children.Clear();

                foreach (Game game in allGames)
                {
                    //Check if the search text is part of the game title
                    if (game.Title.ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        //Add all the games that matches the search
                        if (genre == null || genre.Content.ToString() == allGamesFilter)
                        {
                            GameCell cell = CreateGameCell(game);
                            searchPage.Panel.Children.Add(cell);
                        }

                        //Add all the games that matches the search and the selected genre
                        else if (game.Genre == genre.Content.ToString())
                        {
                            GameCell cell = CreateGameCell(game);
                            searchPage.Panel.Children.Add(cell);
                        }
                    }
                }
            }
        }

        private void SortGames()
        {
            ComboBoxItem item = (ComboBoxItem)cmbSort.SelectedItem;

            //Check if a sort option was chosen
            if (item != null)
            {
                //Sort the games in the allgames list based on the chosen option
                if (item.Content.ToString() == sortTitleA_Z)
                {
                    allGames.Sort((x, y) => string.Compare(x.Title, y.Title));
                }

                else if (item.Content.ToString() == sortTitleZ_A)
                {
                    allGames.Sort((x, y) => string.Compare(y.Title, x.Title));
                }

                else if (item.Content.ToString() == sortPriceL_H)
                {
                    allGames.Sort((x, y) => x.Price < y.Price ? -1 : 1);
                }

                else if (item.Content.ToString() == sortPriceH_L)
                {
                    allGames.Sort((x, y) => x.Price < y.Price ? 1 : -1);
                }
            }

            //Filter the games after sorting
            FilterGames();
        }

        #endregion

        #region CREATE GAME BUTTON/CELL AND COMPARE PAGE

        public void CreateGameButton(Game game, StackPanel panel)
        {
            //Add a button that opens the game page on click
            Button button = new Button();
            button.Click += (object sender, RoutedEventArgs e) => GamePage(game);

            //Add the game cover on the button
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(game.SourceImage, UriKind.Relative));
            button.Content = image;

            //Set the button width and height
            button.Width = 300;
            button.Height = 300;

            //Add button to panel
            panel.Children.Add(button);
        }

        public GameCell CreateGameCell(Game game)
        {
            GameCell cell = new GameCell();

            if (compare == null)
            {
                //Add games to the search page
                if (currentPage as SearchPage == searchPage)
                {
                    cell.gameButton.Click += (object s, RoutedEventArgs ee) => GamePage(game); //Go to gamepage on click
                }

                //Add games to the cart page
                else if (currentPage as CartPage == cartPage)
                {
                    cell.gameButton.Click += (object s, RoutedEventArgs ee) => {
                        //Remove game from the cart
                        if (cartPage.cartPanel.Children.Contains(cell))
                        {
                            cartGames.Remove(game);
                            UpdateCartGames();
                        }

                        //Remove game from the wishlist
                        if (cartPage.wishPanel.Children.Contains(cell))
                        {
                            wishList.Remove(game);
                            UpdateWishList();
                        }
                    };
                }
            }

            //Add games to the compare search
            else
            {
                cell.gameButton.Click += (object s, RoutedEventArgs ee) =>
                {
                    ComparePage page = CreateComparePage(game);
                    screenDock.Children.Remove(compare);
                    compare = page;
                    screenDock.Children.Add(compare);
                };
            }

            //Update cell info
            cell.gameName.Text = game.Title;
            cell.gameCompany.Text = game.Company;
            cell.gamePrice.Content = game.Price == 0 ? "Free" : $"${game.Price}";
            cell.gameIcon.Source = new BitmapImage(new Uri(game.SourceImage, UriKind.Relative));

            return cell;
        }

        public ComparePage CreateComparePage(Game game)
        {
            ComparePage comparePage = new ComparePage();

            comparePage.imgGameCover.Source = new BitmapImage(new Uri(game.SourceImage, UriKind.Relative));
            comparePage.txtGameName.Text = game.Title;
            comparePage.txtGameCompany.Text = game.Company;
            comparePage.txtGameDate.Text = $"Released on {game.ReleaseDate}";
            comparePage.txtGameAgeRating.Text = $"Age Rating: {game.AgeRating}+";
            comparePage.txtGamePrice.Text = game.Price == 0 ? "Free" : $"${game.Price}";

            comparePage.panelGameRating.Children.Clear();
            Label label = new Label();
            label.Content = "Rating : ";
            comparePage.panelGameRating.Children.Add(label);

            for (int i = 0; i < Math.Truncate(game.Rating); i++)
            {
                Image Star = new Image();
                Star.Width = 50;
                Star.Height = 50;
                Star.Source = new BitmapImage(new Uri("Icons/Star.png", UriKind.Relative));
                comparePage.panelGameRating.Children.Add(Star);
            }

            if (selectedGame.Rating - Math.Truncate(game.Rating) == 0.5)
            {
                Image HalfStar = new Image();
                HalfStar.Width = 50;
                HalfStar.Height = 50;
                HalfStar.Source = new BitmapImage(new Uri("Icons/Half Star.png", UriKind.Relative));
                comparePage.panelGameRating.Children.Add(HalfStar);
            }

            return comparePage;
        }

        #endregion

        #region UPDATE TEXT BLOCK

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set background to image
            if (searchBox.Text == "")
            {
                searchBox.Background = LoadImageBrush("Icons/Search.png");
            }

            //Set background to color
            else
            {
                searchBox.Background = LoadColorBrush("#000032");
            }
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set background to image
            if (userName.Text == "")
            {
                userName.Background = LoadImageBrush("Icons/Username.png");
            }

            //Set background to solid color
            else
            {
                userName.Background = LoadColorBrush("#ffffff");
            }
        }

        private void userPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            //Set background to image
            if (userPassword.Password == "")
            {
                userPassword.Background = LoadImageBrush("Icons/Password.png");
            }

            //Set background to solid color
            else
            {
                userPassword.Background = LoadColorBrush("#ffffff");
            }
        }

        public ImageBrush LoadImageBrush(string path)
        {
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
            image.AlignmentX = AlignmentX.Left;
            image.Stretch = Stretch.Fill;

            return image;
        }

        public SolidColorBrush LoadColorBrush(string hex)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString(hex);

            return brush;
        }

        #endregion
    }
}