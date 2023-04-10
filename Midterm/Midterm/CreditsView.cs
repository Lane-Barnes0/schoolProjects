using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    internal class CreditsView : GameStateView
    {
        private SpriteFont m_font;
        private Texture2D m_backgroundCenter;
        private Texture2D m_backgroundLeft;
        private Texture2D m_backgroundRight;
        private const string MESSAGE = "Made by Lane Barnes";

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

            return GameStateEnum.Credits;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_backgroundLeft, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundRight, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 * 2, 0, m_graphics.PreferredBackBufferWidth / 3, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.Draw(m_backgroundCenter, new Rectangle(m_graphics.PreferredBackBufferWidth / 3 - 200, 0, m_graphics.PreferredBackBufferWidth / 3 + 400, m_graphics.PreferredBackBufferHeight), Color.White);
            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font, MESSAGE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y), Color.Yellow);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}

