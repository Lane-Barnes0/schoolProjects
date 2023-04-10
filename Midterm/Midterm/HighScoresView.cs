using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace Midterm
{
    public class HighScoresView : GameStateView
    {
        private SpriteFont m_font;
        private Texture2D m_backgroundCenter;
        private Texture2D m_backgroundLeft;
        private Texture2D m_backgroundRight;
        private bool loading = false;
        private const string MESSAGE = "These are the high scores";

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            m_backgroundCenter = contentManager.Load<Texture2D>("Images/background-center");
            m_backgroundLeft = contentManager.Load<Texture2D>("Images/background-left");
            m_backgroundRight = contentManager.Load<Texture2D>("Images/background-right");

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_backgroundLeft, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundRight, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 * 2, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundCenter, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 - 200, 0, m_graphics.PreferredBackBufferWidth / 3 + 400, m_graphics.PreferredBackBufferHeight), Color.White);

            if (m_loadedState != null)
            {
                Vector2 lineStart = Vector2.Zero;
                m_spriteBatch.DrawString(this.m_font, "High Score: " + m_loadedState.Score, lineStart, Color.Yellow);
                lineStart.Y += this.m_font.MeasureString("High Score: " + m_loadedState.Score).Y;
               

            }

            m_spriteBatch.End();
      
    }

        public override void update(GameTime gameTime)
        {
            loadSomething();
        }

        private void loadSomething()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    finalizeLoadAsync();
                }
            }
        }
        private Scoring m_loadedState = null;

        private async void finalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(Scoring));
                                    m_loadedState = (Scoring)mySerializer.Deserialize(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.loading = false;
            });
        }




    }


}