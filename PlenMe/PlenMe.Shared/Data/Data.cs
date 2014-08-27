using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PlenMe.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;

namespace PlenMe.Data
{
    public class Data
    {
       

        private MobileServiceCollection<Content, Content> _items;
        private MobileServiceCollection<MindMap, MindMap> _mindmaps;
        private IMobileServiceTable<Content> _contentTable = App.MobileService.GetTable<Content>();
        private IMobileServiceTable<MindMap> _mindmapTable = App.MobileService.GetTable<MindMap>();

        public MindMap CurrentMindMap { get; set; }
        public Node RootNode { get; set; }
       

        private async void RefreshMindMap()
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
                CurrentMindMap = _mindmaps[0];
                dynamic json = JValue.Parse(_mindmaps[0].Content);

                //this.DefaultViewModel["MindMap"] = _mindmaps[0];

                RootNode = BuildTree(new Node(), json);
                //this.DefaultViewModel["RootNode"] = rootNode;
                //this.DefaultViewModel["ParentList"] = rootNode.Children;
                //this.DefaultViewModel["JSON"] = json;
                //this.DefaultViewModel["NodeContent"] = new Content { Data = "Test data" };
                //this.DefaultViewModel["SelectionStack"] = new Dictionary<int, Node>();

                //this.DefaultViewModel["ParentSelected"] = null;
                //this.DefaultViewModel["ChildSelected"] = null;
                //this.DefaultViewModel["SubChildSelected"] = null;

                //this.DefaultViewModel["MindMapLoading"] = false;
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
            if (source == null) return null;

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
    }
}
