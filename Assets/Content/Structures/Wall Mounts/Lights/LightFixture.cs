using SS3D.Content.Systems.Construction;
using SS3D.Content.Systems.Interactions;
using SS3D.Engine.Interactions;
using SS3D.Engine.Interactions.Extensions;
using SS3D.Engine.Inventory;
using SS3D.Engine.Tiles;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace SS3D.Content.Structures.WallMounts.Lights
{
    public class LightFixture : InteractionTargetNetworkBehaviour
    {
        // Broken and normal objects for light fixture
        [SerializeField]
        private GameObject brokenLight;
        [SerializeField]
        private GameObject normalLight;
        // Items and prefabs used for installing and dropping
        [SerializeField]
        private GameObject lightItemPrefab;
        private Item lightItem;
        [SerializeField]
        private GameObject lightBrokenItemPrefab;
        [SerializeField]
        private GameObject lightFixtureItemPrefab;
        // Set default state of light fixtures
        [SerializeField]
        private bool lightInstalled = true;
        [SerializeField]
        private bool lightBroken = false;
        // Interaction related variables
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private GameObject loadingBarPrefab;
        [SerializeField]
        private LayerMask obstacleMask;
        private WallFixtureLayers layer;

        private void Start()
        {
            lightItem = lightItemPrefab.GetComponent<Item>();

            // Set fixture related variables
            string layerString = name.Split('_')[2];
            switch (layerString)
            {
                case "highwalleast":
                    layer = WallFixtureLayers.HighWallEast;
                    break;
                case "highwallnorth":
                    layer = WallFixtureLayers.HighWallNorth;
                    break;
                case "highwallsouth":
                    layer = WallFixtureLayers.HighWallSouth;
                    break;
                case "highwallwest":
                    layer = WallFixtureLayers.HighWallWest;
                    break;
                case "lowwalleast":
                    layer = WallFixtureLayers.LowWallEast;
                    break;
                case "lowwallnorth":
                    layer = WallFixtureLayers.LowWallNorth;
                    break;
                case "lowwallsouth":
                    layer = WallFixtureLayers.LowWallSouth;
                    break;
                case "lowwallwest":
                    layer = WallFixtureLayers.LowWallWest;
                    break;
                default:
                    Debug.LogWarning("Light fixture at tile " + transform.parent.name + " has unknown wall fixture layer. Defaulting to HighWallNorth");
                    layer = WallFixtureLayers.HighWallNorth;
                    break;
            }

            // Make sure default state is the actual state of the object
            normalLight.SetActive(lightInstalled && !lightBroken);
            brokenLight.SetActive(lightInstalled && lightBroken);
        }

        public override IInteraction[] GenerateInteractions(InteractionEvent interactionEvent)
        {
            List<IInteraction> interactions = new List<IInteraction>();

            // If hand is empty and fixture has light, remove light interaction
            if (interactionEvent.GetSourceItem() == null)
            {
                if (lightInstalled)
                {
                    interactions.Add(new SimpleInteraction
                    {
                        Name = "Remove light",
                        Interact = ChangeLight,
                        RangeCheck = true
                    });
                }
            }
            else
            {
                // If hand has the correct bulb type and fixture is empty, install light interaction
                if (interactionEvent.GetSourceItem().Name == lightItem.Name && !lightInstalled)
                {
                    interactions.Add(new SimpleInteraction
                    {
                        Name = "Install light",
                        Interact = ChangeLight,
                        RangeCheck = true
                    });
                }

                // If hand has screwdriver and fixture is empty, remove fixture interaction
                if (interactionEvent.GetSourceItem().Name == "Screwdriver" && !lightInstalled)
                {
                    interactions.Add(new FixtureConstructionInteraction
                    {
                        icon = icon,
                        Delay = 0.5f,
                        LoadingBarPrefab = loadingBarPrefab,

                        ObstacleMask = obstacleMask,
                        BuildDimensions = Vector3.zero,
                        RangeCheck = true,

                        Name = "Remove light fixture",
                        Fixture = null,
                        Overwrite = true,
                        WallLayer = layer
                    });
                }
            }

            return interactions.ToArray();
        }

        private void ChangeLight(InteractionEvent interactionEvent, InteractionReference arg2)
        {
            if (lightInstalled)
            {
                if (lightBroken)
                {
                    Item light = ItemHelpers.CreateItem(lightBrokenItemPrefab);
                    interactionEvent.Source.GetHands().Pickup(light.gameObject);

                    brokenLight.SetActive(false);
                }
                else
                {
                    Item light = ItemHelpers.CreateItem(lightItemPrefab);
                    interactionEvent.Source.GetHands().Pickup(light.gameObject);

                    normalLight.SetActive(false);
                }

                lightInstalled = false;
            }
            else
            {
                interactionEvent.Source.GetHands().DestroyHeldItem();

                lightInstalled = true;
                normalLight.SetActive(true);
            }
        }
    }
}