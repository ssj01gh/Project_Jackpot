using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tutorial/TutorialSet")]
public class TutorialSetSO : ScriptableObject
{
    public List<Sprite> TutorialPages;
    public List<Vector2> TutorialTextPos;
    [TextArea(5, 20)]
    public List<string> TutorialText;
}
