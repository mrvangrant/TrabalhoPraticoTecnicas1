﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheGreen.Game.Input;
using TheGreen.Game.Items;
using TheGreen.Game.Tiles;
using TheGreen.Game.UI.Containers;
using TheGreen.Game.UIComponents;

namespace TheGreen.Game.Inventory
{
    public class InventoryManager : UIComponentContainer
    {
        private Hotbar _hotbar;
        private Inventory _inventoryMenu;
        private DragItem _dragItem;
        private InventoryTileData _inventoryTileData;
        private Point _inventoryTileDataCoordinates;
        private Inventory _tileInventory;
        private GridContainer _activeMenu;
        public InventoryManager(int rows, int cols)
        {
            //Temporary inventory
            
            Item[] inventoryItems = new Item[rows * cols];

            for (int i = 0; i <= 6; i++)
            {
                Item item = ItemDatabase.InstantiateItemByID(i);
                item.Quantity = item.MaxStack;
                inventoryItems[i] = item;
            }
            Anchor = Anchor.TopLeft;
            _dragItem = new DragItem(Vector2.Zero);
            _inventoryMenu = new Inventory(cols, _dragItem, inventoryItems, margin: 2, position: new Vector2(20, 20));
            _hotbar = new Hotbar(cols, inventoryItems, margin: 2, position: new Vector2(20, 20));
            _activeMenu = _hotbar;
        }
        public override void HandleInput(InputEvent @event)
        {
            //Don't accept any input if the player is using an item to prevent any weird bugs if the player decides to press random buttons while using the item
            //TODO: maybe want to make this a little less dependent and spaghetti-ish
            if (Main.EntityManager.GetPlayer().ItemCollider.ItemActive) return;
            else if (@event.EventType == InputEventType.KeyDown && @event.InputButton == InputButton.Inventory)
            {
                SetInventoryOpen(!InventoryVisible());
                if (_dragItem.Item != null)
                {
                    Item itemDrop = _inventoryMenu.AddItem(_dragItem.Item);
                    _dragItem.Item = null;
                    if (itemDrop != null)
                    {
                        Main.EntityManager.AddItemDrop(itemDrop, Main.EntityManager.GetPlayer().Position);
                    }
                }
                InputManager.MarkInputAsHandled(@event);
            }
            else
            {
                _activeMenu.HandleInput(@event);
                _tileInventory?.HandleInput(@event);
            }
            if (InputManager.IsEventHandled(@event))
                return;
            //accept input for right mouse down if the inventory is visible
            else if (@event.InputButton == InputButton.RightMouse && @event.EventType == InputEventType.MouseButtonDown && InventoryVisible())
            {
                if (_dragItem.Item == null)
                    return;
                Main.EntityManager.AddItemDrop(_dragItem.Item, InputManager.GetMouseWorldPosition().ToVector2());
                _dragItem.Item = null;
                InputManager.MarkInputAsHandled(@event);
            }
        }

        public override void Update(double delta)
        {
            _activeMenu.Update(delta);
            _tileInventory?.Update(delta);
            _dragItem.Update(delta);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            _activeMenu.Draw(spritebatch);
            _tileInventory?.Draw(spritebatch);
            _dragItem.Draw(spritebatch);
        }
        
        public Item GetSelected()
        {
            if (InventoryVisible())
            {
                return _dragItem.Item;
            }
            return _hotbar.GetSelected();
        }

        public bool InventoryVisible()
        {
            return _activeMenu == _inventoryMenu;
        }

        public void DisplayTileInventory(InventoryTileData inventoryTileData, Point coordinates, Item[] items)
        {
            //TODO: close active tileinventory if there is one
            SetInventoryOpen(true);
            if (_inventoryTileData != null && coordinates != _inventoryTileDataCoordinates)
            {
                _inventoryTileData.CloseInventory(_inventoryTileDataCoordinates.X, _inventoryTileDataCoordinates.Y);
            }
            _tileInventory = new Inventory(inventoryTileData.Cols, _dragItem, items, margin: 2, position: _inventoryMenu.Position + new Vector2(_inventoryMenu.Size.X + 30, 0 ), itemSlotColor: Color.Crimson);
            _tileInventory.SetAnchorMatrix(AnchorMatrix);
            _inventoryTileDataCoordinates = coordinates;
            _inventoryTileData = inventoryTileData;
        }
        public void SetInventoryOpen(bool open)
        {
            _activeMenu = open ? _inventoryMenu : _hotbar;
            if (!InventoryVisible())
            {
                _tileInventory = null;
                
                if (_inventoryTileData != null)
                {
                    _inventoryTileData.CloseInventory(_inventoryTileDataCoordinates.X, _inventoryTileDataCoordinates.Y);
                    _inventoryTileData = null;
                }
            }
        }
        public bool UseSelected()
        {
            Item item = GetSelected();
            if (item == null)
                return false;
            bool itemUsed = item.UseItem();
            if (!item.Stackable || !itemUsed)
                return itemUsed;
            if (InventoryVisible())
            {
                _dragItem.Item.Quantity -= 1;
                if (_dragItem.Item.Quantity <= 0)
                    _dragItem.Item = null;
            }
            else
                _hotbar.SetSelectedQuantity(item.Quantity - 1);
            return itemUsed;
        }
        public Item AddItemToPlayerInventory(Item item)
        {
            return _inventoryMenu.AddItem(item);
        }
        //Why do they need to be updated? - future me, I forgor
        //Since the inventories are tied to this UIComponentContainer and not the UIManager, The anchor matrices will not be updated by default by the UIManager. They must be updated here so input handles correctly
        //For future reference in case I do something stupid like this again
        //Maybe add a list of UIComponentContainers as children, and the UIComponentContainer will auto-magically update it's children
        public override void SetAnchorMatrix(Matrix anchorMatrix)
        {
            base.SetAnchorMatrix(anchorMatrix);
            _inventoryMenu.SetAnchorMatrix(anchorMatrix);
            _hotbar.SetAnchorMatrix(anchorMatrix);
            _tileInventory?.SetAnchorMatrix(anchorMatrix);
        }
    }
}
