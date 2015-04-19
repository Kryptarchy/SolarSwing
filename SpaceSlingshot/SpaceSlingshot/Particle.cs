using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    class Particle
    {
        private Vector2 _position;
        private Texture2D _texture;
        private Vector2 _velocity;
        private Vector2 _origin;

        private float _rotation;
        public float _alpha;
        private float _scale;
        private Color _color;
        public Particle(Vector2 position, Texture2D texture, Vector2 velocity, float scale)
        {
            _position = position;
            _texture = texture;
            _velocity = velocity;

            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            _position.X += Globals._rnd.Next(-6, 6);
            _position.Y += Globals._rnd.Next(-6, 6);
            _rotation = MathHelper.ToRadians(Globals._rnd.Next(1, 360));
            _alpha = (float)Globals._rnd.Next(1,10) / 10.0f;
            _scale = scale + (float)Globals._rnd.Next(1, 10) / 10.0f;
            _color = new Color((int)Globals._rnd.Next(194, 238), (int)Globals._rnd.Next(163, 230), (int)Globals._rnd.Next(55, 97));

        }
        public void Update()
        {
            _alpha -= 0.05f;
            if (_scale > 0)
            {
                _scale += (float)Globals._rnd.Next(-10, 3) / 25.0f;
            }
            _position += _velocity * 0.2f;
        }
        public void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(_texture, _position, null, _color * _alpha, _rotation, _origin, _scale, SpriteEffects.None, 0.25f);
        }


    }
}
