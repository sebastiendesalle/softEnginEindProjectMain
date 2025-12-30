using Microsoft.Xna.Framework;
using MonoFactory.Entities;
using MonoFactory.Managers;


namespace MonoFactory.Inputs
{
    public class AttackCommand : ICommand
    {

        private WorldManager _world;
        private int _damage;
        private float _range;

        public AttackCommand(WorldManager world, int damage = 1, float range = 100f)
        {
            _world = world;
            _damage = damage;
            _range = range;
        }
        public void Execute(Hero hero)
        {
            Vector2 attackOrigin = hero.Rectangle.Center.ToVector2();

            _world.DamageEntitiesInArea(attackOrigin, _range, _damage, hero);
        }
    }
}
