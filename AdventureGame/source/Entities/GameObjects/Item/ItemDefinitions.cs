namespace AdventureGame
{
    internal class ItemDefinitions
    {
        // armour
        public static Item steelHelmet = new("armour/head/steelHelmet/steelHelmet.json");
        public static Item ironHelmet = new("armour/head/ironHelmet/ironHelmet.json");
        public static Item strawHat = new("armour/head/strawHat/strawHat.json");

        public static Item steelBreastplate = new("armour/body/steelBreastplate/steelBreastplate.json");
        public static Item chainmailShirt = new("armour/body/chainmailShirt/chainmailShirt.json");
        public static Item tornShirt = new("armour/body/tornShirt/tornShirt.json");

        public static Item steelLeggings = new("armour/legs/steelLeggings/steelLeggings.json");
        public static Item skirtOfSpinning = new("armour/legs/skirtOfSpinning/skirtOfSpinning.json");
        public static Item raggedPants = new("armour/legs/raggedPants/raggedPants.json");

        public static Item steelGreaves = new("armour/feet/steelGreaves/steelGreaves.json");
        public static Item ironGreaves = new("armour/feet/ironGreaves/ironGreaves.json");
        public static Item leatherShoes = new("armour/feet/leatherShoes/leatherShoes.json");
        public static Item strawSandals = new("armour/feet/strawSandals/strawSandals.json");

        // food
        public static Item bread = new("food/bread/bread.json");

        // potions
        public static Item strengthPotion = new("potions/strengthPotion/strengthPotion.json");

        // weapons
        public static Item smolSword = new("weapons/smolSword/smolSword.json");
        public static Item tallSword = new("weapons/tallSword/tallSword.json");
    }
}
