using System;
using System.Collections.Generic;
using Dock.Model.Controls;
using ZenPlatform.IdeIntegration.Shared.Models;
using ZenPlatform.ThinClient.Models;

namespace ZenPlatform.ThinClient.ViewModels.Configuration
{
    public class ConfTreeViewModel : DocumentTab
    {
        public ConfTreeViewModel()
        {
            Items = new List<ConfItemModel>();

            Items.Add(
                new ConfItemModel()
                {
                    Name = "Root item",
                    ItemId = Guid.NewGuid(),
                    Childs =
                    {
                        new ConfItemModel()
                        {
                            Name = "Component item",
                            ItemId = Guid.NewGuid(),
                            Childs =
                            {
                                new ConfItemModel()
                                {
                                    Name = "Object",
                                    ItemId = Guid.NewGuid()
                                    
                                }
                            }
                        }
                    }
                });
        }

        public List<ConfItemModel> Items { get; set; }
    }
}