using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Sprites;
using Camera;

namespace StateManagement
{
    public class MapObjectManager
    {
        private TiledMap _map;

        private TiledMapRenderer _mapRenderer;

        private ContentManager _content;

        private ScreenManager _sm;

        public List<RoadBlock> RoadBlocks { get; private set; }

        public List<SpeedBoost> SpeedBoosts { get; private set; }

        public Queue<Vector2> Waypoints { get; private set; }

        public Vector2 playerCarPosition { get; private set; }

        public Vector2 opponentCarPosition { get; private set; }

        public MapObjectManager(ScreenManager sm)
        {
            _sm = sm;
            RoadBlocks = new List<RoadBlock>();
            Waypoints = new Queue<Vector2>();
            SpeedBoosts = new List<SpeedBoost>();
            _content = new ContentManager(_sm.Game.Services, "Content");
        }

        public void LoadMap(string mapName)
        {
            _map = _content.Load<TiledMap>(mapName);
            _mapRenderer = new TiledMapRenderer(_sm.Game.GraphicsDevice, _map);
            RoadBlock.LoadContent(_content);
            SpeedBoost.LoadContent(_content);

            foreach(var layer in _map.Layers)
            {
                if(layer is TiledMapObjectLayer objectLayer)
                {
                    ParseObjectLayer(objectLayer);
                }
            }
        }

        private void ParseObjectLayer(TiledMapObjectLayer objectLayer)
        {
            foreach(var obj in objectLayer.Objects)
            {

                switch(obj.Name)
                {
                    case "RoadBlock":
                        RoadBlocks.Add(new RoadBlock(obj.Position));
                        break;
                    case "Waypoint":
                        Waypoints.Enqueue(obj.Position);
                        break;
                    case "SpeedBoost":
                        SpeedBoosts.Add(new SpeedBoost(obj.Position));
                        break;
                    case "OpponentCar":
                        opponentCarPosition = obj.Position;
                        break;
                    case "PlayerCar":
                        playerCarPosition = obj.Position;
                        break;
                }
            }
        }

        public void Draw(CarCamera camera, SpriteBatch spriteBatch)
        {
            _mapRenderer.Draw(camera.GetViewMatrix());

            foreach(var rb in RoadBlocks)
            {
                rb.Draw(spriteBatch);
            }

            foreach(var sb in SpeedBoosts)
            {
                sb.Draw(spriteBatch);
            }
        }
    }
}
