// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
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
using LuxBio.Library.Models;
using LuxBio.WindowsApp.ViewModel;
using LuxBio.WindowsApp.Controllers;
using System.ComponentModel;
using System.Threading;
using System.ServiceModel;

namespace LuxBio.WindowsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // View data
        private List<ChairViewModel> allChairs;
        private List<ChairViewModel> oldSelectedItems;

        // Controllers
        private MovieInfoController movieInfoCtr;
        private CinemaHallController cinemaHallCtr;
        private ReverseController reserveCtr;
        private MoviePlayTimeController moviePlayTimeCtr;

        // Data
        private List<MovieInfo> movies;
        private MovieInfo movie;
        private CinemaHall cinemaHall;
        private MoviePlayTime moviePlayTime;
        private IEnumerable<LuxBio.Library.Models.ExtraPropperties.Chair> statedChairs;
        private Customer customer;

        private readonly BackgroundWorker worker;
        private ComboBox seatCounter;

        private Window windowFirstLoading;

        /// <summary>
        /// Create and Initialize main window flow with controllers
        /// </summary>
        public MainWindow(Customer customer)
        {


            reserveCtr = new ReverseController();
            moviePlayTimeCtr = new MoviePlayTimeController();
            cinemaHallCtr = new CinemaHallController();
            movieInfoCtr = new MovieInfoController();

            allChairs = new List<ChairViewModel>();
            oldSelectedItems = new List<ChairViewModel>();

            // TODO refactor this
            this.customer = customer;

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            InitializeComponent();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Plz wait a little bit
            Thread.Sleep(3000);

            if (allChairs.Count() > 0)
            {
                this.moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);
                this.statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, moviePlayTime.CinemaHall);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    int seatCount = Convert.ToInt32(((ComboBoxItem)SeatCounter.SelectedValue).Content);

                    UpdateGUIChairsStates(seatCount, this.statedChairs.ToList());
                }));
            }

            var chairs = new List<Chair>();
            // Converting chair with extra to chair model
            foreach (var item in oldSelectedItems)
            {
                chairs.Add(new LuxBio.Library.Models.Chair()
                {
                    ID = item.ID,
                    Number = item.Number,
                    Row = item.Row
                });
            }

            reserveCtr.UpdateLockedChairs(moviePlayTime, chairs, customer);
        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Load movies in first tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovieTab_Initialized(object sender, EventArgs e)
        {
            // refresh movies list
            InitializeComponent();

            movies = movieInfoCtr.GetAllMovies().ToList();

            Movie.ItemsSource = movies;
        }

        /// <summary>
        /// Change tab to chairs choices after choosting movie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoosingMovie_Click(object sender, RoutedEventArgs e)
        {
            windowFirstLoading = new Window()
            {
                Title = string.Format("Loading - customer {0}", customer.ID),
                ShowInTaskbar = false,               // don't show the dialog on the taskbar
                Topmost = true,                      // ensure we're Always On Top
                ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                Owner = Application.Current.MainWindow,
                Width = 300,
                Height = 100,
                Content = "Opsætter sædevalg"
            };
            windowFirstLoading.Show();

            // Change tab
            allChairs.Clear(); // reset chairs view
            TabC.SelectedItem = SeatChoise;

            var curItem = (MovieInfo)((ListViewItem)Movie.ContainerFromElement((Button)sender)).Content;
            lblMovieName.Content = curItem.Title;

            // Update movie data
            movies = movieInfoCtr.GetAllMovies().ToList();
            movie = movieInfoCtr.GetAllMovies().First();
            SelectedMovie.DataContext = movie;

            // Update chairs, rows and cinema data
            cinemaHall = cinemaHallCtr.GetCinemaHall(1);
            moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);
            statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, cinemaHall);

            var chairViewModels = GeneratingViewChairs(statedChairs);
            lvSeatsContainer.ItemsSource = chairViewModels;

            seatCounter = SeatCounter; // Used to get combobox from another thread

            if (worker.IsBusy)
            {
                worker.CancelAsync();
                worker.RunWorkerAsync();
            } else
            {
                worker.RunWorkerAsync();
            }

            if (windowFirstLoading.IsVisible)
            {
                windowFirstLoading.Close();
            }
        }

        /// <summary>
        /// generating chairs to the cinema the user can choose
        /// </summary>
        /// <param name="statedChairs"></param>
        /// <returns></returns>
        private List<ChairViewModel> GeneratingViewChairs(IEnumerable<Library.Models.ExtraPropperties.Chair> statedChairs)
        {
            // Transforming Chair's to ViewModels
            foreach (var chair in statedChairs)
            {
                allChairs.Add(new ChairViewModel(chair));
            }

            decimal leftMargin = 0;
            decimal topMargin = 0;
            int ic = 0;
            foreach (var item in allChairs)
            {
                item.MarginLeft = leftMargin;
                item.MarginTop = topMargin;

                // Creating colors for chairs
                UpdateColors(item);

                leftMargin += 20;

                if (ic >= 9)
                {
                    leftMargin = 0;
                    topMargin += 20;
                    ic = 0;
                }
                else
                {
                    ic++;
                }
            }

            return allChairs;
        }

        private static void UpdateColors(ChairViewModel item)
        {
            switch (item.Available)
            {
                case Library.Models.ExtraPropperties.ChairAvailableType.Busy:
                    item.Color = "Red";
                    item.CheckBoxEnabled = false;
                    break;
                case Library.Models.ExtraPropperties.ChairAvailableType.Available:
                    item.Color = "Green";
                    break;
                case Library.Models.ExtraPropperties.ChairAvailableType.OverTime:
                    item.Color = "Blue";
                    item.CheckBoxEnabled = false;
                    break;
                case Library.Models.ExtraPropperties.ChairAvailableType.Locked:
                    item.Color = "Gray";
                    item.CheckBoxEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// Change view to reserve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            TabC.SelectedItem = Confirm;

            ComboBoxItem selected = (ComboBoxItem)SeatCounter.SelectedItem;
            string seats = selected.Content.ToString();

            lblMovie.Content = lblMovieName.Content.ToString();
            lblTickets.Content = seats;
        }

        /// <summary>
        /// change view to payment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            TabC.SelectedItem = Payment;
        }

        /// <summary>
        /// When the user pay with cash, create reserve for chairs for the cinema and movieplaytime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            Customer cus = new Customer() { ID = 1, Name ="Jørn", Phone = "12345678" };

           var playtime = moviePlayTimeCtr.GetMoviePlayTime(1);
           var hall = cinemaHallCtr.GetCinemaHall(1);
           var date = DateTime.Now;

           var chairs = new List<LuxBio.Library.Models.Chair>();
           var seats = lvSeatsContainer.SelectedItems;

            // Converting viewmodel to chair model
           foreach (ChairViewModel item in seats)
            {
                chairs.Add(new LuxBio.Library.Models.Chair()
                {
                    ID = item.ID,
                    Number = item.Number,
                    Row = item.Row
                });
            }

            reserveCtr.CreateReserve(playtime, chairs, cus, date);
        }

        // TODO Flyt noget af denne kode herfra til business logic for at undgå duplikering (refactor)
        private void lvSeatsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seatCount = Convert.ToInt32(((ComboBoxItem)SeatCounter.SelectedValue).Content);

            Window waitingWindow = new Window()
            {
                Title = "Loading",
                ShowInTaskbar = false,               // don't show the dialog on the taskbar
                Topmost = true,                      // ensure we're Always On Top
                ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                Owner = Application.Current.MainWindow,
                Width = 300,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = "Tjekker placeringerne. Vent venligst."
            };
            waitingWindow.Show();

            // Clear selected chairs
            lvSeatsContainer.SelectionChanged -= lvSeatsContainer_SelectionChanged;
            foreach (var oldSelectedItem in oldSelectedItems)
            {
                lvSeatsContainer.SelectedItems.Remove(oldSelectedItem);
            }
            lvSeatsContainer.SelectionChanged += lvSeatsContainer_SelectionChanged;

            // remove locked chairs
            ReleaseLockedChairs();
            // get new status
            statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, moviePlayTime.CinemaHall);

            oldSelectedItems.Clear();

            if (lvSeatsContainer.SelectedItems.Count > 0)
            {
                var firstitem = ((ChairViewModel)lvSeatsContainer.SelectedItems[0]);

                var myListBoxItem2 = (ListBoxItem)(lvSeatsContainer.ItemContainerGenerator.ContainerFromIndex(allChairs.FindIndex(c => c.ID == firstitem.ID)));
                var nameBox2 = FindDescendant<CheckBox>(myListBoxItem2);
                if (!nameBox2.IsHitTestVisible)
                {
                    lvSeatsContainer.SelectionChanged -= lvSeatsContainer_SelectionChanged;
                    nameBox2.IsChecked = false;
                    lvSeatsContainer.SelectionChanged += lvSeatsContainer_SelectionChanged;
                }

                if (nameBox2.IsHitTestVisible)
                {
                    var statedChairsFromRow = statedChairs.Where(c => firstitem.Row.Chairs.FirstOrDefault(rc => rc.ID == c.ID) != null).ToList();

                    var foundChairs = reserveCtr.FindChairsByFirstSelected(statedChairsFromRow, statedChairs.FirstOrDefault(c => c.ID == firstitem.ID), moviePlayTime, seatCount);
                    var chairs = new List<ChairViewModel>();
                    chairs = allChairs.Where(c => foundChairs.FirstOrDefault(fc => fc.ID == c.ID) != null).ToList();

                    var myListBoxItem3 = (ListBoxItem)(lvSeatsContainer.ItemContainerGenerator.ContainerFromIndex(allChairs.FindIndex(c => c.ID == firstitem.ID)));
                    var nameBox3 = FindDescendant<CheckBox>(myListBoxItem3);
                    lvSeatsContainer.SelectionChanged -= lvSeatsContainer_SelectionChanged;
                    nameBox3.IsChecked = false;
                    lvSeatsContainer.SelectionChanged += lvSeatsContainer_SelectionChanged;

                    // find chairs in listview and selected them
                    foreach (ChairViewModel item in chairs)
                    {
                        var myListBoxItem = (ListBoxItem)(lvSeatsContainer.ItemContainerGenerator.ContainerFromIndex(allChairs.FindIndex(c => c.ID == item.ID)));

                        var nameBox = FindDescendant<CheckBox>(myListBoxItem);
                        lvSeatsContainer.SelectionChanged -= lvSeatsContainer_SelectionChanged;
                        if (nameBox.IsHitTestVisible)
                        {
                            nameBox.IsChecked = true;
                        }
                        else
                        {
                            nameBox.IsChecked = false;
                        }
                        lvSeatsContainer.SelectionChanged += lvSeatsContainer_SelectionChanged;
                        oldSelectedItems.Add(item);
                    }

                    // lock chairs
                    var lockedChairs = new List<Chair>();
                    foreach (var item in oldSelectedItems)
                    {
                        lockedChairs.Add(new Chair()
                        {
                            ID = item.ID,
                            Number = item.Number,
                            Row = item.Row
                        });
                    }

                    try
                    {
                        reserveCtr.LockChairs(moviePlayTime, lockedChairs, customer);
                    }
                    catch (FaultException ex)
                    {
                        UpdateBestChoiceChairsSelections(moviePlayTime, seatCount);

                        Window window = new Window()
                        {
                            Title = "Advarsel",
                            ShowInTaskbar = false,               // don't show the dialog on the taskbar
                            Topmost = true,                      // ensure we're Always On Top
                            ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                            Owner = Application.Current.MainWindow,
                            Width = 300,
                            Height = 100,
                            Content = ex.Message
                        };

                        window.ShowDialog();
                    }
                }


                statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, moviePlayTime.CinemaHall);
                UpdateGUIChairsStates(seatCount, statedChairs.ToList());
            }

            waitingWindow.Close();
        }

        private void ReleaseLockedChairs()
        {
            //var oldSelectedLockedChairs = new List<Chair>();
            //foreach (var item in oldSelectedItems)
            //{
            //    oldSelectedLockedChairs.Add(new Chair()
            //    {
            //        ID = item.ID,
            //        Number = item.Number,
            //        Row = item.Row
            //    });
            //}
            //reserveCtr.ReleaseLocked(moviePlayTime, oldSelectedLockedChairs, customer);
        }

        /// <summary>
        /// Helper method from http://stackoverflow.com/questions/5181063/how-to-access-a-specific-item-in-a-listbox-with-datatemplate
        /// - Used to find checkbox in listview
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {
            // Check if this object is the specified type
            if (obj is T)
                return obj as T;

            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;

            // First check all the children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }

            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
        }

        /// <summary>
        /// Find stated chairs and search for the best choices and update the GUI listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSeatsContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (moviePlayTime != null && statedChairs.Count() > 0)
            {
                var seatsCount = Convert.ToInt32(((ComboBoxItem)SeatCounter.SelectedValue).Content);

                var bestChairs = reserveCtr.FindBestChairs(moviePlayTime, seatsCount);

                try
                {
                    reserveCtr.LockChairs(moviePlayTime, bestChairs, customer);
                }
                catch (FaultException ex)
                {
                    UpdateBestChoiceChairsSelections(moviePlayTime, seatsCount);

                    Window window = new Window()
                    {
                        Title = string.Format("Advarsel - customer {0}", customer.ID),
                        ShowInTaskbar = false,               // don't show the dialog on the taskbar
                        Topmost = true,                      // ensure we're Always On Top
                        ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                        Owner = Application.Current.MainWindow,
                        Width = 300,
                        Height = 100,
                        Content = ex.Message
                    };

                    window.ShowDialog();
                }

                UpdateGUIChairsSelection(seatsCount, bestChairs);
            }
        }

        /// <summary>
        /// Method used to update the GUI in listview
        /// </summary>
        /// <param name="seatCount"></param>
        /// <param name="chairs"></param>
        private void UpdateGUIChairsSelection(int seatCount, List<LuxBio.Library.Models.ExtraPropperties.Chair> chairs)
        {
            lvSeatsContainer.SelectedItems.Clear();

            if (lvSeatsContainer.ItemsSource != null)
            {
                allChairs.ForEach(c =>
                {
                    if (chairs.FirstOrDefault(chair => chair.ID == c.ID) != null)
                    {
                        var index = allChairs.FindIndex(ac => ac.ID == c.ID);
                        var myListBoxItem = (ListBoxItem)(lvSeatsContainer.ItemContainerGenerator.ContainerFromIndex(index));

                        var nameBox = FindDescendant<CheckBox>(myListBoxItem);
                        lvSeatsContainer.SelectionChanged -= lvSeatsContainer_SelectionChanged;
                        nameBox.IsChecked = true;
                        lvSeatsContainer.SelectionChanged += lvSeatsContainer_SelectionChanged;

                        oldSelectedItems.Add(c);
                    }
                });
                
                
                lvSeatsContainer.ItemsSource = lvSeatsContainer.ItemsSource;
                lvSeatsContainer.Items.Refresh();
            }
        }

        private void UpdateGUIChairsStates(int seatCount, List<LuxBio.Library.Models.ExtraPropperties.Chair> chairs)
        {
            if (lvSeatsContainer.ItemsSource != null)
            {
                foreach (ChairViewModel c in lvSeatsContainer.Items)
                {
                    var newUpdatedChair = chairs.FirstOrDefault(chair => chair.ID == c.ID);
                    if (newUpdatedChair != null && oldSelectedItems.FirstOrDefault(osi => osi.ID == c.ID) == null)
                    {
                        switch (newUpdatedChair.Available)
                        {
                            case Library.Models.ExtraPropperties.ChairAvailableType.Busy:
                                c.Color = "Red";
                                c.CheckBoxEnabled = false;
                                break;
                            case Library.Models.ExtraPropperties.ChairAvailableType.Available:
                                c.Color = "Green";
                                c.CheckBoxEnabled = true;
                                break;
                            case Library.Models.ExtraPropperties.ChairAvailableType.OverTime:
                                c.Color = "Blue";
                                c.CheckBoxEnabled = false;
                                break;
                            case Library.Models.ExtraPropperties.ChairAvailableType.Locked:
                                c.Color = "Gray";
                                c.CheckBoxEnabled = false;
                                break;
                        }
                    }
                }

                lvSeatsContainer.ItemsSource = allChairs;
                lvSeatsContainer.Items.Refresh();
            }
        }

        /// <summary>
        /// Update choices in GUI (method used by combobox for chairs count user wanted)
        /// </summary>
        /// <param name="statedChairs"></param>
        /// <param name="moviePlayTime"></param>
        /// <param name="seatsCount"></param>
        private void UpdateBestChoiceChairsSelections(MoviePlayTime moviePlayTime, int seatsCount)
        {
            statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, cinemaHall);
            UpdateGUIChairsStates(seatsCount, this.statedChairs.ToList());

            var bestChairs = reserveCtr.FindBestChairs(moviePlayTime, seatsCount);

            try
            {
                reserveCtr.LockChairs(moviePlayTime, bestChairs, customer);
            }
            catch (FaultException ex)
            {
                UpdateBestChoiceChairsSelections(moviePlayTime, seatsCount);

                Window window = new Window()
                {
                    Title = string.Format("Advarsel - Customer {0}", customer.ID),
                    ShowInTaskbar = false,               // don't show the dialog on the taskbar
                    Topmost = true,                      // ensure we're Always On Top
                    ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                    Owner = Application.Current.MainWindow,
                    Width = 300,
                    Height = 100,
                    Content = ex.Message
                };

                window.ShowDialog();
            }

            UpdateGUIChairsSelection(seatsCount, bestChairs);
        }

        /// <summary>
        /// """Event for combobox"""
        /// Update choices in GUI (method used by combobox for chairs count user wanted)
        /// </summary>
        /// <param name="statedChairs"></param>
        /// <param name="moviePlayTime"></param>
        /// <param name="seatsCount"></param>
        private void SeatCounter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allChairs.Count() > 0)
            {
                Window waitingWindow = new Window()
                {
                    Title = string.Format("Loading - Customer {0}", customer.ID),
                    ShowInTaskbar = false,               // don't show the dialog on the taskbar
                    Topmost = true,                      // ensure we're Always On Top
                    ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                    Owner = Application.Current.MainWindow,
                    Width = 300,
                    Height = 100,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = "Finder placering. Vent venligst."
                };
                waitingWindow.Show();

                ReleaseLockedChairs();

                statedChairs = reserveCtr.GetAllChairsState(moviePlayTime, cinemaHall);
                var seatCount = Convert.ToInt32(((ComboBoxItem)SeatCounter.SelectedValue).Content);

                UpdateBestChoiceChairsSelections(moviePlayTime, seatCount);

                waitingWindow.Close();
            }
        }

    }
}
