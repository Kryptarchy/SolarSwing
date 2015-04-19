using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SolarSwing
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SolarSwing : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        View _view;
        float _desiredZoom;
        float _previousScrollValue;

        enum GameState
        {
            Start,
            Game,
            Paused,
            GameOver,
            Highscore
        }

        GameState _gameState;
        Texture2D _start;
        Texture2D _paused;
        Texture2D _gameOver;

        bool _mReleased;
        bool _pReleased;
        bool _enterReleased;
        int _spawnRate;
        float _spawnRateTimer;
        float _spawnTimer;

        List<SoundEffectInstance> _hurtSfxList;
        List<SoundEffectInstance> _explosionSfxList;

        public SolarSwing()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            MediaPlayer.Play(Content.Load<Song>("theme"));
            MediaPlayer.Volume = 0.75f;
            MediaPlayer.IsRepeating = true;
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
            _view = new View();
            _view.Position = new Vector2(500, 200);
            _gameState = GameState.Start;
            _mReleased = true;
            _pReleased = true;
            _enterReleased = true;
            _spawnRate = 2;
            _spawnRateTimer = 0;
            _spawnTimer = 0;
            _desiredZoom = 1;
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _start = Content.Load<Texture2D>("start");
            _paused = Content.Load<Texture2D>("paused");
            _gameOver = Content.Load<Texture2D>("gameOver");
            _hurtSfxList = new List<SoundEffectInstance>();
            _explosionSfxList = new List<SoundEffectInstance>();
            _hurtSfxList.Add(Content.Load<SoundEffect>("hurt").CreateInstance());
            _hurtSfxList.Add(Content.Load<SoundEffect>("hurt2").CreateInstance());
            _hurtSfxList.Add(Content.Load<SoundEffect>("hurt3").CreateInstance());
            for (int i = 0; i < _hurtSfxList.Count; i++)
            {
                _hurtSfxList[i].Volume = 0.5f;
            }

            _explosionSfxList.Add(Content.Load<SoundEffect>("explosion").CreateInstance());
            _explosionSfxList.Add(Content.Load<SoundEffect>("explosion2").CreateInstance());
            _explosionSfxList.Add(Content.Load<SoundEffect>("explosion3").CreateInstance());
            _explosionSfxList.Add(Content.Load<SoundEffect>("explosion4").CreateInstance());
            for (int i = 0; i < _explosionSfxList.Count; i++)
            {
                _explosionSfxList[i].Volume = 0.5f;
            }

            Globals._player = new PlayerShip(new Vector2(500, 500), Content.Load<Texture2D>("player"), Content.Load<Texture2D>("playerCharging"),
                                                Content.Load<Texture2D>("playerBoosting"), Content.Load<Texture2D>("shield"),
                                                Content.Load<Texture2D>("shieldCharging"), Content.Load<Texture2D>("shieldBoosting"));
            Globals._sun = new Sun(new Vector2(0, 0), Content.Load<Texture2D>("sun"));
            Globals._celestialBodyList.Add(Globals._sun);
            Globals._celestialBodyList.Add(new SmallPlanet(new Vector2(1000, 0), Content.Load<Texture2D>("mercury")));
            Globals._celestialBodyList.Add(new MediumPlanet(new Vector2(-650, 2420), Content.Load<Texture2D>("venus")));
            Globals._celestialBodyList.Add(new MediumPlanet(new Vector2(2835, -4835), Content.Load<Texture2D>("earth")));
            Globals._mars = new SmallPlanet(new Vector2(-4215, -4280), Content.Load<Texture2D>("mars"));
            Globals._celestialBodyList.Add(Globals._mars);
            //Globals._enemyShipList.Add(new EnemyShip(Globals._mars._center, Content.Load<Texture2D>("enemy"), Content.Load<Texture2D>("bullet")));
            Globals.font = Content.Load<SpriteFont>("SpriteFont1");
            Globals._enemyBulletSfxList.Add(Content.Load<SoundEffect>("enemyBullet").CreateInstance());
            Globals._enemyBulletSfxList.Add(Content.Load<SoundEffect>("enemyBullet2").CreateInstance());
            Globals._enemyBulletSfxList.Add(Content.Load<SoundEffect>("enemyBullet3").CreateInstance());
            Globals._enemyBulletSfxList.Add(Content.Load<SoundEffect>("enemyBullet4").CreateInstance());
            Globals._enemyBulletSfxList.Add(Content.Load<SoundEffect>("enemyBullet5").CreateInstance());

            
            for (int i = 0; i < Globals._enemyBulletSfxList.Count; i++)
            {
                Globals._enemyBulletSfxList[i].Volume = 0.5f;
            }

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if(Keyboard.GetState().IsKeyUp(Keys.P))
            {
                _pReleased = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                _enterReleased = true;
            }
            switch(_gameState)
            {
                case GameState.Start:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && _enterReleased == true)
                    {
                        _gameState = GameState.Game;
                    }
                    break;
                case GameState.Game:
                    if(Keyboard.GetState().IsKeyDown(Keys.P) && _pReleased == true)
                    {
                        _gameState = GameState.Paused;
                        _pReleased = false;
                    }
                    Globals._player.Update();
                    CollisionChecks();
                    foreach (EnemyShip _enemy in Globals._enemyShipList)
                    {
                        _enemy.Update();
                    }
                    foreach (CelestialBody body in Globals._celestialBodyList)
                    {
                        body.Update();
                    }
                    if (_spawnRate <= 10)
                    {
                        _spawnRateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (_spawnRateTimer >= 20)
                        {
                            _spawnRate++;
                            _spawnRateTimer -= 20;
                        }
                    }
                    _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_spawnTimer >= 3)
                    {
                        if (Globals._enemyShipList.Count < _spawnRate)
                        {
                            Globals._enemyShipList.Add(new EnemyShip(Globals._mars._center, Content.Load<Texture2D>("enemy"), Content.Load<Texture2D>("bullet")));
                        }
                        _spawnTimer -= 3;
                    }

                    break;
                case GameState.Paused:
                    if (Keyboard.GetState().IsKeyDown(Keys.P) && _pReleased == true)
                    {
                        _gameState = GameState.Game;
                        _pReleased = false;
                    }
                    break;
                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        _gameState = GameState.Start;
                        _enterReleased = false;
                        Globals._celestialBodyList.Clear();
                        Globals._enemyShipList.Clear();
                        Initialize();
                        LoadContent();
                    }
                    break;
            }

            _view.Position = Globals._player._position;
            if (_previousScrollValue > Mouse.GetState().ScrollWheelValue && _desiredZoom >= 0.1f)
            {
                _desiredZoom -= 0.1f;
                //_view.Zoom -= 0.1f;
            }
            if (_previousScrollValue < Mouse.GetState().ScrollWheelValue && _desiredZoom <= 1)
            {
                _desiredZoom += 0.1f;
                //_view.Zoom += 0.1f;
            }
            if(_view.Zoom > _desiredZoom)
            {
                _view.Zoom -= 0.01f;
            }
            if(_view.Zoom < _desiredZoom)
            {
                _view.Zoom += 0.01f;
            }

            _previousScrollValue = Mouse.GetState().ScrollWheelValue;
            

            if (MediaPlayer.PlayPosition > TimeSpan.FromSeconds(100) && MediaPlayer.Volume >= 0)
            {
                MediaPlayer.Volume -= 0.002f;
            }
            else if (MediaPlayer.Volume <= 0.7f)
            {
                MediaPlayer.Volume += 0.002f;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.M) && _mReleased == true)
            {
                if (Globals._gameMuted == false)
                {
                    MediaPlayer.Pause();
                    MediaPlayer.Volume = 0;
                    Globals._gameMuted = true;
                }
                else
                {
                    MediaPlayer.Resume();
                    Globals._gameMuted = false;
                }
                _mReleased = false;
            }
            else if(Keyboard.GetState().IsKeyUp(Keys.M))
            {
                _mReleased = true;
            }
            //_view.Rotation = _player._rotation;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void CollisionChecks()
        {
            for (int i = 0; i < Globals._enemyShipList.Count; i++)
            {
                for (int j = 0; j < Globals._enemyShipList[i]._bulletList.Count; j++)
                {
                    if (Globals._enemyShipList[i]._bulletList[j]._life <= 0)
                    {
                        Globals._enemyShipList[i]._bulletList.RemoveAt(j);
                        j--;
                    }
                    else if (Globals._player._shield._turnedOn == true && PixelPerfectCollision(Globals._enemyShipList[i]._bulletList[j]._texture, Globals._player._shield._texture, Globals._enemyShipList[i]._bulletList[j]._rectangle, Globals._player._shield._rectangle))
                    {
                        Globals._enemyShipList[i]._bulletList.RemoveAt(j);
                        j--;
                    }
                    else if (Globals._player._shield._turnedOn == false && PixelPerfectCollision(Globals._enemyShipList[i]._bulletList[j]._texture, Globals._player._texture, Globals._enemyShipList[i]._bulletList[j]._rectangle, Globals._player._rectangle))
                    {
                        Globals._enemyShipList[i]._bulletList.RemoveAt(j);
                        if (Globals._gameMuted == false)
                        {
                            _hurtSfxList[Globals._rnd.Next(0, _hurtSfxList.Count)].Play();
                        }
                        Globals._player._hp -= 20;
                        if (Globals._player._hp <= 0)
                        {
                            if (Globals._gameMuted == false)
                            {
                                _explosionSfxList[Globals._rnd.Next(0, _explosionSfxList.Count)].Play();
                            }
                            _gameState = GameState.GameOver;
                        }
                        j--;
                    }
                }
                if (Globals._player._shield._turnedOn == true && PixelPerfectCollision(Globals._enemyShipList[i]._texture, Globals._player._shield._texture, Globals._enemyShipList[i]._rectangle, Globals._player._shield._rectangle))
                {
                    Globals._enemyShipList.RemoveAt(i);
                    Globals._player._score += 1000;
                    if (Globals._gameMuted == false)
                    {
                        _explosionSfxList[Globals._rnd.Next(0, _explosionSfxList.Count)].Play();
                    }
                    i--;
                }
            }
        }
        public bool PixelPerfectCollision(Texture2D texture1, Texture2D texture2, Rectangle rectangle1, Rectangle rectangle2)
        {
            //Gets the colordata of the two textures.
            Color[] colorData1 = new Color[texture1.Width * texture1.Height];
            Color[] colorData2 = new Color[texture2.Width * texture2.Height];
            texture1.GetData<Color>(colorData1);
            texture2.GetData<Color>(colorData2);

            //Finds the area to check for collision.
            int top = Math.Max(rectangle1.Top, rectangle2.Top);
            int bottom = Math.Min(rectangle1.Bottom, rectangle2.Bottom);
            int left = Math.Max(rectangle1.Left, rectangle2.Left);
            int right = Math.Min(rectangle1.Right, rectangle2.Right);

            //Goes from top to bottom and left to right in search of two pixels that are not transparent on the same spot.
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color a = colorData1[(y - rectangle1.Top) * (rectangle1.Width) + (x - rectangle1.Left)];
                    Color b = colorData2[(y - rectangle2.Top) * (rectangle2.Width) + (x - rectangle2.Left)];

                    if (a.A != 0 && b.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color((int)36, (int)33, (int)33));

            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        _view.get_transformation(_graphics.GraphicsDevice));
            
            
            
            foreach(CelestialBody body in Globals._celestialBodyList)
            {
                body.Draw(_spriteBatch);
            }
            foreach (EnemyShip _enemy in Globals._enemyShipList)
            {
                _enemy.Draw(_spriteBatch);
            }
            Globals._player.Draw(_spriteBatch);
            
            _spriteBatch.End();

            _spriteBatch.Begin();
            
            switch(_gameState)
            {
                case GameState.Start:
                    _spriteBatch.Draw(_start, new Vector2(0, 0), Color.White);
                    break;
                case GameState.Paused:
                    _spriteBatch.DrawString(Globals.font, "HP: " + Globals._player._hp, new Vector2(50, 25), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Score: " + Globals._player._score, new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Energy: " + Globals._player._energy, new Vector2(50, 75), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Speed: " + (int)Globals._player._spd, new Vector2(50, 100), Color.White);
                    _spriteBatch.Draw(_paused, new Vector2(0, 0), Color.White);
                    break;
                case GameState.Game:
                    _spriteBatch.DrawString(Globals.font, "HP: " + Globals._player._hp, new Vector2(50, 25), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Score: " + Globals._player._score, new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Energy: " + Globals._player._energy, new Vector2(50, 75), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Speed: " + (int)Globals._player._spd, new Vector2(50, 100), Color.White);
                    break;
                case GameState.GameOver:
                    _spriteBatch.DrawString(Globals.font, "HP: " + Globals._player._hp, new Vector2(50, 25), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Score: " + Globals._player._score, new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Energy: " + Globals._player._energy, new Vector2(50, 75), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Speed: " + (int)Globals._player._spd, new Vector2(50, 100), Color.White);
                    _spriteBatch.Draw(_gameOver, new Vector2(0, 0), Color.White);
                    _spriteBatch.DrawString(Globals.font, "Final Score: " + (int)Globals._player._score, new Vector2(540, 420), Color.White);
                    break;

            }
            
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
