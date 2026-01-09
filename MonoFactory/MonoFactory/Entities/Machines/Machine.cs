using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Entities.Machines.States;
using MonoFactory.Items;
using MonoFactory.Managers;
using MonoFactory.Components;
using System;
using System.Collections.Generic;
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
        public Rectangle OutputItemSourceRect { get; private set; }

        public Machine(Texture2D texture, Vector2 position, WorldManager world, Texture2D itemTexture)
        {
            Texture = texture;
            Position = position;
            World = world;
            ItemTexture = itemTexture;

            OutputItemSourceRect = new Rectangle(4 * 32, 11 * 32, 32, 32);

            SourceRect = new Rectangle(0, 412, 64, 100);

            SetState(new WaitingForInputState());
        }

        private void InitializeRecipes()
        {
            var swordRecipe = new Recipe(
                new List<String> { "Stick_1", "Iron Ore_1", "Iron Ore_1" },
                new WeaponItem("Iron Sword", 1, 2)
            );
            _recipes.Add(swordRecipe);

            var upgradeRecipe = new Recipe(
            new List<String> { "Iron Sword_1", "Iron Sword_1" },
            new WeaponItem("Iron Sword", 2, 4)
            );
            _recipes.Add(upgradeRecipe);
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
