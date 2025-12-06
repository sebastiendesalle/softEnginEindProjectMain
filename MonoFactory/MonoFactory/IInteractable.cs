using Microsoft.Xna.Framework;

namespace MonoFactory
{
    public interface IInteractable
    {
        Vector2 Position { get; }

        void Interact(Hero hero);
    }
}
