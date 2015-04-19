using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarSwing
{
    class Globals
    {
        public static List<CelestialBody> _celestialBodyList = new List <CelestialBody>();
        public static List<EnemyShip> _enemyShipList = new List<EnemyShip>();
        public static PlayerShip _player;
        public static Sun _sun;
        public static SmallPlanet _mars;
        public static SpriteFont font;
        public static Random _rnd = new Random();
        public static List<SoundEffectInstance> _enemyBulletSfxList = new List<SoundEffectInstance>();
        public static bool _gameMuted;
    }
}
