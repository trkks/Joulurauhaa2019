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

        // This is to hide elf-sprites when they're attached to pukki
        Texture2D square;

        const float elfHangAngle = (float)(45 * (Math.PI / 180));

        uint score;

        uint elfSpawnRate = 600; //Milliseconds//2000
        const uint elfSpawnRateThreshold = 10;
        const uint elfSpawnRateSubtraction = 5;
        const uint elfSpawnRateMin = 50;

        const float elfSlowDownMultiplier = 0.3f;

        //Graphics
        Texture2D sceneFloor;
        Texture2D sceneFraming;
        Texture2D elfGrabFrame;
        Texture2D elfDeathFrame;
        Texture2D playerDeathFrame;
        Texture2D crosshair;
        Texture2D[] elfFrames;
        Texture2D[] playerFrames;
        SpriteFont font;

        //Sounds
        SoundEffectInstance backgroundSound;
        SoundEffectInstance[] bottleHitSounds;
        SoundEffectInstance elfGrabSound;

        //Utility
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rand;

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

        void SpawnElf()
        {
            Vector2 spawnPosition = player.Body.Position;
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
                , Vector2.Normalize(Vector2.Subtract(player.Body.Position, spawnPosition))
                , elfFrames
                , elfDeathFrame);
            elves.Add(theElf);
        }

        Texture2D[] LoadFrames(string tag, int n)
        {
            Texture2D[] textures = new Texture2D[n];
            //Save default sprite into index 0
            for (int i = 0; i < n; i++)
            {
                textures[i] = Content.Load<Texture2D>(tag + "_F" + i);
            }
            return textures;
        }

        void Reset()
        { 
            elves = new List<Tonttu>();
            player = new Pukki(
                new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
                , playerFrames
                , playerDeathFrame);
            score = 0;
            elfSpawnRate = 600;
            backgroundSound.Stop();
            backgroundSound = Content.Load<SoundEffect>("drunkenTipTap").CreateInstance();
            backgroundSound.IsLooped = true;
            backgroundSound.Play();
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

            rand = new Random();
            mousePos = Vector2.Zero;

            score = 0;

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

            square = Content.Load<Texture2D>("unitSquare");

            sceneFloor = Content.Load<Texture2D>("floor_486x320");
            sceneFraming = Content.Load<Texture2D>("framing_486x320");
            elfGrabFrame = Content.Load<Texture2D>("elf_grab_64");
            elfDeathFrame = Content.Load<Texture2D>("elf_dead_64");
            playerDeathFrame = Content.Load<Texture2D>("pukki_Larger_bottle_dead");
            crosshair = Content.Load<Texture2D>("crosshair");
            elfFrames = LoadFrames("elf_64", 4);
            playerFrames = LoadFrames("pukki_Larger_bottle", 4);

            backgroundSound = Content.Load<SoundEffect>("drunkenTipTap").CreateInstance();
            backgroundSound.IsLooped = true;
            backgroundSound.Play();

            bottleHitSounds = new SoundEffectInstance[3];
            bottleHitSounds[0] = Content.Load<SoundEffect>("bottlehit1").CreateInstance();
            bottleHitSounds[1] = Content.Load<SoundEffect>("bottlehit2").CreateInstance();
            bottleHitSounds[2] = Content.Load<SoundEffect>("bottlehit3").CreateInstance();

            elfGrabSound = Content.Load<SoundEffect>("elfGrab").CreateInstance();

            font = Content.Load<SpriteFont>("File");

            player = new Pukki(
                new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
                , playerFrames
                , playerDeathFrame);

            //Debug spawn 
            //SpawnElf();
            //SpawnElf();
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

            mousePos.X = mState.X;
            mousePos.Y = mState.Y;

            if (player.GameOver)
            {
                    
                foreach (Tonttu t in elves)
                    t.Animater.SetDelays(new int[] { 14,4,14,4 });
                if (kState.IsKeyDown(Keys.R))
                    Reset();
                return;
            }

            player.Body.Velocity = Vector2.Zero;
            if (kState.IsKeyDown(Keys.W))
                player.Body.Velocity += -Vector2.UnitY;
            if (kState.IsKeyDown(Keys.A))
                player.Body.Velocity += -Vector2.UnitX;
            if (kState.IsKeyDown(Keys.S))
                player.Body.Velocity += Vector2.UnitY;
            if (kState.IsKeyDown(Keys.D))
                player.Body.Velocity += Vector2.UnitX;

            if (kState.IsKeyDown(Keys.Space))
                player.Swing();

            /*top*/
            if (player.Body.Position.Y - player.Body.Radius <= 0)
            {
                player.Body.Position.Y = player.Body.Radius;
            }
            /*bottom*/                                      // NOTE Dont ask why 3 works here...
            else if (player.Body.Position.Y + player.Body.Radius*3 >= graphics.PreferredBackBufferHeight)
            {
                player.Body.Position.Y = graphics.PreferredBackBufferHeight - player.Body.Radius*3;
            }
            /*left*/
            if (player.Body.Position.X - player.Body.Radius <= 0)
            {
                player.Body.Position.X = player.Body.Radius;
            }
            /*right*/                                       // See NOTE above
            else if (player.Body.Position.X + player.Body.Radius*3 >= graphics.PreferredBackBufferWidth)
            {
                player.Body.Position.X = graphics.PreferredBackBufferWidth - player.Body.Radius*3;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Tonttu t in elves)
            {

                if (t.IsDead) //Dead elf doesnt need to check collisions
                {
                    t.Move(deltaTime);
                    continue;
                }
                else
                    t.ScaleVelocity(1); //reset slowdown

                t.Facer.Facing = Vector2.Subtract(player.Body.Position, t.Facer.Position);

                foreach (Tonttu t2 in elves)
                {
                    if (t2 == t)
                        continue;
                    if (t.Facer.CollidingWith(t2.Facer))
                    {
                        if (t2.IsDead)
                            t.ScaleVelocity(elfSlowDownMultiplier);
                        else
                            CircleFacer.Bounce(ref t.Facer, ref t2.Facer);
                    }
                }

                if (!t.Facer.IsHitBoxActive)
                    continue;

                switch (player.CollidingWith(t.Facer))
                {
                    case Pukki.Collision.Body:
                        player.AddElf(); 
                        elfGrabSound.Play();
                        t.Deactivate(square);
                        break;
                    case Pukki.Collision.Bottle:
                        bottleHitSounds[rand.Next(bottleHitSounds.Length)].Play();
                        t.Die(player.Bottle.Position);
                        score++;
                        break;
                    default: //Pukki.Collision.None
                        break;
                }

                /*top*/
                if (t.Facer.Position.Y - t.Facer.Radius <= 0)
                {
                    t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, Vector2.UnitY);
                    t.Facer.Position.Y = t.Facer.Radius;
                }
                /*bottom*/
                else if (t.Facer.Position.Y + t.Facer.Radius >  graphics.PreferredBackBufferHeight)
                {
                    t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, -Vector2.UnitY);
                    t.Facer.Position.Y = graphics.PreferredBackBufferHeight - t.Facer.Radius;
                }
                /*left*/
                if (t.Facer.Position.X - t.Facer.Radius <= 0)
                {
                    t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, Vector2.UnitX);
                    t.Facer.Position.X = t.Facer.Radius;
                }
                /*right*/
                else if (t.Facer.Position.X + t.Facer.Radius > graphics.PreferredBackBufferWidth)
                {
                    t.Facer.Velocity = Vector2.Reflect(t.Facer.Velocity, -Vector2.UnitX);
                    t.Facer.Position.X = graphics.PreferredBackBufferWidth - t.Facer.Radius;
                }

                t.Move(deltaTime);
            }

            player.ApplySpeed();
            player.Move(deltaTime);

            player.Body.Facing = Vector2.Subtract(mousePos, player.Body.Position);

            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
            {
                SpawnElf();

            }
                if (score != 0 && score % elfSpawnRateThreshold == 0)
                {
                    elfSpawnRate -= elfSpawnRateSubtraction;
                    if (elfSpawnRate < elfSpawnRateMin)
                        elfSpawnRate = elfSpawnRateMin;
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

            spriteBatch.Draw(sceneFloor, new Rectangle(0,0,Window.ClientBounds.Width,Window.ClientBounds.Height), null, Color.White);


            foreach (Tonttu t in elves)
            {
                spriteBatch.Draw(t.Animater.GetCurrentFrame(),
                             new Rectangle(t.Facer.Position.ToPoint(), new Point((int)Tonttu.Radius*2)),
                             null, Color.White, t.Facer.GetRotation(),
                             t.pivot, SpriteEffects.None,0);
            }

            spriteBatch.Draw(player.Animater.GetCurrentFrame(),
                             new Rectangle(player.Body.Position.ToPoint(), new Point(Pukki.SpriteWidth, Pukki.SpriteHeight)),
                             null, Color.White, player.Body.GetRotation(),
                             player.pivot, SpriteEffects.None,0);

            for (int i=0; i<player.HangingElves; i++)
            {
                float hangingDistance = i < 8 ? Pukki.HangingDistance : Pukki.HangingDistance * 0.6f;
                Vector2 elfSpritePosition = player.Body.Position 
                    + Vector2.Transform(hangingDistance * Vector2.Normalize(player.Body.Facing),Matrix.CreateRotationZ(i*elfHangAngle));
                Vector2 elfFacingPlayer = Vector2.Normalize(Vector2.Subtract(player.Body.Position, elfSpritePosition));
                Rectangle elfRectangle = new Rectangle(
                    elfSpritePosition.ToPoint(), 
                    new Point((int)Tonttu.Radius*2));
                spriteBatch.Draw(elfGrabFrame
                             , elfRectangle
                             , null, Color.White, (float)Math.Atan2(elfFacingPlayer.Y, elfFacingPlayer.X),
                             new Vector2(Tonttu.Radius, Tonttu.Radius), SpriteEffects.None, 0);//, ???, Tonttu.Radius) - elfSpritePosition, SpriteEffects.None,0);
            }

            spriteBatch.Draw(crosshair, new Rectangle((int)mousePos.X - 15, (int)mousePos.Y - 15, 30, 30), Color.White);

            spriteBatch.Draw(sceneFraming, new Rectangle(0,0,Window.ClientBounds.Width,Window.ClientBounds.Height), null, Color.White);
            spriteBatch.DrawString(font, "Toivotukset:"+score, new Vector2(15, 0), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
