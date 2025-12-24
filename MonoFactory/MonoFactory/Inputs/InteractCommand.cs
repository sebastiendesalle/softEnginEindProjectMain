using MonoFactory.Entities;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Inputs
{
    public class InteractCommand: ICommand
    {
        private WorldManager _world;
        private float _radius;

        public InteractCommand(WorldManager world, float interactionRadius = 200f)
        {
            _world = world;
            _radius = interactionRadius;
        }

        public void Execute(Hero hero)
        {
            IInteractable target = _world.GetNearestInteractable(hero.Position, _radius);

            if (target != null)
            {
                Debug.WriteLine($"Interacting with object at: {target.Position}");
                target.Interact(hero);
            }
            else
            {
                Debug.WriteLine("Nothing to interact with");
            }
        }
    }
}
