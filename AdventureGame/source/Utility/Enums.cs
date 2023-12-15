namespace AdventureGame
{
    public enum Tags
    {
        Default,
        Player,
        Enemy,
        Item,
        MainViewport
    }

    public enum ItemTypes
    {
        NoType, // if no types are specified in error, this will be the default value
        Accessory,
        Armour,
        Food,
        Key,
        Material,
        Potion,
        Scroll,
        Weapon
    }

    public enum WearableTypes
    {
        NoType, // if no types are specified in error, this will be the default value
        Head,
        Body,
        Legs,
        Feet
    }

    public enum WeaponTypes
    {
        Strength,
        Dexterity,
        Ranged
    }

    [Flags]
    public enum ItemFlags
    {
        Throwable = 1,
        Consumable = 2,
        TwoHanded = 4,
        Fragile = 8,
        Wearable = 16
    }

    
}
