using ComNet;

namespace GameServer
{
    public abstract class Character
    {        
        public CharacterInfo characterInfo;

    }
    public class Barbarian : Character
    {
        public Barbarian()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Barbarian, MapTileInfo.MapTiles.Harbor, 5, 5, 2);
        }
    }
    public class DarkKnight : Character
    {
        public DarkKnight()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.DarkKnight, MapTileInfo.MapTiles.Harbor, 5, 5, 2);
        }
    }
    public class Mage : Character
    {
        public Mage()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Mage, MapTileInfo.MapTiles.City, 4, 6, 3);
        }
    }
    public class Ogre : Character
    {
        public Ogre()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Ogre, MapTileInfo.MapTiles.Harbor, 5, 5, 2);
        }
    }
    public class Assassin : Character
    {
        public Assassin()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Assassin, MapTileInfo.MapTiles.City, 4, 6, 3);
        }
    }
    public class Marksman : Character
    {
        public Marksman()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Marksman, MapTileInfo.MapTiles.Harbor, 5, 5, 2);
        }
    }
    public class Thief : Character
    {
        public Thief()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Thief, MapTileInfo.MapTiles.City, 4, 6, 3);
        }
    }
    public class Warlock : Character
    {
        public Warlock()
        {
            characterInfo = new CharacterInfo(CharacterInfo.Characters.Warlock, MapTileInfo.MapTiles.City, 4, 6, 3);
        }
    }
}
