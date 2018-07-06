using UnityEngine;
using System;
using System.Collections.Generic;
using net.peakgames.gaminglib.util;

namespace SpadesPlus.Card {
    public enum SuitType {
        NONE = 0,
        CLUBS = 1,
        DIAMONDS = 2,
        SPADES = 3,
        HEARTS = 4
    }

    public enum RankType {
        NONE = 0, ACE = 1, TWO = 2, THREE = 3, FOUR = 4,
        FIVE = 5, SIX = 6, SEVEN = 7, EIGHT = 8,
        NINE = 9, TEN = 10, JACK = 11, QUEEN = 12,
        KING = 13
    }

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class PeakCard : MonoBehaviour{
		private static readonly int[] FRONT_INDICES = {
			58, 59, 61, 63, 65, 67, 69,
			71, 73, 75, 77, 79, 81, 83,
			57, 56, 60, 62, 64, 66, 68,
			70, 72, 74, 76, 78, 80, 82
		};

		[SerializeField]
		private string textureAtlas;
		[SerializeField]
		private RankType rankType;
		[SerializeField]	
		private SuitType suitType;
		[SerializeField]
		private Color tint = Color.white;

		private Mesh meshInstance;
		private Vector2[] uvs;

		private static Dictionary<string, SpriteInfo> offsetDictionary;

		public RankType Rank {
			get {
				return rankType;
			}
		}
		public SuitType Suit {
			get {
				return suitType;
			}
		}
		public Color Tint {
			get {
				return tint;
			}
			set {
				SetColor(value);
			}
		}

		private void Awake() {
			meshInstance = GetMesh(gameObject);
			uvs = meshInstance.uv;
		}

		public void Initialize(SuitType suit, RankType rank) {
			this.suitType = suit;
			this.rankType = rank;
			this.name = rank.ToString() + " OF " + suit.ToString();

			if (meshInstance == null) {
				Awake();
			}
			if (offsetDictionary == null) {
				EvaluateAtlas(this.textureAtlas);
			}

			string suitSprite;
			GetSpriteName(out suitSprite);
			SetFace(suitSprite);
		}

		public void SetColor(Color color) {
			tint = color;
			Color[] newColor = new Color[meshInstance.vertexCount];
			for (int i = 0; i < newColor.Length; i++) {
					newColor[i] = tint;
			}
			meshInstance.colors = newColor;
		}

		public void EvaluateAtlas(string path) {
			offsetDictionary = AtlasParser.GetOffsets(path);
		}

		public void SetFace(string face) {
			SpriteInfo spriteInfo;
			if(! offsetDictionary.TryGetValue (face, out spriteInfo)) {
				throw new Exception(String.Format("Cannot find offset in dictionary: {0} {1} {2}", face, Suit, Rank));
			}

			Vector2[] newUV = new Vector2[meshInstance.vertexCount];
			Array.Copy(uvs, newUV, newUV.Length);

			for (int i = 0; i < FRONT_INDICES.Length; i++) {
				int index = FRONT_INDICES[i];
				newUV[index] = uvs[index] + spriteInfo.uv1;
			}

			meshInstance.uv = newUV;
		}

		private void GetSpriteName(out string sprite) {
			sprite = "";

			switch (suitType) {
			case SuitType.CLUBS:
				sprite = "Clubs";
				break;
			case SuitType.DIAMONDS:
				sprite = "Diamonds";
				break;
			case SuitType.SPADES:
				sprite = "Spades";
				break;
			case SuitType.HEARTS:
				sprite = "Hearts";
				break;
			}

			switch (rankType) {
			case RankType.ACE:
				sprite += "Ace";
				break;
			case RankType.JACK:
				sprite += "Jack";
				break;
			case RankType.QUEEN:
				sprite += "Queen";
				break;
			case RankType.KING:
				sprite += "King";
				break;
			default:
				sprite += ((int)rankType).ToString();
				break;
			}
		}

		private Mesh GetMesh(GameObject go) {
			MeshFilter mf = go.GetComponent<MeshFilter>();
			#if UNITY_EDITOR
			Mesh meshCopy = Mesh.Instantiate(mf.sharedMesh) as Mesh;
			return mf.mesh = meshCopy;
			#else
			return mf.mesh;
			#endif
		}
	}
}