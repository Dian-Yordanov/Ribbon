using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ribbon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        Base3DCamera camera;

        List<Ribbon> ribbon=new List<Ribbon>();
        
        Keys lastPressed;

        List<Base3DObject> Cycle=new List<Base3DObject>();

        List<List<Vector3>> BikePath = new List<List<Vector3>>();
        Random rnd;
        List<int> pos = new List<int>();
        
        bool Pause = false;
        const int cycleCount = 4;

        List<HUDButton> CycleButtons = new List<HUDButton>();
        List<HUDButton> CamArrows = new List<HUDButton>();
        HUDButton ChaseButton;
        HUDButton ResetViewButton;
        HUDButton PauseButton;

        Vector3 chasePos = new Vector3(0, 2, 10);

        int ViewingCycle = 0;
        bool chaseCam = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
#if WINDOWS
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
#endif
#if WINDOWS_PHONE
            Content.RootDirectory = "WP7Content";
#endif
            camera = new Base3DCamera(this, .1f, 20000);
            camera.Position = new Vector3(0, 5, 50);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            rnd = new Random();
            int MaxArea = 100;

            for (int c = 0; c < cycleCount; c++)
            {
                ribbon.Add(new Ribbon(this));
                switch(ribbon.Count)
                {
                    case 1:
                        ribbon[ribbon.Count-1].DiffuseColor = Color.Red;
                        ribbon[ribbon.Count-1].AmbientColor = Color.Red;
                        break;
                    case 2:
                        ribbon[ribbon.Count-1].DiffuseColor = Color.Green;
                        ribbon[ribbon.Count-1].AmbientColor = Color.Green;
                        break;
                    case 3:
                        ribbon[ribbon.Count - 1].DiffuseColor = Color.SteelBlue;
                        ribbon[ribbon.Count - 1].AmbientColor = Color.SteelBlue;
                        break;
                    case 4:
                        ribbon[ribbon.Count - 1].DiffuseColor = Color.Gold;
                        ribbon[ribbon.Count - 1].AmbientColor = Color.Gold;
                        break;
                }
                Components.Add(ribbon[ribbon.Count-1]);

                Cycle.Add(new Base3DObject(this, "Models/Cycle1", "Shaders/CycleShader", ""));
                switch(Cycle.Count)
                {
                    case 1:
                        Cycle[Cycle.Count-1].DiffuseColor = Color.Red;
                        Cycle[Cycle.Count-1].AmbientColor = Color.DarkRed;
                        CycleButtons.Add(new HUDButton(this, "Textures/cycle", "Cycle1", 0, 0, 64, 64, Color.Red, 0));
                        break;
                    case 2:
                        Cycle[Cycle.Count-1].DiffuseColor = Color.Green;
                        Cycle[Cycle.Count-1].AmbientColor = Color.DarkGreen;
                        CycleButtons.Add(new HUDButton(this, "Textures/cycle", "Cycle2", 0, 92, 64, 64, Color.Green, 0));
                        break;
                    case 3:
                        Cycle[Cycle.Count - 1].DiffuseColor = Color.SteelBlue;
                        Cycle[Cycle.Count - 1].AmbientColor = Color.DarkSlateGray;
                        CycleButtons.Add(new HUDButton(this, "Textures/cycle", "Cycle3", 0, 184, 64, 64, Color.SteelBlue, 0));
                        break;
                    case 4:
                        Cycle[Cycle.Count - 1].DiffuseColor = Color.Gold;
                        Cycle[Cycle.Count - 1].AmbientColor = Color.DarkGoldenrod;
                        CycleButtons.Add(new HUDButton(this, "Textures/cycle", "Cycle4", 0, 276, 64, 64, Color.Gold, 0));
                        break;
                }
                Cycle[Cycle.Count-1].Scale = Vector3.One*.25f;
                Components.Add(Cycle[Cycle.Count-1]);
                CycleButtons[CycleButtons.Count - 1].OnClick += new ClickEvent(changeCycle);
                Components.Add(CycleButtons[CycleButtons.Count - 1]);

                BikePath.Add(new List<Vector3>());
                BikePath[BikePath.Count-1].Add(Cycle[BikePath.Count-1].Position);
                for (int p = 0; p < 100; p++)
                {
                    Vector3 newPos = new Vector3((float)rnd.Next(-MaxArea, MaxArea), 0, (float)rnd.Next(-MaxArea, MaxArea));
                    bool OK = true;
                    foreach (Vector3 v in BikePath[BikePath.Count-1])
                    {
                        if (Vector3.Distance(newPos, v) < 50)
                            OK = false;
                    }
                    if (OK)
                        BikePath[BikePath.Count-1].Add(newPos);
                }

                pos.Add(0);
            }

            // HUD
            for (int a = 0; a < 4; a++)
            {
                switch (a)
                {
                    case 0:
                        CamArrows.Add(new HUDButton(this, "Textures/ArrowUD", "DownArrow", graphics.PreferredBackBufferWidth - 128, graphics.PreferredBackBufferHeight - 64, 64, 64, Color.SteelBlue, 0));
                        break;
                    case 1:
                        CamArrows.Add(new HUDButton(this, "Textures/ArrowUD", "UpArrow", graphics.PreferredBackBufferWidth - 128, graphics.PreferredBackBufferHeight - 136, 64, 64, Color.SteelBlue, MathHelper.Pi));
                        break;
                    case 2:
                        CamArrows.Add(new HUDButton(this, "Textures/ArrowLR", "RightArrow", graphics.PreferredBackBufferWidth - 64, graphics.PreferredBackBufferHeight - 100, 64, 64, Color.SteelBlue, 0));
                        break;
                    case 3:
                        CamArrows.Add(new HUDButton(this, "Textures/ArrowLR", "LeftArrow", graphics.PreferredBackBufferWidth - 192, graphics.PreferredBackBufferHeight - 100, 64, 64, Color.SteelBlue, MathHelper.Pi));
                        break;
                }
                CamArrows[CamArrows.Count - 1].FullClick = false;
                CamArrows[CamArrows.Count - 1].OnClick += new ClickEvent(translateCamera);

                Components.Add(CamArrows[CamArrows.Count - 1]);
            }

            ChaseButton = new HUDButton(this, "Textures/camera", "Chase", graphics.PreferredBackBufferWidth - 64, 0, 64, 64, Color.LightSteelBlue, 0);
            ChaseButton.OnClick += new ClickEvent(setChaseCam);
            Components.Add(ChaseButton);

            PauseButton = new HUDButton(this, "Textures/pause", "Chase", graphics.PreferredBackBufferWidth - 128, 0, 64, 64, Color.Red, 0);
            PauseButton.OnClick += new ClickEvent(pause);
            Components.Add(PauseButton);

            ResetViewButton = new HUDButton(this, "Textures/reset", "Reset", graphics.PreferredBackBufferWidth - 192, 0, 64, 64, Color.Gold, 0);
            ResetViewButton.OnClick += new ClickEvent(reset);
            Components.Add(ResetViewButton);

            camera.LookAt(Cycle[0].Position,1f);

        }
        protected void reset(object sender)
        {
            for (int c = 0; c < cycleCount; c++)
            {
                ribbon[c].Reset();                
            }
        }
        protected void setChaseCam(object sender)
        {
            chaseCam = !chaseCam;
            if (chaseCam)
            {
                ((HUDButton)sender).Tint = Color.Green;
            }
            else
            {
                ((HUDButton)sender).Tint = Color.LightSteelBlue;
            }
        }
        protected void translateCamera(object sender)
        {
            switch (((HUDButton)sender).Name)
            {
                case "DownArrow":
                    camera.Translate(Vector3.Backward);
                    break;
                case "UpArrow":
                    camera.Translate(Vector3.Forward);
                    break;
                case "RightArrow":
                    camera.Translate(Vector3.Right);
                    break;
                case "LeftArrow":
                    camera.Translate(Vector3.Left);
                    break;
            }
        }
        protected void pause(object sender)
        {
            Pause = !Pause;
            if (Pause)
                ((HUDButton)sender).Tint = Color.Green;
            else
                ((HUDButton)sender).Tint = Color.Red;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Camera controls..
            float speedTran = .1f;
            float speedRot = .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                camera.Translate(Vector3.Forward * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                camera.Translate(Vector3.Backward * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                camera.Translate(Vector3.Left * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                camera.Translate(Vector3.Right * speedTran);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                camera.Rotate(Vector3.Up, speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                camera.Rotate(Vector3.Up, -speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                camera.Rotate(Vector3.Right, speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                camera.Rotate(Vector3.Right, -speedRot);

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                Cycle[ViewingCycle].Rotate(Vector3.Up, .1f);
                ribbon[ViewingCycle].AddSection(Cycle[ViewingCycle].Position, Cycle[ViewingCycle].Rotation);
            }
            else
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                Cycle[ViewingCycle].Rotate(Vector3.Down, .1f);
                ribbon[ViewingCycle].AddSection(Cycle[ViewingCycle].Position, Cycle[ViewingCycle].Rotation);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            {
                Cycle[ViewingCycle].Translate(Vector3.Forward * .1f);                
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                Cycle[ViewingCycle].Translate(Vector3.Backward * .1f);
            
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                camera.LookAt(Cycle[ViewingCycle].Position, 1f);

            if(Keyboard.GetState().IsKeyDown(Keys.P))
                Pause = true;
            
            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Pause = false;
                for (int c = 0; c < Cycle.Count; c++)
                {
                    List<Vector3> path = BikePath[c];
                    Cycle[c].LookAt(path[pos[c]], 1);
                    ribbon[c].AddSection(Cycle[c].Position, Cycle[c].Rotation);
                }
            }

            // Move bike.
            if (!chaseCam)
                camera.LookAt(Cycle[ViewingCycle].Position, 1f);
            else
            {
                camera.Position = Cycle[ViewingCycle].Position + Vector3.Transform(chasePos,Cycle[ViewingCycle].Rotation);
                camera.LookAt(Cycle[ViewingCycle].Position, 1f);                
            }

            for (int c = 0; c < cycleCount; c++)
            {
                List<Vector3> path = BikePath[c];
                if (Vector3.Distance(Cycle[c].Position, path[pos[c]]) < 1)
                {
                    pos[c]++;
                    if (pos[c] >= path.Count)
                        pos[c] = 0;

                    Cycle[c].LookAt(path[pos[c]], 1);
                    ribbon[c].AddSection(Cycle[c].Position, Cycle[c].Rotation);
                }
                else
                {
                    if (!Pause)
                    {
                        // Move cyle towards pos;                    
                        Cycle[c].Translate(Vector3.Forward * .5f);
                    }
                }


                ribbon[c].Top = Cycle[c].Position + Vector3.Transform((Vector3.Backward + Vector3.Up) * .5f, Cycle[c].Rotation);
                ribbon[c].Bottom = Cycle[c].Position + Vector3.Transform((Vector3.Backward + (Vector3.Down * .5f)) * .5f, Cycle[c].Rotation);

            }

            base.Update(gameTime);

            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                lastPressed = Keyboard.GetState().GetPressedKeys()[0];
            else
                lastPressed = Keys.None;
        }

        protected void changeCycle(object sender)
        {
            switch (((HUDButton)sender).Name)
            {
                case "Cycle1":
                    ViewingCycle = 0;
                    break;
                case "Cycle2":
                    ViewingCycle = 1;
                    break;
                case "Cycle3":
                    ViewingCycle = 2;
                    break;
                case "Cycle4":
                    ViewingCycle = 3;
                    break;
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);            
        }
    }
}
