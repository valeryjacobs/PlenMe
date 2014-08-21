//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// This template is intended for Windows Store apps that require a flat system of navigation. 

// The flat navigation pattern is often seen in games, browsers, or document creation apps, where the user can 
// move quickly between a small number of pages, tabs, or modes that all reside at the same hierarchical level. 

// The Flat navigation pattern (http://go.microsoft.com/fwlink/?LinkID=316374) is highlighted in 
// our App features, start to finish series (http://go.microsoft.com/fwlink/?LinkID=316376).
//
// For an overview of navigation design in Windows Store apps, see http://go.microsoft.com/fwlink/?LinkID=276817.
// For an introduction to the Navigation template, see http://go.microsoft.com/fwlink/?LinkId=268354.
// For an introduction to the Page Control template, see http://go.microsoft.com/fwlink/?LinkId=232511.
// For Avoiding common certification failures, see http://go.microsoft.com/fwlink/?LinkId=232506.

// NOTE: 
// For this sample, both home.html and page2.html are provided as standalone, localized HTML files.
// (The rich HTML content can be easily edited in an HTML or text editor.)
// If possible, avoid localizing app code and use standard string resources in .resjson files.
(function () {
    "use strict";

    // Create a namespace to make the data publicly
    // accessible. 
    var publicMembers =
        {
            //itemList: itemList

        };

    WinJS.Namespace.define("DataExample", publicMembers);

    // For Page Control methods, see WinJS.UI.Pages.IPageControlMembers interface at http://go.microsoft.com/fwlink/?LinkID=320074.
    WinJS.UI.Pages.define("/pages/main/main.html", {
        // First method called in the page life cycle.
        // The load method retrieves the information from the uri provided. 
        // The default implementation calls the low level fragment loader to 
        // retrieve the content. Override to take control of the loading process.
        // This result of the promise is received in the render method.
        //
        // Typically, this overload is used to generate your content 
        // through another mechanism such as a JavaScript templating library.
        //
        // For the purposes of this sample, we don't do anything.
        load: function (uri) {
            
        },

        // Second method called in the page lifecycle.
        // Initializes the page before content is set.
        //
        // The init method can return a promise that completes when init is finished.
        // This result of the promise is received in the processed method.
        // Typically, this method is used to initiate lengthy async processes.
        //
        // For the purposes of this sample, this method doesn't return anything.
        //init: function (element, options) {
        //},

        // Third method called in the page lifecycle. 
        // The render method receives the result from the promise in the load method. 
        // This result is inserted into the DOM.
        // You typically don't need to override this method.
        //
        // In this case, we do nothing.
        //render: function (element, options, loadResult) {
        //},

        // Fourth method called in the page lifecycle.
        // Initializes the page after the content is set.
        // The processed method is called after the render method is finished and
        // the system has called WinJS.UI.processAll on the page content.
        // The render method receives the result from the promise in the init method. 
        //
        // You can return a promise here, that will delay the rest of the
        // process until the promise completes.
        //
        // For the purposes of this sample, this method doesn't return anything.
        //processed: function (element) {
        //},

        // Fifth and final method called in the page lifecycle.
        // The ready method is called after the DOM is finalized
        // and controls are processed. 
        //
        // In most cases, this is the only method you'll need.
        ready: function (element, options) {
            // Process app resources.            
            // Replace text-only strings bound to properties through data-win-res.
            WinJS.Resources.processAll();

            ko.applyBindings(new BaseViewModel());

            var client = new WindowsAzure.MobileServiceClient("https://plenmejs.azure-mobile.net/", "iYHFrWKdowybaaDDqetIVSgXxVGRnU90");

            var noCachingFilter = function (request, next, callback) {
                if (request.type === 'GET' && !request.headers['If-Modified-Since']) {
                    request.headers['If-Modified-Since'] = 'Mon, 27 Mar 1972 00:00:00 GMT';
                }
                next(request, callback);
            };


            var todoTable = client.withFilter(noCachingFilter).getTable('TodoItem');
            var mindmapTable = client.withFilter(noCachingFilter).getTable('mindmap');
            var contentTable = client.withFilter(noCachingFilter).getTable('content');

            var todoItems = new WinJS.Binding.List();
            var contentItems = new WinJS.Binding.List();


            var insertTodoItem = function (todoItem) {
                // This code inserts a new TodoItem into the database. When the operation completes
                // and Mobile Services has assigned an id, the item is added to the Binding List
                todoTable.insert(todoItem).done(function (item) {
                    todoItems.push(item);
                });
            };


            var refreshContentItems = function () {

                contentTable.read()
                    .done(function (results) {
                        contentItems = new WinJS.Binding.List(results);
                        contentListItems.winControl.itemDataSource = contentItems.dataSource;
                    });
            };



            var refreshMindMap = function () {

                mindmapTable.where({ name: 'Azure' }).read()
                    .done(function (results) {
                        var mindmapContent = results[0].content;

                        var loadedTree = JSON.parse(mindmapContent);


                    });
            };







            //listItems.addEventListener("change", function (eventArgs) {
            //    var todoItem = eventArgs.target.dataContext.backingData;
            //    todoItem.complete = eventArgs.target.checked;
            //    updateCheckedTodoItem(todoItem);

            //});


            contentListItems.addEventListener("change", function (eventArgs) {
                var contentItem = eventArgs.target.dataContext.backingData;


            });


            //var userId = null;

            //var credential = null;
            //var vault = new Windows.Security.Credentials.PasswordVault();

            //var credential = null;
            //var vault = new Windows.Security.Credentials.PasswordVault();

            // Request authentication from Mobile Services using a Facebook login.
            //var login = function () {
            //    return new WinJS.Promise(function (complete) {
            //        client.login("microsoftaccount").done(function (results) {
            //            // Create a credential for the returned user.
            //            credential = new Windows.Security.Credentials
            //                .PasswordCredential("microsoftaccount", results.userId,
            //                results.mobileServiceAuthenticationToken);
            //            vault.add(credential);
            //            userId = results.userId; 

            //            refreshTodoItems();
            //            var message = "You are now logged in as: " + userId;
            //            var dialog = new Windows.UI.Popups.MessageDialog(message);
            //            dialog.showAsync().done(complete);
            //        }, function (error) {
            //            var dialog = new Windows.UI.Popups
            //                .MessageDialog("An error occurred during login", "Login Required");
            //            dialog.showAsync().done(complete);
            //        });
            //    });
            //}

            //var authenticate = function () {
            //    // Try to get a stored credential from the PasswordVault.                
            //    try {
            //        credential = vault.findAllByResource("microsoftaccount").getAt(0);
            //    }
            //    catch (error) {
            //        // This is expected when there's no stored credential.
            //    }

            //    if (credential) {
            //        // Set the user from the returned credential.   
            //        credential.retrievePassword();
            //        client.currentUser = {
            //            "userId": credential.userName,
            //            "mobileServiceAuthenticationToken": credential.password
            //        };

            //        // Try to return an item now to determine if the cached credential has expired.
            //        todoTable.take(1).read()
            //                    .done(function () {
            //                        refreshTodoItems();
            //                    }, function (error) {
            //                        if (error.request.status === 401) {
            //                            login(credential, vault).then(function () {
            //                                if (!credential) {

            //                                    // Authentication failed, try again.
            //                                    authenticate();
            //                                }
            //                            });
            //                        }
            //                    });
            //    } else {

            //        login().then(function () {
            //            if (!credential) {
            //                // Authentication failed, try again.
            //                authenticate();
            //            }
            //        });
            //    }
            //}

            //authenticate();

            // refreshTodoItems();
            refreshContentItems();
            refreshMindMap();

            // Retrieve the page 2 link and register the event handler. 
            // Don't use a button when the action is to go to another page; use a link instead. 
            // See Guidelines and checklist for buttons at http://go.microsoft.com/fwlink/?LinkID=313598
            // and Quickstart: Using single-page navigation at http://go.microsoft.com/fwlink/?LinkID=320288.
            WinJS.Utilities.query("a").listen("click", linkClickHandler, false);
        },

        // The unload method is called when leaving page.
        unload: function (element, options) {
        },

        // This function is called by _contextChanged event handler in navigator.js when 
        // a resource qualifier (language, scale, contrast, and so on) changes. 
        // The element passed is the root of this page. 
        updateResources: function (element, e) {
            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Language" || e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // Process app resources.            
                // Replace text-only strings bound to properties through data-win-res.
                WinJS.Resources.processAll(element);
                // Update app bar elements.
                WinJS.Resources.processAll(document.getElementById("appBar"));

                // Images with variants from the app package are automatically reloaded 
                // by the platform when a resource context qualifier has changed. 
                // However, this is not done for img elements on a page. 
                // Here, we ensure they are updated.

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }
        },

        // Here's a basic framework for customizing responses to changes in layout. 
        // Layout changes are managed in navigator.js and the window.onresize event handler. 
        // At run time, the onresize event calls this updateLayout function when the app switches 
        // between portrait, snapped, full screen, and filled view states.
        // See Handling view state at http://go.microsoft.com/fwlink/?LinkID=314251.
        // For this example, we use media attributes in default.css to demonstrate basic layouts.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            // TODO: Respond to changes in viewState.
        }
    });

    function handleError(error) {
        var text = error + (error.request ? ' - ' + error.request.status : '');
        console.log(text);
    }

    function linkClickHandler(eventInfo) {
        eventInfo.preventDefault();
        var link = eventInfo.target;
        WinJS.Navigation.navigate(link.href);
    }


    // Overall viewmodel for this screen, along with initial state
    function BaseViewModel() {
        var self = this;

        self.getContent = function (cid) {

            arr = jQuery.grep(vm.contentData.content, function (item) {
                return item.id === cid;
            });

            return $('<div/>').html(arr[0].c).text();
        };

        self.level = ko.observable(1);

        self.setParent = function (o) {
            if (o.c != undefined && o.c().length > 0) {
                for (n = 0; n < o.c().length; n++) {
                    o.c()[n].parent = o;
                    setParent(o.c()[n]);
                }
            }
        };

        self.up = function () {
            //komapping.fromJS(vm.selectedParent(), vm.dataModel);
            if (vm.selectedParent() && vm.selectedParent().parent) {
                vm.selectedParent(vm.selectedParent().parent);
                vm.selectedChild({});

                vm.level(vm.level() - 1);
            }
        };

        self.home = function () {

            vm.selectedParent(vm.dataModel);
            vm.selectedChild(null);
            vm.showChildren(false);
            vm.level(0);
        };

        self.showChildren = ko.observable(false);
        self.selectedChild = ko.observable(null);
        self.selectedNode = ko.observable(null);
        self.selectedParent = ko.observable(null);
        self.selectChild = function () {
            vm.selectedNode(this);
            vm.selectedChild(this);
            if (this.c && this.c().length > 0) {
                vm.showChildren(true);
                vm.level(vm.level() + 1);
            }
            else {
                vm.showChildren(false);
                //vm.selectedChild({});
            }
        };


        self.selectSubChild = function () {
            vm.selectedNode(this);
            //komapping.fromJS(this, vm.dataModel);
            if (this.c && this.c().length > 0) {
                vm.showChildren(false);
                vm.selectedParent(this);
                vm.level(vm.level() + 1);

                vm.selectedChild(null);

            }
        };

        self.handleError = function handleError(error) {
            var text = error + (error.request ? ' - ' + error.request.status : '');
            console.log(text);
        }

        self.activate = function () {

            setParent(self.dataModel);
            self.selectedParent(self.dataModel);
            self.selectedChild(null);
            self.selectedNode(null);
            self.showChildren(false);

            self.serviceClient = new WindowsAzure.MobileServiceClient('https://plenmejs.azure-mobile.net/', 'iYHFrWKdowybaaDDqetIVSgXxVGRnU90');
            self.todoItemTable = self.serviceClient.getTable('todoitem');
            self.mindmapTable = self.serviceClient.getTable('mindmap');
            self.contentTable = self.serviceClient.getTable('content');

            var query = self.todoItemTable.where({ complete: false });
            query.read().then(function (todoItems) {
                var listItems = $.map(todoItems, function (item) {
                    console.log(item.text);
                });
            }, handleError);

            query = self.mindmapTable.where({ name: 'Azure' });
            query.read().then(function (mindmap) {
                var itemcontainer = $.map(mindmap, function (item) {

                    var stringv = mindmap[0].content;
                    var loadedTree = JSON.parse(stringv);

                    self.dataModel = komapping.fromJS(loadedTree);



                    setParent(self.dataModel);

                    self.selectedChild(null);
                    self.selectedNode(null);
                    self.showChildren(false);

                    self.selectedParent(self.dataModel);


                });
            }, handleError);


            self.contentTable.read().then(function (content) {
                var itemcontainer = $.map(content, function (item) {
                    var stringv = item.content;
                    //console.log(stringv);
                });
            }, handleError);
        }




    }
})();
