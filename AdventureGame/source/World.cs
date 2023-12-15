using System.Drawing;

namespace AdventureGame
{
	internal class World
	{
		// i could chuck everything into one list, but then i'd have to perform tons of checks when i want to
		// do things like computing collision
		public List<GameObject> GameObjects { get; private set; }
		public List<UtilityObject> UtilityObjects { get; private set; }
		public List<Entity> AllEntities { get; private set; }

		// though these are objects, since we have specific things for it to do and don't want it to be involved in collision
		// detection, being rendered, etc,
		private readonly Viewport viewport;
		private readonly UIHandler UI;
		//private readonly MenuHandler menu;
		private readonly InputHandler input;

		private Size mapSize = new(50, 14);

		public World()
		{
			GameObjects = new();
			UtilityObjects = new();
			AllEntities = new();

			// for now, we just surround the map with walls, but later we'll have more complex map generation
			for (int y = 0; y < mapSize.Height; y++)
			{
				for (int x = 0; x < mapSize.Width; x++)
				{
					if (y < 1 || y > mapSize.Height - 2 || x < 1 || x > mapSize.Width - 2)
					{
						if (y > 0 && y < mapSize.Height - 1) { AddEntity(new Wall(this, x, y, true)); }
						else { AddEntity(new Wall(this, x, y, false)); }
					}
				}
			}

			// this is temporary until we start doing this kind of thing with a randomiser
			AddEntity(new Player(this, 6, 4, PlayerClass.thief, Tags.Player, "Player"));
			AddEntity(new Enemy(this, 4, 2, 27, 10, 6, 10));
			AddEntity(new Enemy(this, 30, 3, 8, 10, 6, 10));
			AddEntity(new Enemy(this, 23, 6, 13, 10, 6, 10));
			AddEntity(new ItemEntity(this, 2, 1, ItemDefinitions.steelHelmet));
			AddEntity(new ItemEntity(this, 2, 2, ItemDefinitions.ironHelmet));
			AddEntity(new ItemEntity(this, 2, 3, ItemDefinitions.strengthPotion));
			AddEntity(new ItemEntity(this, 2, 4, ItemDefinitions.bread));
			AddEntity(new ItemEntity(this, 2, 5, ItemDefinitions.bread));
			AddEntity(new ItemEntity(this, 2, 6, ItemDefinitions.strawHat));
			AddEntity(new ItemEntity(this, 2, 7, ItemDefinitions.smolSword));
			AddEntity(new ItemEntity(this, 2, 8, ItemDefinitions.tallSword));

			viewport = new(this);
			UI = new(this);
			//menu = new()
			input = new(this);

			AddEntity(viewport);
			AddEntity(UI);
			//AddEntity(menu);
			AddEntity(input);

			UI.RenderUI();
			//RenderUpdate();
		}

		public Entity? FindEntityWithTag(Tags tag)
		{
			foreach (Entity entity in AllEntities)
			{
				if (entity.tag == tag) { return entity; }
			}

			return null;
		}

		// turn this into an array?
		public List<Entity> FindEntitiesWithTag(Tags tag)
		{
			List<Entity> entities = new();
			foreach (Entity entity in AllEntities)
			{
				if (entity.tag == tag) { entities.Add(entity); }
			}

			return entities;
		}

		public Entity? FindEntityWithName(string name)
		{
			foreach (Entity entity in AllEntities)
			{
				if (entity.name == name) { return entity; }
			}

			return null;
		}

		// turn this into an array?
		public List<Entity> FindEntitiesWithName(string name)
		{
			List<Entity> entities = new();
			foreach (Entity entity in AllEntities)
			{
				if (entity.name == name) { entities.Add(entity); }
			}

			return entities;
		}

		public void AddEntity(Entity entity)
		{
			AllEntities.Add(entity);

			if (entity is GameObject gObject) { GameObjects.Add(gObject); }
			else if (entity is UtilityObject uObject) { UtilityObjects.Add(uObject); }
		}

		public void RemoveEntity(Entity entity)
		{
			AllEntities.Remove(entity);

			if (entity is GameObject gObject) { GameObjects.Remove(gObject); }
			else if (entity is UtilityObject uObject) { UtilityObjects.Remove(uObject); }
		}

		public void CheckAllAlive()
		{
			for (int i = 0; i < GameObjects.Count; i++)
			{
				if (GameObjects[i] is Character character && !character.IsAlive()) { GameObjects.RemoveAt(i); }
			}
		}

		public void DoMoveCollision()
		{
			// first, we need to collect together the position of every object
			List<Character> characters = new();
			Point[] objectPositions = new Point[GameObjects.Count];
			for (int i = 0; i < GameObjects.Count; i++)
			{
				if (GameObjects[i] is Character character) { characters.Add(character); }
				objectPositions[i] = GameObjects[i].GetPos();
			}

			// then, we need to look at each character...
			for (int i = 0; i < characters.Count; i++)
			{
				//...and see how many objects share that position
				int numOfInstances = objectPositions.Count(p => p == characters[i].GetPos());
				// if there's more than 1 instance of a position, then that means we have at least one collision
				if (numOfInstances > 1)
				{
					// now we have to retrieve the actual GameObjects that have collided, minus the initial object itself
					GameObject[] collisions = new GameObject[numOfInstances - 1];
					for (int j = 0; j < GameObjects.Count; j++)
					{
						int k = 0;
						if (objectPositions[j] == characters[i].GetPos() && GameObjects[j] != characters[i]) { collisions[k] = GameObjects[j]; k++; }
					}

					// then we tell the original charcater that we collided with stuff, and what the stuff is
					characters[i].MoveCollisionHandler(collisions);
				}
			}
		}

		private void PhysicsUpdate()
		{
			foreach (Entity entity in AllEntities)
			{
				entity.Update();
			}
			DoMoveCollision();
			CheckAllAlive();
		}

		private void RenderUpdate()
		{
			foreach (Entity entity in AllEntities)
			{
				entity.RenderUpdate();
			}
			UI.RenderUI();
			viewport.RenderViewport();
		}

		public void GlobalUpdate()
		{
			// tick (physics, input, etc.)
			//PhysicsUpdate();

			// rendering
			//RenderUpdate();
		}
	}
}
