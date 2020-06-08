using ComNet;

namespace GameServer
{
    public abstract class Character
    {        
        public CharacterInfo characterInfo;

    }
    public class Warrior : Character
    {
        public Warrior()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Warrior, MapTileInfo.MapTiles.Harbor, 5, 5, 2);
        }
    }
}
