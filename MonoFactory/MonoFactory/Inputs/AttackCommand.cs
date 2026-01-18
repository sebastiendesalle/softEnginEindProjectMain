using Microsoft.Xna.Framework;
using MonoFactory.Entities;
using MonoFactory.Managers;
using System.Diagnostics;


namespace MonoFactory.Inputs
{
    public class AttackCommand : ICommand
    {

        private WorldManager _world;
        private float _range;

        public AttackCommand(WorldManager world, float range = 100f)
        {
            _world = world;
            _range = range;
        }
        public void Execute(Hero hero)
        {
            int damage = 1;

            var weapon = hero.Inventory.GetBestWeapon();

            if (weapon != null)
            {
                damage = weapon.Damage;
            }

            Vector2 attackOrigin = hero.Rectangle.Center.ToVector2();

            _world.DamageEntitiesInArea(attackOrigin, _range, damage, hero);
        }
    }
}
