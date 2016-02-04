﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Legend_of_Zelda_Clone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int[,] tileMap = new int[256,88];
        int[,] collisionMap = new int[256, 88];
        int[,] r1T = new int[16, 11];

        KeyboardState oldState;
        KeyboardState newState;

        player Link;

        bool mapChange;
        Vector2 MCDir;
        int tempSteps = 0;

        Vector2 resolution = new Vector2(256,240);
        int resScale = 1;

        Vector2 viewPort;

        private Texture2D empty;
        private Texture2D OWSpriteSheet;
        
        public Game1() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = (int)(resolution.Y) * resScale;
            graphics.PreferredBackBufferWidth = (int)resolution.X * resScale;
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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            empty = Content.Load<Texture2D>("empty");
            OWSpriteSheet = Content.Load<Texture2D>("OverworldTiles");

            string tileString;
            string colString;
            using (StreamReader sr = new StreamReader("OWTileMap.txt"))//"Room 1 test.txt"))
            {
                tileString = sr.ReadLine();
            }
            using (StreamReader sr = new StreamReader("OWCollision.txt"))//"Room 1 test.txt"))
            {
                colString = sr.ReadLine();
                for(int i = 0; i<87; i++)
                {
                    colString += sr.ReadLine();
                }
            }

            string[] tsSplit = tileString.Split(' ');
            string[] colSplit = colString.Split(' ');

            for (int j = 0; j < 88; j++)
            {
                for(int i = 0; i < 256; i++)
                {
                    tileMap[i,j] = int.Parse(tsSplit[(j*256)+i], System.Globalization.NumberStyles.HexNumber); //Convert.ToInt32(tsSplit[(j*16) + i]);
                    if(colSplit[(j*256)+i] == "X")
                    {
                        collisionMap[i,j] = 1;  //1 = collide.
                    }
                    else collisionMap[i,j] = 0;
                }
            }

            Link = new player(new Vector2(7.5f, 5), 6);
            viewPort = new Vector2(112, 77);
            Link.move(viewPort);    //adjusts Link's position based on which map section the camera starts in.

            oldState = Keyboard.GetState();

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (mapChange) updateZone();
            updatePlayer(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected void updatePlayer(double delta)
        {
            newState = Keyboard.GetState();

            Vector2 movAmount = new Vector2(0);
            Vector2 movCheck = new Vector2(0);
            bool moving = false;

            if (newState.IsKeyDown(Keys.W))
            {
                movAmount = new Vector2(0, -1);
                movCheck.Y = -8;
                moving = true;
            }
            else if (newState.IsKeyDown(Keys.A))
            {
                movAmount = new Vector2(-1, 0);
                movCheck.X = -8;
                moving = true;
            }
            else if (newState.IsKeyDown(Keys.S))
            {
                movAmount = new Vector2(0, 1);
                movCheck.Y = 8;
                moving = true;
            }
            else if (newState.IsKeyDown(Keys.D))
            {
                movAmount = new Vector2(1, 0);
                movCheck.X = 8;
                moving = true;
            }

            if (moving && !mapChange)
            {
                Vector2 offsetPos = new Vector2(0.5f);
                offsetPos += Link.getPos();     //uses center of link as position instead of top left, allows for standardised collision checking
                movCheck += movAmount;

                //CHECK 1 - if player position would go off edge of map: zone transition.
                Vector2 zoneCheck = (movCheck/new Vector2(16)) + offsetPos - viewPort;
                if (zoneCheck.X < 0)
                {
                    offsetPos = Link.getPos();
                    mapChange = true;
                    MCDir = new Vector2(-1, 0);
                    tempSteps = 20;
                }
                if (zoneCheck.X > 16)
                {
                    mapChange = true;
                    MCDir = new Vector2(1, 0);
                    tempSteps = 20;
                }
                if (zoneCheck.Y < 0)
                {
                    mapChange = true;
                    MCDir = new Vector2(0, -1);
                    tempSteps = 20;
                }
                if (zoneCheck.Y > 10.5)
                {
                    mapChange = true;
                    MCDir = new Vector2(0, 1);
                    tempSteps = 20;
                }

                //CHECK 2 - if player would start to move inside a wall
                if (!checkCollision(offsetPos, movCheck))
                {
                    Link.move(movAmount/16);
                }
            }

            oldState = newState;
        }

        protected void updateZone()
        {
            if (tempSteps > 0)
            {
                viewPort.X += 0.75f * MCDir.X;
                viewPort.Y += 0.5f * MCDir.Y;
//                Link.move(new Vector2(MCDir.X*0.05f, MCDir.Y*0.075f));    //Turns out Link doesn't move on zone transitions. The viewport moves 15(X) or 10(Y) and Link stands still. Presumably because moving the player and the world at the same time was taxing on NES hardware?
                tempSteps--;
            }
            else mapChange = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            // TODO: Add your drawing code here

            for (int j = 0; j < 11; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 vTemp;
                    vTemp.X = tileMap[i+(int)viewPort.X,j+(int)viewPort.Y]%20;
                    vTemp.Y = (tileMap[i+(int)viewPort.X,j+(int)viewPort.Y] - vTemp.X)/20; 

                    spriteBatch.Draw(OWSpriteSheet, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset)* resScale, 16 * resScale, 16 * resScale),
                        new Rectangle((int)(17*vTemp.X)+1, (int)(17*vTemp.Y)+1, 16, 16) , Color.White);  //draws current room using TileMap.

                    /*
                    if(collisionMap[i + (int)viewPort.X, j + (int)viewPort.Y] == 1)
                    { spriteBatch.Draw(empty, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset) * resScale, 16 * resScale, 16 * resScale), new Color(212, 0, 212)); }      //Overlays map with collision map to confirm it matches.
                    */

               }
            }

            Vector2 lPos = Link.getPos() - viewPort;
            spriteBatch.Draw(empty, new Rectangle((int)(lPos.X*16),(int)(lPos.Y*16)+Globals.UIOffset,16,16), Color.White);

            spriteBatch.Draw(empty, new Rectangle(0, 0, 256, Globals.UIOffset), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected bool checkCollision(Vector2 position, Vector2 direction)
        {
            for (int i = -1; i < 2; i++)
            {

                float eCF = -5 * i;
                Vector2 edgeCheck = new Vector2(eCF/9);
                edgeCheck *= direction; direction.X += edgeCheck.Y; direction.Y += edgeCheck.X;

                int xPos = (int)(position.X + (direction.X / 16));
                int yPos = (int)(position.Y + (direction.Y / 16));
                
                if (collisionMap[xPos, yPos] == 1)
                    return true;

                direction.X -= edgeCheck.Y; direction.Y -= edgeCheck.X;       //originally forget to reset edge check, would check top top middle instead of top middle bottom.
            }
            return false;
        }

    }
}