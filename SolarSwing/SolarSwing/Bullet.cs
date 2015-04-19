using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    class Bullet
    {

        private Vector2 _position;
        private Vector2 _target;
        private Vector2 _direction;
        private Vector2 _origin;
        private float _spd;
        public Texture2D _texture;
        private float _rotation;
        public Rectangle _rectangle;
        public int _life;
        private float _alpha;

        public Bullet(Vector2 position, Texture2D texture, Vector2 target)
        {
            _position = position;
            _texture = texture;
            _rotation = (float)Math.Atan2(target.Y - _position.Y + Globals._rnd.Next(-50, 50), target.X - _position.X + Globals._rnd.Next(-50, 50));
            _direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            _life = 150;
            _alpha = 1;
            _spd = 20;
        }
        public void Update()
        {
            _life--;
            if(_life <= 50)
            {
                _spd -= 0.2f;
                _alpha -= 0.02f;
            }
            _position += _direction * _spd;
            _rectangle.X = (int)_position.X;
            _rectangle.Y = (int)_position.Y;
        }
        public void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(_texture, _position, null, Color.White * _alpha, _rotation, _origin, 1, SpriteEffects.None, 0.20f);
        }
    }
}
