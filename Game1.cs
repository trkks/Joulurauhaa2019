using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Pipo;


namespace Joulurauhaa2019
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //DEBUG
        Texture2D square;
        //DEBUG

        Pukki player;
        List<Tonttu> elves;
        RectangleBoard board;

        Random rand;

        const float playerRad = Pukki.radius;
        const float elfRad = Tonttu.radius;
        const int elfSpawnRate = 2000; //Milliseconds

        Texture2D sceneBackground;
        Texture2D[] playerFrames;
        Texture2D elfGrabFrame;
        Texture2D[] elfFrames;

        SoundEffect backgroundSound;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 972;
            graphics.PreferredBackBufferHeight = 640;
            Content.RootDirectory = "Content";

            rand = new Random();
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
            board = new RectangleBoard(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            square = Content.Load<Texture2D>("unitSquare");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerFrames = LoadAll("pukki_bottle_192", 4);

            sceneBackground = Content.Load<Texture2D>("Background_486x320_gimp"); 

            elfFrames = LoadAll("elf_64", 4);
            //elfGrabFrame = Content.Load<Texture2D>("elf_grab");
            elfGrabFrame = elfFrames[0];

            backgroundSound = Content.Load<SoundEffect>("drunkenTipTap");
            var backSong = backgroundSound.CreateInstance();
            backSong.IsLooped = true;
            backSong.Play();
        
            player = new Pukki(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2),
                               70, playerFrames);
            //base.BeginRun();
            //SpawnElf();
        }

        public Texture2D[] LoadAll(string tag, int n)
        {
            Texture2D[] textures = new Texture2D[n+1];
            //Save default sprite into index 0
            textures[0] = Content.Load<Texture2D>(tag + "_F0");
            for (int i = 0; i < n; i++)
            {
                textures[i+1] = Content.Load<Texture2D>(tag + "_F" + i);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                player.Swing(gameTime.TotalGameTime.Milliseconds);

            player.facer.velocity = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                player.facer.velocity += -Vector2.UnitX;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                player.facer.velocity += Vector2.UnitX;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                player.facer.velocity += -Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                player.facer.velocity += Vector2.UnitY;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.ApplySpeed();
            player.facer.Move(delta);

            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
                SpawnElf();

            foreach (Tonttu t in elves)
            {
                foreach (Tonttu t2 in elves)
                {
                    if (t2 == t)
                        continue;
                    if (t.facer.CircleCollision(t2.facer))
                    {
                        CircleFacer.Bounce(ref t.facer, ref t2.facer);
                    }
                }

                if (t.facer.CircleCollision(player.facer))
                {
                    t.facer.velocity = Vector2.Zero;
                    t.animater.InitializeAnimation();
                    t.animater.SetDefaultSprite(elfGrabFrame);
                }

                Nullable<Vector2> refNormal = board.OnBoard(t.facer); 
                if (refNormal != null)
                {
                    t.facer.velocity = Vector2.Reflect(t.facer.velocity, refNormal.Value);
                }
                t.facer.Move(delta);
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
                spriteBatch.Draw(sceneBackground, 
                    new Rectangle(0,0,972,640),
                    null, Color.White);

                spriteBatch.Draw(player.animater.getNext(), player.facer.position-player.animater.bodyOffset, Color.White);
                spriteBatch.Draw(square, player.facer.position, Color.Pink);

                foreach (Tonttu t in elves)
                {
                    spriteBatch.Draw(t.animater.getNext(), t.facer.position-t.animater.bodyOffset, Color.White);
                    spriteBatch.Draw(square, t.facer.position, Color.Pink);
                }
    
                //spriteBatch.Draw(sceneFraming, 
                //    new Rectangle(0,0,972,640),
                //    null, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SpawnElf()
        {
            int ri = rand.Next(2);
            Vector2 tpos = ri == 0 
                         ? new Vector2(rand.Next((int)elfRad, Window.ClientBounds.Width), (int)elfRad)
                         : new Vector2((int)elfRad, rand.Next((int)elfRad, Window.ClientBounds.Height));
            Vector2 tvelo = ri == 0
                          ? new Vector2(1, 1)
                          : new Vector2(1, -1);
            Tonttu theElf = new Tonttu(tpos, tvelo, elfFrames);
            theElf.animater.isAnimating = true;
            elves.Add(theElf);
        }
    }
}
