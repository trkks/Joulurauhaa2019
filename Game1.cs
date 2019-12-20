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
        Texture2D circle;
        //DEBUG

        const float elfHangAngle = (float) (45 * (Math.PI / 180));
        const int elfSpawnRate = 2000; //Milliseconds

        //Graphics
        Texture2D sceneBackground;
        Texture2D elfGrabFrame;
        Texture2D elfDeathFrame;
        Texture2D playerDeathFrame;
        Texture2D[] elfFrames;
        Texture2D[] playerFrames;

        //Sounds
        SoundEffect backgroundSound;
        SoundEffectInstance[] bottlehitsounds;

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

            bottlehitsounds = new SoundEffectInstance[4];

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
            circle = Content.Load<Texture2D>("circle100");
            //DEBUG

            sceneBackground = Content.Load<Texture2D>("Background_486x320_gimp"); 
            elfGrabFrame = Content.Load<Texture2D>("elf_grab_64");
            elfDeathFrame = Content.Load<Texture2D>("elf_dead_64");
            playerDeathFrame = Content.Load<Texture2D>("pukki_dead");
            elfFrames = LoadFrames("elf_64", 4);
            playerFrames = LoadFrames("pukki_bottle_centered", 4);

            backgroundSound = Content.Load<SoundEffect>("drunkenTipTap");
            var backSong = backgroundSound.CreateInstance();
            backSong.IsLooped = true;
            backSong.Play();
            //base.BeginRun(); TODO what is this?

            bottlehitsounds[0] = Content.Load<SoundEffect>("bottlehit1").CreateInstance();
            bottlehitsounds[1] = Content.Load<SoundEffect>("bottlehit2").CreateInstance();
            bottlehitsounds[2] = Content.Load<SoundEffect>("bottlehit3").CreateInstance();
            bottlehitsounds[3] = Content.Load<SoundEffect>("bottlehit4").CreateInstance();

            player = new Pukki(
                new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
                , playerFrames
                , playerDeathFrame);

            //Debug spawn 
            //SpawnElf();
        }

        Texture2D[] LoadFrames(string tag, int n)
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

            if (player.GameOver)
                return;

            player.Body.Velocity = Vector2.Zero;
            if (kState.IsKeyDown(Keys.W))
                player.Body.Velocity += -Vector2.UnitY;
            if (kState.IsKeyDown(Keys.A))
                player.Body.Velocity += -Vector2.UnitX;
            if (kState.IsKeyDown(Keys.S))
                player.Body.Velocity += Vector2.UnitY;
            if (kState.IsKeyDown(Keys.D))
                player.Body.Velocity += Vector2.UnitX;

            if (mState.LeftButton == ButtonState.Pressed) //Buggy with Thinkpad touchpoint
            //if (kState.IsKeyDown(Keys.Space))
                player.Swing();

            switch (board.Colliding(ref player.Body))
            {
                case Bounds.Left:
                    player.Body.Position.X = player.Body.Radius;
                    break;
                case Bounds.Right:
                    player.Body.Position.X = graphics.PreferredBackBufferWidth - player.Body.Radius;
                    break;
                case Bounds.Top:
                    player.Body.Position.Y = player.Body.Radius;
                    break;
                case Bounds.Bottom:
                    player.Body.Position.Y = graphics.PreferredBackBufferHeight - player.Body.Radius;
                    break;
                default: //Inside
                    break;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.ApplySpeed();
            player.Move(deltaTime);

            mousePos.X = mState.X;
            mousePos.Y = mState.Y;
            player.Body.Facing = Vector2.Subtract(mousePos, player.Body.Position);

            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
                SpawnElf();

            foreach (Tonttu t in elves)
            {
                if (!t.isActive)
                    continue;

                foreach (Tonttu t2 in elves)
                {
                    if (t2 == t)
                        continue;
                    if (t.Facer.CollidingWith(t2.Facer))
                    {
                        CircleFacer.Bounce(ref t.Facer, ref t2.Facer);
                    }
                }

                switch (player.CollidingWith(t.Facer))
                {
                    case Pukki.Collision.Body:
                        player.AddElf(); //CircleFacer.Bounce(ref player.Body, ref t.Facer);
                        t.Die();
                        //TODO memory cleanup
                        break;
                    case Pukki.Collision.Bottle:
                        bottlehitsounds[rand.Next(4)].Play();
                        t.Die();
                        //t.SetTrajectoryFrom(player.Bottle.Position);
                        break;
                    default: //None
                        break;
                    //t.facer.velocity = Vector2.Zero;
                    //t.animater.InitializeAnimation();
                    //t.animater.SetDefaultSprite(elfGrabFrame);
                }

                switch (board.Colliding(ref t.Facer))
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
                t.Facer.Facing = Vector2.Subtract(player.Body.Position, t.Facer.Position);

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

            spriteBatch.Draw(sceneBackground, new Rectangle(0,0,Window.ClientBounds.Width,Window.ClientBounds.Height), null, Color.White);

           // spriteBatch.Draw(circle 
           //     ,new Rectangle(
           //         (int)(player.Bottle.Position.X-player.Bottle.Radius), 
           //         (int)(player.Bottle.Position.Y-player.Bottle.Radius), 
           //         (int)player.Bottle.Radius*2, 
           //         (int)player.Bottle.Radius*2)
           //     ,Color.White);

            foreach (Tonttu t in elves)
            {
                spriteBatch.Draw(t.Animater.GetCurrentFrame(),
                             new Rectangle((int)t.Facer.Position.X, (int)t.Facer.Position.Y, (int)Tonttu.Radius*2, (int)Tonttu.Radius*2),
                             null, Color.White, t.Facer.GetRotation(),
                             t.Animater.Pivot, SpriteEffects.None,0);
                //spriteBatch.Draw(t.animater.getFrame(), t.facer.position-t.animater.bodyOffset, Color.White);
                spriteBatch.Draw(square, t.Facer.Position, Color.Pink);
            }

            spriteBatch.Draw(player.Animater.GetCurrentFrame(),
                             new Rectangle((int)player.Body.Position.X, (int)player.Body.Position.Y, 250, 250),
                             null, Color.White, player.Body.GetRotation(),
                             player.Animater.Pivot, SpriteEffects.None,0);
            spriteBatch.Draw(square, player.Body.Position, Color.Pink);

            for (int i=0; i<player.HangingElves; i++)
            {
                Vector2 elfSpritePosition = player.Body.Position 
                    + Vector2.Transform(Pukki.Radius * Vector2.Normalize(player.Body.Facing),Matrix.CreateRotationZ(i*elfHangAngle));
                Vector2 elfPlayerOffset = Vector2.Normalize(Vector2.Subtract(player.Body.Position, elfSpritePosition));
                Rectangle elfRectangle = new Rectangle(
                    (int)(elfSpritePosition.X-Tonttu.Radius), 
                    (int)(elfSpritePosition.Y-Tonttu.Radius), 
                    (int)Tonttu.Radius*2, 
                    (int)Tonttu.Radius*2);
                spriteBatch.Draw(elfGrabFrame
                             , elfRectangle
                             , Color.White);// null, Color.White, (float)Math.Atan2(elfPlayerOffset.X, elfPlayerOffset.Y)
                             //, ???, Tonttu.Radius) - elfSpritePosition, SpriteEffects.None,0);
            }


            //spriteBatch.Draw(sceneFraming, 
            //    new Rectangle(0,0,972,640),
            //    null, Color.White);
            spriteBatch.Draw(square, mousePos-new Vector2(5,5), Color.Pink);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void SpawnElf()
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
    }
}
