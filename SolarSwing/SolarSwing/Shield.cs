using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    class Shield
    {
        public Vector2 _position;
        public Texture2D _texture;
        public Rectangle _rectangle;
        private Vector2 _origin;
        public float _rotation;
        public bool _turnedOn;

        public Shield(Vector2 position, Texture2D texture)
        {
            _position = position;
            _texture = texture;
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            _turnedOn = false;
        }

        public void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1, SpriteEffects.None, 0.0f);
        }
    }
}
