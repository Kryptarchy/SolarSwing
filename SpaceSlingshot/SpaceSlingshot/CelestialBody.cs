using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    class CelestialBody
    {
        protected Vector2 _position;
        public Vector2 _center;
        protected Texture2D _texture;
        protected Rectangle _rectangle;

        public int _gravityField;
        public int _gravityPull;
        protected float _rotationSpd;
        protected Vector2 _direction;
        protected Vector2 _directionToSun;
        protected float _rotation;
        protected float _rotationToSun;

        public void Update()
        {
            _directionToSun = Globals._sun._center - _center;
            _rotationToSun = (float)Math.Atan2(_directionToSun.Y, _directionToSun.X) + MathHelper.ToRadians(90);
            if (_rotationToSun - _rotation >= MathHelper.ToRadians(180))
            {
                _rotation += MathHelper.Pi * 2;
            }

            if (_rotation != _rotationToSun && _rotation + MathHelper.ToRadians(3) < _rotationToSun)
            {
                _rotation += MathHelper.ToRadians(3);
            }
            else if (_rotation != _rotationToSun && _rotation - MathHelper.ToRadians(3) > _rotationToSun)
            {
                _rotation -= MathHelper.ToRadians(3);
            }
            _direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            _direction.Normalize();

            _position += _direction * _rotationSpd;
            _center = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
        }

        public void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(_texture, _position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        }
    }

    class Sun : CelestialBody
    {
        public Sun(Vector2 position, Texture2D texture)
        {
            _position = position;
            _texture = texture;
            _center = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _gravityField = 1500;
            _gravityPull = 3;
            _rotationSpd = 0;
        }
    }
    class SmallPlanet : CelestialBody
    {
        public SmallPlanet(Vector2 position, Texture2D texture)
        {
            _position = position;
            _texture = texture;
            _center = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _gravityField = 750;
            _gravityPull = 3;
            _rotationSpd = 7;
        }
    }
    class MediumPlanet : CelestialBody
    {
        public MediumPlanet(Vector2 position, Texture2D texture)
        {
            _position = position;
            _texture = texture;
            _center = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _gravityField = 1000;
            _gravityPull = 5;
            _rotationSpd = 5;
        }
    }
}
