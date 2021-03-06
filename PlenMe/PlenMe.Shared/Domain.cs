﻿using Microsoft.WindowsAzure.MobileServices;
using PlenMe.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Core;

namespace PlenMe
{
    public class Domain : INotifyPropertyChanged
    {

        private IMobileServiceTable<Content> _contentTable = MobileService.GetTable<Content>();
        private IMobileServiceTable<MindMap> _mindmapTable = MobileService.GetTable<MindMap>();

        private MobileServiceCollection<Content, Content> _items;
        private MobileServiceCollection<MindMap, MindMap> _mindmaps;

        private ObservableCollection<Node> _parentList;
        private ObservableCollection<Node> _childList;
        private ObservableCollection<Node> _subChildList;

        private Node _rootNode;
        private Node _selectedNode;
        private Content _selectedNodeContent;
        private MindMap _currentMindMap;

        private static MobileServiceClient MobileService = new MobileServiceClient("https://plenmejs.azure-mobile.net/", "iYHFrWKdowybaaDDqetIVSgXxVGRnU90");

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddChildNode()
        {
            CreateNode(true, SelectedNode);
        }

        public Content AddContent()
        {
            Content instance = new Content
            {
                Id = Guid.NewGuid().ToString()
            };
            _contentTable.InsertAsync(instance);
            _items.Add(instance);
            return instance;
        }

        public void AddSiblingNode()
        {
            CreateNode(false, SelectedNode);
        }

        private JsonNode BuildJSON(JsonNode jsonNode, Node source)
        {
            if (source == null)
            {
                return null;
            }
            JsonNode node = new JsonNode
            {
                id = source.Id,
                cid = source.ContentId,
                d = source.Description,
                n = source.Title
            };
            if (source.Children != null)
            {
                foreach (Node node2 in source.Children)
                {
                    node.c.Add(BuildJSON(node, node2));
                }
            }
            return node;
        }

        private Node BuildTree(Node node, dynamic source)
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

        private void CreateNode(bool addAsChild, Node parentNode = null)
        {
            if (parentNode == null)
            {
                parentNode = RootNode;
            }
            Node node = new Node();
            if (addAsChild)
            {
                node.Parent = parentNode;
                parentNode.Children.Add(node);
            }
            else
            {
                if (parentNode.Parent == null)
                {
                    return;
                }
                node.Parent = parentNode.Parent;
                parentNode.Parent.Children.Insert(parentNode.Parent.Children.IndexOf(parentNode) + 1, node);
            }
            SelectedNode = node;
        }

        public void DeleteNode()
        {
            SelectedNode.Parent.Children.Remove(SelectedNode);
        }

        public void DeleteNode(Node targetNode)
        {
            targetNode.Parent.Children.Remove(targetNode);
        }

        public void EditContent()
        {
            if ((SelectedNode.ContentId == null) || (SelectedNode.ContentId == "1"))
            {
                SelectedNode.ContentId = App.Domain.AddContent().Id;
            }
        }

        public async void Init()
        {
            await InitContentItems();
            await InitMindMaps();
        }


        private async Task InitContentItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                _items = await _contentTable.Take(500).ToCollectionAsync();
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
                WebContentTemplate.HTML = _items.Where(x => x.Id == "13").Single().Data;
            }
        }


        private async Task InitMindMaps()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                _mindmaps = await _mindmapTable.ToCollectionAsync();
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
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        CurrentMindMap = _mindmaps[0];
                        dynamic json = JValue.Parse(_mindmaps[0].Content);

                        RootNode = BuildTree(new Node(), json);
                        ParentList = RootNode.Children;
                    });
            }
        }

        public void MoveOrderDown()
        {
            MoveOrderDown(SelectedNode);
        }

        public void MoveOrderDown(Node targetNode)
        {
            int index = targetNode.Parent.Children.IndexOf(targetNode);
            if (index < (Enumerable.Count<Node>((IEnumerable<Node>)targetNode.Parent.Children) - 1))
            {
                targetNode.Parent.Children.Move(index, index + 1);
            }
        }

        public void MoveOrderUp()
        {
            MoveOrderUp(SelectedNode);
        }

        public void MoveOrderUp(Node targetNode)
        {
            int index = targetNode.Parent.Children.IndexOf(targetNode);
            if (index > 0)
            {
                targetNode.Parent.Children.Move(index, index - 1);
            }
        }

        public void MoveUp()
        {
            if (SelectedNode.Parent.Parent == null) return;

            SelectedNode.Parent.Parent.Children.Add(SelectedNode);
            SelectedNode.Parent.Children.Remove(SelectedNode);
            SelectedNode.Parent = SelectedNode.Parent.Parent;
        }

        public void NavigateUp()
        {
            SubChildList = SelectedSubChild.Parent.Children;
            ChildList = SelectedSubChild.Parent.Parent.Children;
            ParentList = SelectedSubChild.Parent.Parent.Parent.Children;
            SelectedSubChild = SelectedSubChild.Parent;
            SelectedChild = SelectedChild.Parent;
            SelectedParent = SelectedParent.Parent;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void Save()
        {
            JsonNode jsonNode = new JsonNode();
            JsonNode jsonObject = BuildJSON(jsonNode, RootNode);
            MindMap instance = new MindMap
            {
                Name = CurrentMindMap.Name + "_BACKUP",
                Id = Guid.NewGuid().ToString(),
                Content = CurrentMindMap.Content
            };
            await _mindmapTable.InsertAsync(instance);
            CurrentMindMap.Content = JsonConvert.SerializeObject(jsonObject);
            UpdateMindMap(CurrentMindMap);
        }

        public void SelectChild(Node selectedChild)
        {
            SelectedNode = selectedChild;
            SelectedChild = SelectedNode;
            SubChildList = SelectedChild.Children;
            SetContent();
        }

        public void SelectParent(Node selectedParent)
        {
            SelectedNode = selectedParent;
            SelectedParent = SelectedNode;
            ChildList = SelectedNode.Children;
            if ((SelectedNode.Children != null) && (SelectedNode.Children.Count() > 0))
            {
                SubChildList = SelectedNode.Children[0].Children;
            }
            else
            {
                SubChildList = null;
            }
            SetContent();
        }

        public void SelectSubChild(Node selectedSubChild)
        {
            SelectedNode = selectedSubChild;
            SelectedSubChild = SelectedNode;
            if ((SelectedNode.Children != null) && (SelectedNode.Children.Count() > 0))
            {
                SelectedParent = SelectedChild;
                SelectedChild = selectedSubChild;
                if (SelectedNode.Parent.Parent != null)
                {
                    ParentList = SelectedNode.Parent.Parent.Children;
                }
                ChildList = SelectedNode.Parent.Children;
                SubChildList = SelectedNode.Children;
            }
            SetContent();
        }

        public void SetContent()
        {
            Func<Content, bool> func = null;
            if (Enumerable.Any<Content>((IEnumerable<Content>)_items, delegate(Content x)
            {
                return x.Id == SelectedNode.ContentId;
            }))
            {
                if (func == null)
                {
                    func = delegate(Content x)
                    {
                        return x.Id == SelectedNode.ContentId;
                    };
                }
                SelectedNodeContent = Enumerable.Single<Content>(Enumerable.Where<Content>((IEnumerable<Content>)_items, func));
            }
            else
            {
                Content content = new Content
                {
                    Id = SelectedNode.ContentId,
                    Data = "Content with ID " + SelectedNode.ContentId + " not found."
                };
                SelectedNodeContent = content;
            }
        }

        public async void UpdateContent(string newContent)
        {
            SelectedNodeContent.Data = newContent;
            await _contentTable.UpdateAsync(SelectedNodeContent);
        }

        private async void UpdateMindMap(MindMap mindmap)
        {
            await _mindmapTable.UpdateAsync(mindmap);
        }

        public ObservableCollection<Node> ChildList
        {
            get
            {
                return _childList;
            }
            set
            {
                _childList = value;
                NotifyPropertyChanged("ChildList");
            }
        }

        public MindMap CurrentMindMap
        {
            get
            {
                return _currentMindMap;
            }
            set
            {
                _currentMindMap = value;
                NotifyPropertyChanged("CurrentMindMap");
            }
        }

        public string InitializationStatus { get; set; }

        public bool Initializing { get; set; }

        public bool IsChildSelected { get; set; }

        public bool IsParentSelected { get; set; }

        public bool IsSubChildSelected { get; set; }

        public ObservableCollection<Node> ParentList
        {
            get
            {
                return _parentList;
            }
            set
            {
                _parentList = value;
                NotifyPropertyChanged("ParentList");
            }
        }

        public Node RootNode
        {
            get
            {
                return _rootNode;
            }
            set
            {
                _rootNode = value;
                NotifyPropertyChanged("RootNode");
            }
        }

        private double _zoom;
        public double Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                NotifyPropertyChanged("Zoom");
            }
        }

        public Node SelectedChild { get; set; }

        public Node SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
                NotifyPropertyChanged("SelectedNode");
            }
        }

        public Content SelectedNodeContent
        {
            get
            {
                return _selectedNodeContent;
            }
            set
            {
                _selectedNodeContent = value;
                NotifyPropertyChanged("SelectedNodeContent");
            }
        }

        public Node SelectedParent { get; set; }

        public Node SelectedSubChild { get; set; }

        public ObservableCollection<Node> SubChildList
        {
            get
            {
                return _subChildList;
            }
            set
            {
                _subChildList = value;
                NotifyPropertyChanged("SubChildList");
            }
        }

        // initcontentitems()

        // props
        // selectedparent etc

        // editnode

        // move up

        // na toevoegen async commando gebruiken om prop changed property op de ui thread te updaten

        // artikel over binding 

        //selectednode

        //parentlist etc

        //    movemup

        //    save




    }
}
