using Microsoft.Xna.Framework;
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
        int[,] OWMap = new int[256, 88];
        int[,] UWMap1 = new int[256, 88];
        int[,] UWMap2 = new int[256, 88];
        //int[,] caveMap = new int[64, 44];
        int[,] collisionMap = new int[256, 88];
        int[,] UWCMap1 = new int[256, 88];
        int[,] UWCMap2 = new int[256, 88];
        int[,] caveCMap = new int[64, 44];
        int[,] r1T = new int[16, 11];

        Vector2[] tStart;
        Globals.gState[] tSMap;
        Vector2[] tDest;
        Globals.gState[] tDMap;
        int numT;
        
        KeyboardState oldState;
        KeyboardState newState;

        Globals.gState currentState;

        player Link;

        bool mapChange;
        Vector2 MCDir;
        int tempSteps = 0;

        Vector2 resolution = new Vector2(256,224);  //IS NOT 240, is 224!
        int resScale = 1;
        
        Vector2 viewPort;

        private Texture2D empty;
        private Texture2D OWSpriteSheet;
        private Texture2D UW1Sheet;
        private Texture2D heartsSheet;
        private Texture2D caveMap;
        private Texture2D equippedFrame;

        private SpriteFont LoZText;
        private int LTLength;

        private string[] caveText;  //38 of them, not counting stairs down or dungeons.
        private int[] caveNPC;
        //LIST OF ITEMS IN CAVE, SOMEHOW.
        
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
            heartsSheet = Content.Load<Texture2D>("Heart Tiles");

            caveMap = Content.Load<Texture2D>("Cave map");
            equippedFrame = Content.Load<Texture2D>("ItemFrame");
            LoZText = Content.Load<SpriteFont>("Pixel Emulator");

            UW1Sheet = Content.Load<Texture2D>("UW");

            currentState = Globals.gState.overworld;

            numT = 6;
            tStart = new Vector2[numT];
            tSMap = new Globals.gState[numT];
            tDest = new Vector2[numT];
            tDMap = new Globals.gState[numT];

            string tileString;
            string colString;
            string caveColString;
            string UW1ColString;
            using (StreamReader sr = new StreamReader("OWTileMap.txt"))
            {
                tileString = sr.ReadLine();
            }
            using (StreamReader sr = new StreamReader("OWCollision.txt"))
            {
                colString = sr.ReadLine();
                for(int i = 0; i<87; i++)
                {
                    colString += sr.ReadLine();
                }
            }
            using (StreamReader sr = new StreamReader("Cavelision.txt"))
            {
                caveColString = sr.ReadLine();
                for (int i = 0; i < 45; i++)
                {
                    caveColString += sr.ReadLine();
                }
            }
            using (StreamReader sr = new StreamReader("UW1Collision.txt"))
            {
                UW1ColString = sr.ReadLine();
                for (int i = 0; i < 87; i++)
                {
                    UW1ColString += sr.ReadLine();
                }
            }
            using (StreamReader sr = new StreamReader("Transitions.txt"))
            {
                for (int i = 0; i < numT; i++)
                {
                    string transition = sr.ReadLine();
                    string[] tSplit = transition.Split('|');

                    string[] vtS = tSplit[0].Split(',');
                    tStart[i].X = float.Parse(vtS[0]); tStart[i].Y = float.Parse(vtS[1]);
                    vtS = tSplit[2].Split(',');
                    tDest[i].X = float.Parse(vtS[0]); tDest[i].Y = float.Parse(vtS[1]);

                    Globals.gState test;
                    if(Enum.TryParse<Globals.gState>(tSplit[1], out test))
                        tSMap[i] = test;
                    if (Enum.TryParse<Globals.gState>(tSplit[3], out test))
                        tDMap[i] = test;
                    


                }
            }

            string[] tsSplit = tileString.Split(' ');
            string[] colSplit = colString.Split(' ');
            string[] caveSplit = caveColString.Split(' ');
            string[] uw1Split = UW1ColString.Split(' ');

            for (int j = 0; j < 88; j++)
            {
                for(int i = 0; i < 256; i++)
                {
                    tileMap[i,j] = int.Parse(tsSplit[(j*256)+i], System.Globalization.NumberStyles.HexNumber); //Convert.ToInt32(tsSplit[(j*16) + i]);
                    
                    if(colSplit[(j*256)+i] == "X")
                    {
                        collisionMap[i,j] = 1;  //1 = collide.
                    }
                    else if (colSplit[(j * 256) + i] == "T")
                    {
                        collisionMap[i, j] = 2;  //2 = transition tile
                    }
                    else
                        collisionMap[i, j] = 0;

                    if (uw1Split[(j * 256) + i] == "X" || uw1Split[(j * 256) + i] == "0")
                    {
                        UWCMap1[i, j] = 1;
                    }
                    else if (uw1Split[(j * 256) + i] == "T")
                    {
                        UWCMap1[i, j] = 2;
                    }
                    else
                        UWCMap1[i, j] = 0;
                       

                    if (i < 64 && j < 44)
                    {
                        if (caveSplit[(j * 64) + i] == "X")
                        {
                            caveCMap[i, j] = 1;  //1 = collide.
                        }
                        else if (caveSplit[(j * 64) + i] == "T")
                        {
                            caveCMap[i, j] = 2; //2 = transition tile
                        }
                        else caveCMap[i, j] = 0;
                    }
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
            //
            if(currentState == Globals.gState.caves) caveUpdate();

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

            //CHECK 0.5 - if player is on a transition tile.
            //N.B. - Has to be at start so as to be before movement position checks are made, because collision uses offsetPos. Will also mean player finishes movement before activating animation
            for (int i = 0; i < numT; i++)
            {
                Vector2 lTemp = new Vector2();
                lTemp.X = Convert.ToSingle(Math.Round(Link.getPos().X));
                lTemp.Y = Convert.ToSingle(Math.Round(Link.getPos().Y));

                if (currentState == tSMap[i])
                {
                    if (tStart[i] == lTemp)
                    {
                        //DO THINGS.
                        Link.setPos(new Vector2(0));
                        Vector2 vTemp = new Vector2(tDest[i].X%16, tDest[i].Y%11);
                        viewPort = tDest[i] - vTemp;
                        currentState = tDMap[i];
                        Link.move(tDest[i]);
                    }
                }
                /*else if (currentState == tDMap[i])
                {
                    if (tDest[i] == lTemp)
                    {
                        DO OTHER THINGS.      -- Maybe not.
                        Link.setPos(new Vector2(0));
                        Vector2 vTemp = new Vector2(tStart[i].X % 16, tStart[i].Y % 11);
                        viewPort = tStart[i] - vTemp;
                        currentState = tSMap[i];
                        Link.move(tStart[i]);
                    }
                } //*/
            }

            if (moving && !mapChange)
            {

                Vector2 offsetPos = new Vector2(0.5f);
                offsetPos += Link.getPos();     //uses center of link as position instead of top left, allows for standardised collision checking
                movCheck += movAmount;

                //CHECK 1 - if player position would go off edge of map: zone transition.
                if (currentState == Globals.gState.overworld) //Check not needed for caves as caves are all singular zones that operate off transition tiles instead of edges.
                {
                    Vector2 zoneCheck = (movCheck / new Vector2(16)) + offsetPos - viewPort;
                    if (zoneCheck.X < 0)
                    {
                        offsetPos = Link.getPos();
                        mapChange = true;
                        MCDir = new Vector2(-1, 0);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.X > 16)
                    {
                        mapChange = true;
                        MCDir = new Vector2(1, 0);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.Y < 0)
                    {
                        mapChange = true;
                        MCDir = new Vector2(0, -1);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.Y > 10.5)
                    {
                        mapChange = true;
                        MCDir = new Vector2(0, 1);
                        tempSteps = 20;
                    }
                }
                else if (currentState != Globals.gState.caves)
                {
                    Vector2 zoneCheck = (movCheck / new Vector2(16)) + offsetPos - viewPort;
                    if (zoneCheck.X < 1)
                    {
                        offsetPos = Link.getPos();
                        mapChange = true;
                        MCDir = new Vector2(-1, 0);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.X > 15)
                    {
                        mapChange = true;
                        MCDir = new Vector2(1, 0);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.Y < 1)
                    {
                        mapChange = true;
                        MCDir = new Vector2(0, -1);
                        tempSteps = 20;
                    }
                    else if (zoneCheck.Y > 9.5)
                    {
                        mapChange = true;
                        MCDir = new Vector2(0, 1);
                        tempSteps = 20;
                    }
                }

                //CHECK 2 - if player would start to move inside a wall
                switch (currentState)
                {
                    case Globals.gState.overworld:
                        if(!checkCollision(offsetPos, movCheck, collisionMap))
                        {
                        Link.move(movAmount/16);
                        }
                        break;

                    case Globals.gState.caves:
                        if (!checkCollision(offsetPos, movCheck, caveCMap))
                        {
                            Link.move(movAmount / 16);
                        }
                        break;

                    case Globals.gState.dungeon1:
                        if (!checkCollision(offsetPos, movCheck, UWCMap1))
                        {
                            Link.move(movAmount / 16);
                        }
                        break;

                    case Globals.gState.dungeon2:
                        if (!checkCollision(offsetPos, movCheck, UWCMap2))
                        {
                            Link.move(movAmount / 16);
                        }
                        break;

                }
                
                
                

            }

            oldState = newState;
        }

        protected void updateZone()
        {

            if (tempSteps > 0)
            {
                //viewPort.X += 16 * MCDir.X;   //instant movement, rather than gradual.
                //viewPort.Y += 11 * MCDir.Y;
                //Link.move(new Vector2(MCDir.X, MCDir.Y * 1.5f));
                //tempSteps = 0;

                viewPort.X += 0.8f * MCDir.X;   //moving up or left from start zone breaks collision. Down and right do not. - FIXED
                viewPort.Y += 0.55f * MCDir.Y;

                if (currentState == Globals.gState.overworld)
                {
                    Link.move(new Vector2(MCDir.X * 0.05f, MCDir.Y * 0.075f));    //Link moves one tile so as to stay on screen
                    tempSteps--;
                }
                else if (currentState != Globals.gState.caves)
                {
                    Link.move(new Vector2(MCDir.X * 0.15f, MCDir.Y * 0.225f));    //Link moves two extra tiles to offset the walls.
                    tempSteps--;
                }
            }
            else
            {
                mapChange = false;
                viewPort.X = (float)Math.Round(viewPort.X, 0);  //cause of error was Viewport moving by an extra 0.0001 when changing negatively.
                viewPort.Y = (float)Math.Round(viewPort.Y, 0);  //as a result, int conversions truncated the value from .9999 to .0, rather than round.

            }
        }

        protected void caveUpdate()
        {
            //IF FIRST TIME IN CAVE, DO INTRO SHIZZLE (draw text).

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

            //OVERWORLD DRAWING
            if (currentState == Globals.gState.overworld)
            {
                for (int j = 0; j < 11; j++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 vTemp;
                        vTemp.X = tileMap[i + (int)viewPort.X, j + (int)viewPort.Y] % 20;
                        vTemp.Y = (tileMap[i + (int)viewPort.X, j + (int)viewPort.Y] - vTemp.X) / 20;

                        spriteBatch.Draw(OWSpriteSheet, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset) * resScale, 16 * resScale, 16 * resScale),
                            new Rectangle((int)(17 * vTemp.X) + 1, (int)(17 * vTemp.Y) + 1, 16, 16), Color.White);  //draws current room using TileMap.


                        //if(collisionMap[i + (int)viewPort.X, j + (int)viewPort.Y] == 1)
                        //{ spriteBatch.Draw(empty, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset) * resScale, 16 * resScale, 16 * resScale), new Color(212, 0, 212)); }      //Overlays map with collision map to confirm it matches.

                    }
                }
                                
            }
            //DUNGEON SET 1 DRAW
            else if (currentState == Globals.gState.dungeon1)
            {
                spriteBatch.Draw(UW1Sheet, new Rectangle(0, Globals.UIOffset * resScale, 256 * resScale, 176 * resScale),
                    new Rectangle((int)(viewPort.X*16), (int)(viewPort.Y*16), 256, 176), Color.White);  //draws current room using TileMap.

                /*for (int j = 0; j < 11; j++)
                {
                    Vector2 temp = Link.getPos();

                    for (int i = 0; i < 16; i++)
                    {
                        if (UWCMap1[i + (int)viewPort.X, j + (int)viewPort.Y] == 1)
                        { spriteBatch.Draw(empty, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset) * resScale, 16 * resScale, 16 * resScale), new Color(212, 0, 212)); }      //Overlays map with collision map to confirm it matches.
                    } 
                } // */
            }
            //CAVE DRAW
            else if (currentState == Globals.gState.caves)
            {
                Vector2 vTemp = new Vector2();
//                vTemp.X = tileMap[(int)viewPort.X, (int)viewPort.Y] % 20;
//                vTemp.Y = (tileMap[(int)viewPort.X, (int)viewPort.Y] - vTemp.X) / 20;

                spriteBatch.Draw(caveMap, new Rectangle(0, Globals.UIOffset * resScale, 256 * resScale, 176 * resScale), Color.White);

                /*for (int j = 0; j < 11; j++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if(caveCMap[i + (int)viewPort.X, j + (int)viewPort.Y] == 1)
                        { spriteBatch.Draw(empty, new Rectangle(i * 16 * resScale, ((j * 16) + Globals.UIOffset) * resScale, 16 * resScale, 16 * resScale), new Color(212, 0, 212)); }      //Overlays map with collision map to confirm it matches.
                    }
                }   //*/

                //DRAW ANIMATED FIRE.






            }

            Vector2 lPos = Link.getPos() - viewPort;
            spriteBatch.Draw(empty, new Rectangle((int)(lPos.X * 16 * resScale), ((int)(lPos.Y * 16) + Globals.UIOffset)*resScale, 16*resScale, 16*resScale), Color.White);
            

            //UI DRAWING
            if (currentState != Globals.gState.menus)
            {
                //back box
                spriteBatch.Draw(empty, new Rectangle(0, 0, 256 * resScale, Globals.UIOffset * resScale), Color.Black);

                //draw frames
                spriteBatch.Draw(equippedFrame, new Rectangle(123*resScale, 19*resScale, 18*resScale, 26*resScale), Color.White);   
                spriteBatch.Draw(equippedFrame, new Rectangle(147*resScale, 19*resScale, 18*resScale, 26*resScale), Color.White);

                //draw minimap
                spriteBatch.Draw(empty, new Rectangle(16*resScale, 16*resScale, 64*resScale, 32*resScale), Color.Gray);
                spriteBatch.Draw(empty, new Rectangle(((int)(viewPort.X/3.84f) + 16)*resScale, ((int)(viewPort.Y / 2.75f) + 16)*resScale, 3*resScale, 3*resScale), Color.LimeGreen); 

                //draw text
                spriteBatch.DrawString(LoZText, "B", new Vector2(128*resScale, 13*resScale), Color.White); //all text positions -3 from where they should be to offset blank space at top of font.
                spriteBatch.DrawString(LoZText, "A", new Vector2(152*resScale, 13*resScale), Color.White);
                spriteBatch.DrawString(LoZText, "-LIFE-", new Vector2(184*resScale, 13*resScale), Color.Red);

                spriteBatch.DrawString(LoZText, "X0", new Vector2(96*resScale, 13*resScale), Color.White);
                spriteBatch.DrawString(LoZText, "X0", new Vector2(96*resScale, 29*resScale), Color.White);
                spriteBatch.DrawString(LoZText, "X0", new Vector2(96*resScale, 37*resScale), Color.White);

                for (int i = 1; i <= Link.getMax(); i++)
                {
                    int j = 0; if(i%8 == 1) j= (i-1) / 8;
                    if (i % 2 == 1)
                    {
                        spriteBatch.Draw(heartsSheet, new Rectangle((175 + (4 * i)) * resScale, (40 - (j * 10)) * resScale, 7 * resScale, 8 * resScale),
                              new Rectangle(2, 10, 7, 8), Color.White);      //MAX HP.

                        spriteBatch.Draw(heartsSheet, new Rectangle((175 + (4*i)) * resScale, (40 - (j*10)) * resScale, 4 * resScale, 8 * resScale),
                            new Rectangle(1, 1, 4, 8), Color.White);        //Current HP, left half.
                    }
                    else
                    {
                        spriteBatch.Draw(heartsSheet, new Rectangle((174 + (4 * i)) * resScale, (40 - (j * 10)) * resScale, 4 * resScale, 8 * resScale),
                            new Rectangle(6, 1, 4, 8), Color.White);        //Current HP, right half.
                    }
                }

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected bool checkCollision(Vector2 position, Vector2 direction, int[,] map)
        {
            for (int i = -1; i < 2; i++)
            {

                float eCF = -5 * i;
                Vector2 edgeCheck = new Vector2(eCF/9);
                edgeCheck *= direction; direction.X += edgeCheck.Y; direction.Y += edgeCheck.X;

                int xPos = (int)(position.X + (direction.X / 16));
                int yPos = (int)(position.Y + (direction.Y / 16));
                
                if (map[xPos, yPos] == 1)
                    return true;

                direction.X -= edgeCheck.Y; direction.Y -= edgeCheck.X;       //originally forget to reset edge check, would check top top middle instead of top middle bottom.
            }
            return false;
        }

    }
}
