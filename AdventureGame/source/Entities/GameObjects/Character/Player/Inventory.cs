namespace AdventureGame
{
	internal struct Inventory
	{
		public List<Item> Items { get; private set; }

		public Item? Head { get; private set; }
		public Item? Body { get; private set; }
		public Item? Legs { get; private set; }
		public Item? Feet { get; private set; }
		public Item? MainHand { get; private set; }
		public Item? OffHand { get; private set; }
		public Item? Accessory1 { get; private set; }
		public Item? Accessory2 { get; private set; }

		public int WeightCap { get; private set; }
		public int CurrentWeight
        {
			get
			{
				int totalWeight = 0;
				foreach (Item i in Items) { totalWeight += i.weight; }

				return totalWeight;
			}
        }
		public ulong Money { get; private set; }

		public Inventory(int weightCap)
		{
			Items = new List<Item>();

			Head = null;
			Body = null;
			Legs = null;
			Feet = null;
			MainHand = null;
			OffHand = null;
			Accessory1 = null;
			Accessory2 = null;

			WeightCap = weightCap;
			Money = 0;
		}

		public void AddCapacity(int amount)
		{
			WeightCap += amount;
		}

		public void AddMoney(ulong amount)
		{
			Money += amount;
		}

		public bool Equip(Item item)
        {
			if (item.type == ItemTypes.Accessory)
			{
				if (Accessory1 is null) { Accessory1 = item; return true; }
				if (Accessory2 is null) { Accessory2 = item; return true; }
				return false;
			}
            if(item.type == ItemTypes.Armour)
            {
                switch (item.wearableType)
                {
                    case WearableTypes.Head:
						if(Head is null) { Head = item; return true; }
						return false;

					case WearableTypes.Body:
						if(Body is null) { Body = item; return true; }
						return false;

					case WearableTypes.Legs:
						if(Legs is null) { Legs = item; return true; }
						return false;

					case WearableTypes.Feet:
						if(Feet is null) { Feet = item; return true; }
						return false;

					default:
						return false;
                }
            }
			if(MainHand is null) { MainHand = item; return true; }
			if(OffHand is null) { OffHand = item; return true; }
			return false;
        }

		public bool Dequip(Item item)
        {
			if(Head is not null && ((Item)Head).Equals(item)) { Head = null; return true; }
			if(Body is not null && ((Item)Body).Equals(item)) { Body = null; return true; }
			if(Legs is not null && ((Item)Legs).Equals(item)) { Legs = null; return true; }
			if(Feet is not null && ((Item)Feet).Equals(item)) { Feet = null; return true; }
			if(MainHand is not null && ((Item)MainHand).Equals(item)) { MainHand = null; return true; }
			if(OffHand is not null && ((Item)OffHand).Equals(item)) { OffHand = null; return true; }
			if(Accessory1 is not null && ((Item)Accessory1).Equals(item)) { Accessory1 = null; return true; }
			if(Accessory2 is not null && ((Item)Accessory2).Equals(item)) { Accessory2 = null; return true; }
			return false;
		}

		public bool AddItem(Item item)
		{
			if (item.weight + CurrentWeight <= WeightCap) { Items.Add(item); return true; }
			return false;
		}
	}
}
