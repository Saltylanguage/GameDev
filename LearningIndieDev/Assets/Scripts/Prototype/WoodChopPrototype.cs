using UnityEngine;
using UnityEngine.InputSystem;

public sealed class WoodChopPrototype : MonoBehaviour
{
    const float TreeRange = 1.6f;
    const int TreeHealth = 6;

    Transform player;
    Transform tree;
    SpriteRenderer treeTop;
    int health = TreeHealth;
    int wood;
    bool chopping;
    float meter;
    GUIStyle titleStyle;
    GUIStyle bodyStyle;

    void Awake()
    {
        var camera = Camera.main;
        camera.orthographic = true;
        camera.orthographicSize = 5.5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.backgroundColor = new Color(0.20f, 0.63f, 0.77f);

        MakeSprite("Sand", Vector2.zero, new Vector3(13f, 9f, 1f), new Color(0.94f, 0.78f, 0.43f));
        MakeSprite("WaterEdge", new Vector2(0f, -4.2f), new Vector3(13f, 0.5f, 1f), new Color(0.11f, 0.48f, 0.72f));
        player = MakeSprite("Player", new Vector2(-2.8f, -0.8f), new Vector3(0.65f, 0.85f, 1f), new Color(0.94f, 0.33f, 0.22f)).transform;
        MakeSprite("TreeTrunk", new Vector2(2.2f, -0.2f), new Vector3(0.58f, 2.15f, 1f), new Color(0.39f, 0.20f, 0.08f));
        treeTop = MakeSprite("TreeLeaves", new Vector2(2.2f, 1.0f), new Vector3(2.05f, 1.75f, 1f), new Color(0.09f, 0.42f, 0.17f));
        tree = treeTop.transform;
        MakeSprite("Rock", new Vector2(-0.4f, 1.9f), new Vector3(1.2f, 0.8f, 1f), new Color(0.45f, 0.46f, 0.42f));

        // titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        // bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = Color.white } };
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || health <= 0)
            return;

        if (!chopping)
        {
            var move = new Vector2(
                (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            player.position += (Vector3)(move.normalized * 3.4f * Time.deltaTime);

            if (NearTree() && keyboard.eKey.wasPressedThisFrame)
                chopping = true;
        }
        else
        {
            meter = Mathf.PingPong(Time.time * 1.25f, 1f);
            if (keyboard.escapeKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame)
                chopping = false;
            else if (keyboard.spaceKey.wasPressedThisFrame)
                Chop(meter);
        }
    }

    void Chop(float timing)
    {
        var strongHit = DamageForTiming(timing) == 2;
        health -= DamageForTiming(timing);
        treeTop.color = strongHit ? new Color(0.22f, 0.72f, 0.24f) : new Color(0.17f, 0.50f, 0.19f);
        if (health <= 0)
        {
            wood += 3;
            tree.gameObject.SetActive(false);
            chopping = false;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(24, 18, 500, 45), "Island Chores: Wood Chopping", titleStyle);
        GUI.Label(new Rect(26, 60, 300, 30), $"Wood: {wood}", bodyStyle);

        if (health <= 0)
        {
            GUI.Label(new Rect(Screen.width / 2 - 160, 25, 350, 35), "Tree felled! +3 wood", bodyStyle);
            return;
        }

        if (!chopping)
        {
            var text = NearTree() ? "Press [E] to chop the tree" : "Move with WASD";
            GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height - 70, 300, 35), text, bodyStyle);
            return;
        }

        var bar = new Rect(Screen.width / 2 - 180, Screen.height - 105, 360, 24);
        GUI.Box(bar, GUIContent.none);
        GUI.color = new Color(0.2f, 0.8f, 0.25f);
        GUI.Box(new Rect(bar.x + bar.width * 0.4f, bar.y, bar.width * 0.2f, bar.height), GUIContent.none);
        GUI.color = Color.white;
        GUI.Box(new Rect(bar.x + bar.width * meter - 4, bar.y - 7, 8, bar.height + 14), GUIContent.none);
        GUI.Label(new Rect(Screen.width / 2 - 185, Screen.height - 72, 400, 30), $"Press [Space] in green - tree health: {health}", bodyStyle);
    }

    bool NearTree() => Vector2.Distance(player.position, tree.position) <= TreeRange;

    static int DamageForTiming(float timing) => timing >= 0.4f && timing <= 0.6f ? 2 : 1;

    static SpriteRenderer MakeSprite(string name, Vector2 position, Vector3 scale, Color color)
    {
        var texture = Texture2D.whiteTexture;
        var sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        var item = new GameObject(name);
        item.transform.position = position;
        item.transform.localScale = scale;
        var renderer = item.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        return renderer;
    }

    [ContextMenu("Validate reward rules")]
    void ValidateRewardRules()
    {
        Debug.Assert(DamageForTiming(0.5f) == 2, "The green center should be a strong hit.");
        Debug.Assert(DamageForTiming(0.2f) == 1, "Outside the green center should be a weak hit.");
    }
}
