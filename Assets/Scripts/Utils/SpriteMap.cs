using UnityEngine;

[CreateAssetMenu(fileName = "SpriteMap", menuName = "Game/SpriteMap", order = 2)]
public class SpriteMap : ScriptableObject
{
    [SerializeField]
    SpriteDef[] m_sprites;

    public SpriteDef GetSpriteDef(string pName)
    {
        for (int i = 0; i < m_sprites.Length; i++)
        {
            if (m_sprites[i].Name.Equals(pName))
            {
                return m_sprites[i];
            }
        }
        return null;
    }

    public Sprite GetSprite(string pName)
    {
        var def = GetSpriteDef(pName);
        if (def != null)
        {
            return def.Sprite;
        }
        else
        {
            return null;
        }
    }
}

[System.Serializable]
public class SpriteDef
{
    [SerializeField]
    string m_spriteName;
    [SerializeField]
    Sprite m_sprite;

    public string Name { get { return m_spriteName; } }
    public Sprite Sprite { get { return m_sprite; } }
}