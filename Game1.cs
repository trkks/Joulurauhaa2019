using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pipo;


namespace Joulurauhaa2019
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Pukki player;
        List<Tonttu> elves;

        int tonttuFrame = 0;
        Vector2[] tonttuSpawn;
        Vector2 tonttuVelo;
        float tonttuSpeed;
        float elfRad = 64;
        Vector2 tonttuSpriteOffset;

        Texture2D backGround;
        Texture2D[] elfFrames;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 972;
            graphics.PreferredBackBufferHeight = 640;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            elves = new List<Tonttu>();
            tonttuSpeed = 400.0f;
            tonttuSpriteOffset = new Vector2(elfRad, elfRad);

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
            backGround = Content.Load<Texture2D>("Background_486x320"); 
            elfFrames = LoadAll("elf_64", 4);
            // TODO: use this.Content to load your game content here
        }

        public Texture2D[] LoadAll(string tag, int n)
        {
            Texture2D[] textures = new Texture2D[n+1];
            textures[0] = Content.Load<Texture2D>(tag + "_F0");
            for (int i = 0; i < n; i++)
            {
                textures[i] = Content.Load<Texture2D>(tag + "_F" + i);
            }
            return textures;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameTime.TotalGameTime.Milliseconds % 2000 == 0)
                SpawnElf();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Tonttu t in elves)
            {
                foreach (Tonttu t2 in elves)
                {
                    if (t2 == t)
                        continue;
                    if (t.spaFac.CircleCollision(t2.spaFac))
                    {
                        t.spaFac.velocity *= -1;
                        t2.spaFac.velocity *= -1;
                    }
                }
                if (t.spaFac.position.X <= 0)
                {
                    t.spaFac.position.X = 0;
                    t.spaFac.velocity = Vector2.Reflect(t.spaFac.velocity, new Vector2(1, 0));
                }
                else if (Window.ClientBounds.Width - elfRad*2 < t.spaFac.position.X)
                {
                    t.spaFac.position.X = Window.ClientBounds.Width - elfRad*2;
                    t.spaFac.velocity = Vector2.Reflect(t.spaFac.velocity, new Vector2(1, 0));
                }
                if (t.spaFac.position.Y <= 0)
                {
                    t.spaFac.position.Y = 0;
                    t.spaFac.velocity = Vector2.Reflect(t.spaFac.velocity, new Vector2(0, 1));
                }
                else if (Window.ClientBounds.Height - elfRad*2 < t.spaFac.position.Y)
                {
                    t.spaFac.position.Y = Window.ClientBounds.Height - elfRad*2;
                    t.spaFac.velocity = Vector2.Reflect(t.spaFac.velocity, new Vector2(0, 1));
                }

                t.spaFac.Move(delta);
            }

            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(backGround, 
                new Rectangle(0,0,972,640),
                null, Color.White);
            foreach (Tonttu t in elves)
            {
                spriteBatch.Draw(t.sprAnim.getNext(), t.spaFac.position + new Vector2(t.spaFac.size), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SpawnElf()
        {
            Random rand = new Random();
            Vector2 tpos = new Vector2((float)rand.NextDouble() * Window.ClientBounds.Width, (float)rand.NextDouble() * Window.ClientBounds.Height);
            Vector2 tvelo = tonttuSpeed* Vector2.Normalize(new Vector2(rand.Next(0, 150), rand.Next(0, 150)));
            elves.Add(new Tonttu(tpos, tvelo, elfRad, elfFrames));
        }

        private bool CirclesAreColliding(Vector2 p1, Vector2 p2, int rad1, int rad2)
        {
            int a = (int)Vector2.Distance(p1, p2);
            return a < (rad1 + rad2);
        }
    }
}
