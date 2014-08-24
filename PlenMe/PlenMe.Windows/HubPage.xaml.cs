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

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace PlenMe
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public Node _rootNode;
        public MindMap _currentMindMap;

        private MobileServiceCollection<Content, Content> items;
        private MobileServiceCollection<MindMap, MindMap> mindmaps;
        private IMobileServiceTable<Content> contentTable = App.MobileService.GetTable<Content>();
        private IMobileServiceTable<MindMap> mindmapTable = App.MobileService.GetTable<MindMap>();

        //private MobileServiceCollection<TodoItem, TodoItem> items;
        //private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        private async void RefreshContentItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await contentTable
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {

                this.DefaultViewModel["ContentItems"] = items.ToList();

                WebContentTemplate.HTML = ((List<Content>)this.DefaultViewModel["ContentItems"]).Where(x => x.Id == "13").Single().Data;
                // ListItems.ItemsSource = items;
                //this.ButtonSave.IsEnabled = true;
            }
        }

        private async void RefreshMindMap()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                mindmaps = await mindmapTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading mindmap data.").ShowAsync();
            }
            else
            {
                _currentMindMap = mindmaps[0];
                dynamic json = JValue.Parse(mindmaps[0].Content);

                this.DefaultViewModel["MindMap"] = mindmaps[0];

                var rootNode = new Node();
                var tree = BuildTree(rootNode, json);

                _rootNode = tree;
                this.DefaultViewModel["RootNode"] = _rootNode;
                this.DefaultViewModel["ParentList"] = _rootNode.Children;
                this.DefaultViewModel["JSON"] = json;
                this.DefaultViewModel["NodeContent"] = new Content { Data = "Test data" };
                this.DefaultViewModel["SelectionStack"] = new Dictionary<int, Node>();

                this.DefaultViewModel["ParentSelected"] = null;
                this.DefaultViewModel["ChildSelected"] = null;
                this.DefaultViewModel["SubChildSelected"] = null;


            }
        }


        private static Node BuildTree(Node node, dynamic source)
        {
            var newNode = new Node
            {
                Id = source.id,
                ContentId = source.cid,
                Description = source.d,
                Title = source.n,
                Parent = node
            };

            if (source.c != null)
            {
                foreach (dynamic d in source.c)
                {
                    newNode.Children.Add(BuildTree(newNode, d));
                }
            }

            return newNode;
        }

        private static JsonNode BuildJSON(JsonNode jsonNode, Node source)
        {
            var newNode = new JsonNode
            {
                id = source.Id,
                cid = source.ContentId,
                d = source.Description,
                n = source.Title
            };

            if (source.Children != null)
            {
                foreach (Node n in source.Children)
                {
                    newNode.c.Add(BuildJSON(newNode, n));
                }
            }

            return newNode;
        }


        private void EditNode(object sender, RoutedEventArgs e)
        {
            if (!editNodePopup.IsOpen)
            {
                editNodePopup.HorizontalOffset = Window.Current.Bounds.Width / 2;
                editNodePopup.VerticalOffset = Window.Current.Bounds.Height - 500;
                editNodePopup.IsOpen = true;
            }
        }

        private void DeleteNode(object sender, RoutedEventArgs e)
        {
           var nodeToBeRemoved =  this.DefaultViewModel["SelectedNode"] as Node;

           nodeToBeRemoved.Parent.Children.Remove(nodeToBeRemoved);
        }

        private void AddSiblingNode(object sender, RoutedEventArgs e)
        {
            AddNode(false);
        }

        private void AddChildNode(object sender, RoutedEventArgs e)
        {
            AddNode(true);
        }


        private void Up(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["SubChildList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Children;
            this.DefaultViewModel["ChildList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Parent.Children;
            this.DefaultViewModel["ParentList"] = ((Node)this.DefaultViewModel["SubChildSelected"]).Parent.Parent.Parent.Children;
        }

        private void Save(object sender, RoutedEventArgs e)
        {

            var jsonRootNode = new JsonNode();
            var jsonObject = BuildJSON(jsonRootNode,_rootNode);
            
            _currentMindMap.Content = JsonConvert.SerializeObject(jsonObject);
            //await App.MobileService.GetTable<Item>().InsertAsync(item);
            UpdateMindMap(_currentMindMap);
        }

        private async void UpdateMindMap(MindMap mindmap)
        {
            await mindmapTable.UpdateAsync(mindmap);
        }

   

        private void AddNode(bool addAsChild)
        {
            Node target = null;
            if (this.DefaultViewModel.ContainsKey("SelectedNode") && this.DefaultViewModel["SelectedNode"] != null)
                target = (Node)this.DefaultViewModel["SelectedNode"];

            if (target == null)
                target = _rootNode;

            var newNode = new Node { };
            if (addAsChild)
            {
                newNode.Parent = target;
                target.Children.Add(newNode);
            }
            else
            {
                if (target.Parent == null)
                    return;

                newNode.Parent = target.Parent;

                target.Parent.Children.Add(newNode);
            }

            this.DefaultViewModel["SelectedNode"] = newNode;

            if (!editNodePopup.IsOpen)
            {
                editNodePopup.HorizontalOffset = Window.Current.Bounds.Width / 2;
                editNodePopup.VerticalOffset = Window.Current.Bounds.Height - 500;
                editNodePopup.IsOpen = true;
            }
        }


        private void NodeEditOK_Click(object sender, RoutedEventArgs e)
        {
            // ((Node)this.DefaultViewModel["SelectedNode"]).Parent.Children.Add(new Node { Title="Test", Description="Tesst" , ContentId = "1"});

            //// this.DefaultViewModel["ParentList"] = _rootNode.Children;
            editNodePopup.IsOpen = false;
        }

        private void ShowFlyoutPopup(object sender, RoutedEventArgs e)
        {
            if (!logincontrol1.IsOpen)
            {
                RootPopupBorder.Width = 646;
                logincontrol1.HorizontalOffset = Window.Current.Bounds.Width / 2 - (RootPopupBorder.Width / 2);
                logincontrol1.VerticalOffset = Window.Current.Bounds.Height / 2 - (RootPopupBorder.Height / 2);
                logincontrol1.IsOpen = true;
            }
        }




        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
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
            RefreshMindMap();
            RefreshContentItems();
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;

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

                var selectedNode = ((Node)e.AddedItems[0]);
                if (selectedNode == null) return;
             
                this.DefaultViewModel["SelectedNode"] = selectedNode;
                this.DefaultViewModel["ParentSelected"] = selectedNode;

                ((Dictionary<int, Node>)this.DefaultViewModel["SelectionStack"])[0] = selectedNode;
                this.DefaultViewModel["ChildList"] = selectedNode.Children;

                if (selectedNode.Children != null && selectedNode.Children.Count > 0)
                {
                    this.DefaultViewModel["SubChildList"] = selectedNode.Children[0].Children;
                }
                else
                {
                    this.DefaultViewModel["SubChildList"] = null;
                }

                this.DefaultViewModel["NodeContent"] = ((List<Content>)this.DefaultViewModel["ContentItems"]).Where(x => x.Id == selectedNode.ContentId).Single();   
        }

        private void Child_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var selectedNode = ((Node)e.AddedItems[0]);
            if (selectedNode == null) return;

          
            this.DefaultViewModel["SelectedNode"] = selectedNode;
            this.DefaultViewModel["ChildSelected"] = selectedNode;
            ((Dictionary<int, Node>)this.DefaultViewModel["SelectionStack"])[1] = selectedNode;
            this.DefaultViewModel["SubChildList"] = selectedNode.Children;
            this.DefaultViewModel["NodeContent"] = ((List<Content>)this.DefaultViewModel["ContentItems"]).Where(x => x.Id == selectedNode.ContentId).Single();
        }


        private void SubChild_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var selectedNode = ((Node)e.AddedItems[0]);
            if (selectedNode == null) return;

            this.DefaultViewModel["SelectedNode"] = selectedNode;
            this.DefaultViewModel["SubChildSelected"] = selectedNode;

            ((Dictionary<int, Node>)this.DefaultViewModel["SelectionStack"])[2] = selectedNode;

            if (selectedNode.Children != null && selectedNode.Children.Count > 0)
            {
                this.DefaultViewModel["ParentSelected"] = this.DefaultViewModel["ChildSelected"];
                this.DefaultViewModel["ChildSelected"] = this.DefaultViewModel["SubChildSelected"];


                if (selectedNode.Parent.Parent != null)
                {
                    this.DefaultViewModel["ParentList"] = selectedNode.Parent.Parent.Children;
                }
                this.DefaultViewModel["ChildList"] = selectedNode.Parent.Children;
                this.DefaultViewModel["SubChildList"] = selectedNode.Children;
                this.DefaultViewModel["NodeContent"] = ((List<Content>)this.DefaultViewModel["ContentItems"]).Where(x => x.Id == selectedNode.ContentId).Single();
            }
        }
    }
}
