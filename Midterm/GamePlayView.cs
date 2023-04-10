using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CS5410.Input;

//
// Added to support serialization
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System;
using System.Reflection.Metadata;

namespace Midterm
{
    public class GamePlayView : GameStateView
    {

        private bool saving = false;

        private bool m_wait;
       
        private double gameOverTime;
        private int leftStart;
        private int rightStart;
        private double playArea;
        private double gapSize;
        List<Rectangle> m_bricks;
       
        private bool m_pause;
        bool newGame = true;
        bool m_quit;
        double newBrickTime;
        ContentManager m_contentManager;
        private SpriteFont m_font;
        private Texture2D m_squareTexture;
        ParticleEmitter m_emitter1;
        private double m_score;
        private const float SPRITE_MOVE_PIXELS_PER_MS = 900.0f / 1000.0f;
        private const int Wall_THICKNESS = 30;
        private KeyboardInput m_inputKeyboard;
        private bool m_gameOver;
        Rectangle m_paddle;
        Rectangle leftWall;
        Rectangle rightWall;
        private Texture2D m_backgroundCenter;
        private Texture2D m_backgroundLeft;
        private Texture2D m_backgroundRight;
        private Rectangle previewLeft;
        private Rectangle previewRight;

        private string[] PauseState =
        {
            "Resume",
            "Exit",
        };
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private int m_selection;
        private bool m_waitforkey;

        public void initializeNewGameState()
        {
            //Setup Input
            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Left, false, new InputDeviceHelper.CommandDelegate(onMoveLeft));
            m_inputKeyboard.registerCommand(Keys.Right, false, new InputDeviceHelper.CommandDelegate(onMoveRight));
            m_inputKeyboard.registerCommand(Keys.Escape, false, new InputDeviceHelper.CommandDelegate(onEscape));
            m_paddle = new Rectangle(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 100, 30, 30);
            gameOverTime = 5;
            m_bricks = new List<Rectangle>();
          
           
            newBrickTime = 1;
            
            newGame = true;
            m_waitforkey = false;
            m_selection = 0;
            m_score = 0;
            m_pause = false;
            m_quit = false;
            leftStart = m_graphics.PreferredBackBufferWidth / 3 - 220;
            rightStart = m_graphics.PreferredBackBufferWidth / 3 * 2 + 190;
            m_gameOver = false;
            rightWall = new Rectangle(rightStart, 0, Wall_THICKNESS, m_graphics.PreferredBackBufferHeight);
            leftWall = new Rectangle(leftStart, 0, Wall_THICKNESS, m_graphics.PreferredBackBufferHeight);
            playArea = m_graphics.PreferredBackBufferWidth / 3 + 400;
            gapSize = playArea * 0.15;
            playArea = playArea - gapSize;


            Random rand = new Random();
            int totalBrickLength = (int)playArea;
            int leftLength = (int)(totalBrickLength * (rand.NextDouble() + 0.1));
            previewLeft = new Rectangle(leftStart + 20, 0, leftLength, 30);
            previewRight = new Rectangle(leftStart + 20 + leftLength + (int)gapSize, 0, totalBrickLength - leftLength, 30);

        }
        public override void loadContent(ContentManager contentManager)
        {
            m_contentManager = contentManager;
            m_backgroundCenter = contentManager.Load<Texture2D>("Images/background-center");
            m_backgroundLeft = contentManager.Load<Texture2D>("Images/background-left");
            m_backgroundRight = contentManager.Load<Texture2D>("Images/background-right");
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            m_squareTexture = contentManager.Load<Texture2D>("Images/square");
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");

            
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {

            if (gameOverTime <= 0)
            {
                newGame = true;
                saveScore();
                return GameStateEnum.MainMenu;
            }

            if (m_quit)
            {
                if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    newGame = true;
                    return GameStateEnum.MainMenu;
                }
            }
            return GameStateEnum.NewGame;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_backgroundLeft, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundRight, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 * 2, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundCenter, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 - 200, 0, m_graphics.PreferredBackBufferWidth / 3 + 400, m_graphics.PreferredBackBufferHeight), Color.White);
            if (m_pause)
            {
                renderMenu();
            }
            else
            {
                m_spriteBatch.Draw(m_squareTexture, previewLeft, Color.Green);
                m_spriteBatch.Draw(m_squareTexture, previewRight, Color.Green);
                drawScore();
                m_spriteBatch.Draw(m_squareTexture, m_paddle, Color.Blue);
                drawBricks();
                


                if (m_gameOver)
                {
                    m_emitter1.draw(m_spriteBatch);
                    Vector2 stringSize = m_font.MeasureString("Game Over" + m_score.ToString());
                    m_spriteBatch.DrawString(
                       m_font,
                        "Game Over",
                       new Vector2((m_graphics.PreferredBackBufferWidth - stringSize.X) / 2, m_graphics.PreferredBackBufferHeight / 2),
                       Color.Red);
                }

            }
            m_spriteBatch.End();

        }

        public override void update(GameTime gameTime)
        {
            if (newGame)
            {
                initializeNewGameState();
                newGame = false;
            }
            if (!m_pause)
            {
                    if (!m_gameOver)
                    {
                       
                        m_inputKeyboard.Update(gameTime);
                        updateBricks(gameTime);
                        newBrickTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        if (newBrickTime <= 0)
                        {
                            makeBricks();
                            newBrickTime = 0.5;
                        }

                        foreach (Rectangle brick in m_bricks)
                        {
                            if (intersect(brick, m_paddle))
                            {
                                m_gameOver = true;
                                
                                
                                    m_emitter1 = new ParticleEmitter(
                                        m_contentManager,
                                        new TimeSpan(0, 0, 0, 0, 5),
                                        m_paddle.Center.X, m_paddle.Center.Y,
                                        20,
                                        2,
                                        new TimeSpan(0, 0, 4),
                                        new TimeSpan(0, 0, 0, 0, 3000));
                                

                            }
                        }
                        m_score += gameTime.ElapsedGameTime.Milliseconds;
                    } else
                    {
                        
                        m_emitter1.update(gameTime);
                        gameOverTime -= gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            else
            {
                pauseInput();
            }
        }

        private void drawBricks()
        {
            foreach(Rectangle brick in m_bricks)
            {
                m_spriteBatch.Draw(m_squareTexture, brick, Color.Red);
            }

           
        }

        private void makeBricks()
        {
            Random rand = new Random();
            int totalBrickLength = (int)playArea;
            double leftLength = (totalBrickLength * (rand.NextDouble()));

            if (leftLength / previewLeft.Width < 0.85)
            {
                leftLength = previewLeft.Width * 0.85;
            } else if(leftLength / previewLeft.Width > 1.15)
            {
                leftLength = previewLeft.Width * 1.15;
            }
            Rectangle left = new Rectangle(leftStart + 20, 0, (int)leftLength, 30);
            Rectangle right = new Rectangle(leftStart + 20 + (int)leftLength + (int)gapSize, 0, totalBrickLength - (int)leftLength, 30);
           
            m_bricks.Add(previewLeft);
            m_bricks.Add(previewRight);
            previewLeft = left;
            previewRight = right;
           


        }

        private void updateBricks(GameTime gameTime)
        {
           
            for(int i = 0; i < m_bricks.Count; i++)
            {
                
                m_bricks[i] = new Rectangle(m_bricks[i].X, m_bricks[i].Y + (int) (0.4 * gameTime.ElapsedGameTime.TotalMilliseconds), m_bricks[i].Width, m_bricks[i].Height);
               
            }
        }
        private void drawScore()
        {
            Vector2 stringSize = m_font.MeasureString("Time " + m_score.ToString());
            m_spriteBatch.DrawString(
               m_font,
                "Time " + (m_score / 1000).ToString(),
               new Vector2(m_graphics.PreferredBackBufferWidth - stringSize.X - 100, m_graphics.PreferredBackBufferHeight - 100),
               Color.Yellow);
  
        }

        private void onMoveLeft(GameTime gameTime, float scale)
        {

            int moveDistance = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * SPRITE_MOVE_PIXELS_PER_MS * scale);
            if (!intersect(leftWall, m_paddle))
            {
                m_paddle.X = m_paddle.X - moveDistance;
            }


        }

        private void onMoveRight(GameTime gameTime, float scale)
        {

            int moveDistance = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * SPRITE_MOVE_PIXELS_PER_MS * scale);
            if (!intersect(rightWall, m_paddle))
            {
                m_paddle.X = m_paddle.X + moveDistance;
            }


        }

        private void onEscape(GameTime gameTime, float scale)
        {
            m_pause = !m_pause;
        }
        private bool intersect(Rectangle r1, Rectangle r2)
        {
            bool theyDo = !(
            r2.Left > r1.Right ||
            r2.Right < r1.Left ||
            r2.Top > r1.Bottom ||
            r2.Bottom < r1.Top);
            return theyDo;
        }

  


        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }


        private void renderMenu()
        {
            Vector2 stringSize = m_font.MeasureString("Game Paused");
            m_spriteBatch.DrawString(
               m_font,
                "Game Paused",
               new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, 100),
               Color.White);

            float bottom = drawMenuItem(
                m_selection == 0 ? m_fontMenuSelect : m_fontMenu,
                "Resume",
                200,
                m_selection == 0 ? Color.Yellow : Color.Blue);
            drawMenuItem(m_selection == 1 ? m_fontMenuSelect : m_fontMenu, "Quit", bottom, m_selection == 1 ? Color.Yellow : Color.Blue);

        }

        public void pauseInput()
        {
            // This is the technique I'm using to ensure one keypress makes one menu navigation move
            if (!m_waitforkey)
            {
                // Arrow keys to navigate the menu
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {

                    if (m_selection != 1)
                    {
                        m_selection = m_selection + 1;
                    }

                    m_waitforkey = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (m_selection != 0)
                    {
                        m_selection = m_selection - 1;
                    }

                    m_waitforkey = true;
                }

                // If enter is pressed, return the appropriate new state
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_selection == 0)
                {
                    m_pause = false;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_selection == 1)
                {

                    m_quit = true;
                    m_waitforkey = true;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitforkey = false;
            }


        }

        private void saveScore()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    //
                    // Create something to save
                    Scoring myState = new Scoring((int)m_score/1000);
                    finalizeSaveAsync(myState);
                }
            }
        }

        private async void finalizeSaveAsync(Scoring state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(Scoring));
                                mySerializer.Serialize(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.saving = false;
            });
        }

    }
}