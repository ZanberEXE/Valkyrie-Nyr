using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valkyrie_Nyr
{
    class Trigger : GameObject
    {
        string triggerType;

        public Trigger(string name, bool isStationary, int mass, int height, int width, Vector2 position, string _triggerType) : base(name, isStationary, true, mass, height, width, position)
        {
            triggerType = _triggerType;
        }
    }
}
