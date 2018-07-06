using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace net.peakgames.gaminglib.util {
	public class AtlasParser {

		private static Dictionary<string, Dictionary<string, SpriteInfo>> mainDictionary;

		static AtlasParser() {
			mainDictionary = new Dictionary<string, Dictionary<string, SpriteInfo>>();
		}

		public static Dictionary<string, SpriteInfo> GetOffsets(string path){
			Dictionary<string, SpriteInfo> pathDictionary;
			if(!mainDictionary.TryGetValue(path, out pathDictionary)) {
				pathDictionary = Parse(path);
			}
			return pathDictionary;
		}

		public static SpriteInfo GetOffset(string path, string name){
			Dictionary<string, SpriteInfo> pathDictionary;
			if(!mainDictionary.TryGetValue(path, out pathDictionary)) {
				pathDictionary = Parse(path);
			}
			return pathDictionary[name];
		}

		private static Dictionary<string, SpriteInfo> Parse(string path) {
			Sprite[] sprites = Resources.LoadAll<Sprite>(path);
			Dictionary<string, SpriteInfo> dict = new Dictionary<string, SpriteInfo>();
			for(int i = 0; i < sprites.Length ; i++){
				Sprite sprite = sprites[i];
				Vector2 uv1 = new Vector2(sprite.rect.xMin / sprite.texture.width, sprite.rect.yMin / sprite.texture.height);
				Vector2 uv2 = new Vector2(sprite.rect.xMax / sprite.texture.width, sprite.rect.yMax / sprite.texture.height);
				dict.Add(sprite.name, new SpriteInfo(uv1, uv2));
			}
			return dict;
		}
	}

	public class SpriteInfo {
		public Vector2 uv1;
		public Vector2 uv2;

		public SpriteInfo(Vector2 uv1, Vector2 uv2){
			this.uv1 = uv1;
			this.uv2 = uv2;
		}
	}
}