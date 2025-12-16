using Microsoft.Xna.Framework;

namespace MonoFactory.Entities.Interfaces
{
    public interface IInteractable
    {
        Vector2 Position { get; }
        public void Interact(Hero hero);
    }
}
