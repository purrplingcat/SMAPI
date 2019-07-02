using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace StardewModdingAPI.Mods.VirtualKeyboard
{
    class VirtualToggle : IClickableMenu
    {
        private readonly IModHelper helper;
        private readonly IMonitor Monitor;

        private bool enabled = false;
        private ClickableTextureComponent virtualToggleButton;

        private List<KeyButton> keyboard = new List<KeyButton>();
        private ModConfig modConfig;
        private Texture2D texture;

        public VirtualToggle(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.helper = helper;
            this.texture = this.helper.Content.Load<Texture2D>("assets/togglebutton.png", ContentSource.ModFolder);
            this.virtualToggleButton = new ClickableTextureComponent(new Rectangle(Game1.virtualJoypad.buttonToggleJoypad.bounds.X + 36, 12, 64, 64), this.texture, new Rectangle(0, 0, 16, 16), 5.75f, false);

            this.modConfig = helper.ReadConfig<ModConfig>();
            for (int i = 0; i < this.modConfig.buttons.Length; i++)
            {
                this.keyboard.Add(new KeyButton(helper, this.modConfig.buttons[i], this.Monitor));
            }
            helper.WriteConfig(this.modConfig);
            this.helper.Events.Display.RenderingHud += this.OnRenderingHUD;
            this.helper.Events.Input.ButtonPressed += this.VirtualToggleButtonPressed;
            this.helper.Events.Input.ButtonReleased += this.VirtualToggleButtonReleased;
        }

        private void VirtualToggleButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!this.enabled && this.shouldTrigger())
            {
                this.hiddenKeys(true, false);
            }
            else if (this.enabled && this.shouldTrigger())
            {
                this.hiddenKeys(false, true);
                if (Game1.activeClickableMenu is IClickableMenu menu)
                {
                    menu.exitThisMenu();
                }
            }
        }

        private void hiddenKeys(bool enabled, bool hidden)
        {
            this.enabled = enabled;
            foreach (var keys in this.keyboard)
            {
                keys.hidden = hidden;
            }
        }

        private bool shouldTrigger()
        {
            int nativeZoomLevel = (int)Game1.NativeZoomLevel;
            int x1;
            int y1;
            if (nativeZoomLevel != 0)
            {
                x1 = Mouse.GetState().X / nativeZoomLevel;
                y1 = Mouse.GetState().Y / nativeZoomLevel;
            }
            else
            {
                x1 = (int)((float)Mouse.GetState().X / Game1.NativeZoomLevel);
                y1 = (int)((float)Mouse.GetState().Y / Game1.NativeZoomLevel);
            }

            if (this.virtualToggleButton.containsPoint(x1, y1))
            {
                Toolbar.toolbarPressed = true;
                return true;
            }
            return false;
        }

        private void VirtualToggleButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
        }

        private void OnRenderingHUD(object sender, EventArgs e)
        {
            if (Game1.options.verticalToolbar)
                this.virtualToggleButton.bounds.X = Game1.toolbarPaddingX + Game1.toolbar.itemSlotSize + 150;
            else
                this.virtualToggleButton.bounds.X = Game1.toolbarPaddingX + Game1.toolbar.itemSlotSize + 50;

            if (Game1.toolbar.alignTop == true && !Game1.options.verticalToolbar)
            {
                object toolbarHeight = this.helper.Reflection.GetField<int>(Game1.toolbar, "toolbarHeight").GetValue();
                this.virtualToggleButton.bounds.Y = (int)toolbarHeight + 50;
            }
            else
            {
                this.virtualToggleButton.bounds.Y = 10;
            }

            float scale = 1f;
            if (!this.enabled)
            {
                scale = 0.5f;
            }
            if(!Game1.eventUp && Game1.activeClickableMenu is GameMenu == false && Game1.activeClickableMenu is ShopMenu == false)
                this.virtualToggleButton.draw(Game1.spriteBatch, Color.White * scale, 0.000001f);
        }
    }
}