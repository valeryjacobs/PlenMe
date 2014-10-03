using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlenMe.Data;
using PlenMe.Common;
using Microsoft.WindowsAzure.MobileServices;
using PlenMe.DataModel;
using Windows.UI.Popups;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PlenMe.Helpers;
using Windows.System;
using PlenMe.Helpers;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace PlenMe
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private StreamUriWinRTResolver streamResolver;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Domain domain;

        //private MobileServiceCollection<TodoItem, TodoItem> items;
        //private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        public HubPage()
        {
            domain = App.Domain;
            streamResolver = new StreamUriWinRTResolver();
            this.InitializeComponent();
             
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        void Hub_Loaded(object sender, RoutedEventArgs e)
        {
           

         }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {

            if (e.OriginalSource is TextBox || e.OriginalSource is WebView)
            {
                base.OnKeyDown(e);
            }
            else
            {
                e.Handled = true;
                switch (e.Key)
                {
                    case VirtualKey.C:
                        AddNode(true);
                        break;
                    case VirtualKey.S:
                        AddNode(false);
                        break;
                    case VirtualKey.Delete:
                        App.Domain.DeleteNode();
                        break;
                    case VirtualKey.N:
                        EditNode();
                        break;
                    case VirtualKey.E:
                        EditContent();
                        break;
                    case VirtualKey.Up:
                        if (e.Key == VirtualKey.Control)
                            App.Domain.MoveOrderUp();
                        break;
                    case VirtualKey.Down:
                        if (e.Key == VirtualKey.Control)
                            App.Domain.MoveOrderDown();
                        break;
                    case VirtualKey.Left:
                        if (e.Key == VirtualKey.Control)
                            App.Domain.MoveUp();
                        break;
                }
            }
        }

        private void Bold(object sender, RoutedEventArgs e)
        {
           ControlLocator.ContentEditor.InvokeScript("CallCommand", new string[] { "Bold" });
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            ControlLocator.ContentView.InvokeScriptAsync("SetZoom", new string[] { "2" });
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            ControlLocator.ContentView.InvokeScriptAsync("SetZoom", new string[] { "0.1" });
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ControlLocator.ContentView == null) return;
            ControlLocator.ContentView.InvokeScriptAsync("SetZoom", new string[] { (e.NewValue / 100).ToString() });
        }

        private void ZoomSliderEditor_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ControlLocator.ContentEditorReady)
                ControlLocator.ContentEditor.InvokeScript("SetZoom", new string[] { (e.NewValue / 100).ToString() });
        }

        private void EditContent(object sender, RoutedEventArgs e)
        {
            EditContent();
        }

        private void EditContent()
        {
            editSection.Width = Window.Current.Bounds.Width;
            Domain.EditContent();
            //Hub.ScrollToSection(editSection);

            //if (!editContentPopup.IsOpen)
            //{
            //    ControlLocator.ContentView.Width = Window.Current.Bounds.Width - 100;
            //    ControlLocator.ContentView.Height = Window.Current.Bounds.Height - 100;
            //    editContentPopup.HorizontalOffset = ((Window.Current.Bounds.Width / 2) * -1) + 200;
            //    editContentPopup.VerticalOffset = ((Window.Current.Bounds.Height / 2) * -1) + 230;
            //    ControlLocator.ContentView.InvokeScriptAsync("SetZoom", new string[] { "180" });
            //    editContentPopup.IsOpen = true;
            //}
        }        

        private async void Confirm(object sender, RoutedEventArgs e)
        {
            string newContent = await ControlLocator.ContentEditor.InvokeScriptAsync("GetContent", null);

            App.Domain.UpdateContent(newContent);
        }

        private void EditNode(object sender, RoutedEventArgs e)
        {
            EditNode();
        }

        private void EditNode()
        {
            if (!editNodePopup.IsOpen)
            {
                editNodePopup.HorizontalOffset = (Window.Current.Bounds.Width / 2) - (editNodePopup.ActualWidth / 2);
                editNodePopup.VerticalOffset = (Window.Current.Bounds.Height / 2) - (editNodePopup.ActualHeight / 2);
                editNodePopup.IsOpen = true;
                nodeTitle.Focus(FocusState.Keyboard);
            }
        }


        private void DeleteNode(object sender, RoutedEventArgs e)
        {
            App.Domain.DeleteNode();
        }


        private void AddSiblingNode(object sender, RoutedEventArgs e)
        {
            AddNode(false);
        }

        private void AddChildNode(object sender, RoutedEventArgs e)
        {
            AddNode(true);
        }

        private void AddNode(bool asaChild)
        {
            if (asaChild)
            {
                App.Domain.AddChildNode();
            }
            else
            {
                App.Domain.AddSiblingNode();
            }

            if (!editNodePopup.IsOpen)
            {
                editNodePopup.HorizontalOffset = (Window.Current.Bounds.Width / 2) - (editNodePopup.ActualWidth / 2);
                editNodePopup.VerticalOffset = (Window.Current.Bounds.Height / 2) - (editNodePopup.ActualHeight / 2);
                editNodePopup.IsOpen = true;
                nodeTitle.Focus(FocusState.Keyboard);
            }
        }

        private void AddChildNode()
        {
            App.Domain.AddChildNode();

            if (!editNodePopup.IsOpen)
            {
                editNodePopup.HorizontalOffset = (Window.Current.Bounds.Width / 2) - (editNodePopup.ActualWidth / 2);
                editNodePopup.VerticalOffset = (Window.Current.Bounds.Height / 2) - (editNodePopup.ActualHeight / 2);
                editNodePopup.IsOpen = true;
                nodeTitle.Focus(FocusState.Keyboard);
            }
        }

        private void Up(object sender, RoutedEventArgs e)
        {
            App.Domain.NavigateUp();
        }

        private void MoveOrderUp(object sender, RoutedEventArgs e)
        {
            App.Domain.MoveOrderUp();
        }

        private void MoveOrderDown(object sender, RoutedEventArgs e)
        {

            App.Domain.MoveOrderDown();
        }

        private void MoveUp(object sender, RoutedEventArgs e)
        {
            App.Domain.MoveUp();
        }

        private void Up()
        {
            //    if (((Node)this.DefaultViewModel["ParentSelected"]).Parent == App.Domain.RootNode) return;

            //    this.DefaultViewModel["SubChildList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Children;
            //    this.DefaultViewModel["ChildList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Parent.Children;
            //    this.DefaultViewModel["ParentList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Parent.Parent.Children;

            //    this.DefaultViewModel["SubChildSelected"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent;
            //    this.DefaultViewModel["ChildSelected"] = ((Node)this.DefaultViewModel["ChildSelected"]).Parent;
            //    this.DefaultViewModel["ParentSelected"] = ((Node)this.DefaultViewModel["ParentSelected"]).Parent;
        }


        private void Save(object sender, RoutedEventArgs e)
        {
            App.Domain.Save();
        }

        private void NodeEditOK_Click(object sender, RoutedEventArgs e)
        {
            // ((Node)this.DefaultViewModel["SelectedNode"]).Parent.Children.Add(new Node { Title="Test", Description="Tesst" , ContentId = "1"});

            //// this.DefaultViewModel["ParentList"] = _rootNode.Children;
            editNodePopup.IsOpen = false;
        }

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }



        public Domain Domain
        {
            get { return this.domain; }
        }



        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
           
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }


        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          //  ControlLocator.ContentEditor = contentEditor;
            
            ControlLocator.StreamResolver = new StreamUriWinRTResolver();

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Parent_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            App.Domain.SelectParent((Node)e.AddedItems[0]);
        }


        private void Child_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            App.Domain.SelectChild((Node)e.AddedItems[0]);
        }


        private void SubChild_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            App.Domain.SelectSubChild((Node)e.AddedItems[0]);
        }
    }
}
