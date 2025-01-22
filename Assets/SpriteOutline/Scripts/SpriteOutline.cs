using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO: Outlines currently do not get sorted when "Use Exported Frame" is true.

// TODO: Since the sorting sprite is only updated on regeneration, if a different child sprite happens to be sorted underneath, the outline will appear on top until it is regenerated.

// TODO: UI.Image sorting (to ensure the outline appears below an image, it must *not* be a child of the game object, but instead be positioned just above the game object in the hierarchy).

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class SpriteOutline : MonoBehaviour {

	public enum SortMethod {
		SORTING_ORDER,
		Z_AXIS,
	}

	public const string RESOURCE_DIR = "Assets/SpriteOutline/Resources/";
	public const string RESOURCE_EXT = ".outline";
	public const string IMAGE_EXT    = RESOURCE_EXT + ".png";

	[UnityEngine.Serialization.FormerlySerializedAs("outlineSize")]
	[Tooltip("Adjusts the total thickness of the outline, in pixels.")]
	[Range(1, 20)]
	public int size = 1;

	[UnityEngine.Serialization.FormerlySerializedAs("outlineBlur")]
	[Tooltip("Blurs the outline by gradually fading the number of outer edges equivalent to the specified value.")]
	[Range(0, 19)]
	public int blurSize;

	[UnityEngine.Serialization.FormerlySerializedAs("outlineColor")]
	[Tooltip("Defines the color (and overall opacity) of the outline.")]
	public Color color = Color.white;

	[Tooltip("Adjusts the opacity of *only* the blurred edges.")]
	[Range(0, 1)]
	public float blurAlphaMultiplier = 0.7f;

	[Tooltip("Adjusts how quickly the blurred edges fade away.")]
	[Range(0, 1)]
	public float blurAlphaChoke = 1;

	[Tooltip("Reverses the fade direction of the blurring (from the inside out to the outside in).")]
	public bool invertBlur;

	[Tooltip("Defines the minimum amount of opacity a sprite pixel must have for an outline to be placed around it.")]
	[Range(0.01f, 1)]
	public float alphaThreshold = 0.05f;

	[Tooltip("Adds a buffer of transparent pixels between the sprite(s) and the outline.")]
	[Range(0, 20)]
	public int buffer;

	[Tooltip("Include child sprites in the outline.")]
	public bool includeChildren;

	[Tooltip("Filter child sprites on a per-layer basis (only those that belong to one of the checked layers will be included).")]
	public LayerMask childLayers = 1 << 0; // Use the 'Default' layer by default.

	[Tooltip("Exclude child sprites by their game object name.")]
	public string[] ignoreChildNames = new string[0];

	[Tooltip("Change how the outline is sorted (either the lowest sorting order - 1; or the highest z-axis value + 1).")]
	public SortMethod sortMethod = SortMethod.Z_AXIS;

	[Tooltip("Auto-regenerate the outline when the main sprite frame changes (does not track child sprites).")]
	public bool isAnimated;

	[Tooltip("Use the pre-rendered image of the outline instead of rendering in real time (you must \"Export\" the outline first).")]
	public bool useExportedFrame;

	[Tooltip("Override the file name of the exported outline (use to allow multiple game objects sharing the same name to export unique outlines).")]
	public string customFrameName = string.Empty;

	[Tooltip("Auto-regenerate the outline on game start.")]
	public bool generatesOnStart;

#if UNITY_EDITOR
	[Tooltip("Auto-regenerate the outline when the component is loaded in the editor or when any value is changed via the Inspector.")]
	public bool generatesOnValidate = true;
#endif

	static Material outlineMaterial;
	static Material particleMaterial;

	SpriteRenderer spriteRenderer;
	Image          image;
	Sprite         sprite;
	GameObject     outline;
	SpriteRenderer outlineSpriteRenderer;
	Image          outlineImage;
	Texture2D      texture;

	float   _boundsMinX;
	float   _boundsMinY;
	float   _boundsMaxX;
	float   _boundsMaxY;
	Vector3 _pos;
	Vector3 _scale;
	Vector2 _anchor;
	Rect    _textureRect = Rect.zero;

	SpriteRenderer           _sortingSpriteRenderer;
	Dictionary<int, Sprite>  _cachedOutlineSprites = new Dictionary<int, Sprite> ();
	Dictionary<int, Vector2> _cachedOutlineAnchors = new Dictionary<int, Vector2> ();
	int                      _lastSpriteFrameId;

	void Start() {
#if UNITY_EDITOR
		if (!Application.isPlaying) {
			if (generatesOnValidate) {
				Regenerate ();
			}

			return;
		}
#endif

		if (!generatesOnStart)
			return;

		Regenerate ();
	}

	void TryGetOutline() {
		Transform outlineTransform = transform.Find ("Outline");

		if (outlineTransform) {
			outline               = outlineTransform.gameObject;
			outlineSpriteRenderer = outline.GetComponent<SpriteRenderer> ();
			outlineImage          = outline.GetComponent<Image> ();
		}
	}

	void TryGetSprite() {
		sprite = null;

		if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer> ();
		if (!image)          image          = GetComponent<Image> ();

		if (!spriteRenderer && !image) {
			LogError ("Outline cannot be created (SpriteRenderer/Image not found; add one to this game object to fix)");
			return;
		} else if (spriteRenderer && image) {
			LogError ("Outline cannot be created (only one SpriteRenderer/Image can be attached, not both)");
			return;
		} else if (image && !image.canvas) {
			LogError ("Outline cannot be created (Image must use a Canvas; add one to this game object or a parent to fix)");
			return;
		}

		if (spriteRenderer) {
			sprite = spriteRenderer.sprite;
		} else if (image) {
			sprite = image.sprite;
		}

		if (!sprite) {
			LogError ("Outline cannot be created (there is no sprite assigned to the SpriteRenderer/Image)");
			return;
		}
	}

	void LateUpdate() {
		SortOutline ();

		if (!Application.isPlaying || !isAnimated)
			return;

		if (useExportedFrame) {
			LogError ("Cannot use the exported frame when \"Is Animated\" is enabled (disable \"Is Animated\" or \"Use Exported Frame\" to fix)");
			return;
		}

		if (!outline) {
			TryGetOutline ();

			if (!outline)
				return;
		}

		TryGetSprite ();

		if (!sprite)
			return;

		int spriteFrameId = sprite.GetInstanceID ();

		if (spriteFrameId == _lastSpriteFrameId)
			return;

		if (_cachedOutlineSprites.ContainsKey (spriteFrameId)) {
			if (outlineSpriteRenderer) {
				outlineSpriteRenderer.sprite = _cachedOutlineSprites [spriteFrameId];
			} else if (outlineImage) {
				outlineImage.sprite                         = _cachedOutlineSprites [spriteFrameId];
				outlineImage.rectTransform.anchoredPosition = _cachedOutlineAnchors [spriteFrameId];
			}
		} else {
			Regenerate ();
		}

		_lastSpriteFrameId = spriteFrameId;
	}

	public void Regenerate() {
		TryGetSprite ();

		if (!sprite)
			return;

		if (!outline) {
			TryGetOutline ();

			if (!outline) {
				outline = new GameObject ("Outline");
			}
		}

		outline.transform.SetParent (transform, false);

		if (spriteRenderer && !outlineSpriteRenderer) {
			outlineSpriteRenderer = outline.AddComponent<SpriteRenderer> ();
		} else if (image && !outlineImage) {
			outlineImage = outline.AddComponent<Image> ();
		}

		Vector3    cachedPosition = transform.position;
		Quaternion cachedRotation = transform.rotation;
		Vector3    cachedScale    = transform.localScale;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		_scale.x = transform.localScale.x / transform.lossyScale.x;
		_scale.y = transform.localScale.y / transform.lossyScale.y;
		_scale.z = transform.localScale.z / transform.lossyScale.z;

		transform.localScale = _scale;

		_boundsMinX = float.MaxValue;
		_boundsMinY = float.MaxValue;
		_boundsMaxX = float.MinValue;
		_boundsMaxY = float.MinValue;

		if (outlineSpriteRenderer) {
			SpriteRendererExt.GetActiveBounds (spriteRenderer, ref _boundsMinX, ref _boundsMinY, ref _boundsMaxX, ref _boundsMaxY, includeChildren, ShouldIgnoreSprite);
		} else if (outlineImage) {
			ImageExt.GetActiveBounds (image, ref _boundsMinX, ref _boundsMinY, ref _boundsMaxX, ref _boundsMaxY, includeChildren, ShouldIgnoreSprite);
		}

		if (_boundsMinX == float.MaxValue) {
			SetTransformValues (cachedPosition, cachedRotation, cachedScale);

			LogError ("Outline cannot be created (there are no active sprites)");
			return;
		}

		SetupMaterials ();

		if (outlineSpriteRenderer) {
			outlineSpriteRenderer.material = useExportedFrame ? particleMaterial : outlineMaterial;
		} else if (outlineImage) {
			outlineImage.material = useExportedFrame ? particleMaterial : outlineMaterial;
		}

		if (useExportedFrame) {
			string sanitizedName = GetSanitizedName ((customFrameName != string.Empty) ? customFrameName : name);
			string resourcePath  = sanitizedName + RESOURCE_EXT;

			texture = Resources.Load<Texture2D> (resourcePath);

			if (!texture) {
				SetTransformValues (cachedPosition, cachedRotation, cachedScale);

				string texturePath = RESOURCE_DIR + sanitizedName + IMAGE_EXT;

				LogError ("Exported frame \"{0}\" not found (disable \"Use Exported Frame\" and press \"Export\" to fix)", texturePath);
				return;
			}
		} else {
			SetupTexture ();
			ClearTexture ();

			_sortingSpriteRenderer = null;

			try {
				FillTexture (gameObject, sprite);
			} catch (UnityException e) {
				SetTransformValues (cachedPosition, cachedRotation, cachedScale);

				int startIndex = e.Message.IndexOf ("'");
				int endIndex   = e.Message.IndexOf ("'", startIndex + 1) + 1;

				if (startIndex > -1) {
					string textureName = e.Message.Substring (startIndex, endIndex - startIndex);

					LogError ("Texture {0} is not readable (turn on \"Read/Write Enabled\" in its Import Settings to fix)", textureName);
				} else {
					LogError (e.Message);
				}

				return;
			}

			texture.Apply ();

			outlineMaterial.SetInt   ("_Size",                size);
			outlineMaterial.SetInt   ("_BlurSize",            blurSize);
			outlineMaterial.SetColor ("_Color",               color);
			outlineMaterial.SetFloat ("_BlurAlphaMultiplier", blurAlphaMultiplier);
			outlineMaterial.SetFloat ("_BlurAlphaChoke",      blurAlphaChoke);
			outlineMaterial.SetInt   ("_InvertBlur",          invertBlur ? 1 : 0);
			outlineMaterial.SetFloat ("_AlphaThreshold",      alphaThreshold);
			outlineMaterial.SetInt   ("_Buffer",              buffer);
		}

		_textureRect.width  = texture.width;
		_textureRect.height = texture.height;

		_anchor.x = (sprite.pivot.x + GetOffsetX (gameObject, sprite)) / texture.width;
		_anchor.y = (sprite.pivot.y + GetOffsetY (gameObject, sprite)) / texture.height;

		Sprite outlineSprite = Sprite.Create (texture, _textureRect, _anchor, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);

		if (outlineSpriteRenderer) {
			outlineSpriteRenderer.sprite = outlineSprite;
		} else if (outlineImage) {
			outlineImage.sprite = outlineSprite;

			float pixelsPerUnit = (image.canvas.referencePixelsPerUnit / sprite.pixelsPerUnit);

			_anchor.x = -(GetOffsetX (gameObject, sprite) + sprite.textureRect.width  / 2 - texture.width  / 2f) * 2 * pixelsPerUnit;
			_anchor.y = -(GetOffsetY (gameObject, sprite) + sprite.textureRect.height / 2 - texture.height / 2f) * 2 * pixelsPerUnit;

			float offset = (size + buffer) * pixelsPerUnit;

			_scale.x = image.rectTransform.sizeDelta.x / (sprite.textureRect.width  * pixelsPerUnit);
			_scale.y = image.rectTransform.sizeDelta.y / (sprite.textureRect.height * pixelsPerUnit);

			Vector2 offsetMin = -offset * _scale;
			Vector2 offsetMax =  offset * _scale;

			if      (_anchor.x < 0) offsetMin.x += _anchor.x;
			else if (_anchor.x > 0) offsetMax.x += _anchor.x;

			if      (_anchor.y < 0) offsetMin.y += _anchor.y;
			else if (_anchor.y > 0) offsetMax.y += _anchor.y;

			if (image.preserveAspect) {
				float aspectX = image.rectTransform.sizeDelta.x / sprite.textureRect.width;
				float aspectY = image.rectTransform.sizeDelta.y / sprite.textureRect.height;

				Vector2 pivot = image.rectTransform.pivot - Vector2.one / 2;

				if (aspectX < aspectY) {
					pivot.y *= image.rectTransform.sizeDelta.y - sprite.textureRect.height * aspectX;

					offsetMin.y += pivot.y;
					offsetMax.y += pivot.y;
				} else if (aspectX > aspectY) {
					pivot.x *= image.rectTransform.sizeDelta.x - sprite.textureRect.width * aspectY;

					offsetMin.x += pivot.x;
					offsetMax.x += pivot.x;
				}
			}

			outlineImage.rectTransform.anchorMin = Vector2.zero;
			outlineImage.rectTransform.anchorMax = Vector2.one;
			outlineImage.rectTransform.offsetMin = offsetMin;
			outlineImage.rectTransform.offsetMax = offsetMax;

			outlineImage.preserveAspect = image.preserveAspect;
#if UNITY_2018_3_OR_NEWER
			outlineImage.useSpriteMesh  = image.useSpriteMesh;
#endif

			outlineImage.canvasRenderer.Clear();
		}

		SetTransformValues (cachedPosition, cachedRotation, cachedScale);

		SortOutline (); // NOTE: Must sort after resetting the transform values to calculate the correct Z-axis value.

		if (useExportedFrame || !particleMaterial)
			return;

		Texture2D screenshot = RenderScreenshot ();

		Sprite screenshotSprite = Sprite.Create (screenshot, _textureRect, _anchor, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);

		if (outlineSpriteRenderer) {
			outlineSpriteRenderer.sprite   = screenshotSprite;
			outlineSpriteRenderer.material = particleMaterial;
		} else if (outlineImage) {
			outlineImage.sprite   = screenshotSprite;
			outlineImage.material = particleMaterial;
		}

		if (!Application.isPlaying || !isAnimated)
			return;

		int spriteFrameId = sprite.GetInstanceID ();

		_cachedOutlineSprites [spriteFrameId] = screenshotSprite;

		if (outlineImage) {
			_cachedOutlineAnchors [spriteFrameId] = outlineImage.rectTransform.anchoredPosition;
		}
	}

	void SetupMaterials() {
		if (!outlineMaterial) {
			string shaderName = "Sprites/Outline";
			Shader shader     = Shader.Find (shaderName);

			if (!shader) {
				LogError ("Material cannot be created (\"{0}\" shader is missing)", shaderName);
				return;
			}

			outlineMaterial = new Material (shader);
		}

		if (!particleMaterial) {
#if UNITY_2018_1_OR_NEWER
			string shaderName = "Legacy Shaders/Particles/Alpha Blended Premultiply";
#else
			string shaderName = "Particles/Alpha Blended Premultiply";
#endif
			Shader shader     = Shader.Find (shaderName);

			if (!shader) {
				LogError ("Material cannot be created (\"{0}\" shader is missing)", shaderName);
				return;
			}

			particleMaterial = new Material (shader);
		}
	}

	void SetupTexture() {
		int padding = (size + buffer) * 2;
		int width   = Mathf.CeilToInt ((_boundsMaxX - _boundsMinX) * sprite.pixelsPerUnit) + padding;
		int height  = Mathf.CeilToInt ((_boundsMaxY - _boundsMinY) * sprite.pixelsPerUnit) + padding;

		texture            = new Texture2D (width, height, TextureFormat.RGBA32, false);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode   = TextureWrapMode.Clamp;
	}

	void ClearTexture() {
		Color32[] pixels      = texture.GetPixels32 ();
		int       pixelsCount = pixels.Length;

		for (int i = 0; i < pixelsCount; i++) {
			pixels [i].a = 0;
		}

		texture.SetPixels32 (pixels);
	}

	void FillTexture(GameObject instance, Sprite sprite) {
		if (!ShouldIgnoreSprite (instance, sprite)) {
			Rect rect = sprite.packed ? sprite.textureRect : sprite.rect;

			int width  = (int)rect.width;
			int height = (int)rect.height;

			Color[] pixels = sprite.texture.GetPixels ((int)rect.x, (int)rect.y, width, height);

			int offsetX = GetOffsetX (instance, sprite);
			int offsetY = GetOffsetY (instance, sprite);

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int index = width * y + x;

					if (pixels [index].a > 0) {
						texture.SetPixel (x + offsetX, y + offsetY, pixels [index]);
					}
				}
			}

			SpriteRenderer instanceSpriteRenderer = instance.GetComponent<SpriteRenderer> ();

			if (outlineSpriteRenderer && instanceSpriteRenderer) {
				switch (sortMethod) {

				case SortMethod.SORTING_ORDER: if (!_sortingSpriteRenderer || instanceSpriteRenderer.sortingOrder         < _sortingSpriteRenderer.sortingOrder)         _sortingSpriteRenderer = instanceSpriteRenderer; break;
				case SortMethod.Z_AXIS:        if (!_sortingSpriteRenderer || instanceSpriteRenderer.transform.position.z > _sortingSpriteRenderer.transform.position.z) _sortingSpriteRenderer = instanceSpriteRenderer; break;

				}
			}
		}

		if (!includeChildren)
			return;

		int childCount = instance.transform.childCount;

		for (int i = 0; i < childCount; i++) {
			Transform child = instance.transform.GetChild (i);

			if (outlineSpriteRenderer) {
				SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer> ();

				if (childSpriteRenderer) {
					FillTexture (childSpriteRenderer.gameObject, childSpriteRenderer.sprite);
				}
			} else if (outlineImage) {
				Image childImage = child.GetComponent<Image> ();

				if (childImage) {
					FillTexture (childImage.gameObject, childImage.sprite);
				}
			}
		}
	}

	int GetOffsetX(GameObject instance, Sprite sprite) {
		float spriteMinX = instance.transform.position.x / (image ? image.canvas.referencePixelsPerUnit : 1) + sprite.bounds.min.x;

		return size + buffer + Mathf.RoundToInt ((spriteMinX - _boundsMinX) * sprite.pixelsPerUnit);
	}

	int GetOffsetY(GameObject instance, Sprite sprite) {
		float spriteMinY = instance.transform.position.y / (image ? image.canvas.referencePixelsPerUnit : 1) + sprite.bounds.min.y;

		return size + buffer + Mathf.RoundToInt ((spriteMinY - _boundsMinY) * sprite.pixelsPerUnit);
	}

	void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) {
		transform.position   = position;
		transform.rotation   = rotation;
		transform.localScale = scale;
	}

	public virtual bool ShouldIgnoreSprite(GameObject instance, Sprite sprite) {
		return !instance.activeInHierarchy || !sprite ||
			(outlineSpriteRenderer && (sprite == outlineSpriteRenderer.sprite || !instance.GetComponent<SpriteRenderer> ().enabled)) ||
			(outlineImage && (sprite == outlineImage.sprite || !instance.GetComponent<Image> ().enabled)) ||
			(instance != gameObject && (!LayerMaskExt.ContainsLayer (childLayers, instance.layer) || System.Array.IndexOf (ignoreChildNames, instance.name) > -1));
	}

	public void SortOutline(float zOffset = 1, int? sortingOrder = null, int? sortingLayerId = null) {
		if (!outline)
			return;

		outline.layer = gameObject.layer;

		if (!_sortingSpriteRenderer)
			return;

		outlineSpriteRenderer.flipX          = spriteRenderer.flipX;
		outlineSpriteRenderer.flipY          = spriteRenderer.flipY;
		outlineSpriteRenderer.sortingLayerID = sortingLayerId.HasValue ? sortingLayerId.Value : _sortingSpriteRenderer.sortingLayerID;
		outlineSpriteRenderer.sortingOrder   = sortingOrder  .HasValue ? sortingOrder  .Value : _sortingSpriteRenderer.sortingOrder - ((sortMethod == SortMethod.SORTING_ORDER) ? 1 : 0);

		_pos.x = outline.transform.localPosition.x;
		_pos.y = outline.transform.localPosition.y;
		_pos.z = (sortMethod == SortMethod.Z_AXIS) ? zOffset + ((_sortingSpriteRenderer != spriteRenderer) ? _sortingSpriteRenderer.transform.localPosition.z : 0) : 0;

		outline.transform.localPosition = _pos;
	}

	public void Show() {
		if (outline) {
			outline.SetActive (true);
		}
	}

	public void Hide() {
		if (outline) {
			outline.SetActive (false);
		}
	}

	public void Clear() {
		if (!outline) {
			TryGetOutline ();

			if (!outline)
				return;
		}

		if (Application.isPlaying) {
			Destroy (outline);
		} else {
			DestroyImmediate (outline);
		}

		sprite = null;
	}

	public void Export() {
		if (isAnimated || useExportedFrame) {
			LogError ("Cannot export when \"Is Animated\" or \"Use Exported Frame\" is enabled");
			return;
		}

		TryGetOutline ();

		bool clearAfterExport = outline == null;

		texture = null;

		Regenerate ();

		if (!texture) {
			LogError ("Nothing to export (the outline does not exist)");
			return;
		}

		string texturePath = RESOURCE_DIR + GetSanitizedName ((customFrameName != string.Empty) ? customFrameName : name) + IMAGE_EXT;

		System.IO.FileStream   image       = System.IO.File.Open (texturePath, System.IO.FileMode.Create);
		System.IO.BinaryWriter imageWriter = new System.IO.BinaryWriter (image);

		Sprite outlineSprite = outlineSpriteRenderer ? outlineSpriteRenderer.sprite : outlineImage.sprite;

		imageWriter.Write (outlineSprite.texture.EncodeToPNG ());

		image.Close ();

		if (clearAfterExport) {
			Clear ();
		}

		Log ("Outline exported to \"{0}\"", texturePath);
	}

	Texture2D RenderScreenshot() {
		RenderTexture cachedRenderTexture = RenderTexture.active;
		RenderTexture renderTexture       = RenderTexture.GetTemporary (texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

		RenderTexture.active = renderTexture;

		GL      .Clear (false,   true,          Color.clear);
		Graphics.Blit  (texture, renderTexture, outlineMaterial);

		Texture2D screenshot = new Texture2D (texture.width, texture.height, TextureFormat.RGBA32, false);

		screenshot.filterMode = FilterMode.Point;

		screenshot.ReadPixels (_textureRect, 0, 0, false);
		screenshot.Apply ();

		RenderTexture.active = cachedRenderTexture;

		RenderTexture.ReleaseTemporary (renderTexture);

		return screenshot;
	}

	string GetSanitizedName(string name) {
		return string.Concat (name.Split (System.IO.Path.GetInvalidFileNameChars ())).ToLower ();
	}

	void Log(string message, params object[] args) {
		Debug.LogFormat ("{0}: {1}", this, string.Format (message, args));
	}

	void LogError(string error, params object[] args) {
		Debug.LogErrorFormat ("{0}: {1}", this, string.Format (error, args));
	}

#if UNITY_EDITOR
	bool _shouldRegenerate;

	void OnValidate() {
		if (!gameObject.activeInHierarchy || !generatesOnValidate)
			return;

		_shouldRegenerate = true;
	}

	void Update() {
		if (_shouldRegenerate) {
			Regenerate ();
			_shouldRegenerate = false;
		}
	}
#endif

}
