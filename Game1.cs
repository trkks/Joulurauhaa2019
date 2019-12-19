using System.Diagnostics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Pipo;


namespace Joulurauhaa2019
{
    public enum Bounds
    {
        Top,
        Right,
        Bottom,
        Left,
        Inside
    }
    
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //DEBUG
        Texture2D square;
        //DEBUG

        const int elfSpawnRate = 2000; //Milliseconds

        //Graphics
        Texture2D sceneBackground;
        Texture2D elfGrabFrame;
        Texture2D[] elfFrames;
        Texture2D[] playerFrames;

        //Sounds
        SoundEffect backgroundSound;

        //Utility
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rand;

        RectangleBoard board;
        Pukki player;
        List<Tonttu> elves;

        Vector2 mousePos;
 
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
            board = new RectangleBoard(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            rand = new Random();
            mousePos = Vector2.Zero;

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

            //DEBUG
            square = Content.Load<Texture2D>("unitSquare");
            //DEBUG

            sceneBackground = Content.Load<Texture2D>("Background_486x320_gimp"); 
            elfGrabFrame = Content.Load<Texture2D>("elf_grab_64");
            elfFrames = LoadAll("elf_64", 4);
            playerFrames = LoadAll("pukki_bottle_centered", 4);

            //backgroundSound = Content.Load<SoundEffect>("drunkenTipTap");
            //var backSong = backgroundSound.CreateInstance();
            //backSong.IsLooped = true;
            //backSong.Play();
            //base.BeginRun(); TODO what is this?
        
            player = new Pukki(
                new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
                , playerFrames);
        }

        Texture2D[] LoadAll(string tag, int n)
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
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape))
                Exit();

            player.Facer.Velocity = Vector2.Zero;
            if (kState.IsKeyDown(Keys.W))
                player.Facer.Velocity += -Vector2.UnitY;
            if (kState.IsKeyDown(Keys.A))
                player.Facer.Velocity += -Vector2.UnitX;
            if (kState.IsKeyDown(Keys.S))
                player.Facer.Velocity += Vector2.UnitY;
            if (kState.IsKeyDown(Keys.D))
                player.Facer.Velocity += Vector2.UnitX;

            //if (mState.LeftButton == ButtonState.Pressed) //Buggy with Thinkpad touchpoint
            if (kState.IsKeyDown(Keys.Space))
                player.Swing();

            switch (board.Colliding(player.Facer))
            {
                case Bounds.Left:
                    player.Facer.Position.X = player.Facer.Radius;
                    break;
                case Bounds.Right:
                    player.Facer.Position.X = graphics.PreferredBackBufferWidth - player.Facer.Radius;
                    break;
                case Bounds.Top:
                    player.Facer.Position.Y = player.Facer.Radius;
                    break;
                case Bounds.Bottom:
                    player.Facer.Position.Y = graphics.PreferredBackBufferHeight - player.Facer.Radius;
                    break;
                default: //Inside
                    break;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.ApplySpeed();
            player.Facer.Move(deltaTime);

            mousePos.X = mState.X;
            mousePos.Y = mState.Y;
            player.Facer.Facing = Vector2.Subtract(mousePos, player.Facer.Position);

            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
                SpawnElf();

            foreach (Tonttu t in elves)
            {
                foreach (Tonttu t2 in elves)
                {
                    if (t2 == t)
                        continue;
                    if (t.Facer.CollidingWith(t2.Facer))
                    {
                        CircleFacer.Bounce(ref t.Facer, ref t2.Facer);
                    }
                }

                if (t.Facer.CollidingWith(player.Facer))
                {
                    CircleFacer.Bounce(ref player.Facer, ref t.Facer);
                    //t.facer.velocity = Vector2.Zero;
                    //t.animater.InitializeAnimation();
                    //t.animater.SetDefaultSprite(elfGrabFrame);
                }

                switch (board.Colliding(t.Facer))
                {
                    case Bounds.Left:
                        t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, Vector2.UnitX);
                        t.Facer.Position.X = t.Facer.Radius;
                        break;
                    case Bounds.Right:
                        t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, -Vector2.UnitX);
                        t.Facer.Position.X = graphics.PreferredBackBufferWidth - t.Facer.Radius;
                        break;
                    case Bounds.Top:
                        t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, Vector2.UnitY);
                        t.Facer.Position.Y = t.Facer.Radius;
                        break;
                    case Bounds.Bottom:
                        t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, -Vector2.UnitY);
                        t.Facer.Position.Y = graphics.PreferredBackBufferHeight - t.Facer.Radius;
                        break;
                    default:
                        break;
                }
                t.Facer.Facing = Vector2.Subtract(player.Facer.Position, t.Facer.Position);

                t.Facer.Move(deltaTime);
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

            spriteBatch.Draw(sceneBackground, new Rectangle(0,0,972,640), null, Color.White);

            spriteBatch.Draw(player.Animater.GetCurrentFrame(),
                             new Rectangle((int)player.Facer.Position.X, (int)player.Facer.Position.Y, 250, 250),
                             null, Color.White, player.Facer.GetRotation(),
                             player.Animater.Pivot, SpriteEffects.None,0);
            //spriteBatch.Draw(player.animater.getFrame(), 
            //                 player.facer.position-player.animater.bodyOffset,
            //                 Color.White);
            spriteBatch.Draw(square, player.Facer.Position, Color.Pink);

            foreach (Tonttu t in elves)
            {
                spriteBatch.Draw(t.Animater.GetCurrentFrame(),
                             new Rectangle((int)t.Facer.Position.X, (int)t.Facer.Position.Y, (int)Tonttu.Radius*2, (int)Tonttu.Radius*2),
                             null, Color.White, t.Facer.GetRotation(),
                             t.Animater.Pivot, SpriteEffects.None,0);
                //spriteBatch.Draw(t.animater.getFrame(), t.facer.position-t.animater.bodyOffset, Color.White);
                spriteBatch.Draw(square, t.Facer.Position, Color.Pink);
            }
    
            //spriteBatch.Draw(sceneFraming, 
            //    new Rectangle(0,0,972,640),
            //    null, Color.White);
            spriteBatch.Draw(square, mousePos, Color.Pink);
            spriteBatch.Draw(elfGrabFrame, player.Facer.Position+player.Facer.Facing, Color.Pink);
            
            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void SpawnElf()
        {
            Vector2 spawnPosition = player.Facer.Position;
            switch (rand.Next(4))
            {
                case 0:  //top
                    spawnPosition = new Vector2(
                        rand.Next((int)Tonttu.Radius, Window.ClientBounds.Width)
                        ,Tonttu.Radius);
                    break;
                case 1:  //right
                    spawnPosition = new Vector2(
                        Window.ClientBounds.Width - Tonttu.Radius
                        ,rand.Next((int)Tonttu.Radius, Window.ClientBounds.Height));
                    break;
                case 2:  //bottom
                    spawnPosition = new Vector2(
                        rand.Next((int)Tonttu.Radius, Window.ClientBounds.Width)
                        ,Window.ClientBounds.Height - Tonttu.Radius);
                    break;
                default: //left
                    spawnPosition = new Vector2(
                        Tonttu.Radius
                        ,rand.Next((int)Tonttu.Radius, Window.ClientBounds.Height));
                    break;
            }
            Tonttu theElf = new Tonttu(
                spawnPosition
                , Vector2.Normalize(Vector2.Subtract(player.Facer.Position, spawnPosition))
                , elfFrames);
            elves.Add(theElf);
        }
    }
}
