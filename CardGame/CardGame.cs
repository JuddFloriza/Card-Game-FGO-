using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace AssemblyCSharp
{
	public class CardGame: MonoBehaviour
	{
		private enum State
		{
			Initializing,
			Waiting,
			Attacking,
			Swapping,
			Calculating,
			End,

			None
		}

		#region variables

		private State 	 	m_gameState 		= State.None;
		private CardGame 	m_cardGame;
		
		private GameObject 			m_parent 		= new GameObject("Card Game");
		private List<GameObject> 	m_cardsOne		= new List<GameObject>();
		private List<GameObject> 	m_cardsTwo		= new List<GameObject>();
		private List<UIButton>		m_cardBtnsOne	= new List<UIButton>();
		private List<UIButton>		m_cardBtnsTwo	= new List<UIButton>();
		private Tweener[] 			m_tweens 		= new Tweener[8];
		private Vector3[]			m_cardPosOne	= new Vector3[4];
		private Vector3[]			m_cardPosTwo	= new Vector3[4];
		private int[]				m_health		= new int[2];
		private int[]				m_attack		= new int[2];
		private GameObject			m_cardRoot		= null;
		private Player 				m_player1		= null;
		private Player 				m_player2		= null;
		private Arbiter 			m_arbiter		= null;
		private Transform		 	m_atkTrans		= null;
		private Transform		 	m_swapTrans		= null;
		private UIButton			m_atkBtn 		= null;
		private UIButton			m_swapBtn 		= null;
		private Transform			m_atkTxtTrans 	= null;
		private Transform			m_hpTxtTrans	= null;
		private Transform			m_effect		= null;
		private int					m_pTurn			= 0;
		private bool				b_isSwapping	= false;

		#endregion // variables

		#region methods

		public void _Initialize () 
		{
			m_player1 = new Player ("Hann");
			m_player2 = new Player ("Emar");
			m_arbiter = new Arbiter (m_player1, m_player2);

			m_cardRoot = Instantiate ((GameObject)Resources.Load ("Prefabs/CardGameRoot"));

			while(!m_arbiter.PlayerTwo.HandIsFull())
			{
				Card card;
				int rand = Random.Range(0, 19);
				card = CardLib.GetCard(rand);
				CreateCard(rand);
				m_arbiter.DealCard(card);
			}
			
			m_parent.transform.parent = m_cardRoot.transform;
			SetupButtons ();
			SetValues ();
			SetActiveValues ();
			SetCards ();
			m_pTurn = 0;
			m_gameState = State.Initializing;
		}
		
		// Update is called once per frame
		public void _Update (float dt) 
		{
			Debug.Log (m_gameState);
			switch(m_gameState)
			{
			case State.Initializing:
				SetActiveCard(1);
				SetActiveCard(2);
				m_gameState = State.Waiting;
				break;
			case State.Waiting:
				m_atkBtn.RunButtonFunction();
				m_swapBtn.RunButtonFunction();
				break;
			case State.Attacking:
				SetActiveValues ();
				CheckHp();
				break;
			case State.Swapping:
				ActivateCardButtons();
				break;
			case State.Calculating:
				m_gameState = State.Waiting;
				break;
			case State.End:
				DisplayGameOver();
				break;
			}
		}

		#endregion // methods


		#region functions
		private void CreateCard(int rand)
		{
			GameObject cardObj = null;
			SpriteRenderer rend = null;
			cardObj = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
			rend = cardObj.transform.FindChild ("BG").GetComponent<SpriteRenderer> ();
			rend.sprite = CardLib.GetCardSprite(rand);
			cardObj.transform.parent = m_parent.transform;

			if (m_arbiter.Counter % 2 == 0)
				DealPlayerOne (cardObj, m_arbiter.Counter);
			else if (m_arbiter.Counter % 2 == 1)
				DealPlayerTwo (cardObj, m_arbiter.Counter);
		}

		private void AnchorPoint(Transform trans, float x, float y)
		{
			Camera main = Camera.main;
			Vector3 pos = main.WorldToViewportPoint (trans.position);
			pos.x = x;
			pos.y = y;
			trans.position = main.ViewportToWorldPoint (pos);
		}

		private Vector3 ScreenPoint(Transform trans, float x, float y)
		{
			Camera main = Camera.main;
			Vector3 pos = main.WorldToViewportPoint (trans.position);
			pos.x = x;
			pos.y = y;
			return main.ViewportToWorldPoint (pos);
		}

		private void DealPlayerOne(GameObject obj,int index)
		{
			obj.transform.DOMove(ScreenPoint(obj.transform.FindChild ("BG"), 0.2f + ((index/2) * 0.2f), 0.05f), 0.4f);
			m_cardPosOne [index / 2] = ScreenPoint (obj.transform.FindChild ("BG"), 0.2f + ((index / 2) * 0.2f), 0.05f);
			obj.transform.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 1 + index;
			m_cardsOne.Add (obj);
		}

		private void DealPlayerTwo(GameObject obj,int index)
		{
			Quaternion rotation = obj.transform.FindChild("BG").localRotation;
			rotation.z = 180;
			obj.transform.FindChild("BG").localRotation = rotation;
			obj.transform.DOMove(ScreenPoint(obj.transform.FindChild ("BG"), 0.2f + ((index/2) * 0.2f), 0.95f), 0.4f);
			m_cardPosTwo [(index - 1) / 2] = ScreenPoint (obj.transform.FindChild ("BG"), 0.2f + ((index / 2) * 0.2f), 0.95f);
			obj.transform.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 1 + index;
			m_cardsTwo.Add (obj);		
		}

		private void SetActiveCard(int playerIndex)
		{
			Transform trans;
			switch(playerIndex)
			{
			case 1:
				trans = m_cardsOne[m_player1.ActiveCardIndex].transform;
				trans.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 10;
				trans.DOMove(ScreenPoint(trans, 0.5f, 0.35f), 0.4f);
				break;
			case 2:
				trans = m_cardsTwo[m_player2.ActiveCardIndex].transform;
				trans.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 10;
				trans.DOMove(ScreenPoint(trans, 0.5f, 0.65f), 0.4f);
				break;
			}
		}

		private void SetupButtons()
		{
			m_atkTrans = m_cardRoot.transform.FindChild ("Attack");
			m_swapTrans = m_cardRoot.transform.FindChild ("Swap");
			m_atkBtn = new UIButton (m_atkTrans.gameObject, Resources.Load<Sprite> ("ButtonPressed"));
			m_atkBtn.OnClickButtonFunction = delegate {
				_onAtkPressed();
			};
			m_atkBtn.OnButtonReleaseFunction = delegate {
				_onAtkRelease();
			};
			m_swapBtn = new UIButton (m_swapTrans.gameObject, Resources.Load<Sprite> ("ButtonPressed"));
			m_swapBtn.OnButtonReleaseFunction = delegate {
				_onSwapReleased();
			};

			m_atkTrans.position = ScreenPoint (m_atkTrans, 0.2f, 0.5f);
			m_swapTrans.position = ScreenPoint (m_swapTrans, 0.8f, 0.5f);
		}

		private void SetValues()
		{
			m_atkTxtTrans = m_cardRoot.transform.FindChild ("AttackPower");
			m_hpTxtTrans = m_cardRoot.transform.FindChild ("Health");
			m_effect = m_cardRoot.transform.FindChild ("Effect");

			m_atkTxtTrans.position = ScreenPoint (m_atkTxtTrans, 0.2f, 0.58f);
			m_hpTxtTrans.position = ScreenPoint (m_hpTxtTrans, 0.8f, 0.58f);
			m_effect.position = ScreenPoint (m_effect, 0.5f, 0.5f);
		}

		private void SetActiveValues()
		{
			m_attack[0] = m_player1.GetActiveCard().GetPower();
			m_health[0] = m_player1.GetActiveCard().GetHitPoints();
			m_attack[1] = m_player2.GetActiveCard().GetPower();
			m_health[1] = m_player2.GetActiveCard().GetHitPoints();

			for(int i = 1; i < 3; i++)
			{
				m_atkTxtTrans.FindChild(i.ToString()).FindChild("Value").gameObject.GetComponent<TextMesh>().text = m_attack[i-1].ToString();
				m_hpTxtTrans.FindChild(i.ToString()).FindChild("Value").gameObject.GetComponent<TextMesh>().text = m_health[i-1].ToString();
			}
		}

		private void CheckHp()
		{
			switch(m_pTurn)
			{
			case 1:
				if(m_player2.GetActiveCard().GetHitPoints() == 0)
				{
					m_player1.ClaimPrize();
					if(!CheckIfOver())
						m_gameState = State.Swapping;
					else
						m_gameState = State.End;
				}
				else
					m_gameState = State.Calculating;
				break;
			case 0:
				if(m_player1.GetActiveCard().GetHitPoints() == 0)
				{
					m_player2.ClaimPrize();
					if(!CheckIfOver())
						m_gameState = State.Swapping;
					else
						m_gameState = State.End;
				}
				else
					m_gameState = State.Calculating;
				break;
			}
		}

		private bool CheckIfOver()
		{
			if(m_player1.GetPrizeCount() == 3 || m_player2.GetPrizeCount() == 3)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void SetCards()
		{
			for(int i = 0; i < m_cardsOne.Count; i++)
			{
				int index = i;

				UIButton cardButton = new UIButton(m_cardsOne[i].transform.FindChild("BG").gameObject);
				cardButton.OnClickButtonFunction = delegate {
					_onCardPressed(index);
				};
				m_cardBtnsOne.Add(cardButton);

				UIButton cardButtonTwo = new UIButton(m_cardsTwo[i].transform.FindChild("BG").gameObject);
				cardButtonTwo.OnClickButtonFunction = delegate {
					_onCardPressed(index);
				};
				m_cardBtnsTwo.Add(cardButtonTwo);
			}
		}

		private void ActivateCardButtons()
		{
			switch(m_pTurn)
			{
			case 0:
				for(int i = 0;i < m_cardBtnsOne.Count; i++)
				{
					if(i != m_player1.ActiveCardIndex)
						m_cardBtnsOne[i].RunButtonFunction();
				}
				break;
			case 1:
				for(int i = 0;i < m_cardBtnsOne.Count; i++)
				{
					if(i != m_player2.ActiveCardIndex)
						m_cardBtnsTwo[i].RunButtonFunction();
				}
				break;
			}

		}

		private void DisplayGameOver()
		{
			if(m_player1.GetPrizeCount() == 3)
			{
				m_effect.gameObject.SetActive(true);
				m_effect.GetComponent<TextMesh>().text = m_player1.GetName() + " WINS!";
			}
			else if (m_player2.GetPrizeCount() == 3)
			{
				m_effect.gameObject.SetActive(true);
				m_effect.GetComponent<TextMesh>().text = m_player2.GetName() + " WINS!";
			}
		}
		#endregion // functions

		#region delegates
		private void _onAtkPressed()
		{
			switch(m_pTurn)
			{
			case 0:
				m_arbiter.DealDamage (m_player2.GetActiveCard(), m_player1.GetActiveCard());
				if(m_arbiter.CheckWeakness(m_player2.GetActiveCard().GetCardType(), m_player1.GetActiveCard().GetCardType()))
				{
					m_effect.gameObject.SetActive(true);
					m_effect.GetComponent<TextMesh>().text = "WEAK!";
				}
				else if(m_arbiter.CheckResistance(m_player2.GetActiveCard().GetCardType(), m_player1.GetActiveCard().GetCardType()))
				{
					m_effect.gameObject.SetActive(true);
					m_effect.GetComponent<TextMesh>().text = "RESIST!";
				}
				break;
			case 1:
				m_arbiter.DealDamage (m_player1.GetActiveCard(), m_player2.GetActiveCard());
				if(m_arbiter.CheckWeakness(m_player1.GetActiveCard().GetCardType(), m_player2.GetActiveCard().GetCardType()))
				{
					m_effect.gameObject.SetActive(true);
					m_effect.GetComponent<TextMesh>().text = "WEAK!";
				}
				else if(m_arbiter.CheckResistance(m_player1.GetActiveCard().GetCardType(), m_player2.GetActiveCard().GetCardType()))
				{
					m_effect.gameObject.SetActive(true);
					m_effect.GetComponent<TextMesh>().text = "RESIST!";
				}
				break;
			}
		}

		private void _onAtkRelease()
		{	
			switch(m_pTurn)
			{
			case 0:
				m_effect.gameObject.SetActive(false);
				m_pTurn = 1;
				break;
			case 1:
				m_effect.gameObject.SetActive(false);
				m_pTurn = 0;
				break;
			}
			m_gameState = State.Attacking;
		}
		private void _onSwapReleased()
		{
			b_isSwapping = true;
			m_gameState = State.Swapping;
		}

		private void _onCardPressed(int index)
		{
			switch(m_pTurn)
			{
			case 0:
				if(m_player1.GetActiveCard().GetHitPoints() != 0)
					m_cardsOne[m_player1.ActiveCardIndex].transform.DOMove(m_cardPosOne[m_player1.ActiveCardIndex], 0.4f);
				else
					m_cardsOne[m_player1.ActiveCardIndex].transform.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 1 + m_player1.ActiveCardIndex;

				m_player1.ActiveCardIndex = index;
				m_player1.SetActiveCard();
				SetActiveCard(1);
				SetActiveValues();

				if(b_isSwapping)
				{
					m_pTurn = 1;
					b_isSwapping = false;
				}

				m_gameState = State.Waiting;
				break;
			case 1:
				if(m_player2.GetActiveCard().GetHitPoints() != 0)
					m_cardsTwo[m_player2.ActiveCardIndex].transform.DOMove(m_cardPosTwo[m_player2.ActiveCardIndex], 0.4f);
				else
					m_cardsTwo[m_player2.ActiveCardIndex].transform.FindChild ("BG").GetComponent<SpriteRenderer> ().sortingOrder = 1 + m_player2.ActiveCardIndex;
				
				m_player2.ActiveCardIndex = index;
				m_player2.SetActiveCard();
				SetActiveCard(2);
				SetActiveValues();

				if(b_isSwapping)
				{
					m_pTurn = 0;
					b_isSwapping = false;
				}

				m_gameState = State.Waiting;
				break;
			}
		}
		#endregion // delegates
	}
}

