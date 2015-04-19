using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    

    class Ship
    {
        public Vector2 _position;
        protected Vector2 _lastPosition;
        protected Vector2 _origin;
        protected Vector2 _direction;
        protected Vector2 _velocity;
        protected Vector2 _gravity;
        public Rectangle _rectangle;
        public Texture2D _texture;
        public Texture2D _defaultTexture;
        public Texture2D _chargingTexture;
        public Texture2D _boostingTexture;
        protected Texture2D _shieldDefaultTexture;
        protected Texture2D _shieldChargingTexture;
        protected Texture2D _shieldBoostingTexture;
        protected float _maxSpd;
        protected float _currentSpd;
        public Shield _shield;
        
        public double _spd;
        public float _rotation;

        protected List<Particle> _particleList;

        public virtual void Update()
        {
            GravityPull();
            Movement();
            //_velocity =
            _lastPosition = _position;
            _position += _direction * _currentSpd + _gravity + _velocity;
            _rectangle.X = (int)_position.X;
            _rectangle.Y = (int)_position.Y;
            if (_shield != null)
            {
                _shield._position += _direction * _currentSpd + _gravity + _velocity;
                _shield._rectangle.X = (int)_position.X;
                _shield._rectangle.Y = (int)_position.Y;
            }
            _spd = Math.Sqrt((_lastPosition.X - _position.X) * (_lastPosition.X - _position.X) + (_lastPosition.Y - _position.Y) * (_lastPosition.Y - _position.Y));

            
            for (int i = 0; i < _particleList.Count; i++)
            {
                _particleList[i].Update();
                if(_particleList[i]._alpha < 0)
                {
                    _particleList.RemoveAt(i);
                    i--;
                }
            }
            if(_spd > 0)
            {
                for (int i = 0; i < _currentSpd; i++)
                {
                    _particleList.Add(new Particle(_position, _texture, _direction * -_currentSpd / 2, _currentSpd / 16));
                }
            }
        }

        public virtual void Draw(SpriteBatch _spriteBatch)
        {
            foreach(Particle _particle in _particleList)
            {
                _particle.Draw(_spriteBatch);
            }
            _spriteBatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1, SpriteEffects.None, 0.1f);
            
            
            
        }

        protected virtual void Movement() {}
        protected virtual void GravityPull() {}
    }

    class PlayerShip : Ship
    {
        public int _hp;
        public int _energy;
        public int _score;
        private bool _shiftReleased;

        public PlayerShip(Vector2 position, Texture2D texture, Texture2D chargingTexture, Texture2D boostingTexture, Texture2D shieldTexture, Texture2D shieldChargingTexture, Texture2D shieldBoostingTexture)
        {
            _position = position;
            _texture = texture;
            _defaultTexture = _texture;
            _chargingTexture = chargingTexture;
            _boostingTexture = boostingTexture;
            _shieldDefaultTexture = shieldTexture;
            _shieldChargingTexture = shieldChargingTexture;
            _shieldBoostingTexture = shieldBoostingTexture;

            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            _shield = new Shield(_position, shieldTexture);
            _energy = 100;
            _hp = 100;
            _score = 0;
            _maxSpd = 13;
            _currentSpd = 0;
            _rotation = MathHelper.ToRadians(90);
            _shiftReleased = true;

            _particleList = new List<Particle>();
        }
        
        public override void Update()
        {
            base.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && _energy - 1 >= 0)
            {
                if (_shiftReleased == true)
                {
                    _currentSpd = _maxSpd;
                    _currentSpd += 15;
                    _shiftReleased = false;
                }
                _energy -= 1;
                _texture = _boostingTexture;
                _shield._texture = _shieldBoostingTexture;
            }
            else if (_spd > 15.5 && Keyboard.GetState().IsKeyUp(Keys.LeftShift))
            {
                if (_energy + 1 <= 100)
                {
                    _energy += 1;
                }
                _texture = _chargingTexture;
                _shield._texture = _shieldChargingTexture;
            }
            else
            {
                _texture = _defaultTexture;
                _shield._texture = _shieldDefaultTexture;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && _energy - 1 >= 0)
            {
                _energy -= 1;
                _shield._turnedOn = true;
            }
            else
            {
                _shield._turnedOn = false;
            }

            
            if((Keyboard.GetState().IsKeyUp(Keys.LeftShift) || _energy <= 2) && _shiftReleased == false)
            {
                _currentSpd -= 15;
                _shiftReleased = true;
            }
        }
        public override void Draw(SpriteBatch _spriteBatch)
        {
            base.Draw(_spriteBatch);
            if (_shield._turnedOn == true)
            {
                _shield.Draw(_spriteBatch);
            }
        }
        protected override void Movement()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.W) && _currentSpd < _maxSpd)
            {
                _currentSpd += 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) && _currentSpd > 0)
            {
                _currentSpd -= 0.5f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _currentSpd = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _rotation -= MathHelper.ToRadians(2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _rotation += MathHelper.ToRadians(2);
            }
            _shield._rotation = _rotation +MathHelper.ToRadians(90);
            _direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            _direction.Normalize();
        }
        protected override void GravityPull()
        {
            foreach (CelestialBody body in Globals._celestialBodyList)
            {
                Vector2 _a = body._center;
                Vector2 _b = _position;
                Vector2 _c = new Vector2(_b.X, _a.Y);
                int _ac = (int)_c.X - (int)_a.X;
                int _bc = (int)_b.Y - (int)_c.Y;

                int _ab = (int)Math.Sqrt(_ac * _ac + _bc * _bc);
                if (_ab < body._gravityField)
                {
                    _gravity += (_b - _a) * -0.0002f * body._gravityPull * ((float)(body._gravityField - _ab) / (float)body._gravityField);// *body._gravityPull;
                    Console.WriteLine();
                }
                else
                {
                    _gravity -= _gravity * 0.005f;
                }
            }
        }
    }
    class EnemyShip : Ship
    {
        private Vector2 _directionToPlayer;
        private float _rotationToPlayer;
        private Texture2D _bulletTexture;
        private int _timer;
        public List<Bullet> _bulletList;

        public EnemyShip(Vector2 position, Texture2D texture, Texture2D bulletTexture)
        {
            _position = position;
            _texture = texture;
            _bulletTexture = bulletTexture;
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _origin = new Vector2(_texture.Width / 2, texture.Height / 2);

            _maxSpd = 13;
            _currentSpd = 0;
            _rotation = MathHelper.ToRadians(90);
            _particleList = new List<Particle>();
            _bulletList = new List<Bullet>();
        }

        public override void Update()
        {
            base.Update();
            if(_timer == -1 && Vector2.Distance(_position, Globals._player._position) < 1000)
            {
                _bulletList.Add(new Bullet(_position, _bulletTexture, Globals._player._position));
                if (Globals._gameMuted == false)
                {
                    int _sfxNumber = Globals._rnd.Next(0, Globals._enemyBulletSfxList.Count);
                    Globals._enemyBulletSfxList[_sfxNumber].Volume = Math.Max(1000 - Vector2.Distance(_position, Globals._player._position), 200) / 1000;
                    Globals._enemyBulletSfxList[_sfxNumber].Play();
                }
                _timer = 0;
            }

            if(_timer >= 50)
            {
                _timer = -1;
            }
            else if (_timer != -1)
            {
                _timer += Globals._rnd.Next(0, 3);
            }
            foreach(Bullet _bullet in _bulletList)
            {
                _bullet.Update();
            }
        }
        public override void Draw(SpriteBatch _spriteBatch)
        {
            foreach (Bullet _bullet in _bulletList)
            {
                _bullet.Draw(_spriteBatch);
            }
            base.Draw(_spriteBatch);
        }

        protected override void Movement()
        {
            _directionToPlayer = Globals._player._position - _position;
            if(Vector2.Distance(Globals._player._position, _position) > 500)
            {
                _rotationToPlayer = (float)Math.Atan2(_directionToPlayer.Y, _directionToPlayer.X);
            }
            else
            {
                _rotationToPlayer = (float)Math.Atan2(_directionToPlayer.Y, _directionToPlayer.X) + MathHelper.ToRadians(90);
            }
            if(_rotationToPlayer - _rotation >= MathHelper.ToRadians(180))
            {
                _rotation += MathHelper.Pi * 2;
            }

            if(_rotation != _rotationToPlayer && _rotation + MathHelper.ToRadians(2) < _rotationToPlayer)
            {
                _rotation += MathHelper.ToRadians(2);
            }
            else if (_rotation != _rotationToPlayer && _rotation - MathHelper.ToRadians(2) > _rotationToPlayer)
            {
                _rotation -= MathHelper.ToRadians(2);
            }
            _direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            _direction.Normalize();

            _currentSpd = 10;
        }
    }
}
