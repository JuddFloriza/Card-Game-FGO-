using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class CardLib
{
	private static Dictionary<int, string> m_cardName = new Dictionary<int, string>()
	{
		{0, "Saber"},
		{1, "Alter"},
		{2, "Artemis"},
		{3, "Atalanta"},
		{4, "Euryale"},
		{5, "Scathach"},
		{6, "Elizabeth"},
		{7, "Cu"},
		{8, "Drake"},
		{9, "Marie"},
		{10, "Medusa"},
		{11, "Jack"},
		{12, "Carmilla"},
		{13, "Jekyll"},
		{14, "Tamamo"},
		{15, "Nursery"},
		{16, "Medea"},
		{17, "Vlad"},
		{18, "Frankie"}
	};

	private static Dictionary<int, Card.ServantType> m_cardType = new Dictionary<int, Card.ServantType>()
	{
		{0, Card.ServantType.Saber},
		{1, Card.ServantType.Saber},
		{2, Card.ServantType.Archer},
		{3, Card.ServantType.Archer},
		{4, Card.ServantType.Archer},
		{5, Card.ServantType.Lancer},
		{6, Card.ServantType.Lancer},
		{7, Card.ServantType.Lancer},
		{8, Card.ServantType.Rider},
		{9, Card.ServantType.Rider},
		{10, Card.ServantType.Rider},
		{11, Card.ServantType.Assassin},
		{12, Card.ServantType.Assassin},
		{13, Card.ServantType.Assassin},
		{14, Card.ServantType.Caster},
		{15, Card.ServantType.Caster},
		{16, Card.ServantType.Caster},
		{17, Card.ServantType.Berserker},
		{18, Card.ServantType.Berserker}
	};

	private static Dictionary<int, int> m_cardPower = new Dictionary<int, int>()
	{
		{0, 20},
		{1, 15},
		{2, 18},
		{3, 13},
		{4, 9},
		{5, 17},
		{6, 14},
		{7, 10},
		{8, 18},
		{9, 13},
		{10, 8},
		{11, 17},
		{12, 12},
		{13, 9},
		{14, 16},
		{15, 13},
		{16, 8},
		{17, 25},
		{18, 16}
	};

	private static Dictionary<int, int> m_cardHealth = new Dictionary<int, int>()
	{
		{0, 180},
		{1, 140},
		{2, 185},
		{3, 140},
		{4, 110},
		{5, 170},
		{6, 140},
		{7, 110},
		{8, 170},
		{9, 150},
		{10, 110},
		{11, 170},
		{12, 145},
		{13, 125},
		{14, 190},
		{15, 160},
		{16, 130},
		{17, 170},
		{18, 140}
	};

	private static Dictionary<int, string> m_cardPath = new Dictionary<int, string> ()
	{
		{0, "Artoria"},
		{1, "Alter"},
		{2, "Artemis"},
		{3, "Atalanta"},
		{4, "Euryale"},
		{5, "Scathach"},
		{6, "Elizabeth"},
		{7, "Cu"},
		{8, "Drake"},
		{9, "Marie"},
		{10, "Medusa"},
		{11, "JtR"},
		{12, "Carmilla"},
		{13, "Jekyll"},
		{14, "Tamamae"},
		{15, "Nursery"},
		{16, "Medea"},
		{17, "Vlad"},
		{18, "Frankie"}
	};

	public static Card GetCard(int key)
	{
		Card card = null;
		string name = "";
		Card.ServantType type = Card.ServantType.Size;
		int power = 0;
		int health = 0;

		m_cardName.TryGetValue (key, out name);
		m_cardType.TryGetValue (key, out type);
		m_cardPower.TryGetValue (key, out power);
		m_cardHealth.TryGetValue (key, out health);

		if(name != "" && type != Card.ServantType.Size && power != 0 && health != 0)
		{
			card = new Card(name, type, power, health);
			return card;
		}
		else
			return card;
	}

	public static Sprite GetCardSprite(int key)
	{
		Sprite sprite = null;
		string path = "";

		m_cardPath.TryGetValue (key, out path);
		if (path != "")
			sprite = Resources.Load<Sprite> (path);

		return sprite;
	}
}
