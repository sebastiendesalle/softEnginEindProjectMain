using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Components;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Entities.Machines.States;
using MonoFactory.Items;
using MonoFactory.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoFactory.Entities.Machines
{
    public class Machine : IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        public Texture2D Texture { get; private set; }
        public WorldManager World{ get; private set; }

        // state pattern
        public IMachineState CurrentState { get; private set; }

        private List<IItem> _inputBuffer = new List<IItem>();
        private List<Recipe> _recipes = new List<Recipe>();

        public string InputItemName { get; } = "Iron Ore_1";
        public IItem OutputItem { get; } = new ResourceItem("Iron Bar");
        public float ProcessTime { get; } = 3.0f;

        private int _width = 64;
        private int _height = 100;

        public float Scale { get; private set; } = 2.0f;

        public Rectangle SourceRect { get; set; }
        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)(_width * Scale), (int)(_height* Scale));

        public Texture2D ItemTexture { get; private set; }
        public Texture2D SwordTexture { get; private set; }
        public Rectangle OutputItemSourceRect { get; private set; }

        private string _machineType;

        public Machine(Texture2D texture, Vector2 position, WorldManager world, Texture2D itemTexture, string machineType, Texture2D swordTexture)
        {
            Texture = texture;
            Position = position;
            World = world;
            ItemTexture = itemTexture;
            SwordTexture = swordTexture;
            _machineType = machineType;

            OutputItemSourceRect = new Rectangle(4 * 32, 11 * 32, 32, 32);

            SourceRect = new Rectangle(0, 412, 64, 100);

            InitializeRecipes();
            SetState(new WaitingForInputState());

        }

        public (Texture2D texture, Rectangle sourceRect, float scale) GetItemVisuals(IItem item)
        {
            if (item is WeaponItem weapon)
            {
                int spriteSize = 128;
                int column = Math.Min(weapon.Level - 1, 3);
                int row = 4;

                Rectangle sourceRect = new Rectangle(column * spriteSize, row * spriteSize, spriteSize, spriteSize);
                return (SwordTexture, sourceRect, 32f / spriteSize);
            }
            else
            {
                return (ItemTexture, OutputItemSourceRect, 32f / 350f);
            }
        }

        private void InitializeRecipes()
        {

            if (_machineType == "Furnace")
            {
                var smeltRecipe = new Recipe(
                    new List<string> { "Iron Ore_1" },
                    new ResourceItem("Iron Bar")
                );
                _recipes.Add(smeltRecipe);
                Debug.WriteLine("Initialized Furnace recipes.");
            }
            else if (_machineType == "Anvil")
            {
                var swordRecipe = new Recipe(
                new List<String> { "Stick_1", "Iron Bar_1", "Iron Bar_1" },
                new WeaponItem("Iron Sword", 1, 2)
                );
                _recipes.Add(swordRecipe);

                var upgrade2Recipe = new Recipe(
                new List<String> { "Iron Sword_1", "Iron Sword_1" },
                new WeaponItem("Iron Sword", 2, 4)
                );
                _recipes.Add(upgrade2Recipe);

                var upgrade3Recipe = new Recipe(
                new List<String> { "Iron Sword_2", "Iron Sword_2" },
                new WeaponItem("Iron Sword", 3, 6)
                );
                _recipes.Add(upgrade3Recipe);

                var upgrade4Recipe = new Recipe(
                  new List<String> { "Iron Sword_3", "Iron Sword_3" },
                new WeaponItem("Iron Sword", 3, 10)
                );
                _recipes.Add(upgrade4Recipe);
                Debug.WriteLine(" Initialized Anvil recipes.");
            }

        }

        public void SetState(IMachineState newState)
        {
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        public void AddToBuffer(IItem item)
        {
            _inputBuffer.Add(item);
        }

        public void ClearBuffer()
        {
            _inputBuffer.Clear();
        }

        public List<IItem> GetBuffer()
        {
            return new List<IItem>(_inputBuffer);
        }

        public bool IsIngredient(string itemId)
        {
            return _recipes.Any(r => r.Ingredients.Contains(itemId));
        }

        public bool TryCraft()
        {
            List<String> bufferIds = _inputBuffer.Select(i => i.GetId()).ToList();

            foreach (var recipe in _recipes)
            {
                if (recipe.Matches(bufferIds))
                {
                    _inputBuffer.Clear();
                    SetState(new ProcessingState(ProcessTime, recipe.Result));
                    return true;
                }
            }
            return false;
        }

        public bool CouldMatchRecipe(List<string> testBufferIds)
        {
            foreach (var recipe in _recipes)
            {
                var testCounts = testBufferIds.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
                var recipeCounts = recipe.Ingredients.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

                bool couldMatch = true;
                foreach (var testItem in testCounts)
                {
                    if (!recipeCounts.ContainsKey(testItem.Key) || testItem.Value > recipeCounts[testItem.Key])
                    {
                        couldMatch = false;
                        break;
                    }
                }

                if (couldMatch)
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            CurrentState.Update(gameTime, this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(Texture, Position, SourceRect, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            CurrentState.Draw(spriteBatch, this);
        }

        public void Interact(Hero hero)
        {
            CurrentState.Interact(hero, this);
        }
    }
}
