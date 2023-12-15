using System.Text.Json;

namespace AdventureGame
{
    // TODO: rewrite this to make it look nicer pls lol
    internal struct Item
    {
        public bool equipped;

        public readonly ItemFlags[] flags;
        public readonly ItemTypes type;
        public readonly WearableTypes wearableType;


        public readonly string name;
        public readonly string description;
        public readonly string imageLocation;

        public readonly char sprite;

        public readonly long value;
        public readonly int weight;
        public readonly int? damage;
        public readonly int? attack;
        public readonly int? defence;
        public readonly int? speed;
        public readonly int? sustenance;


        private static readonly string root = "assets/items/";

        public Item(string itemJsonFile)
        {
            string itemJsonString = File.ReadAllText(root + itemJsonFile);
            ItemParser itemJson = JsonSerializer.Deserialize<ItemParser>(itemJsonString); //TODO: add a try|catch here

            flags = new ItemFlags[itemJson.Flags.Count];
            for (int i = 0; i < itemJson.Flags.Count; i++)
            {
                if (!Enum.TryParse(itemJson.Flags[i], true, out flags[i])) {/*log incorrect flag error*/}
            }

            if (!Enum.TryParse(itemJson.Type, true, out type)) { /*log incorrect type error*/ }

            if (!Enum.TryParse(itemJson.WearableType, true, out wearableType)) { /*log incorrect type error*/ }

            name = itemJson.Name is null ? "NULL" : itemJson.Name;
            description = itemJson.Description is null ? "NULL" : itemJson.Description;
            imageLocation = itemJson.ImageLocation is null ? "NULL" : root + itemJson.ImageLocation;

            sprite = itemJson.Sprite;

            value = itemJson.Value;
            weight = itemJson.Weight;
            damage = itemJson.Damage;
            attack = itemJson.Attack;
            defence = itemJson.Defence;
            speed = itemJson.Speed;
            sustenance = itemJson.Sustenance;

            equipped = false;
        }

        public Item(ItemFlags[] flags, ItemTypes type, WearableTypes wearableType, string name, string description, string imageLocation, char sprite, long value, int weight, int? damage, int? attack, int? defence, int? speed, int? sustenance, bool equipped)
        {
            this.flags = flags;
            this.type = type;
            this.wearableType = wearableType;

            this.name = name;
            this.description = description;
            this.imageLocation = imageLocation;

            this.sprite = sprite;

            this.value = value;
            this.weight = weight;
            this.damage = damage;
            this.attack = attack;
            this.defence = defence;
            this.speed = speed;
            this.sustenance = sustenance;

            this.equipped = equipped;
        }

        public Item InstantiateNew()
        {
            return new(flags, type, wearableType, name, description, imageLocation, sprite, value, weight, damage, attack, defence, speed, sustenance, equipped);
        }
    }
}
