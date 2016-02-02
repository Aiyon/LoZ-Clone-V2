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
        int[,] r1T = new int[16, 11];

        Vector2 resolution = new Vector2(256,168);
        int resScale = 1;

        Vector2 viewPort = new Vector2(112,77);

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
            using (StreamReader sr = new StreamReader("OWTileMap.txt"))//"Room 1 test.txt"))
            {
                tileString = sr.ReadLine();
            }

            string[] tsSplit = tileString.Split(' ');

            for (int j = 0; j < 88; j++)
            {
                for(int i = 0; i < 256; i++)
                {
                    tileMap[i,j] = int.Parse(tsSplit[(j*256)+i], System.Globalization.NumberStyles.HexNumber); //Convert.ToInt32(tsSplit[(j*16) + i]);
                }
            }

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int j = 0; j < 11; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 vTemp;
                    vTemp.X = tileMap[i+(int)viewPort.X,j+(int)viewPort.Y]%20;
                    vTemp.Y = (tileMap[i+(int)viewPort.X,j+(int)viewPort.Y] - vTemp.X)/20; 
                    //spriteBatch.Draw(empty, new Rectangle(i*16*resScale, j*16*resScale, 16*resScale, 16*resScale), new Color(i*14, i*j, j*23));
                    spriteBatch.Draw(OWSpriteSheet, new Rectangle(i * 16 * resScale, j * 16 * resScale, 16 * resScale, 16 * resScale), new Rectangle((int)(17*vTemp.X)+1, (int)(17*vTemp.Y)+1, 16, 16) , Color.White);  //draws current room using TileMap.
               }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
