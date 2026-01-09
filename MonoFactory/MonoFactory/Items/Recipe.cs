using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public class Recipe
    {
        public List<String> Ingredients { get; private set; }

        public IItem Result { get; private set; }

        public Recipe(List<string> ingredients, IItem result)
        {
            Ingredients = ingredients;
            Result = result;
        }

        public bool Matches(List<string> inputItemIds)
        {
            if (inputItemIds.Count != Ingredients.Count)
            {
                return false;
            }

            var sortedIngredients = Ingredients.OrderBy(x => x).ToList();
            var sortedInput = inputItemIds.OrderBy(x => x).ToList();

            return sortedIngredients.SequenceEqual(sortedInput);
        }
    }
}
