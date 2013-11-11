using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;

namespace Ribbon
{
    public delegate void ClickEvent(object sender);

    class HUDButton : DrawableGameComponent
    {
        public int Top;
        public int Left;
        public int Width;
        public int Height;
        public Color Tint;
        public string Sprite;

        Texture2D texture;
        Vector2 Origin;
        Rectangle target;
        Rectangle source;
        Rectangle bounds;

        public float Rotation;

        public string Name;

        public ClickEvent OnClick;
        Rectangle HIRec;
        MouseState lastState;

        public bool FullClick = true;

#if WINDOWS_PHONE
        TouchCollection touches;
        int clickID = 0;
#endif

        private SpriteBatch spriteBatch
        {
            get { return ((Game1)Game).spriteBatch; }
        }

        public HUDButton(Game game,string sprite,string name, int left, int top, int width, int height, Color tint,float rotation) : base(game)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
            Tint = tint;    
            Sprite = sprite;
            Rotation = rotation;
            Name = name;

            // Always want to draw the hud last.
            DrawOrder = 9999;
        }
        protected override void LoadContent()
        {
            base.LoadContent();

            texture = Game.Content.Load<Texture2D>(Sprite);
            Origin = new Vector2(texture.Width, texture.Height) * .5f;
            target = new Rectangle(Left + Width / 2, Top + Height / 2, Width, Height);
            bounds = new Rectangle(Left, Top, Width, Height);
            source = new Rectangle(0, 0, texture.Width, texture.Height);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
#if WINDOWS_PHONE
                touches = TouchPanel.GetState();
                if (touches.Count == 0)
                    clickID = -1;
                foreach (TouchLocation tl in touches)
                {
                    HIRec = new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 16, 16);

                    if (tl.State != TouchLocationState.Released && tl.State != TouchLocationState.Invalid && clickID != tl.Id)
                    {
                        if(FullClick)
                            clickID = tl.Id;
#endif
#if WINDOWS
            MouseState ms = Mouse.GetState();
            bool clicked = false;

            if (!FullClick)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                    clicked = true;
            }
            else
            {
                if (ms.LeftButton == ButtonState.Pressed && lastState.LeftButton == ButtonState.Released)
                    clicked = true;
            }

            if (clicked)
            {
                HIRec = new Rectangle(ms.X, ms.Y, 1, 1);
#endif
                if (bounds.Intersects(HIRec))
                {
                    if (OnClick != null)
                    {
                        OnClick(this);
                    }
                }
#if WINDOWS_PHONE
                    }
#endif
            }
#if WINDOWS
            lastState = ms;
#endif
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(texture, target, source, Tint, Rotation, Origin, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}
