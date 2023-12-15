//TODO: make player input seperate UtilityObject

namespace AdventureGame
{
	internal class Player : Character
	{
		public PlayerStats stats;
		public Inventory inventory;

		public int Health { get { return health; } }
		public int MaxHealth { get; private set; }

		private int attackMod;
		public int Attack { get { return attack + attackMod; } }

		private int defenceMod;
		public int Defence { get { return defence + defenceMod; } }

		private int speedMod;
		public int Speed { get { return speed + speedMod; } }

		public int Satiety { get; private set; }


		public Player(World world, int xPos, int yPos, PlayerClass pClass, Tags tag = Tags.Default, string name = "")
			: base(world, xPos, yPos, tag, name)
		{
			switch (pClass)
			{
				/*case PlayerClass.template:
					stats = new(con, str, dex, agi, obs, aim, ste, mag);
					sprite = "@";
					break;*/

				case PlayerClass.fighter:
					stats = new(15, 13, 8, 8, 12, 10, 6, 0.5f);
					sprite = 'f';
					break;

				case PlayerClass.thief:
					stats = new(8, 6, 13, 10, 12, 12, 15, 0);
					sprite = 't';
					break;

				case PlayerClass.mage:
					stats = new(12, 8, 6, 10, 15, 13, 10, 2);
					sprite = 'm';
					break;

				case PlayerClass.acrobat:
					stats = new(12, 13, 12, 15, 8, 10, 6, 1);
					sprite = 'a';
					break;

				default:
					stats = new(10, 10, 10, 10, 10, 10, 10, 1);
					sprite = '@';
					break;
			}

			// TODO: make a proper item weight thingo words
			inventory = new((int)Math.Round(100 * stats.strengthStat.GetNetValue() * (2f/3f)));

			MaxHealth = (int)stats.constitutionStat.GetNetValue() * 10;
			health = MaxHealth;

			attack = (int)(stats.strengthStat.GetNetValue() >= stats.agilityStat.GetNetValue() ? stats.strengthStat.GetNetValue() : stats.agilityStat.GetNetValue());
			defence = (int)(stats.strengthStat.GetNetValue() >= stats.constitutionStat.GetNetValue() ? stats.strengthStat.GetNetValue() : stats.constitutionStat.GetNetValue());
			speed = (int)stats.agilityStat.GetNetValue();

			Satiety = 100;
		}

		protected override void MoveCollisionHandler_v(GameObject[] collisions)
		{
			foreach (GameObject collision in collisions)
			{
				if (collision is Enemy enemy)
				{
					enemy.Damage((int)stats.strengthStat.GetNetValue());
				}
				else if (collision is ItemEntity itemEntity)
				{
					if (inventory.AddItem(itemEntity.Item)) { itemEntity.Destroy(); }
				}
			}
		}
	}
}
