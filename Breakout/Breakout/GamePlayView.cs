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


namespace Breakout
{
    public class GamePlayView : GameStateView
    {

        private bool saving = false;
        private bool loading = false;


        private bool m_wait;
        private double m_waitTime;
        private bool m_pause;
        bool newGame = true;
        bool m_quit;
        private SpriteFont m_font;
        private Texture2D m_squareTexture;
        private bool m_shrink;
        private int m_score;
        private const float SPRITE_MOVE_PIXELS_PER_MS = 700.0f / 1000.0f;
        private const int Wall_THICKNESS = 30;
        private KeyboardInput m_inputKeyboard;
        int pointsToNewBall = 0;
        private bool m_gameOver;
        Rectangle m_paddle;
        List<List<Brick>> m_bricks;
        Rectangle leftWall;
        Rectangle rightWall;
        Rectangle topWall;
        private int BRICK_WIDTH;
        private int BRICK_HEIGHT;
        private Texture2D m_background;
        private int livesLeft;
        List<Ball> ballsInPlay;
        private List<Brick> m_bricktoBeRemoved = new List<Brick>();
        private List<Ball> m_ballstoRemove = new List<Ball>();

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
            BRICK_WIDTH = (m_graphics.PreferredBackBufferWidth - (Wall_THICKNESS * 2) - 30) / 14;
            BRICK_HEIGHT = 30;
            //Setup Paddle and Bricks
            m_paddle = new Rectangle(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 100, 300, 30);
            m_bricks = new List<List<Brick>>();
            for (int row = 0; row < 8; row++)
            {
                m_bricks.Add(new List<Brick>());
                Microsoft.Xna.Framework.Color brick_color;
                if (row < 2)
                {
                    brick_color = Color.Green;
                }
                else if (row < 4)
                {
                    brick_color = Color.Blue;
                }
                else if (row < 6)
                {
                    brick_color = Color.Orange;
                }
                else
                {
                    brick_color = Color.Yellow;
                }

                for (int col = 0; col < 14; col++)
                {

                    m_bricks[row].Add(new Brick(row, col, 5 + col * BRICK_WIDTH + Wall_THICKNESS, row * BRICK_HEIGHT + 200, brick_color));
                    m_bricks[row][col].myRectangle = new Rectangle(m_bricks[row][col].m_xPos + (col * 2), m_bricks[row][col].m_yPos + (row * 2), BRICK_WIDTH, BRICK_HEIGHT);
                }
            }

            m_wait = true;
            m_waitTime = 3.9;
            newGame = true;
            m_waitforkey = false;
            m_selection = 0;
            m_score = 0;
            livesLeft = 3;
            m_pause = false;
            m_shrink = true;
            m_quit = false;
            m_gameOver = false;
            ballsInPlay = new List<Ball>();
            ballsInPlay.Add(new Ball((m_paddle.Left + m_paddle.Right) / 2, m_paddle.Top - 50));
            topWall = new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, Wall_THICKNESS);
            rightWall = new Rectangle(m_graphics.PreferredBackBufferWidth - Wall_THICKNESS, 0, Wall_THICKNESS, m_graphics.PreferredBackBufferHeight);
            leftWall = new Rectangle(0, 0, Wall_THICKNESS, m_graphics.PreferredBackBufferHeight);
        }
        public override void loadContent(ContentManager contentManager)
        {
            m_background = contentManager.Load<Texture2D>("Images/background");
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            m_squareTexture = contentManager.Load<Texture2D>("Images/square");
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {

            if (m_gameOver)
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
            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.White);
            if (m_pause)
            {
                renderMenu();
            }
            else
            {
                drawLives();
                drawScore();
                m_spriteBatch.Draw(m_squareTexture, m_paddle, Color.White);
                drawWalls();
                drawBricks();
                drawBalls();
                
                if (m_wait)
                {
                    drawTimeLeft();
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
                if (!m_wait)
                {
                    if (pointsToNewBall >= 100)
                    {
                        ballsInPlay.Add(new Ball(m_paddle.Center.X, m_paddle.Top - 50));
                        pointsToNewBall -= 100;
                    }
                    if (m_bricks[0].Count < 14)
                    {
                        if (m_shrink)
                        {
                            m_paddle = new Rectangle(m_paddle.Center.X, m_paddle.Top, 150, 30);
                        }
                        m_shrink = false;
                    }



                    m_inputKeyboard.Update(gameTime);
                    foreach (Ball ball in ballsInPlay)
                    {
                        //Check to see if the Ball is off the screen
                        if (ball.yPos > m_graphics.PreferredBackBufferHeight)
                        {
                            m_ballstoRemove.Add(ball);
                        }

                        //Check Collisions 
                        //Check Paddle
                        if (intersect(m_paddle, ball.myRectangle))
                        {
                            ball.handlePaddleCollision(gameTime, m_paddle);
                        }
                        //Check Walls
                        if (intersect(ball.myRectangle, topWall))
                        {
                            ball.yDirection = -ball.yDirection;
                        }

                        if (intersect(ball.myRectangle, leftWall) || intersect(ball.myRectangle, rightWall))
                        {
                            ball.xDirection = -ball.xDirection;
                        }
                        //Check Bricks
                        foreach (List<Brick> brickRow in m_bricks)
                        {
                            foreach (Brick brick in brickRow)
                            {
                                if (intersect(ball.myRectangle, brick.myRectangle))
                                {
                                    m_bricktoBeRemoved.Add(brick);
                                    ball.handleCollision(gameTime);
                                    break;
                                }
                            }

                        }
                        foreach (Brick brick in m_bricktoBeRemoved)
                        {
                            m_bricks[brick.m_row].Remove(brick);
                            if (m_bricks[brick.m_row].Count == 0)
                            {
                                pointsToNewBall += 25;
                                m_score += 25;
                            }
                            if (brick.m_color == Color.Yellow)
                            {
                                pointsToNewBall += 1;
                                m_score += 1;
                            }
                            else if (brick.m_color == Color.Orange)
                            {
                                pointsToNewBall += 2;
                                m_score += 2;
                            }
                            else if (brick.m_color == Color.Blue)
                            {
                                pointsToNewBall += 3;
                                m_score += 3;
                            }
                            else
                            {
                                m_score += 5;
                                pointsToNewBall += 5;


                            }
                        }
                        m_bricktoBeRemoved.Clear();
                        ball.update(gameTime);
                    }
                    foreach (Ball deadBall in m_ballstoRemove)
                    {
                        ballsInPlay.Remove(deadBall);
                    }
                    if (ballsInPlay.Count == 0)
                    {

                        livesLeft -= 1;
                        m_wait = true;


                        if (livesLeft <= 0)
                        {
                            m_wait = false;
                            m_gameOver = true;
                            
                        }
                        else
                        {
                            ballsInPlay.Add(new Ball(m_paddle.Center.X, m_paddle.Top - 50));
                        }
                    }
                }
                else
                {
                    m_waitTime -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_waitTime <= 1) { m_wait = false; m_waitTime = 3.9; }
                }

            }
            else
            {
                pauseInput();
            }
        }

        private void drawTimeLeft()
        {
            Vector2 stringSize = m_font.MeasureString(System.Math.Floor(m_waitTime).ToString());
            m_spriteBatch.DrawString(
               m_font,
                System.Math.Floor(m_waitTime).ToString(),
               new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2 - Wall_THICKNESS / 2, Wall_THICKNESS + 100),
               Color.Yellow);
        }

        private void drawWalls()
        {
            m_spriteBatch.Draw(m_squareTexture, topWall, Color.Black);
            m_spriteBatch.Draw(m_squareTexture, leftWall, Color.Black);
            m_spriteBatch.Draw(m_squareTexture, rightWall, Color.Black);
        }

        private void drawLives()
        {
            for(int i = 0; i < livesLeft; i++)
            {
                m_spriteBatch.Draw(m_squareTexture, new Rectangle(Wall_THICKNESS + 5 + 30 * i, Wall_THICKNESS + 10, 20, 20), Color.Red);
            }
            
        }
        private void drawBricks()
        {
            for (int i = 0; i < m_bricks.Count; i++)
            {
                for (int j = 0; j < m_bricks[i].Count; j++)
                {
                    m_spriteBatch.Draw(m_squareTexture, m_bricks[i][j].myRectangle, m_bricks[i][j].m_color);

                }
            }
        }

        private void drawScore()
        {
            Vector2 stringSize = m_font.MeasureString("Score " + m_score.ToString());
            m_spriteBatch.DrawString(
               m_font,
                "Score " + m_score.ToString(),
               new Vector2(m_graphics.PreferredBackBufferWidth - stringSize.X - Wall_THICKNESS, m_graphics.PreferredBackBufferHeight - Wall_THICKNESS - 100),
               Color.Yellow);
  
        }
        private void drawBalls()
        {
            foreach (Ball ball in ballsInPlay)
            {
                m_spriteBatch.Draw(m_squareTexture, ball.myRectangle, Color.Red);
            }
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



        private class Ball
        {
            private int m_bricksDestroyed = 0;
            private int m_nextSpeedUp = 4;
            bool stopSpeedUp = false;
            public double m_speed = 0.4;
            public int xPos;
            public int yPos;
            public double xDirection = 1;
            public double yDirection = -1;


            public Rectangle myRectangle;
            public Ball(int x, int y)
            {
                xPos = x;
                yPos = y;
                myRectangle = new Rectangle(xPos, yPos, 30, 30);

            }

            public void update(GameTime gameTime)
            {
                if (m_bricksDestroyed >= m_nextSpeedUp && !stopSpeedUp)
                {
                    m_speed += 0.1;
                    m_nextSpeedUp = m_nextSpeedUp * 3;
                    if (m_nextSpeedUp > 36 && m_nextSpeedUp < 185) { m_nextSpeedUp = 62; }
                    else if (m_nextSpeedUp > 185) { stopSpeedUp = true; }
                }
                xPos += (int)(xDirection * gameTime.ElapsedGameTime.TotalMilliseconds * m_speed);
                yPos -= (int)(yDirection * gameTime.ElapsedGameTime.TotalMilliseconds * m_speed);
                myRectangle = new Rectangle(xPos, yPos, 30, 30);


            }




            public void handleCollision(GameTime gameTime)
            {
                yDirection = -yDirection;
                m_bricksDestroyed++;
            }

            public void handlePaddleCollision(GameTime gameTime, Rectangle otherRectangle)
            {
                yDirection = -yDirection;
                xDirection = ((float)myRectangle.Center.X - otherRectangle.Center.X) / (otherRectangle.Width / 2);
            }




        }

        private class Brick
        {
            public Microsoft.Xna.Framework.Color m_color = Color.Yellow;
            public int m_xPos;
            public int m_yPos;
            public int m_row;
            public int m_col;
            public Rectangle myRectangle;

            public Brick(int row, int col, int xPos, int yPos, Microsoft.Xna.Framework.Color color)
            {
                m_row = row;
                m_col = col;
                m_xPos = xPos;
                m_yPos = yPos;
                m_color = color;

            }
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
                    Scoring myState = new Scoring(m_score);
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