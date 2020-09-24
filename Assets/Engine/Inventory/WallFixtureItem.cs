using System.Collections.Generic;
using System.Linq;
using SS3D.Content.Systems.Construction;
using SS3D.Content.Systems.Interactions;
using SS3D.Engine.Interactions;
using SS3D.Engine.Inventory;
using SS3D.Engine.Tiles;
using UnityEngine;

namespace SS3D.Engine.Inventory
{
    public class WallFixtureItem : Item
    {
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private GameObject loadingBarPrefab;
        [SerializeField]
        private LayerMask obstacleMask;
        [SerializeField]
        private Fixture fixture;
        [SerializeField]
        private bool highWallFixture;
        private WallFixtureLayers layer;

        public override void CreateInteractions(IInteractionTarget[] targets, List<InteractionEntry> interactions)
        {
            base.CreateInteractions(targets, interactions);

            if ((IGameObjectProvider)targets[0] != null)
            {
                TileObject tileReference = ((IGameObjectProvider)targets[0]).GameObject.GetComponentInParent<TileObject>();
                int direction = tileReference.GetLayerDirectionByPlayerPosition(transform.position);

                switch (direction)
                {
                    case 1:
                        if (highWallFixture)
                        {
                            layer = WallFixtureLayers.HighWallNorth;
                        }
                        else
                        {
                            layer = WallFixtureLayers.LowWallNorth;
                        }
                        break;
                    case 2:
                        if (highWallFixture)
                        {
                            layer = WallFixtureLayers.HighWallSouth;
                        }
                        else
                        {
                            layer = WallFixtureLayers.LowWallSouth;
                        }
                        break;
                    case 3:
                        if (highWallFixture)
                        {
                            layer = WallFixtureLayers.HighWallEast;
                        }
                        else
                        {
                            layer = WallFixtureLayers.LowWallEast;
                        }
                        break;
                    case 4:
                        if (highWallFixture)
                        {
                            layer = WallFixtureLayers.HighWallWest;
                        }
                        else
                        {
                            layer = WallFixtureLayers.LowWallWest;
                        }
                        break;
                    default:
                        break;
                }

                if (direction != 0 && tileReference.Tile.turf.name.Substring(0,4).ToLower() == "wall")
                {
                    interactions.Insert(0, new InteractionEntry(targets[0], new FixtureConstructionInteraction
                    {
                        icon = icon,
                        Delay = 0.5f,
                        LoadingBarPrefab = loadingBarPrefab,

                        ObstacleMask = obstacleMask,
                        BuildDimensions = Vector3.zero,
                        RangeCheck = true,

                        Name = "Construct" + Name,
                        Fixture = fixture,
                        Overwrite = false,
                        WallLayer = layer
                    }));
                }
            }
        }
    }
}
