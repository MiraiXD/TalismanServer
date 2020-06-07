using ComNet;

namespace GameServer
{
    public abstract class Character
    {
        
        public CharacterInfo characterInfo;
        
    }
    public class Warrior : Character
    {
        public Warrior(MapTileInfo startingTile)
        {            
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Warrior,startingTile, 5, 5, 2);
        }
    }
}
